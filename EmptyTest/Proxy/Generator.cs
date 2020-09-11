//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Reflection.Emit;
//using System.Text;

//namespace EmptyTest.Proxy
//{

//    static class Generator
//    {
//        static readonly ProxyAssembly assembly = new ProxyAssembly();
//        public static Type CreateProxyType(string name, Type baseType, Type interfaceType)
//        {
//            var l = interfaceType.GetTypeInfo().ImplementedInterfaces;
//            foreach (var item in l)
//            {

//            }
//        }
//    }

//    class ProxyAssembly
//    {
//        internal AssemblyBuilder assemblyBuilder;
//        internal ModuleBuilder moduleBuilder;

//        public ProxyAssembly()
//        {
//            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly
//                (new AssemblyName("Proxy.Gen, Version=1.0.0.0"), AssemblyBuilderAccess.Run);
//            moduleBuilder = assemblyBuilder.DefineDynamicModule("module");
//        }
//    }

//    class ProxyBuilder
//    {

//        Type baseType, interfaceType;
//        TypeBuilder typeBuilder;
//        public ProxyBuilder(ProxyAssembly assembly, Type baseType, Type interfaceType)
//        {
//            this.baseType = baseType;
//            this.interfaceType = interfaceType;
//            typeBuilder = assembly.moduleBuilder.DefineType("", TypeAttributes.Public, baseType);
//            var l = interfaceType.GetTypeInfo().ImplementedInterfaces;
//            foreach (var item in l)
//            {
//                InitProperties(item);
//            }
//        }

//        void InitProperties(Type iface)
//        {
//            // Инициализация свойств
//            foreach (PropertyInfo runtimeProperty in iface.GetRuntimeProperties())
//            {
//                PropertyBuilder pb = typeBuilder.DefineProperty(
//                    runtimeProperty.Name,
//                    runtimeProperty.Attributes,
//                    runtimeProperty.PropertyType, 
//                    runtimeProperty.GetIndexParameters().Select(a => a.ParameterType).ToArray());

//                MethodInfo getMethod = runtimeProperty.GetGetMethod();
//                MethodInfo setMethod = runtimeProperty.GetSetMethod();

//                if(getMethod!=null)
//                {
//                    pb.SetGetMethod(AddMethodInfo(getMethod));
//                }

//                if (setMethod != null)
//                {
//                    pb.SetSetMethod(AddMethodInfo(setMethod));
//                }
//            }
//        }

//        MethodBuilder AddMethodInfo(MethodInfo mi)
//        {
//            ParameterInfo[] parameters = mi.GetParameters();
//            Type[] array = parameters.Select(a => a.ParameterType).ToArray();
//            MethodBuilder methodBuilder = typeBuilder.DefineMethod(mi.Name, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual, mi.ReturnType, array);
//            if (mi.ContainsGenericParameters)
//            {
//                Type[] genericArguments = mi.GetGenericArguments();
//                string[] array2 = new string[genericArguments.Length];
//                for (int i = 0; i < genericArguments.Length; i++)
//                {
//                    array2[i] = genericArguments[i].Name;
//                }
//                GenericTypeParameterBuilder[] array3 = methodBuilder.DefineGenericParameters(array2);
//                for (int j = 0; j < array3.Length; j++)
//                {
//                    array3[j].SetGenericParameterAttributes(genericArguments[j].GetTypeInfo().GenericParameterAttributes);
//                }
//            }
//            ILGenerator iLGenerator = methodBuilder.GetILGenerator();
//            ParametersArray parametersArray = new ParametersArray(iLGenerator, array);
//            iLGenerator.Emit(OpCodes.Nop)
//        }
//    }



//    class GenericArray<T>
//    {
//        private ILGenerator _il;

//        private LocalBuilder _lb;

//        internal GenericArray(ILGenerator il, int len)
//        {
//            _il = il;
//            _lb = il.DeclareLocal(typeof(T[]));
//            il.Emit(OpCodes.Ldc_I4, len);
//            il.Emit(OpCodes.Newarr, typeof(T));
//            il.Emit(OpCodes.Stloc, _lb);
//        }

//        internal void Load()
//        {
//            _il.Emit(OpCodes.Ldloc, _lb);
//        }

//        internal void Get(int i)
//        {
//            _il.Emit(OpCodes.Ldloc, _lb);
//            _il.Emit(OpCodes.Ldc_I4, i);
//            _il.Emit(OpCodes.Ldelem_Ref);
//        }

//        internal void BeginSet(int i)
//        {
//            _il.Emit(OpCodes.Ldloc, _lb);
//            _il.Emit(OpCodes.Ldc_I4, i);
//        }

//        internal void EndSet(Type stackType)
//        {
//            Convert(_il, stackType, typeof(T), isAddress: false);
//            _il.Emit(OpCodes.Stelem_Ref);
//        }


