using System;
using System.Collections.Generic;


namespace Tests.Diagnostics
{

    // Name should be <rule> followed by Example
    class FieldNameExample

    {
        private int _myfunnyfield; //NonCompliant
        public int _MyFunnyfield; //NonCompliant
        private MysticObject m_MysticObject; 
    }
}
