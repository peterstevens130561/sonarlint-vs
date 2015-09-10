using System;
using System.Collections.Generic;

namespace Tests.Diagnostics
{

    class Program
    {

        static void Main(string[] args)
        {
            Double d, e;
            if (d != 4.0 && e != Double.NaN) // Noncompliant
            {

            }

            Double d;
            if(d != Double.NaN) // Noncompliant 
            {
                ++d;
            }

            if (d == Double.NaN) // Noncompliant
            {
                ++d;
            }

            if(d >= Double.NaN) // Noncompliant
            {

            }

            if (d < Double.NaN) // Noncompliant
            {
                ++d;
            }

            if(d != 4.0) // compliant
            {

            }


            if ((d != 4.0) && (d != Double.NaN)) // Noncompliant
            {

            }

            if ((Double.Nan != 4.0) && (d != Double.NaN)) // Noncompliant
            {

            }


        }
    }
}
