using System;
using System.Collections.Generic;
using System.Text;

namespace EmptyTest.TCommand
{
    public class CommandLogIn : Command
    {
        string _name;
        Guid _guid;

        public CommandLogIn(BinaryReaderE reader) : base(reader)
        {
        }

        public string Name => _name;
        public Guid Guid => _guid;

        public override CommandType CmdType => CommandType.LogIn;

        protected override void InvokeReadWrite(IReaderWriterAlliaces alliaces)
        {
            int v = valid;
            alliaces.InvokeInt32(ref v);
            if (v != valid) throw new Exception("AAAA");
            alliaces.InvokeGuid(ref _guid);
            alliaces.InvokeString(ref _name);
        }

        static readonly int valid = 725999372;
    }
}
