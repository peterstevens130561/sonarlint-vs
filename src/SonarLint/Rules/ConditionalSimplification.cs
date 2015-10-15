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

using System;
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
    [SqaleConstantRemediation("2min")]
    [Rule(DiagnosticId, RuleSeverity, Title, IsActivatedByDefault)]
    [Tags("clumsy")]
    public class ConditionalSimplification : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "S3240";
        internal const string Title = "The simplest possible condition syntax should be used";
        internal const string Description =
            "In the interests of keeping code clean, the simplest possible conditional syntax should be used. That " +
            "means using the \"??\" operator for an assign-if-not-null operator, and using the ternary operator \"?:\" " +
            "for assignment to a single variable.";
        internal const string MessageFormat = "Use the \"{0}\" operator here.";
        internal const string Category = "SonarQube";
        internal const Severity RuleSeverity = Severity.Minor;
        internal const bool IsActivatedByDefault = true;

        internal static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category,
                RuleSeverity.ToDiagnosticSeverity(), IsActivatedByDefault,
                helpLinkUri: DiagnosticId.GetHelpLink(),
                description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        private static readonly ExpressionSyntax NullExpression = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        internal const string IsNullCoalescingKey = "isNullCoalescing";

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeActionInNonGenerated(
                c =>
                {
                    var conditional = (ConditionalExpressionSyntax)c.Node;

                    var condition = TernaryOperatorPointless.RemoveParentheses(conditional.Condition);
                    var whenTrue = TernaryOperatorPointless.RemoveParentheses(conditional.WhenTrue);
                    var whenFalse = TernaryOperatorPointless.RemoveParentheses(conditional.WhenFalse);

                    if (EquivalenceChecker.AreEquivalent(whenTrue, whenFalse))
                    {
                        return;
                    }

                    ExpressionSyntax compared;
                    bool comparedIsNullInTrue;
                    if (!TryGetComparedVariable(condition, out compared, out comparedIsNullInTrue) ||
                        !ExpressionCanBeNull(compared, c.SemanticModel))
                    {
                        return;
                    }

                    if (CanBeNullCoalescing(whenTrue, whenFalse, compared, comparedIsNullInTrue, c.SemanticModel))
                    {
                        c.ReportDiagnostic(Diagnostic.Create(Rule, conditional.GetLocation(), "??"));
                    }
                },
                SyntaxKind.ConditionalExpression);

            context.RegisterSyntaxNodeActionInNonGenerated(
                c =>
                {
                    var ifStatement = (IfStatementSyntax)c.Node;
                    if (ifStatement.Else == null ||
                        ifStatement.Parent is ElseClauseSyntax)
                    {
                        return;
                    }

                    var whenTrue = ExtractSingleStatement(ifStatement.Statement);
                    var whenFalse = ExtractSingleStatement(ifStatement.Else.Statement);

                    if (whenTrue == null ||
                        whenFalse == null ||
                        EquivalenceChecker.AreEquivalent(whenTrue, whenFalse))
                    {
                        return;
                    }

                    ExpressionSyntax compared;
                    bool comparedIsNullInTrue;
                    var possiblyTernary =
                        TryGetComparedVariable(ifStatement.Condition, out compared, out comparedIsNullInTrue) &&
                        ExpressionCanBeNull(compared, c.SemanticModel);

                    bool isNullCoalescing;
                    if (CanBeSimplified(whenTrue, whenFalse,
                        possiblyTernary ? compared : null,
                        comparedIsNullInTrue,
                        c.SemanticModel, out isNullCoalescing))
                    {
                        c.ReportDiagnostic(Diagnostic.Create(Rule, ifStatement.IfKeyword.GetLocation(),
                            ImmutableDictionary<string, string>.Empty.Add(IsNullCoalescingKey, isNullCoalescing.ToString()),
                            isNullCoalescing ? "??" : "?:"));
                    }
                },
                SyntaxKind.IfStatement);
        }

        private bool CanBeSimplified(StatementSyntax statement1, StatementSyntax statement2,
            ExpressionSyntax compared, bool comparedIsNullInTrue, SemanticModel semanticModel, out bool isNullCoalescing)
        {
            isNullCoalescing = false;
            var return1 = statement1 as ReturnStatementSyntax;
            var return2 = statement2 as ReturnStatementSyntax;

            if (return1 != null && return2 != null)
            {
                var retExpr1 = TernaryOperatorPointless.RemoveParentheses(return1.Expression);
                var retExpr2 = TernaryOperatorPointless.RemoveParentheses(return2.Expression);

                if (compared != null && CanBeNullCoalescing(retExpr1, retExpr2, compared, comparedIsNullInTrue, semanticModel))
                {
                    isNullCoalescing = true;
                }

                return true;
            }

            var expressionStatement1 = statement1 as ExpressionStatementSyntax;
            var expressionStatement2 = statement2 as ExpressionStatementSyntax;

            if (expressionStatement1 == null || expressionStatement2 == null)
            {
                return false;
            }

            var expression1 = TernaryOperatorPointless.RemoveParentheses(expressionStatement1.Expression);
            var expression2 = TernaryOperatorPointless.RemoveParentheses(expressionStatement2.Expression);

            if (AreCandidateAssignments(expression1, expression2, compared, comparedIsNullInTrue, semanticModel, out isNullCoalescing))
            {
                return true;
            }

            if (compared != null && CanBeNullCoalescing(expression1, expression2, compared, comparedIsNullInTrue, semanticModel))
            {
                isNullCoalescing = true;
                return true;
            }

            if (AreCandidateInvocations(expression1, expression2, semanticModel, null, false))
            {
                return true;
            }

            return false;
        }

        private static bool AreCandidateAssignments(ExpressionSyntax expression1, ExpressionSyntax expression2,
            ExpressionSyntax compared, bool comparedIsNullInTrue, SemanticModel semanticModel, out bool isNullCoalescing)
        {
            isNullCoalescing = false;
            var assignment1 = expression1 as AssignmentExpressionSyntax;
            var assignment2 = expression2 as AssignmentExpressionSyntax;
            var canBeSimplified =
                assignment1 != null &&
                assignment2 != null &&
                EquivalenceChecker.AreEquivalent(assignment1.Left, assignment2.Left) &&
                assignment1.Kind() == assignment2.Kind();

            if (!canBeSimplified)
            {
                return false;
            }

            if (compared != null && CanBeNullCoalescing(assignment1.Right, assignment2.Right, compared, comparedIsNullInTrue, semanticModel))
            {
                isNullCoalescing = true;
            }

            return true;
        }

        internal static StatementSyntax ExtractSingleStatement(StatementSyntax statement)
        {
            var block = statement as BlockSyntax;
            if (block != null)
            {
                if (block.Statements.Count != 1)
                {
                    return null;
                }
                return block.Statements.First();
            }

            return statement;
        }

        private static bool AreCandidateInvocations(ExpressionSyntax expression1, ExpressionSyntax expression2,
            SemanticModel semanticModel, ExpressionSyntax compared, bool comparedIsNullInTrue)
        {
            var methodCall1 = expression1 as InvocationExpressionSyntax;
            var methodCall2 = expression2 as InvocationExpressionSyntax;

            if (methodCall1 == null || methodCall2 == null)
            {
                return false;
            }

            var methodSymbol1 = semanticModel.GetSymbolInfo(methodCall1).Symbol;
            var methodSymbol2 = semanticModel.GetSymbolInfo(methodCall2).Symbol;

            if (methodSymbol1 == null ||
                methodSymbol2 == null ||
                !methodSymbol1.Equals(methodSymbol2))
            {
                return false;
            }

            if (methodCall1.ArgumentList == null ||
                methodCall2.ArgumentList == null ||
                methodCall1.ArgumentList.Arguments.Count != methodCall2.ArgumentList.Arguments.Count)
            {
                return false;
            }

            var numberOfDifferences = 0;
            var numberOfComparisonsToCondition = 0;
            for (int i = 0; i < methodCall1.ArgumentList.Arguments.Count; i++)
            {
                var arg1 = methodCall1.ArgumentList.Arguments[i];
                var arg2 = methodCall2.ArgumentList.Arguments[i];

                if (!EquivalenceChecker.AreEquivalent(arg1.Expression, arg2.Expression))
                {
                    numberOfDifferences++;

                    if (compared != null)
                    {
                        var arg1IsCompared = EquivalenceChecker.AreEquivalent(arg1.Expression, compared);
                        var arg2IsCompared = EquivalenceChecker.AreEquivalent(arg2.Expression, compared);

                        if (arg1IsCompared && !comparedIsNullInTrue)
                        {
                            numberOfComparisonsToCondition++;
                        }

                        if (arg2IsCompared && comparedIsNullInTrue)
                        {
                            numberOfComparisonsToCondition++;
                        }
                    }
                }
                else
                {
                    if (compared != null && EquivalenceChecker.AreEquivalent(arg1.Expression, compared))
                    {
                        return false;
                    }
                }
            }

            return numberOfDifferences == 1 && (compared == null || numberOfComparisonsToCondition == 1);
        }

        private static bool CanBeNullCoalescing(ExpressionSyntax whenTrue, ExpressionSyntax whenFalse,
            ExpressionSyntax compared, bool comparedIsNullInTrue, SemanticModel semanticModel)
        {
            if (EquivalenceChecker.AreEquivalent(whenTrue, compared))
            {
                return !comparedIsNullInTrue;
            }

            if (EquivalenceChecker.AreEquivalent(whenFalse, compared))
            {
                return comparedIsNullInTrue;
            }

            return AreCandidateInvocations(whenTrue, whenFalse, semanticModel, compared, comparedIsNullInTrue);
        }

        internal static bool TryGetComparedVariable(ExpressionSyntax expression,
            out ExpressionSyntax compared, out bool comparedIsNullInTrue)
        {
            compared = null;
            comparedIsNullInTrue = false;
            var binary = expression as BinaryExpressionSyntax;
            if (binary == null ||
                !EqualsOrNotEquals.Contains(binary.Kind()))
            {
                return false;
            }

            comparedIsNullInTrue = binary.IsKind(SyntaxKind.EqualsExpression);

            if (EquivalenceChecker.AreEquivalent(binary.Left, NullExpression))
            {
                compared = binary.Right;
                return true;
            }

            if (EquivalenceChecker.AreEquivalent(binary.Right, NullExpression))
            {
                compared = binary.Left;
                return true;
            }

            return false;
        }

        private static bool ExpressionCanBeNull(ExpressionSyntax expression, SemanticModel semanticModel)
        {
            var info = semanticModel.GetTypeInfo(expression).Type;
            return info != null &&
                   (info.IsReferenceType ||
                    info.SpecialType == SpecialType.System_Nullable_T);
        }

        private static readonly SyntaxKind[] EqualsOrNotEquals = { SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression };
    }
}
