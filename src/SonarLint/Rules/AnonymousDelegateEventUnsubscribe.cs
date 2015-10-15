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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using SonarLint.Common;
using SonarLint.Common.Sqale;
using SonarLint.Helpers;
using Microsoft.CodeAnalysis.Text;

namespace SonarLint.Rules
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [SqaleConstantRemediation("15min")]
    [SqaleSubCharacteristic(SqaleSubCharacteristic.InstructionReliability)]
    [Rule(DiagnosticId, RuleSeverity, Title, IsActivatedByDefault)]
    [Tags("bug")]
    public class AnonymousDelegateEventUnsubscribe : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "S3244";
        internal const string Title = "Anonymous delegates should not be used to unsubscribe from Events";
        internal const string Description =
            "It is possible to subscribe to events with anonymous delegates, but having done so, it is impossible to unsubscribe " +
            "from them. That's because the process of subscribing adds the delegate to a list. The process of unsubscribing essentially " +
            "says: remove this item from the subscription list. But because an anonymous delegate was used in both cases, the unsubscribe " +
            "attempt tries to remove a different item from the list than was added. The result: \"NOOP\".";
        internal const string MessageFormat = "Unsubscribe with the same delegate that was used for the subscription.";
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
                    var assignment = (AssignmentExpressionSyntax)c.Node;

                    var @event = c.SemanticModel.GetSymbolInfo(assignment.Left).Symbol as IEventSymbol;

                    if (@event != null &&
                        assignment.Right is AnonymousFunctionExpressionSyntax)
                    {
                        var location = Location.Create(c.Node.SyntaxTree,
                            new TextSpan(assignment.OperatorToken.SpanStart, assignment.Span.End - assignment.OperatorToken.SpanStart));

                        c.ReportDiagnostic(Diagnostic.Create(Rule, location));
                    }
                },
                SyntaxKind.SubtractAssignmentExpression);
        }
    }
}
