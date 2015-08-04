using Microsoft.CodeAnalysis.Diagnostics;
// <copyright file="CommentedOutCodeTest.cs" company="SonarSource">Copyright © SonarSource 2015</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SonarLint.Rules;

namespace SonarLint.Rules.Tests
{
    [TestClass]
    [PexClass(typeof(CommentedOutCode))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class CommentedOutCodeTest
    {

        [PexMethod]
        [PexAllowedException(typeof(NullReferenceException))]
        public void Initialize([PexAssumeUnderTest]CommentedOutCode target, AnalysisContext context)
        {
            target.Initialize(context);
            // TODO: add assertions to method CommentedOutCodeTest.Initialize(CommentedOutCode, AnalysisContext)
        }
    }
}
