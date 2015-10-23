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
using System.Linq;

namespace SonarLint.Rules
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [SqaleSubCharacteristic(SqaleSubCharacteristic.MaintainabilityCompliance)]
    [SqaleConstantRemediation("10min")]
    [Rule(DiagnosticId, RuleSeverity, Description, IsActivatedByDefault)]
    [Tags("convention")]
    public class Obsolete : DiagnosticAnalyzer
    {

        internal const string DiagnosticId = "BHI1010";
        internal const string Description = "Obsolete requires at least one argument, of which the first one must match regex";
        internal const string MessageFormat = "[Obsolete(\"{0}\")] should follow convention as defined through regular expression {1}";
        internal const string Category = "SonarQube";
        internal const Severity RuleSeverity = Severity.Critical;
        internal const bool IsActivatedByDefault = true;

        internal static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category,
                RuleSeverity.ToDiagnosticSeverity(), IsActivatedByDefault);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        [RuleParameter("format", PropertyType.RegularExpression, "Regular expression for obsolete to match","^since ([0-9]\\.)+[0-9]+ use .*$]")]
        public string Convention { get; set; }


        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeActionInNonGenerated(
                c =>
                {
                    var attributeList = (AttributeListSyntax)c.Node;

                    var obsoleteSyntax= attributeList.Attributes.FirstOrDefault(a => {
                        var name = a.Name.GetText().ToString();
                        var isMatch= Regex.IsMatch(name, @"^(System.)?Obsolete(Attribute)?$");
                        return isMatch;
                    });
                    if(obsoleteSyntax == null)
                    {
                        return;
                    }
                    var argumentList = obsoleteSyntax.ArgumentList;
                    string firstArgument = "";
                    if (argumentList != null)
                    {
                        var count = argumentList.Arguments.Count;
                        if (count > 0)
                        {
                            firstArgument = argumentList.Arguments.First().ToString();
                            if (Regex.IsMatch(firstArgument, Convention))
                            {
                                return;
                            }
                        }
                    }
                    c.ReportDiagnostic(Diagnostic.Create(Rule, attributeList.GetLocation(), firstArgument, Convention));


                },
                SyntaxKind.AttributeList);
        }
    }
}

