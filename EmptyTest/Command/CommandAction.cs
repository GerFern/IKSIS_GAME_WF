using System;
using System.Collections.Generic;
using System.Text;

namespace EmptyTest.TCommand
{
    public class CommandAction : Command
    {
        public CommandAction(BinaryReaderE reader) : base(reader) { }

        public override CommandType CmdType => CommandType.Action;

        protected override void InvokeReadWrite(IReaderWriterAlliaces alliaces)
        {
            throw new NotImplementedException();
        }
    }
}
