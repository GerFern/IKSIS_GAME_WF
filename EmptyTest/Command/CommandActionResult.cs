using EmptyTest.TStreamHandler;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmptyTest.TCommand
{
    public class CommandActionResult : Command
    {
        int _idAction;
        TypeCodeExtande _typeCode;
        object _result;

        public CommandActionResult(BinaryReaderE reader) : base(reader)
        {
        }

        public int IdAction => _idAction;
        public TypeCodeExtande TypeCode => _typeCode;
        public object Result => _result;

        public override CommandType CmdType => CommandType.ActionResult;

        protected override void InvokeReadWrite(IReaderWriterAlliaces alliaces)
        {
            throw new NotImplementedException();
        }
    }
}
