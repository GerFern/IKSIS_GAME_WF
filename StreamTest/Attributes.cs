using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NotifyTest
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public class ProxyOffline : Attribute
    {

    }

    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class ProxyOnServer : Attribute
    {

    }

    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class ProxyAttribute : Attribute
    {
        public ProxyAttribute()
        {
          
        }
    }

    //[System.AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    //public class LinkClassAttribute : ProxyAttribute
    //{
    //    public Type Type { get; }
    //    public LinkClassAttribute(Type type)
    //    {
    //        Type = type;
    //    }
    //}

    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class LinkInterfaceAttribute : ProxyAttribute
    {
        public Type IType { get; }
        public LinkInterfaceAttribute(Type type)
        {
            IType = type;
        }
    }


    public static class Extensions
    {
        public static Type GetSingledInterface(this Type type)
        {

            var i = type.GetInterfaces();
            if (i.Length == 1) return i[0];
            else if (i.Length > 1)
            {
                var t = type.GetCustomAttribute<LinkInterfaceAttribute>()?.IType;
                if (t != null && i.Contains(t))
                    return t;
            }
            return null;
        }

        public static bool IsListInterface(this Type itype)
        {
            if (itype == null) return false;
            if (itype == typeof(IList)||itype == typeof(IList<>)) return true;
            if (!itype.IsGenericType) return false;
            if (itype.GetGenericTypeDefinition() == typeof(IList<>)) return true;
            return false;
        }

        public static Type GetListInterface(this Type type)
        {
            if (type == null) return null;
            if (type == typeof(IList) || type == typeof(IList<>)) return type;
            if (type.IsGenericType) 
            if (type.GetGenericTypeDefinition() == typeof(IList<>)) return type;
            foreach (var item in type.GetInterfaces())
            {
                Type t = item.GetListInterface();
                if (t != null) return t;
            }
            return null;
        }

        //public static Type GetLinkedType(this Type type) =>
        //    type.GetCustomAttribute<LinkClassAttribute>()?.Type;

        //public static bool IsProxyType(this Type type) =>
        //    type.GetCustomAttribute<ProxyAttribute>() != null;
    }
}
