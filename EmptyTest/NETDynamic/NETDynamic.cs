using EmptyTest.TExtensions;
using EmptyTest.TStreamHandler;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace EmptyTest.TNode
{
    public class NETDynamic : DynamicNode
    {
        public NETDynamic(DynamicNode parent, Type type) : base(parent, type)
        {
            __TypeCode = type.GetTypeCodeExtande();
        }


        public override TypeCodeExtande __TypeCode { get; }
        public override bool __IsCollection => false;

        public override bool __HasChild => false;
        //public override int __StructID => -1;
        //public bool __ReadOnly { get; }
        //Dictionary<string, NETDynamic> __Members = new Dictionary<string, NETDynamic>();
        public DictCounter<string> __MemberNames => __StreamWRHandler.__MemberNames;

        object __this;
        //Class not support! Only unmannaged types, string, arrays and structes without classes 

        internal override void UpdateValue(object value)
        {
            if (value.GetType().IsSimple())
            {
                __this = value;
                this.WriteNode();
            }
            else throw new Exception("AAAAAAAAAAAAAAAAAAAAAAAAAA");
        }

        internal override void SetValue(object value)
        {
            if (value.GetType().IsSimple())
                __this = value;
            else throw new Exception("AAAAAAAAAAAAAAAAAAAAAAAAAA");
        }

        internal override void SendNodeRecursive(BinaryWriterE writer, bool start)
        {
            //if(start)
                this.WriteNode(writer);
        }
     
        public void WriteNode(BinaryWriterE writer = null)
        {

            if (writer == null)
                __StreamWRHandler.SendObject(this);
            else
            {
                writer.Write7(__UID);
                writer.WriteObject(__this);
            }
        }

        public object __This
        {
            get => __this;
            //protected set
            //{
            //    __this = value;
            //    TypeThis = __this.GetType();
            //}
        }
        //public Type TypeThis
        //{
        //    get => typeThis;
        //    protected set
        //    {
        //        typeThis = value;
        //    }
        //}
        protected MethodInfo __get_index;
        protected MethodInfo __set_index;


        public List<int> __Parents { get; protected set; }
        public List<int> __Childs { get; protected set; }
        public override bool __Inited { get; protected set; }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return base.GetDynamicMemberNames();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return base.GetMetaObject(parameter);
        }

        public override string ToString()
        {
            return __this.ToString();
            //return base.ToString();
        }

        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            return base.TryBinaryOperation(binder, arg, out result);
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.Type != typeof(NETDynamic))
            {
                if (__Type == binder.Type)
                {
                    result = __This;
                }
                else
                {
                    result = Convert.ChangeType(__This, binder.Type);
                    return true;
                }
            }
            else result = this;
            return true;
            //return base.TryConvert(binder, out result);
        }

        public override bool TryCreateInstance(CreateInstanceBinder binder, object[] args, out object result)
        {
            return base.TryCreateInstance(binder, args, out result);
        }

        public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
        {
            return base.TryDeleteIndex(binder, indexes);
        }

        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            return base.TryDeleteMember(binder);
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = null;
            return false;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            return false;
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            return base.TryInvoke(binder, args, out result);
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;
            return false;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
           return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return false;
        }

        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            return base.TryUnaryOperation(binder, out result);
        }

     

     

        public override void GetRecourciveUID(List<int> list)
        {
            list.Add(this.__UID);
        }

        public override void GetRecourciveSimpleNode(List<NETDynamic> list)
        {
            list.Add(this);
        }

        //public bool __Inited => __this != null;

        //public bool __IsClientStorage { get; }
    }
}
