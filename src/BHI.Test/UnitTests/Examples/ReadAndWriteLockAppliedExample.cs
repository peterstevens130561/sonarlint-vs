using System;
using System.Collections.Generic;


namespace Tests.Diagnostics
{

    class ReadAndWriteLockAppliedExample
    {
        protected override void BindDataSet()
        {
            String fun = "fun";
            //This one should be ok
            using (ReadLock readLock = new Gotham(fun))
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


            // The first one is a test, but is this the right pattern
            using (ReadLock readLock = new Gotham(fun))
            {
                if (!readLock.LockApplied()) // What to do with this one?
                {
                    var gemDataSet = simCase.Input.DataSet as joaGEMDataSet;
                    if (gemDataSet != null)
                    {
                        m_DataViewUNLOADSTR.Table = gemDataSet.UNLOADSTR;
                    }
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
            }

            // bit of nonsense, should we deal with this one?
            using (ReadLock readLock = new ReadLock()) ;

            //
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
