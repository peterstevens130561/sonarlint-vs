using System;
using System.Collections.Generic;


namespace Tests.Diagnostics
{

    class StringLocalizationExample
    { 

        static void Main(string[] args)
        {
            String mies = "mies"; //Noncompliant
            String john = "aap" + mies; // Noncompliant
            var myvar = "myvar"; // Noncompliant
            var literal = @"mystring";
        }

    }
}
