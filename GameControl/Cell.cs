#define test

using System;
using System.Drawing;

namespace GameCore
{
    public class Cell
    {
        public Game Game { get; }
        public Point Point { get; }
        int owner;
        Border border;

        public int PlayerOwner
        {
            get => owner;
            set
            {
                owner = value;
            }
        }

        public Border Border
        {
            get => border;
            set
            {
                border = value;
            }
        }
        public Player Player => Game.Players[owner];

       

        internal Cell(Game game, Point point)
        {
            Game = game ?? throw new ArgumentNullException(nameof(game));
            Point = point;
            owner = 0;
            border = Border.None;
        }
    }
}

