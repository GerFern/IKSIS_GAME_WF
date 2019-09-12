using System;
using System.Collections.Generic;
using System.Text;

namespace EmptyTest.Proxy
{
    [System.AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class WaitResponseAttribute : Attribute
    {
        public WaitResponseAttribute() { }
    }
}
