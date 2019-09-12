using EmptyTest.TExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmptyTest.TCommand
{
    public class CommandSendTypeList : Command
    {
        public Dictionary<int, Type> DictTypes => _dictTypes;

        public override CommandType CmdType => CommandType.SendTypeList;

        private Dictionary<int, Type> _dictTypes;

        public CommandSendTypeList(BinaryReaderE reader) : base(reader)
        {
        }

        protected override void InvokeReadWrite(IReaderWriterAlliaces alliaces)
        {
            int length = default;
            if (alliaces.IsWrite) length = _dictTypes.Count;
            alliaces.InvokeInt7(ref length);
            if (alliaces.IsWrite)
            {
                _dictTypes.ForEachGet((int key, Type value) =>
                {
                    alliaces.InvokeInt7(ref key);
                    string name = value.FullName;
                    alliaces.InvokeString(ref name);
                });
            }
            else
            {
                _dictTypes = new Dictionary<int, Type>();
                for (int i = 0; i < length; i++)
                {
                    int key = default;
                    alliaces.InvokeInt7(ref key);
                    string name = default;
                    alliaces.InvokeString(ref name);
                    _dictTypes[key] = Type.GetType(name);
                }
            }
        }
    }
}
