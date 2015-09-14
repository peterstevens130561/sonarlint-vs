using System;
using System.Collections.Generic;


namespace Tests.Diagnostics
{

    // Name should be <rule> followed by Example
    class ConstructorShouldHaveFewParametersExample

    {

        ConstructorShouldHaveFewParametersExample(String a, String b)
        {

        }

        ConstructorShouldHaveFewParametersExample(String a, String b, String c)
        {

        }

        ConstructorShouldHaveFewParametersExample(String a, String b, String c, String d) // Noncompliant
        {

        }

        ConstructorShouldHaveFewParametersExample(String a, String b, String c, String d, String e) // Noncompliant
        {

        }
    }
}
