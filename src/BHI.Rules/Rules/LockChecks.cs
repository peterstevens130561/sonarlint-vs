using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using SonarLint.Common;
using SonarLint.Common.Sqale;
using SonarLint.Helpers;
using System.Linq;

namespace SonarLint.Rules
{
    class LockChecks
    {
        private SyntaxNodeAnalysisContext analysisContext;
        private UsingStatementSyntax usingStatement;
        private bool reportedIssue;
        private DiagnosticDescriptor rule;

        public LockChecks Start(SyntaxNodeAnalysisContext AnalysisContext,DiagnosticDescriptor rule)
        {
            this.analysisContext = AnalysisContext;
            this.usingStatement = (UsingStatementSyntax)analysisContext.Node;
            this.rule = rule;
            reportedIssue = false;
            return this;
        }

        /// <summary>
        /// 
        /// check if the current node is a using statement which declares  a Read- or WriteLock
        /// </summary>
        /// <returns></returns>
        internal  bool isLock()
        {
            VariableDeclarationSyntax declaration = usingStatement.Declaration;
            var symbolInfo = analysisContext.SemanticModel.GetSymbolInfo(declaration.Type);
            ITypeSymbol typeSymbol = (ITypeSymbol)symbolInfo.Symbol;
            while (typeSymbol != null && isNotLock(typeSymbol))
            {
                typeSymbol = typeSymbol.BaseType;
            }
            return typeSymbol != null;
        }

        internal SyntaxNode GetUsingBlock(ref SyntaxNodeAnalysisContext c)
        {
            SyntaxNode block = usingStatement.ChildNodes().Where(n => n as BlockSyntax != null).FirstOrDefault();
            if (block == null)
            {
                var diagnostic = Diagnostic.Create(rule, usingStatement.GetLocation(), "using followed by empty statement");
                c.ReportDiagnostic(diagnostic);
                reportedIssue = true;
            }

            return block;
        }

        internal bool LockCheckinSimpleMemberAccess(SyntaxNode usingBlock)
        {
            if (reportedIssue) return false;

            var simpleMembers = usingBlock.DescendantNodes().OfType<MemberAccessExpressionSyntax>();
            string expression = GetVariableName() + ".LockApplied";
            int checksOnLock = 0;
            if (simpleMembers.Count() > 0)
            {
                checksOnLock = simpleMembers.Where(n => n.ToString() == expression).Count();
            }
            if(checksOnLock == 0)
            {
                ReportIssue("there is no check in the using block whether the lock was successful");
            }
            return checksOnLock > 0;
        }

        private bool isMemberAccessExpression(SyntaxNode n)
        {
            var node = n as MemberAccessExpressionSyntax;
            return node != null;
        }
        internal IfStatementSyntax GetFirstIfStatementInUsingBlock(SyntaxNode block)
        {
            if (reportedIssue)
            {
                return null;
            }
            IfStatementSyntax firstIfStatement = block.ChildNodes().FirstOrDefault() as IfStatementSyntax;
            if (firstIfStatement == null)
            {
                // first should check on Applied
                var diagnostic = Diagnostic.Create(rule, usingStatement.GetLocation(), "not checked whether lock was succcessful");
                analysisContext.ReportDiagnostic(diagnostic);
                reportedIssue = true;
            }

            return firstIfStatement;
        }

        internal SyntaxNode CheckIfStatementNotEmpty(IfStatementSyntax firstIfStatement)
        {
            if (reportedIssue) { return null; }
            var ifAppliedNode = firstIfStatement.ChildNodes().Where(node => node as BlockSyntax != null).FirstOrDefault();
            if (ifAppliedNode == null)
            {
                ReportIssue( "empty if block");
            }

            return ifAppliedNode;
        }

        internal void CheckExpressionIsNotLockApplied(IfStatementSyntax firstIfStatement)
        {
            if (reportedIssue) { return; }
            var expression = ((IfStatementSyntax)firstIfStatement).Condition as PrefixUnaryExpressionSyntax;
            var appliedCondition = GetVariableName() + ".LockApplied";
            if (expression == null || !expression.ToString().Contains(appliedCondition))
            {
                ReportIssue("incorrect check on lock");
            }

        }

        internal bool isCommand()
        {
            SyntaxNode parent = usingStatement.Parent;
            while(! (parent is ClassDeclarationSyntax))
            {
                parent = parent.Parent;
            }
            ClassDeclarationSyntax classDeclaration = (ClassDeclarationSyntax)parent;
            return true;
        }
        internal void CheckReturnOrThrow(SyntaxNode ifAppliedNode)
        {
            if (reportedIssue) { return; }
            var returnStatement = ifAppliedNode.ChildNodes().Where(node => node as ReturnStatementSyntax != null).LastOrDefault();
            var throwStatement = ifAppliedNode.ChildNodes().Where(node => node as ThrowStatementSyntax != null).LastOrDefault();
            if (returnStatement == null && throwStatement == null)
            {
                ReportIssue( "no return/throw statement");
            }
        }

        internal string GetVariableName()
        {
            VariableDeclarationSyntax declaration = usingStatement.Declaration;

            return declaration.Variables.First().Identifier.ToString();
        }
        private bool isNotLock(ITypeSymbol typeSymbol)
        {
            return !(typeSymbol.Name.Equals("ReadLock") || typeSymbol.Name.Equals("WriteLock"));
        }

        private void ReportIssue(string msg)
        {
            var diagnostic = Diagnostic.Create(rule, usingStatement.GetLocation(), msg);
            analysisContext.ReportDiagnostic(diagnostic);
            reportedIssue = true;
        }

    }


}
