using System;
using System.Collections.Generic;
using System.Text;

namespace EmptyTest.TCommand
{
    public class CommandWelcomeClient : Command
    {
        public CommandWelcomeClient(BinaryReaderE reader) : base(reader)
        {
        }

        public override CommandType CmdType => CommandType.WelcomeClient;

        protected override void InvokeReadWrite(IReaderWriterAlliaces alliaces)
        {
            throw new NotImplementedException();
        }
    }
}
