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

namespace SonarLint.Rules
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [SqaleConstantRemediation("5min")]
    [SqaleSubCharacteristic(SqaleSubCharacteristic.LogicReliability)]
    [Rule(DiagnosticId, RuleSeverity, Title, IsActivatedByDefault)]
    [Tags("bug")]
    public class ComparisonNaN : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "BHI1002";
        internal const string Title = "Can't compare with Double.NaN";
        internal const string Description =
            "Can't compare with Double.NaN";
        internal const string MessageFormat = "Use DoubleIsNan() function";
        internal const string Category = "SonarQube";
        internal const Severity RuleSeverity = Severity.Blocker;
        internal const bool IsActivatedByDefault = true;

        internal static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, DiagnosticId + " : " + Title, MessageFormat, Category,
                RuleSeverity.ToDiagnosticSeverity(), IsActivatedByDefault,
                helpLinkUri: DiagnosticId.GetHelpLink(),
                description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeActionInNonGenerated(
                c =>
                {
                    var expression = (BinaryExpressionSyntax)c.Node;
                    
                    var isComparison= expression.OperatorToken.IsKind(SyntaxKind.ExclamationEqualsToken);
                    foreach (SyntaxNode child in expression.ChildNodes())
                    {
                        var member = child as MemberAccessExpressionSyntax;
                        
                        if (member == null)
                        {
                            continue;
                        }
                        var exp = member.Expression as IdentifierNameSyntax;
                        if(exp==null)
                        {
                            continue;
                        }
                        if (exp.Identifier.Text == "Double" && member.Name.Identifier.Text == "NaN")
                        {
                            var diagnostic = Diagnostic.Create(Rule, expression.GetLocation());
                            c.ReportDiagnostic(diagnostic);
                        }

                    }

                },
                SyntaxKind.NotEqualsExpression,
                SyntaxKind.EqualsExpression,
                SyntaxKind.GreaterThanExpression,
                SyntaxKind.GreaterThanOrEqualExpression,
                SyntaxKind.LessThanOrEqualExpression,
                SyntaxKind.LessThanExpression
             );
        }
    }
}
