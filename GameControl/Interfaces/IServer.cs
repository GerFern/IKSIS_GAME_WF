using System.Collections.Generic;
using System.Drawing;
using EmptyTest.Proxy;

namespace GameCore.Interfaces
{
    public interface IServer
    {
        [WaitResponse]
        PlayerState LogIn(string Name);
        [WaitResponse]
        bool Place(int prefabID, Point location, GameCore.Prefab.Rotate rotate);
        [WaitResponse]
        bool IsReady { get; set; }
        [WaitResponse]
        Color Color { get; set; }
        [WaitResponse]
        Game Game { get; }
        [WaitResponse]
        bool IsGameRunning { get; }
        /// <summary>
        /// Key - PlayerID
        /// </summary>
        [WaitResponse]
        Dictionary<int, PlayerState> Players { get; }
        void Message(string text);
        void GiveUp();
    }


}
