using System;
using System.Collections.Generic;


namespace BHI.JewelEarth.Daffodil
{

    public class ShouldBeInApi
    {
        public void MyMethod()
        {

        }

        void InternalMethod() { }

        internal void AnotherInternalMethod()
        {

        }
    }

    public enum MyEnum
    {
        One,Two,Three

    }

    public interface PublicInterface
    {

    }
}
