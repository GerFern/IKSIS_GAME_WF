using System;
using System.Collections.Generic;
using System.Text;

namespace EmptyTest.TCommand
{
    public class CommandDisconnect : Command
    {
        public CommandDisconnect(BinaryReaderE reader) : base(reader)
        {
        }

        public override CommandType CmdType => CommandType.Disconnect;

        protected override void InvokeReadWrite(IReaderWriterAlliaces alliaces)
        {
            //throw new NotImplementedException();
        }
    }
}
