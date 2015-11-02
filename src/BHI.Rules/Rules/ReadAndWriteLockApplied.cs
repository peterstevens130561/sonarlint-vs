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
    [SqaleConstantRemediation("10min")]
    [SqaleSubCharacteristic(SqaleSubCharacteristic.LogicReliability)]
    [Rule(DiagnosticId, RuleSeverity, Title, IsActivatedByDefault)]
    [Tags("bug")]
    public class ReadAndWriteLockApplied : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "BHI1011";
        internal const string Title = "Incorrect use of Read/Write Lock";
        internal const string Description =
            "Incorrect use of Read/Write Lock";
        internal const string MessageFormat = "Incorrect use of Read/Write Lock reason: {0}";
        internal const string Category = "SonarQube";
        internal const Severity RuleSeverity = Severity.Critical;
        internal const bool IsActivatedByDefault = false;
        private bool reportedIssue = false;
        private UsingStatementSyntax usingStatement;
        private SyntaxNodeAnalysisContext analysisContext;
        internal static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, DiagnosticId + ": " + Title, MessageFormat, Category,
                RuleSeverity.ToDiagnosticSeverity(), IsActivatedByDefault,
                helpLinkUri: DiagnosticId.GetHelpLink(),
                description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {

            context.RegisterSyntaxNodeActionInNonGenerated(
(System.Action<SyntaxNodeAnalysisContext>)                (                c =>
{
    reportedIssue = false;
    analysisContext = (SyntaxNodeAnalysisContext)c;
    usingStatement = (UsingStatementSyntax)c.Node;

    var lockChecks = new LockChecks();
    lockChecks.Start(c, Rule);
    if (!lockChecks.isLock())
    {
        return;
    }

    SyntaxNode block = lockChecks.GetUsingBlock(ref c);
    lockChecks.LockCheckinSimpleMemberAccess(block);
    /* IfStatementSyntax firstIfStatement = lockChecks.GetFirstIfStatementInUsingBlock(block);

    lockChecks.CheckExpressionIsNotLockApplied(firstIfStatement);
    SyntaxNode ifAppliedNode = lockChecks.CheckIfStatementNotEmpty(firstIfStatement);
    lockChecks.CheckReturnOrThrow( ifAppliedNode);
    if (!"false".Equals((string)((ReturnStatementSyntax)returnStatement).Expression.ToString()))
    {
        var diagnostic = Diagnostic.Create(Rule, returnStatement.GetLocation(), "does not return literal false");
        c.ReportDiagnostic(diagnostic);
        return;
    }
    */



}),
                SyntaxKind.UsingStatement
             );

        }



        private void ReportIssue(string msg)
        {
            var diagnostic = Diagnostic.Create(Rule, usingStatement.GetLocation(), msg);
            analysisContext.ReportDiagnostic(diagnostic);
            reportedIssue = true;
        }
    }


}
