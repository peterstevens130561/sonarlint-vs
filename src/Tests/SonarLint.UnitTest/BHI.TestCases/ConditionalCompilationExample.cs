using System;
using System.Collections.Generic;


namespace Tests.Diagnostics
{

    abstract class Fruit { }
#if true // Noncompliant
    class Banana : Fruit { }
#endif
    class Apple : Fruit { }
    class Orange : Fruit { }


    class Program
    {

#if SECOND // Noncompliant
        public static object[] os = new int[0];
        public static object[] os2 = new object[0];
#endif
        static void Main(string[] args)
        {
#if THIRD // Noncompliant
#endif
            Fruit[] fruits = new Apple[1]; 
            fruits = new Apple[1];
            FillWithOranges(fruits);
            var fruits2 = new Apple[1];
            FillWithOranges(fruits2);
            var fruits3 = (Fruit[])new Apple[1];
        }

        static void FillWithOranges(Fruit[] fruits)
        {
        }
    }
}
