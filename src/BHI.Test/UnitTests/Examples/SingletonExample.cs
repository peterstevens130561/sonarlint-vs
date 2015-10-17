using System;
using System.Collections.Generic;


namespace Tests.Diagnostics
{

    class MySingleton
    {
        private MySingleton instance;
        public MySingleton()
        {
            if(instance== null)
            {
                instance = new MySingleton();
            }
        }
    }
}
