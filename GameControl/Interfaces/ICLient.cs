using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace GameCore.Interfaces
{
    public interface IClient
    {
        void PlayerReadyChange(bool isReady, int playerID);
        void PlayerColorChange(Color color, int playerID);
        void NewPlayer(PlayerState playerState);
        void PlayerExit(int playerID);
        void Place(int playerID, int prefabID, Point location, GameCore.Prefab.Rotate rotate);
        void ChangePlayer(int playerID);
        void GameStart(Size gameSize, IDictionary<int, PrefabLimit> prefabs , IDictionary<int, (int index, Color color)> players);
        void Message(int idPlayer, string text);
        void ReadyTimer(int time);
        void ServerMessage(string text);
        void GiveUpPlayer(int playerIndex);
        //void GameSizeChange(Size size);
        ////void AddPrefabLimit(int id, PrefabLimit prefabLimit);
        //void UpdatePrefabLimit(int prefabID, PrefabLimit prefabLimit);
        //void SetPrefabLimits(PrefabLimit[] prefabLimits);
    }


}
