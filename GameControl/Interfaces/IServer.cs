using System.Collections.Generic;
using System.Drawing;
using EmptyTest.Proxy;

namespace GameCore.Interfaces
{
    public interface IServer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns>PrivateID</returns>
        [WaitResponse]
        PlayerState LogIn(string Name);

        //void SetGameSize(Size size);
        //void SetPrefabLimits(PrefabLimit[] prefabLimits);
        //void SetStandartPrefabLimit();
        //void AddPrefabLimit(PrefabLimit prefabLimit);
        //void UpdatePrefabLimit(int prefabID, int countLimit);

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
