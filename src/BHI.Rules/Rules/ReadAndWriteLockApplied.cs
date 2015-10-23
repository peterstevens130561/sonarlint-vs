/*
 * SonarLint for Visual Studio
 * Copyright (C) 2015 SonarSource
 * sonarqube@googlegroups.com
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02
 */

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
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [SqaleConstantRemediation("5min")]
    [SqaleSubCharacteristic(SqaleSubCharacteristic.LogicReliability)]
    [Rule(DiagnosticId, RuleSeverity, Title, IsActivatedByDefault)]
    [Tags("bug")]
    public class ReadAndWriteLockApplied : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "BHI1011";
        internal const string Title = "Incorrect use of Read/Write Lock";
        internal const string Description =
            "Incorrect use of Read/Write Lock";
        internal const string MessageFormat = "Incorrect use of Read/Write Lock";
        internal const string Category = "SonarQube";
        internal const Severity RuleSeverity = Severity.Critical;
        internal const bool IsActivatedByDefault = true;

        internal static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category,
                RuleSeverity.ToDiagnosticSeverity(), IsActivatedByDefault,
                helpLinkUri: DiagnosticId.GetHelpLink(),
                description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeActionInNonGenerated(
                c =>
                {
                    UsingStatementSyntax usingStatement = (UsingStatementSyntax)c.Node;

                    VariableDeclarationSyntax declaration = usingStatement.Declaration;
                    var symbolInfo = c.SemanticModel.GetSymbolInfo(declaration.Type);
                    ITypeSymbol typeSymbol = (ITypeSymbol)symbolInfo.Symbol;
                    while (typeSymbol != null && isNotLock(typeSymbol))
                    {
                        typeSymbol = typeSymbol.BaseType;
                    }
                    if (typeSymbol == null)
                    {
                        return;
                    }
                    var variableName = declaration.Variables.First().Identifier;

                    SyntaxNode block = usingStatement.ChildNodes().Where(n => n as BlockSyntax != null).FirstOrDefault();
                    if (block == null)
                    {
                        return;
                    }
                    var firstStatement = block.ChildNodes().First() as IfStatementSyntax;
                    if (firstStatement == null)
                    {
                        // first should check on Applied
                        var diagnostic = Diagnostic.Create(Rule, usingStatement.GetLocation());
                        c.ReportDiagnostic(diagnostic);
                        return;
                    }

                    var expression = ((IfStatementSyntax)firstStatement).Condition;
                    var appliedCondition = variableName + ".LockApplied()";
                    if (!expression.ToString().Contains(appliedCondition))
                    {
                        var diagnostic = Diagnostic.Create(Rule, usingStatement.GetLocation());
                        c.ReportDiagnostic(diagnostic);
                        return;
                    }
                },
                SyntaxKind.UsingStatement
             );

        }

        private bool isNotLock(ITypeSymbol typeSymbol)
        {
            return !(typeSymbol.Name.Equals("ReadLock") || typeSymbol.Name.Equals("WriteLock"));
        }
    }


}
