using System;
using System.Collections.Generic;
using System.Text;

namespace EmptyTest.TCommand
{
    public class CommandRecieveObject : Command
    {
        public CommandRecieveObject(BinaryReaderE reader) : base(reader)
        {
        }

        public override CommandType CmdType => CommandType.RecieveObject;

        protected override void InvokeReadWrite(IReaderWriterAlliaces alliaces)
        {
            int count = default;
            if (alliaces.IsWrite)
            {
                count = NodeValues.Count;
                alliaces.InvokeInt7(ref count);
                foreach (var item in NodeValues)
                {
                    int key = item.Key;
                    object value = item.Value;
                    alliaces.InvokeInt7(ref key);
                    alliaces.InvokeSimpleObject(ref value);
                }
            }
            else
            {
                NodeValues = new Dictionary<int, object>();
                alliaces.InvokeInt7(ref count);
                for (int i = 0; i < count; i++)
                {
                    int key = default;
                    object value = null;
                    alliaces.InvokeInt7(ref key);
                    alliaces.InvokeSimpleObject(ref value);
                    NodeValues.Add(key, value);
                }
            }
        }
    }
}
