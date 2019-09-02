using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Interfaces
{
    public struct ServerContainer
    {
        public Game game;
        public List<PlayerState> players; 
    }
}
