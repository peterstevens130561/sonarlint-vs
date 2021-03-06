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
    [SqaleSubCharacteristic(SqaleSubCharacteristic.Understandability)]
    [SqaleConstantRemediation("5min")]
    [Rule(DiagnosticId, RuleSeverity, Title, IsActivatedByDefault)]
    [Tags("unused")]
    public class VariableUnused : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "S1481";
        internal const string Title = "Unused local variables should be removed";
        internal const string Description =
            "If a local variable is declared but not used, it is dead code and should be removed. " +
            "Doing so will improve maintainability because developers will not wonder what the variable " +
            "is used for.";
        internal const string MessageFormat = "Remove this unused \"{0}\" local variable.";
        internal const string Category = "SonarQube";
        internal const Severity RuleSeverity = Severity.Major;
        internal const bool IsActivatedByDefault = true;

        internal static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category,
                RuleSeverity.ToDiagnosticSeverity(), IsActivatedByDefault,
                helpLinkUri: DiagnosticId.GetHelpLink(),
                description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCodeBlockStartActionInNonGenerated<SyntaxKind>(cbc =>
            {
                var unusedLocals = new List<ISymbol>();

                cbc.RegisterSyntaxNodeAction(c =>
                {
                    unusedLocals.AddRange(
                        ((LocalDeclarationStatementSyntax) c.Node).Declaration.Variables
                            .Select(variable => c.SemanticModel.GetDeclaredSymbol(variable))
                            .Where(symbol => symbol != null));
                },
                SyntaxKind.LocalDeclarationStatement);

                cbc.RegisterSyntaxNodeAction(c =>
                {
                    var symbolInfo = c.SemanticModel.GetSymbolInfo(c.Node);
                    unusedLocals.Remove(symbolInfo.Symbol);

                    foreach (var candidateSymbol in symbolInfo.CandidateSymbols)
                    {
                        unusedLocals.Remove(candidateSymbol);
                    }
                },
                SyntaxKind.IdentifierName);

                cbc.RegisterCodeBlockEndAction(c =>
                {
                    foreach (var unused in unusedLocals)
                    {
                        c.ReportDiagnostic(Diagnostic.Create(Rule, unused.Locations.First(), unused.Name));
                    }
                });
            });
        }
    }
}
