using System;

namespace GameCore
{
    [Flags]
    public enum Border : byte
    {
        None = 0x00,
        Top = 0x01,
        Left = 0x02,
        Rigth = 0x04,
        Down = 0x08,
        All = Top | Left | Rigth | Down
    }

}