using EmptyTest.TStreamHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace EmptyTest.TExtensions
{
   

    public static class Extensions
    {
        public static void ForEachSet(this IDictionary dictionary, Func<object, object>func)
        {
            foreach (var item in dictionary.Keys)
            {
                dictionary[item] = func.Invoke(item);
            }
        }

        public static void ForEachSet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<TKey, TValue> func)
        {
            foreach (var item in dictionary.Keys)
            {
                dictionary[item] = func.Invoke(item);
            }
        }


        public static void ForEachGet(this IDictionary dictionary, Action<object, object> func)
        {
            foreach (var item in dictionary.Keys)
            {
                func.Invoke(item, dictionary[item]);
            }
        }

        public static void ForEachGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Action<TKey, TValue> func)
        {
            foreach (var item in dictionary)
            {
                func.Invoke(item.Key, item.Value);
            }
        }

        public static void ForEachSet(this int[] vs, Func<int> func)
        {
            for (int i = 0; i < vs.Length; i++)
            {
                vs[i] = func.Invoke();
            }
        }
        public static void ForEachSet(this int[] vs, Func<int, int> func)
        {
            for (int i = 0; i < vs.Length; i++)
            {
                vs[i] = func.Invoke(i);
            }
        }
        public static bool IsSimple(this Type type)
        {
            return type.GetTypeCodeExtande().IsSimple();
        }

        public static bool IsSimple(this TypeCodeExtande typeCode)
        {
            int code = (int)typeCode;
            if (code >= 3 && code <= 18) return true;
            else return false;
        }

        public static void SetForEach<T>(this Array array, Func<int[], T> func)
        {
            int rank = array.Rank;
            int[] lb = new int[rank];
            int[] ub = new int[rank];
            int[] indices = new int[rank];
            for (int i = 0; i < rank; i++)
            {
                int lowerBound = array.GetLowerBound(i);
                int upperBound = array.GetUpperBound(i);
                indices[i] = lb[i] = lowerBound;
                ub[i] = upperBound;
            }
            int crank = 0;
            while(crank < rank)
            {
                array.SetValue(func.Invoke(indices), indices);
                int v = ++indices[crank];
                if(v==ub[crank])
                {
                    ++crank;
                    if (crank == rank) break;
                    for (int i = crank-1; i >=0; i--)
                    {
                        indices[i] = lb[i];
                    }
                    ++indices[crank];
                }
            }
        }

        public static void SetForEach<T>(this Array array, Func<T> func)
        {
            int rank = array.Rank;
            int[] lb = new int[rank];
            int[] ub = new int[rank];
            int[] indices = new int[rank];
            for (int i = 0; i < rank; i++)
            {
                int lowerBound = array.GetLowerBound(i);
                int upperBound = array.GetUpperBound(i);
                indices[i] = lb[i] = lowerBound;
                ub[i] = upperBound;
            }
            int crank = 0;
            while (crank < rank)
            {
                array.SetValue(func.Invoke(), indices);
                int v = ++indices[crank];
                if (v == ub[crank])
                {
                    ++crank;
                    if (crank == rank) break;
                    for (int i = crank - 1; i >= 0; i--)
                    {
                        indices[i] = lb[i];
                    }
                    ++indices[crank];
                }
            }
        }
        public static TypeCodeExtande GetTypeCodeExtande(this Type type)
        {
            TypeCode typeCode = Type.GetTypeCode(type);
            if (typeCode != TypeCode.Object) return (TypeCodeExtande)typeCode;
            if (type.IsArray) return TypeCodeExtande.Array;
            if (type.IsList()) return TypeCodeExtande.List;
            if (type.IsDictionary()) return TypeCodeExtande.Dictionary;
            else return TypeCodeExtande.Structure;
            return TypeCodeExtande.Error;
        }

        public static object SetAllNullStringEmpty(this object value)
        {
            Type type = value.GetType();
            Stack<Type> types = new Stack<Type>();
            foreach (var item in type.GetProperties())
            {
                object v = item.GetValue(value);
                if (item.PropertyType == typeof(string))
                {
                    if (v == null) item.SetValue(value, string.Empty);
                }
                else
                {
                    if (v != null && item.PropertyType.GetTypeCodeExtande() == TypeCodeExtande.Structure) item.SetValue(value, SetAllNullStringEmpty(v));
                }
            }
            foreach (var item in type.GetFields())
            {
                object v = item.GetValue(value);
                if (item.FieldType == typeof(string))
                {
                    if (v == null) item.SetValue(value, string.Empty);
                }
                else
                {
                    if (v != null && item.FieldType.GetTypeCodeExtande() == TypeCodeExtande.Structure) item.SetValue(value, SetAllNullStringEmpty(v));
                }
            }
            return value;
        }

        //public static Type GetTypeFromCodeExtande(this TypeCodeExtande type)
        //{
        //    switch (type)
        //    {
        //        case TypeCodeExtande.Array:
        //            break;
        //        case TypeCodeExtande.List:
        //            break;
        //        case TypeCodeExtande.Dictionary:
        //            break;
        //        case TypeCodeExtande.UntypedArray:
        //            break;
        //        case TypeCodeExtande.UntypedList:
        //            break;
        //        case TypeCodeExtande.UntypedDictionary:
        //            break;
        //        case TypeCodeExtande.Error:
        //            break;
        //        default:
        //            return Type.GetType("System." + type);
        //    }
        //}

        public static bool IsList(this Type type)
        {
            return false;
        }

        public static bool IsDictionary(this Type type)
        {
            return false;
        }
    }
}
