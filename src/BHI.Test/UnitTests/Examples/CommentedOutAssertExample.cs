using System;
using System.Collections.Generic;


namespace Tests.Diagnostics
{

    class StringLocalizationExample
    {

           [TestMethod]
            public void MyOkTest()
        {
            int i = 1;
            int b = 2;
            Assert.AssertEquals(i, 4);

        }

        [TestMethod]
        public void MyFailingTest()
        {
            int i = 1;
            // Noncompliant: i.Should().BeEquivalentTo
            int b = 2;
            // i=i+1;
            // Noncompliant: Assert.AssertEquals(i, 4); 
        }

    }
}
