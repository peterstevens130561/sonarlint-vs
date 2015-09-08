using System;
using System.Collections.Generic;


namespace Tests.Diagnostics
{

    class Program
    {
        #region FieldDeclarations // Noncompliant
        public static object[] os = new int[0];
        public static object[] os2 = new object[0];
        private const int myConst = 4;

        #region propertyregion //Noncompliant
        public String month { get; set; }
        #endregion



        static void Main(string[] args)
        {
            Fruit[] fruits = new Apple[1]; 
            fruits = new Apple[1];
            FillWithOranges(fruits);
            #region oldguy // dealt with by other rule
            var fruits2 = new Apple[1];
            #endregion
            FillWithOranges(fruits2);
            var fruits3 = (Fruit[])new Apple[1];
        }

        #region MethodRegion //Noncompliant
        static void FillWithOranges(Fruit[] fruits)
        {
        }

        #endregion

        #region AnotherMethodRegion //Noncompliant
        public void SomeMethod()
        {

        }
        #endregion


    }

    abstract class Fruit { }
    class Banana : Fruit { }
    class Apple : Fruit { }
    class Orange : Fruit { }
}
