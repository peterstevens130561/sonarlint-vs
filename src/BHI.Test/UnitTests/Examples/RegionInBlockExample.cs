using System;
using System.Collections.Generic;


namespace Tests.Diagnostics
{

    abstract class Fruit { }
    class Banana : Fruit { }
    class Apple : Fruit { }
    class Orange : Fruit { }


    class RegionInBlockExample

    {

        #region myregion
        public static object[] os = new int[0];
        public static object[] os2 = new object[0];
        #endregion
        static void Main(string[] args)
        {
            Fruit[] fruits = new Apple[1]; 
            fruits = new Apple[1];
            FillWithOranges(fruits);
            #region oldguy //Noncompliant
            var fruits2 = new Apple[1];
            #endregion
            FillWithOranges(fruits2);
            var fruits3 = (Fruit[])new Apple[1];
        }

        #region thisisok
        static void FillWithOranges(Fruit[] fruits)
        {
        }

        #endregion
    }
}
