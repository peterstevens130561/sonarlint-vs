using System;
using System.Collections.Generic;


namespace Tests.Diagnostics
{

    class ComparisonNanExample
    {
        public void compare()
        {
            double a; 

            double c = Double.NaN;

            if (a != Double.NaN) // Noncompliant
            {
                a = 4.0;
            }

            if (a >= Double.NaN) // Noncompliant
            {
                a = 4.0;
            }

            if (a <= Double.NaN) // Noncompliant
            {
                a = 4.0;
            }

            if (myfun() != Double.NaN) // Noncompliant 
            {
                a = 5.0;
            }

            if (a == Double.NaN) // Noncompliant
            {
                a = 4.0;
            }

        }

        private double myfun()
        {
            return 5.0;
        }

    }
}
