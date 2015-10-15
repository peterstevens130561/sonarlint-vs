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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.IO;
using System.Linq;

namespace SonarLint.Helpers
{
    public static class DiagnosticAnalyzerContextHelper
    {
        #region Register*ActionInNonGenerated

        public static void RegisterSyntaxNodeActionInNonGenerated<TLanguageKindEnum>(
            this AnalysisContext context,
            Action<SyntaxNodeAnalysisContext> action,
            params TLanguageKindEnum[] syntaxKinds) where TLanguageKindEnum : struct
        {
            context.RegisterSyntaxNodeAction(
                c =>
                {
                    if (!c.Node.SyntaxTree.IsGenerated())
                    {
                        action(c);
                    }
                },
                syntaxKinds);
        }

        public static void RegisterSyntaxTreeActionInNonGenerated(
            this AnalysisContext context,
            Action<SyntaxTreeAnalysisContext> action)
        {
            context.RegisterSyntaxTreeAction(
                c =>
                {
                    if (!c.Tree.IsGenerated())
                    {
                        action(c);
                    }
                });
        }

        public static void RegisterCodeBlockStartActionInNonGenerated<TLanguageKindEnum>(
            this AnalysisContext context,
            Action<CodeBlockStartAnalysisContext<TLanguageKindEnum>> action) where TLanguageKindEnum : struct
        {
            context.RegisterCodeBlockStartAction<TLanguageKindEnum>(
                c =>
                {
                    if (!c.CodeBlock.SyntaxTree.IsGenerated())
                    {
                        action(c);
                    }
                });
        }

        #endregion

        #region ReportDiagnosticIfNonGenerated

        public static void ReportDiagnosticIfNonGenerated(this CompilationAnalysisContext context, Diagnostic diagnostic)
        {
            if (!diagnostic.Location.SourceTree.IsGenerated())
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        public static void ReportDiagnosticIfNonGenerated(this SymbolAnalysisContext context, Diagnostic diagnostic)
        {
            if (!diagnostic.Location.SourceTree.IsGenerated())
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        #endregion

        #region SyntaxTree.IsGenerated

        private static bool IsGenerated(this SyntaxTree tree)
        {
            if (tree == null)
            {
                return false;
            }

            return tree.HasGeneratedFileName();
        }

        #endregion

        #region Utilities

        private static readonly string[] GeneratedFileParts =
            {
                ".g.",
                ".generated.",
                ".designer.",
                ".generated.",
                "_generated.",
                "temporarygeneratedfile_"
            };

        private static bool HasGeneratedFileName(this SyntaxTree tree)
        {
            if (string.IsNullOrEmpty(tree.FilePath))
            {
                return false;
            }

            var fileName = Path.GetFileName(tree.FilePath).ToLowerInvariant();
            return GeneratedFileParts.Any(part => fileName.Contains(part));
        }

        #endregion
    }
}
