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
using System.Text.RegularExpressions;
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
    [SqaleSubCharacteristic(SqaleSubCharacteristic.MaintainabilityCompliance)]
    [SqaleConstantRemediation("5min")]
    [Rule(DiagnosticId, RuleSeverity, Description, IsActivatedByDefault)]
    [Tags("convention")]
    public class InvalidUsing : DiagnosticAnalyzer
    {

        internal const string DiagnosticId = "BHI1010";
        internal const string Description = "Some references should no longer be used";
        internal const string MessageFormat = "This reference \"{0}\" should not be used for new code";
        internal const string Category = "SonarQube";
        internal const Severity RuleSeverity = Severity.Blocker;
        internal const bool IsActivatedByDefault = true;

        internal static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category,
                RuleSeverity.ToDiagnosticSeverity(), IsActivatedByDefault);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        [RuleParameter("format", PropertyType.PropertySet, "Namespaces of invalid references, as regex")]
        public string Convention { get; set; }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeActionInNonGenerated(
                c =>
                {
                    var usingDirective = (UsingDirectiveSyntax)c.Node;
                    foreach (var child in usingDirective.ChildNodes())
                    {
                        var name = child.GetText().ToString();
                        Regex regex = new Regex(Convention);
                        if (regex.Matches(name).Count > 0)
                        {
                            c.ReportDiagnostic(Diagnostic.Create(Rule, c.Node.GetLocation(), name));
                        }
                    }

                },
                SyntaxKind.UsingDirective);
        }
    }
}

