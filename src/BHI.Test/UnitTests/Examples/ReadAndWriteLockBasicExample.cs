using System;
using System.Collections.Generic;
using joa.ReadLock;
namespace Tests.Diagnostics
{
    class someCommand : joaCommand
    {
        public void Execute()
        {
            using(var myLock = new ReadLock()) //Noncompliant
            {
                int a = 4;
                int fun= somefun();
            }  
                
        }
    }

    class someCommand : joaCommand
    {
        public void Execute() // is ok for now, there is a check
        {
            using (var myLock = new ReadLock()) 
            {
                int a = 4;
                if(myLock.LockApplied)
                {
                    somebougs();
                }
                int fun = somefun();
            }

        }
    }

}
namespace joa.ReadLock
{
    internal class ReadLock
    {
        Boolean LockApplied { get { return false; } }

    }

    internal class WriteLock
    {
        Boolean LockApplied { get { return false; } }
    }
}
