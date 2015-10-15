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

using System.Collections.Generic;
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
    [SqaleConstantRemediation("2min")]
    [SqaleSubCharacteristic(SqaleSubCharacteristic.Readability)]
    [Rule(DiagnosticId, RuleSeverity, Title, IsActivatedByDefault)]
    [Tags("clumsy")]
    public class RedundantArgument : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "S3254";
        internal const string Title = "Default parameter values should not be passed as arguments";
        internal const string Description =
            "Specifying the default parameter values in a method call is redundant. Such values should be omitted in the interests of readability.";
        internal const string MessageFormat = "Remove this default value assigned to parameter \"{0}\".";
        internal const string Category = "SonarQube";
        internal const Severity RuleSeverity = Severity.Minor;
        internal const bool IsActivatedByDefault = true;
        private const IdeVisibility ideVisibility = IdeVisibility.Hidden;

        internal static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category,
                RuleSeverity.ToDiagnosticSeverity(ideVisibility), IsActivatedByDefault,
                helpLinkUri: DiagnosticId.GetHelpLink(),
                description: Description,
                customTags: ideVisibility.ToCustomTags());

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeActionInNonGenerated(
                c =>
                {
                    var methodCall = (InvocationExpressionSyntax) c.Node;
                    var methodParameterLookup = new ArrayCovariance.MethodParameterLookup(methodCall, c.SemanticModel);
                    var argumentMappings = methodCall.ArgumentList.Arguments.Select(argument =>
                        new KeyValuePair<ArgumentSyntax, IParameterSymbol>(argument,
                            methodParameterLookup.GetParameterSymbol(argument)))
                        .ToList();

                    var methodSymbol = methodParameterLookup.MethodSymbol;
                    if (methodSymbol == null)
                    {
                        return;
                    }

                    foreach (var argumentMapping in argumentMappings)
                    {
                        if (ArgumentHasDefaultValue(argumentMapping, c.SemanticModel))
                        {
                            var argument = argumentMapping.Key;
                            var parameter = argumentMapping.Value;
                            c.ReportDiagnostic(Diagnostic.Create(Rule, argument.GetLocation(), parameter.Name));
                        }
                    }
                },
                SyntaxKind.InvocationExpression);
        }

        internal static bool ArgumentHasDefaultValue(
            KeyValuePair<ArgumentSyntax, IParameterSymbol> argumentMapping,
            SemanticModel semanticModel)
        {
            var argument = argumentMapping.Key;
            var parameter = argumentMapping.Value;

            if (!parameter.HasExplicitDefaultValue)
            {
                return false;
            }

            var defaultValue = parameter.ExplicitDefaultValue;
            var argumentValue = semanticModel.GetConstantValue(argument.Expression);
            return argumentValue.HasValue &&
                   object.Equals(argumentValue.Value, defaultValue);
        }
    }
}
