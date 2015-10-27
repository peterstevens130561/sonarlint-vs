using System;
using System.Collections.Generic;


namespace Tests.Diagnostics
{

    class ReadAndWriteLockAppliedExample
    {
        private void MissingReturn()
        {
            using (ReadLock readLock = new Gotham(fun)) //Noncompliant
            // The first one is a test, but is this the right pattern
            { 
                if (!readLock.LockApplied()) 
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
                if (!readLock.LockApplied())
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
                if (!readLock.LockApplied()) 
                {
                    return true; //Noncompliant
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
                if (!readLock.LockApplied())
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
            using (ReadLock readLock = new Gotham(fun)) //Noncompliant
            {
                if (readLock.LockApplied())
                {
                    return;
                }

                var gemDataSet = simCase.Input.DataSet as joaGEMDataSet;
                if (gemDataSet != null)
                {
                    m_DataViewUNLOADSTR.Table = gemDataSet.UNLOADSTR;
                }

            }
            //This one should be not ok
            using (ReadLock readLock = new Gotham(fun)) //Noncompliant
            {
                if (readLock.LockApplied())
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




            using (ReadLock readLock = new Gotham(fun)) //Noncompliant
            {
                DoSomethingNaughty(); // This is why
                if (!readLock.LockApplied()) 
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
        internal class ReadLock
        {
            Boolean IsApplied()
            {
                return false;
            }
        }

        internal class WriteLock
        {
            Boolean IsApplied()
            {
                return false;
            }
        }

        internal class Gotham : ReadLock
        {
        }




    }
}
