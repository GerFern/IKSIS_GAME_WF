using System;
using System.Collections.Generic;
using System.Text;

namespace EmptyTest.TCommand
{
    public class CommandGetObjectRequest : Command
    {
        private int _UIDNode;

        public CommandGetObjectRequest(BinaryReaderE reader) : base(reader)
        {
        }

        public int UIDNode => _UIDNode;

        public override CommandType CmdType => CommandType.GetObjectRequest;

        protected override void InvokeReadWrite(IReaderWriterAlliaces alliaces)
        {
            alliaces.InvokeInt7(ref _UIDNode);
        }
    }
}
