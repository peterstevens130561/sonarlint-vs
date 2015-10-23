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

using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SonarLint.Utilities;

namespace BHI.SonarLint.DocGenerator
{
    public static class Program
    {
        public static void Main()
        {
            var productVersion = FileVersionInfo.GetVersionInfo(typeof(Program).Assembly.Location).ProductVersion;
            var content = GenerateRuleJson(productVersion);

            Directory.CreateDirectory(productVersion);
            File.WriteAllText(Path.Combine(productVersion, "rules.json"), content);
        }

        public static string GenerateRuleJson(string productVersion)
        {
            var ruleDetails = RuleDetailBuilder.GetParameterlessRuleDetails()
                .Select(ruleDetail =>
                    RuleDescription.Convert(ruleDetail, productVersion))
                .ToList();

            return JsonConvert.SerializeObject(ruleDetails,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Formatting = Formatting.Indented
                    });
        }
    }
}
