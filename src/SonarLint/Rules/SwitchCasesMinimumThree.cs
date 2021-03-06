﻿/*
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
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using SonarLint.Common;
using SonarLint.Common.Sqale;
using SonarLint.Helpers;

namespace SonarLint.Rules
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [SqaleSubCharacteristic(SqaleSubCharacteristic.Readability)]
    [SqaleConstantRemediation("5min")]
    [Rule(DiagnosticId, RuleSeverity, Title, IsActivatedByDefault)]
    [Tags("misra")]
    public class SwitchCasesMinimumThree : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "S1301";
        internal const string Title = "\"switch\" statements should have at least 3 \"case\" clauses";
        internal const string Description =
            "\"switch\" statements are useful when there are many different cases depending on " +
            "the value of the same expression. For just one or two cases however, the code will " +
            "be more readable with \"if\" statements.";
        internal const string MessageFormat = "Replace this \"switch\" statement with \"if\" statements to increase readability.";
        internal const string Category = "SonarQube";
        internal const Severity RuleSeverity = Severity.Minor;
        internal const bool IsActivatedByDefault = false;


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
                    var switchNode = (SwitchStatementSyntax)c.Node;
                    if (!HasAtLeastThreeLabels(switchNode))
                    {
                        c.ReportDiagnostic(Diagnostic.Create(Rule, switchNode.SwitchKeyword.GetLocation()));
                    }
                },
                SyntaxKind.SwitchStatement);
        }

        private static bool HasAtLeastThreeLabels(SwitchStatementSyntax node)
        {
            return node.Sections.Sum(section => section.Labels.Count) >= 3;
        }
    }
}
