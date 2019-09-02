using GameCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Structs
{
    public class ClientData
    {
        public DateTime enterTime;
        public bool logIn = false;
        public bool mainInLobby = false;
        public string name;
        public PlayerState playerState;
    }
}
