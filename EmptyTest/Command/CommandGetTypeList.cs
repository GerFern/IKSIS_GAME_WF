using System;
using System.Collections.Generic;
using System.Text;

namespace EmptyTest.TCommand
{
    public class CommandGetTypeListRequest : Command
    {
        public CommandGetTypeListRequest(BinaryReaderE reader) : base(reader)
        {

        }

        public override CommandType CmdType => CommandType.GetTypeListRequest;

        protected override void InvokeReadWrite(IReaderWriterAlliaces alliaces)
        {

        }
    }
}
