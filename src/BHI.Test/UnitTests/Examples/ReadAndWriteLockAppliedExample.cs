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
    /*
    class ReadAndWriteLockAppliedExample
    {

        private void CompliantPositiveCheckCoversComplete()
        {
            for (int i = 0; ;)
            {
                using (var domainLock = new ReadLock(MarkerInterpretation, SeismicInterpretation))
                {
                    if (!domainLock.LockApplied)
                    {
                        continue;
                    }
                }
            }
        }
        private void CompliantPositiveCheckCoversComplete()
        {
            using (var domainLock = new ReadLock(MarkerInterpretation, SeismicInterpretation))
            {
                if (domainLock.LockApplied)
                {
                    int a = 10;
                }
            }
        }
    }
        private void CompliantThrowingException()
        {
            using (var domainLock = new ReadLock(MarkerInterpretation, SeismicInterpretation))
            {
                if (!domainLock.LockApplied)
                {
                    throw new InvalidOperationException(@"Failed to obtain readlock on required domain objects.");
                }
            }
        }
        private void MissingReturn()
        {
            using (ReadLock readLock = new Gotham(fun)) 
            { 
                if (!readLock.LockApplied) 
                {
                    var gemDataSet = simCase.Input.DataSet as joaGEMDataSet;
                    if (gemDataSet != null)
                    {
                        m_DataViewUNLOADSTR.Table = gemDataSet.UNLOADSTR;
                    }
                }
            }

        }
        private void uselessLock()
        {
            String fun = "fun";

            using (ReadLock readLock = new ReadLock()) //Noncompliant
            {

            }

            using (ReadLock readLock = new ReadLock()) ; //Noncompliant
            {
                var gemDataSet = simCase.Input.DataSet as joaGEMDataSet;
                if (gemDataSet != null)
                {
                    m_DataViewUNLOADSTR.Table = gemDataSet.UNLOADSTR;
                }
            }

        }
        private Boolean CompliantLock() {
            using (ReadLock readLock = new Gotham(fun))
            {
                if (!readLock.LockApplied)
                {
                    return false;
                }

                var gemDataSet = simCase.Input.DataSet as joaGEMDataSet;
                if (gemDataSet != null)
                {
                    m_DataViewUNLOADSTR.Table = gemDataSet.UNLOADSTR;
                }

            }

        }

        private Boolean NonCompliantReturningTrue()
        {
            using (ReadLock readLock = new Gotham(fun))
            {
                if (!readLock.LockApplied) 
                {
                    return true;
                }

                var gemDataSet = simCase.Input.DataSet as joaGEMDataSet;
                if (gemDataSet != null)
                {
                    m_DataViewUNLOADSTR.Table = gemDataSet.UNLOADSTR;
                }

            }
        }

        private Boolean CompliantLock() {
            using (ReadLock readLock = new Gotham(fun))
            {
                if (!readLock.LockApplied)
                {
                    return false;
                }

                var gemDataSet = simCase.Input.DataSet as joaGEMDataSet;
                if (gemDataSet != null)
                {
                    m_DataViewUNLOADSTR.Table = gemDataSet.UNLOADSTR;
                }

            }

        }
        }
        private void NonCompliantLocks() { 
            using (ReadLock readLock = new Gotham(fun)) 
            {
                if (readLock.LockApplied)
                {
                    return;
                }

                var gemDataSet = simCase.Input.DataSet as joaGEMDataSet;
                if (gemDataSet != null)
                {
                    m_DataViewUNLOADSTR.Table = gemDataSet.UNLOADSTR;
                }

            }

            using (ReadLock readLock = new Gotham(fun)) 
            {
                if (readLock.LockApplied)
                {
                    var gemDataSet = simCase.Input.DataSet as joaGEMDataSet;
                    if (gemDataSet != null)
                    {
                        m_DataViewUNLOADSTR.Table = gemDataSet.UNLOADSTR;
                    }
                }
            }

            //This one should fail, as there is no test
            using (var writeLock = new WriteLock()) //Noncompliant
            {
                var gemDataSet = simCase.Input.DataSet as joaGEMDataSet;
                if (gemDataSet != null)
                {
                    m_DataViewUNLOADSTR.Table = gemDataSet.UNLOADSTR;
                }
            }

            //This one should fail, as there is no test
            using (var readLock = new Gotham(fun)) //Noncompliant
            {
                var gemDataSet = simCase.Input.DataSet as joaGEMDataSet;
                if (gemDataSet != null)
                {
                    m_DataViewUNLOADSTR.Table = gemDataSet.UNLOADSTR;
                }
            }

            // This one should fail as the test is not on the readLock
            using (var readLock = new ReadLock(fun)) //Noncompliant
            {
                if (simCase != null) // first if should check on applied
                {
                    var gemDataSet = simCase.Input.DataSet as joaGEMDataSet;
                    if (gemDataSet != null)
                    {
                        m_DataViewUNLOADSTR.Table = gemDataSet.UNLOADSTR;
                    }
                }
            }


            // This one should fail as the first statement is not a test
            using (var readLock = new Gotham(fun)) //Noncompliant
            {
                var gemDataSet = simCase.Input.DataSet as joaGEMDataSet;
                if (gemDataSet != null)
                {
                    m_DataViewUNLOADSTR.Table = gemDataSet.UNLOADSTR;
                }
            }




            using (ReadLock readLock = new Gotham(fun)) 
            {
                DoSomethingNaughty(); 
                if (!readLock.LockApplied) 
                {
                    var gemDataSet = simCase.Input.DataSet as joaGEMDataSet;
                    if (gemDataSet != null)
                    {
                        m_DataViewUNLOADSTR.Table = gemDataSet.UNLOADSTR;
                    }
                }
                int a = 4 == 5 ? 0 : 3;
            }

            // bit of nonsense, should we deal with this one?



        }

        //Used to find cases where we use a method to return a lock (yikes)
        ReadLock GetSimulationCase()
        {
            return new ReadLock();
        }

    // Make sure that the type is defined
 

        internal class Gotham : ReadLock
        {
        }

    }

namespace joa.ReadLock
{
    internal class ReadLock
    {
        Boolean LockApplied { get { return false; }  }

    }

    internal class WriteLock
    {
        Boolean LockApplied { get { return false; } }
    }
}