//        private static void Convert(ILGenerator il, Type source, Type target, bool isAddress)
//        {
//            if (target == source)
//            {
//                return;
//            }
//            TypeInfo typeInfo = source.GetTypeInfo();
//            TypeInfo typeInfo2 = target.GetTypeInfo();
//            if (source.IsByRef)
//            {
//                Type elementType = source.GetElementType();
//                Ldind(il, elementType);
//                Convert(il, elementType, target, isAddress);
//            }
//            else if (typeInfo2.IsValueType)
//            {
//                if (typeInfo.IsValueType)
//                {
//                    OpCode opcode = s_convOpCodes[GetTypeCode(target)];
//                    il.Emit(opcode);
//                    return;
//                }
//                il.Emit(OpCodes.Unbox, target);
//                if (!isAddress)
//                {
//                    Ldind(il, target);
//                }
//            }
//            else if (typeInfo2.IsAssignableFrom(typeInfo))
//            {
//                if (typeInfo.IsValueType || source.IsGenericParameter)
//                {
//                    if (isAddress)
//                    {
//                        Ldind(il, source);
//                    }
//                    il.Emit(OpCodes.Box, source);
//                }
//            }
//            else if (target.IsGenericParameter)
//            {
//                il.Emit(OpCodes.Unbox_Any, target);
//            }
//            else
//            {
//                il.Emit(OpCodes.Castclass, target);
//            }
//        }


//        private static void Ldind(ILGenerator il, Type type)
//        {
//            OpCode opcode = s_ldindOpCodes[GetTypeCode(type)];
//            if (!opcode.Equals(OpCodes.Nop))
//            {
//                il.Emit(opcode);
//            }
//            else
//            {
//                il.Emit(OpCodes.Ldobj, type);
//            }
//        }


//        private static int GetTypeCode(Type type)
//        {
//            if (type == null)
//            {
//                return 0;
//            }
//            if (type == typeof(bool))
//            {
//                return 3;
//            }
//            if (type == typeof(char))
//            {
//                return 4;
//            }
//            if (type == typeof(sbyte))
//            {
//                return 5;
//            }
//            if (type == typeof(byte))
//            {
//                return 6;
//            }
//            if (type == typeof(short))
//            {
//                return 7;
//            }
//            if (type == typeof(ushort))
//            {
//                return 8;
//            }
//            if (type == typeof(int))
//            {
//                return 9;
//            }
//            if (type == typeof(uint))
//            {
//                return 10;
//            }
//            if (type == typeof(long))
//            {
//                return 11;
//            }
//            if (type == typeof(ulong))
//            {
//                return 12;
//            }
//            if (type == typeof(float))
//            {
//                return 13;
//            }
//            if (type == typeof(double))
//            {
//                return 14;
//            }
//            if (type == typeof(decimal))
//            {
//                return 15;
//            }
//            if (type == typeof(DateTime))
//            {
//                return 16;
//            }
//            if (type == typeof(string))
//            {
//                return 18;
//            }
//            if (type.GetTypeInfo().IsEnum)
//            {
//                return GetTypeCode(Enum.GetUnderlyingType(type));
//            }
//            return 1;
//        }

//        private static OpCode[] s_convOpCodes = new OpCode[19]
//        {
//            OpCodes.Nop,
//            OpCodes.Nop,
//            OpCodes.Nop,
//            OpCodes.Conv_I1,
//            OpCodes.Conv_I2,
//            OpCodes.Conv_I1,
//            OpCodes.Conv_U1,
//            OpCodes.Conv_I2,
//            OpCodes.Conv_U2,
//            OpCodes.Conv_I4,
//            OpCodes.Conv_U4,
//            OpCodes.Conv_I8,
//            OpCodes.Conv_U8,
//            OpCodes.Conv_R4,
//            OpCodes.Conv_R8,
//            OpCodes.Nop,
//            OpCodes.Nop,
//            OpCodes.Nop,
//            OpCodes.Nop
//        };



//        private static OpCode[] s_ldindOpCodes = new OpCode[19]
//        {
//            OpCodes.Nop,
//            OpCodes.Nop,
//            OpCodes.Nop,
//            OpCodes.Ldind_I1,
//            OpCodes.Ldind_I2,
//            OpCodes.Ldind_I1,
//            OpCodes.Ldind_U1,
//            OpCodes.Ldind_I2,
//            OpCodes.Ldind_U2,
//            OpCodes.Ldind_I4,
//            OpCodes.Ldind_U4,
//            OpCodes.Ldind_I8,
//            OpCodes.Ldind_I8,
//            OpCodes.Ldind_R4,
//            OpCodes.Ldind_R8,
//            OpCodes.Nop,
//            OpCodes.Nop,
//            OpCodes.Nop,
//            OpCodes.Ldind_Ref
//        };

//    }


//    class ParametersArray
//    {
//        private ILGenerator _il;

//        private Type[] _paramTypes;

//        internal ParametersArray(ILGenerator il, Type[] paramTypes)
//        {
//            _il = il;
//            _paramTypes = paramTypes;
//        }

//        internal void Get(int i)
//        {
//            _il.Emit(OpCodes.Ldarg, i + 1);
//        }

//        internal void BeginSet(int i)
//        {
//            _il.Emit(OpCodes.Ldarg, i + 1);
//        }

//        internal void EndSet(int i, Type stackType)
//        {
//            Type elementType = _paramTypes[i].GetElementType();
//            Convert(_il, stackType, elementType, isAddress: false);
//            Stind(_il, elementType);
//        }

//        private void Stind(ILGenerator il, Type elementType)
//        {
//            throw new NotImplementedException();
//        }

//        private void Convert(ILGenerator il, Type stackType, Type elementType, bool isAddress)
//        {
//            throw new NotImplementedException();
//        }
//    }


//}
