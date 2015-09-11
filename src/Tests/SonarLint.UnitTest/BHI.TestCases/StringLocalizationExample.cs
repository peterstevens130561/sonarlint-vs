using System;
using System.Collections.Generic;


namespace Tests.Diagnostics
{

    class StringLocalizationExample
    {
        String myString = "12345"; //Noncompliant

        static void Main(string[] args)
        {
            String mies = "mies"; //Noncompliant
            String john = "aap" + mies; // Noncompliant
            var myvar = "myvar"; // Noncompliant
            var literal = @"mystring";
            var myint = 4;
            bool mybool = 6;
            var someexpression = 4 + 5;
            var somethingwithString = myvar.StartsWith("shouldBeInternationalized"); //Noncompliant
            var somethingElse = myvar.StartsWith(@"shouldNotBeInternationalized"); 
        }

    }
}
