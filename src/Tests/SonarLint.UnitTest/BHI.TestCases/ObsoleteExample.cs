using System;
using System.Collections.Generic;
using Windows.Foundation.Metadata;

namespace Tests.Diagnostics
{

    // Name should be <rule> followed by Example
    class StringLocalizationExample

    {
        [Obsolete] // Noncompliant
        public void  IncompleteObsoleteAttribute()
        {

        }

        [Obsolete] // Noncompliant
        public void IncompleteObsoleteAttribute()
        {

        }

        [Deprecated]
        public void DeprecatedAttribute()
        {

        }

    }
}
