﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NSonarQubeAnalyzer.Diagnostics.Helpers;

namespace NSonarQubeAnalyzer.Diagnostics
{
    public class ConditionalStructureSameCondition : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "S1862";
        internal const string Description = @"Related ""if/else if"" statements should not have the same condition";
        internal const string MessageFormat = @"This branch duplicates the one on line {0}.";
        internal const string Category = "SonarQube";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(
                c =>
                {
                    var ifStatement = c.Node as IfStatementSyntax;

                    if (ifStatement == null)
                    {
                        return;
                    }

                    var currentCondition = ifStatement.Condition;

                    var preceedingCondition = ifStatement
                        .GetPreceedingConditionsInConditionChain()
                        .FirstOrDefault(preceeding => SyntaxFactory.AreEquivalent(currentCondition, preceeding));

                    if (preceedingCondition != null)
                    {
                        c.ReportDiagnostic(Diagnostic.Create(
                                Rule,
                                currentCondition.GetLocation(),
                                preceedingCondition.GetLocation().GetLineSpan().StartLinePosition.Line + 1));
                    }
                },
                SyntaxKind.IfStatement);
        }        
    }
}
