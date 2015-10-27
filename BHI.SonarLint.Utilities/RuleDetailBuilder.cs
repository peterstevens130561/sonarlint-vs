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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using SonarLint.Common.Sqale;
using SonarLint.Helpers;
using SonarLint.Common;

namespace BHI.SonarLint.Utilities
{
    public static class RuleDetailBuilder
    {
        private const string RuleDescriptionPathPattern = "Rules.Description.{0}.html";

        public static IEnumerable<RuleDetail> GetAllRuleDetails()
        {
            return new RuleFinder().GetAllAnalyzerTypes().Select(GetRuleDetail);
        }
        public static IEnumerable<RuleDetail> GetParameterlessRuleDetails()
        {
            return new RuleFinder().GetParameterlessAnalyzerTypes().Select(GetRuleDetail);
        }

        private static RuleDetail GetRuleDetail(Type analyzerType)
        {
            var rule = analyzerType.GetCustomAttributes<RuleAttribute>().Single();

            var ruleDetail = new RuleDetail
            {
                Key = rule.Key,
                Title = rule.Title,
                Severity = rule.Severity.ToString().ToUpper(CultureInfo.InvariantCulture),
                IsActivatedByDefault = rule.IsActivatedByDefault,
                Description = GetResourceHtml(analyzerType, rule),
                IsTemplate = RuleFinder.IsRuleTemplate(analyzerType)
            };

            GetParameters(analyzerType, ruleDetail);
            GetTags(analyzerType, ruleDetail);
            GetSqale(analyzerType, ruleDetail);

            return ruleDetail;
        }

        private static void GetSqale(Type analyzerType, RuleDetail ruleDetail)
        {
            var sqaleRemediation = analyzerType.GetCustomAttributes<SqaleRemediationAttribute>().FirstOrDefault();

            if (sqaleRemediation == null || sqaleRemediation is NoSqaleRemediationAttribute)
            {
                ruleDetail.SqaleDescriptor = null;
                return;
            }

            var sqaleSubCharacteristic = analyzerType.GetCustomAttributes<SqaleSubCharacteristicAttribute>().First();
            var sqaleDescriptor = new SqaleDescriptor
            {
                SubCharacteristic = sqaleSubCharacteristic.SubCharacteristic.ToSonarQubeString()
            };
            var constantRemediation = sqaleRemediation as SqaleConstantRemediationAttribute;
            if (constantRemediation == null)
            {
                ruleDetail.SqaleDescriptor = sqaleDescriptor;
                return;
            }

            sqaleDescriptor.Remediation.Properties.AddRange(new[]
            {
                new SqaleRemediationProperty
                {
                    Key = SqaleRemediationProperty.RemediationFunctionKey,
                    Text = SqaleRemediationProperty.ConstantRemediationFunctionValue
                },
                new SqaleRemediationProperty
                {
                    Key = SqaleRemediationProperty.OffsetKey,
                    Value = constantRemediation.Value,
                    Text = string.Empty
                }
            });

            ruleDetail.SqaleDescriptor = sqaleDescriptor;
        }

        private static void GetTags(Type analyzerType, RuleDetail ruleDetail)
        {
            var tags = analyzerType.GetCustomAttributes<TagsAttribute>().FirstOrDefault();
            if (tags != null)
            {
                ruleDetail.Tags.AddRange(tags.Tags);
            }
        }

        private static void GetParameters(Type analyzerType, RuleDetail ruleDetail)
        {
            var typeToGetParametersFrom = analyzerType;
            var templateInterface = analyzerType.GetInterfaces()
                .FirstOrDefault(type => type.IsGenericType &&
                                        type.GetGenericTypeDefinition() == typeof (IRuleTemplate<>));

            if (templateInterface != null)
            {
                typeToGetParametersFrom = templateInterface.GetGenericArguments().First();
            }

            var parameters = typeToGetParametersFrom.GetProperties()
                .Select(p => p.GetCustomAttributes<RuleParameterAttribute>().SingleOrDefault());

            foreach (var ruleParameter in parameters
                .Where(attribute => attribute != null))
            {
                ruleDetail.Parameters.Add(
                    new RuleParameter
                    {
                        DefaultValue = ruleParameter.DefaultValue,
                        Description = ruleParameter.Description,
                        Key = ruleParameter.Key,
                        Type = ruleParameter.Type.ToSonarQubeString()
                    });
            }
        }

        private static string GetResourceHtml(Type analyzerType, RuleAttribute rule)
        {
            var resources = analyzerType.Assembly.GetManifestResourceNames();
            var resource = resources.SingleOrDefault(r => r.EndsWith(
                string.Format(CultureInfo.InvariantCulture, RuleDescriptionPathPattern, rule.Key),
                StringComparison.OrdinalIgnoreCase));

            if (resource == null)
            {
                throw new InvalidDataException(string.Format("Could not locate resource for rule {0} {1} in assembly {2}", rule.Key,analyzerType.FullName,analyzerType.Assembly.FullName));
            }

            using (var stream = analyzerType.Assembly.GetManifestResourceStream(resource))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
