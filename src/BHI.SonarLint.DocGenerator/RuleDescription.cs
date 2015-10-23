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

using SonarLint.Common;
using SonarLint.Helpers;
using System.Text.RegularExpressions;

namespace BHI.SonarLint.DocGenerator
{
    public class RuleDescription
    {
        public static RuleDescription Convert(RuleDetail detail, string productVersion)
        {
            return new RuleDescription
            {
                Key = detail.Key,
                Title = detail.Title,
                Description = AddLinksBetweenRulesToDescription(detail.Description, productVersion),
                Tags = string.Join(", ", detail.Tags)
            };
        }

        public string Key { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Tags { get; set; }

        public const string CrosslinkPattern = "(Rule )(S[0-9]+)";

        private static string AddLinksBetweenRulesToDescription(string description, string productVersion)
        {
            var urlRegexPattern = string.Format(DiagnosticReportHelper.HelpLinkPattern, productVersion, "$2");
            var linkPattern = string.Format("<a href=\"{0}\">{1}</a>", urlRegexPattern, "$1$2");
            return Regex.Replace(description, CrosslinkPattern, linkPattern);
        }
    }
}