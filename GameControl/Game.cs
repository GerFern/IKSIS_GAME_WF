#define test

using System.Collections.Generic;
using System.Drawing;

namespace GameCore
{
    public class Game
    {
        Dictionary<int, List<Point>> allowPointsForPlayer = new Dictionary<int, List<Point>>();
        public PrefabCollection Prefabs { get; } = new PrefabCollection();
        public PlayerDicrionary Players { get; } = new PlayerDicrionary();
        /// <summary>
        /// Текущий игрок с правом хода
        /// </summary>
        public int CurrentPlayer
        {
            get;
#if !test
            private
#endif
            set;
        }
        public int PlayersCount;
        /// <summary>
        /// Игровое поле
        /// </summary>
        public Cell[,] Cells { get; private set; }
#if test
        public Cell this[Point point]
        {
            get => Cells[point.X, point.Y];
            set => Cells[point.X, point.Y] = value;
        }
        public Cell this[int x, int y]
        {
            get => Cells[x, y];
            set => Cells[x, y] = value;
        }
#else

        public Cell this[Point point] => Cells[point.X, point.Y];
        public Cell this[int x, int y] => Cells[x, y];
#endif
        public int Row, Columns;
        public void Init(int columns, int rows)
        {
            Cells = new Cell[columns, rows];
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    Cells[i, j] = new Cell(this, new Point(i, j));
                }
            }
        }

        /// <summary>
        /// Установить фигуру на поле
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="offset"></param>
        /// <param name="places"></param>
        /// <param name="player"></param>
        /// <param name="validate"></param>
        /// <returns></returns>
        public bool SetPrefab(Prefab prefab, Point offset, out PointWithBorder[] places, int player = 0, bool validate = true)
        {
            places = prefab.GetPointsCursors(offset); // Получить координаты всех ячеек фигуры
            List<Point> allow; // Точки, где разрешено размещение
            List<Point> remoweAllow = new List<Point>(); // Точки, где запрещено размещение
            if (validate)
            {
                player = CurrentPlayer;
                bool succes1 = true;
                bool succes2 = true;
                foreach (var item in places) // Для всех координат проверка
                {
                    if (this[item.Point].PlayerOwner != 0) // Если координата занята другой фигурой
                    {
                        succes1 = false;
                        break;
                    }
                    try
                    {
                        if (this[item.Point.Up()].PlayerOwner == player)
                        {
                            succes1 = false;
                            break;
                        }
                    }
                    catch { }
                    try
                    {
                        if (this[item.Point.Down()].PlayerOwner == player)
                        {
                            succes1 = false;
                            break;
                        }
                    }
                    catch { }
                    try
                    {
                        if (this[item.Point.Left()].PlayerOwner == player)
                        {
                            succes1 = false;
                            break;
                        }
                    }
                    catch { }
                    try
                    {
                        if (this[item.Point.Right()].PlayerOwner == player)
                        {
                            succes1 = false;
                            break;
                        }
                    }
                    catch { }
                }
                if (succes1)
                {
                    if (!allowPointsForPlayer.ContainsKey(player))
                    {
                        allow = new List<Point>();
                        allowPointsForPlayer.Add(player, allow);
                    }
                    else
                    {
                        succes2 = false;
                        allow = allowPointsForPlayer[player];
                        foreach (var item in places)
                        {
                            if (allow.Contains(item.Point))
                            {
                                succes2 = true;
                                remoweAllow.Add(item.Point);
                                //break;
                            }
                        }
                    }


                    if (succes2)
                    {
                        foreach (var item in places)
                        {
                            //Point t = item.Up();
                            //if(!places.Contains(t)&&this[item].Owner!=currentPlayer)
                            //{
                            //    Point t2 = item.Left();
                            //    if(!places.Contains(t)&&this[item].Owner!=currentPlayer)
                            //}
                            Cell cell = this[item.Point];
                            cell.PlayerOwner = player;
                            cell.Border = item.Border;
                        }
                        foreach (var item in prefab.AnglePoints)
                        {
                            Point fixPoint = new Point(item.X + offset.X, item.Y + offset.Y);
                            if (!allow.Contains(fixPoint)) allow.Add(fixPoint);
                        }
                        if (validate)
                            CurrentPlayer = (CurrentPlayer % PlayersCount) + 1;
                        //foreach (var item in remoweAllow)
                        //{
                        //    allow.Remove(item);
                        //}
                        //UpdateCellView();
                        //if (Game.СurrentPlayer == 1) Game.СurrentPlayer = 2;
                        //else if (Game.СurrentPlayer == 2) Game.СurrentPlayer = 1;
                    }
                }
                return succes2;
            }
            else
            {
                if (!allowPointsForPlayer.ContainsKey(player))
                {
                    allow = new List<Point>();
                    allowPointsForPlayer.Add(player, allow);
                }
                else
                {
                    allow = allowPointsForPlayer[player];
                    foreach (var item in places)
                        if (allow.Contains(item.Point))
                            remoweAllow.Add(item.Point);
                }
                foreach (var item in places)
                {
                    Cell cell = this[item.Point];
                    cell.PlayerOwner = player;
                    cell.Border = item.Border;
                }
                foreach (var item in prefab.AnglePoints)
                {
                    Point fixPoint = new Point(item.X + offset.X, item.Y + offset.Y);
                    if (!allow.Contains(fixPoint)) allow.Add(fixPoint);
                }
                return true;
            }
        }
        public Game(Player[] players, PrefabCollection prefabs)
        {
            Players.Add(0, new EmptyCell(SystemColors.Control, Color.AliceBlue));
            //foreach (var item in players) { Players.Add(item.Key, item.Value); }
            for (int i = 0; i < players.Length; i++)
            {
                Players.Add(i+1, players[i]);
            }
            foreach (var item in prefabs) { Prefabs.Add(item); }
            PlayersCount = players.Length;
        }
        //public Game()
        //{
        //    Players.Add(0, new EmptyCell(SystemColors.Control, Color.AliceBlue));
        //    Players.Add(1, new PlayerCell(Color.Blue, Color.DeepSkyBlue, Color.CadetBlue, Color.AliceBlue));
        //    Players.Add(2, new PlayerCell(Color.Red, Color.IndianRed, Color.DarkRed, Color.AliceBlue));
        //    Prefabs.Add(new Point(0, 0));
        //    Prefabs.Add(new Point(0, 0), new Point(0, 1));
        //    Prefabs.Add(new Point(0, 0), new Point(0, 1), new Point(1, 0));
        //    Prefabs.Add(new Point(0, 0), new Point(0, 1), new Point(1, 0), new Point(1, 1));
        //    Prefabs.Add(new Point(0, 0), new Point(0, 1), new Point(0, 2));
        //    Prefabs.Add(new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(1, 0));
        //    Prefabs.Add(new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(1, 1));
        //    Prefabs.Add(new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(0, 3));
        //    Prefabs.Add(new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(1, 0), new Point(2, 0));
        //    Prefabs.Add(new Point(0, 0), new Point(0, 2), new Point(2, 0), new Point(2, 2));
        //    Prefabs.Add(new Point(0, 0), new Point(0, 2), new Point(2, 0), new Point(2, 2), new Point(2, 1));
        //}
        //public bool AddPrefab()
        public class PrefabCollection : List<Prefab>
        {
            public void Add(params Point[] points) => this.Add(new Prefab(points));
        }
        public class PlayerDicrionary : Dictionary<int, Player>
        {

        }
    }
}

