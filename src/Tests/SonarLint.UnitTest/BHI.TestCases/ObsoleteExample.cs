using System;
using System.Collections.Generic;
using Windows.Foundation.Metadata;

namespace Tests.Diagnostics
{

    // Name should be <rule> followed by Example
    class StringLocalizationExample

    {
        public voikd myfun()
        {
            MethodWithDeprecatedAttribute();
        }

        /// <summary>
        /// Deprecated method 
        /// </summary>
        /// <deprecated>
        /// <see cref="myotherfunction"/>
        /// </deprecated>
        [Obsolete("since 2016.1 use anothermethod")]
        public void MethodWithDeprecatedAttribute()
        {

        }
        [Obsolete("goblediegok")] //Noncompliant
        public void MethodWithDeprecatedAttribute()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <deprecated since="2016.1">
        /// <see cref="somefunction"/>
        /// </deprecated>
        [ObsoleteAttribute] // Noncompliant
        public void  IncompleteObsoleteAttributeFull()
        {

        }

        [Obsolete] // Noncompliant
        public void IncompleteObsoleteAttribute()
        {

        }

        [System.Obsolete] // Noncompliant
        public void IncompleteObsoleteAttribute()
        {

        }
        [MyObsolete] 
        public void IncompleteObsoleteAttribute()
        {

        }


    }
}
