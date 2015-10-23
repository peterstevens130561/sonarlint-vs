using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BHI.SonarLint.Utilities;
using Microsoft.CodeAnalysis;
using System.Linq;
using System.Reflection;
namespace BHI.UnitTest.Utilities
{
    // The numbers in these tests will have to be adjusted when rules are added, or when
    // a new sonarlint release is merged
    [TestClass]
    public class RuleFinderTest
    {
        [TestMethod]
        public void CheckNumberOfParameterAnalyzers()
        {
            var ruleFinder = new RuleFinder();
            var count=ruleFinder.GetParameterlessAnalyzerTypes().Count();
            Assert.AreEqual(97, count, "Should have number of analyzers");
        }

        [TestMethod]
        public void CheckNumberOfAnalyzers()
        {
            var ruleFinder = new RuleFinder();
            var count = ruleFinder.GetAllAnalyzerTypes().Count();
            Assert.AreEqual(111, count, "Should have number of analyzers");
        }

        [TestMethod]
        public void CheckNumberOfBHIAnalyzers()
        {
            var ruleFinder = new RuleFinder();
            var analyzers = ruleFinder.GetAllAnalyzerTypes();
            var bhiAnalyzers = analyzers.Where( c  => c.Assembly.FullName.Contains("BHI"));
            Assert.AreEqual(10, bhiAnalyzers.Count(), "Should have number of analyzers");
        }

        [TestMethod]
        public void CheckBHIAnalyzers()
        {
            var ruleFinder = new RuleFinder();
            var analyzers = ruleFinder.GetAllAnalyzerTypes();
            var bhiAnalyzers = analyzers.Where(c => c.Assembly.FullName.Contains("BHI"));
            foreach(var analyzer in bhiAnalyzers)
            {
                MemberInfo[] info = analyzer.GetMember("Rule", BindingFlags.NonPublic | BindingFlags.Instance);
                Assert.IsNotNull(info, "getting analyzer " + analyzer.Name);

            }
        }
    }
}
