#define test

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GameCore
{
    public partial class Game
    {
        #region TESTREG
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
        #endregion

        #region PUBLICMEMBERS
        #region public properties
        public Color BackgroundColor
        {
            get => backgroundColor;
            set
            {
                backgroundColor = value;
            }
        }
        /// <summary>
        /// Игровое поле
        /// </summary>
        public Cell[,] Cells { get; private set; }
        public PrefabCollection Prefabs { get; } = new PrefabCollection();
        public PlayerDicrionary Players { get; } = new PlayerDicrionary();

        public bool MultiplayerGame { get; private set; }
        public int PlayerIndex { get; private set; }

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
        /// <summary>
        /// Количество игроков
        /// </summary>
        public int PlayersCount { get; private set; }
        public int Rows { get; private set; }
        public int Columns { get; private set; }
        public bool IsStarted { get; private set; }
        public bool AllowSetPrefab => !MultiplayerGame || PlayerIndex == CurrentPlayer;
        #endregion // public properties
        #region public methods
        /// <summary>
        /// Инициализация поля
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        public void Start(int columns, int rows)
        {
            Cells = new Cell[columns, rows];
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    Cells[i, j] = new Cell(this, new Point(i, j));
                }
            }
            Rows = rows;
            Columns = columns;
            PlayersCount = Players.Count;
            //Players.InitPrefabs(((Dictionary<int,(int,Prefab)>)Prefabs).ToDictionary(a => a.Key, a => a.Value.Item1));
            Players.InitPrefabs(Prefabs.ToDictionary(a => a.Key, b => b.Value.Count));
            IsStarted = true;
            MultiplayerGame = false;
            PlayerIndex = -1;
            CurrentPlayer = 1;
            Started?.Invoke(this, EventArgs.Empty);
        }


        public void StartMultiplayer(Size gameSize, int playerIndex)
        {
            int columns = gameSize.Width, rows = gameSize.Height;
            Cells = new Cell[columns, rows];
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    Cells[i, j] = new Cell(this, new Point(i, j));
                }
            }
            Rows = rows;
            Columns = columns;
            PlayersCount = Players.Count;
            //Players.InitPrefabs(((Dictionary<int,(int,Prefab)>)Prefabs).ToDictionary(a => a.Key, a => a.Value.Item1));
            Players.InitPrefabs(Prefabs.ToDictionary(a => a.Key, b => b.Value.Count));
            IsStarted = true;
            MultiplayerGame = true;
            PlayerIndex = playerIndex;
            CurrentPlayer = 1;
            Started?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Установить фигуру на поле
        /// </summary>
        /// <param name="prefabID">ID размещаемой фигура</param>
        /// <param name="rotate">Поворот фигуры</param>
        /// <param name="offset">Смещение фигуры</param>
        /// <param name="places">Точки</param>
        /// <param name="player">Номер игрока</param>
        /// <param name="validate">Проверять правильность растановки</param>
        /// <param name="decrement">Уменьшать счетчик фигуры</param>
        /// <returns></returns>
        public bool SetPrefab(int prefabID, Prefab.Rotate rotate, Point offset, out PointWithBorder[] places, int player = 0, bool validate = true, bool decrement = true)
        {
            places = new PointWithBorder[0];
            int p = 0;
            if (decrement)
            {
                p = Players[player].PrefabCountAllow[prefabID];
                if (p == 0) return false;
                //if (p > 0) Players[player].PrefabCountAllow[prefabID] = p - 1;
            }
            if (SetPrefab(Prefabs[prefabID].Prefab.GetRotatedPrefab(rotate), offset, out places, player, validate))
            {
                if (decrement & p > 0) Players[player].PrefabCountAllow[prefabID] = p - 1;
                return true;
            }
            else return false;
        }

        public Game() { }
        //public Game(PlayerCellBase[] players, PrefabCollection prefabs)
        //{
        //    Players.Add(0, new EmptyCell());
        //    for (int i = 0; i < players.Length; i++) Players.Add(i + 1, players[i]);
        //    Prefabs = prefabs;
        //    foreach (KeyValuePair<int, Prefab> item in prefabs) { Prefabs.Add(item.Value, item.Key); }
        //    //PlayersCount = players.Length;
        //}
        #endregion // public methods
        #region public event
        public event EventHandler Started;
        //public event EventHandler Started;
        public event EventHandler<EventArgsPrefab> PrefabPlaced;
        public event EventHandler PlayerChanged;
        public event EventHandler GameOver;
        public event EventHandler BackgroungColorChanged;
        public event EventHandler<EventArgsPrefab> AddPrefabInList;
        #endregion
        #endregion // PUBLICMEMBERS

        #region PRIVATEMEMBERS
        Dictionary<int, List<Point>> allowPointsForPlayer = new Dictionary<int, List<Point>>();
        private Color backgroundColor;
        #region private methods
        /// <summary>
        /// Установить фигуру на поле
        /// </summary>
        /// <param name="prefab">Размещаемаяя фигура</param>
        /// <param name="offset">Смещение фигуры</param>
        /// <param name="places">Точки</param>
        /// <param name="player">Номер игрока</param>
        /// <param name="validate">Проверять правильность растановки</param>
        /// <returns></returns>
        bool SetPrefab(Prefab prefab, Point offset, out PointWithBorder[] places, int player = -1, bool validate = true, int prefabID = -1, Prefab.Rotate rotate = Prefab.Rotate.r0)
        {
            places = prefab.GetPointsCursors(offset); // Получить координаты всех ячеек фигуры
            List<Point> allow; // Точки, где разрешено размещение
            List<Point> remoweAllow = new List<Point>(); // Точки, где запрещено размещение
            if (validate)
            {
                Rectangle rectValidate = new Rectangle(Point.Empty, new Size(Columns, Rows));
                player = CurrentPlayer;
                bool succes1 = true;
                bool succes2 = true;
                foreach (var item in places) // Для всех координат проверка
                {
                    Point t = item.Point;
                    if (rectValidate.Contains(t))
                        if (this[t].PlayerOwner != 0) // Если координата занята другой фигурой
                        {
                            succes1 = false;
                            break;
                        }
                    t = item.Point.Up();
                    if (rectValidate.Contains(t))
                        if (this[t].PlayerOwner == player)
                        {
                            succes1 = false;
                            break;
                        }
                    t = item.Point.Down();
                    if (rectValidate.Contains(t))
                        if (this[t].PlayerOwner == player)
                        {
                            succes1 = false;
                            break;
                        }
                    t = item.Point.Left();
                    if (rectValidate.Contains(t))
                        if (this[t].PlayerOwner == player)
                        {
                            succes1 = false;
                            break;
                        }
                    t = item.Point.Right();
                    if (rectValidate.Contains(t))
                        if (this[t].PlayerOwner == player)
                        {
                            succes1 = false;
                            break;
                        }
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
                        PrefabPlaced?.Invoke(this, new EventArgsPrefab(places.Select(a => a.Point).ToArray(), prefabID, offset, rotate, player));
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
                    //cell.Border = item.Border;
                }
                foreach (var item in prefab.AnglePoints)
                {
                    Point fixPoint = new Point(item.X + offset.X, item.Y + offset.Y);
                    if (!allow.Contains(fixPoint)) allow.Add(fixPoint);
                }
                PrefabPlaced?.Invoke(this, new EventArgsPrefab(places.Select(a => a.Point).ToArray(), prefabID, offset, rotate, player));
                return true;
            }
        }
        #endregion // private methods
        #endregion // PRIVATEMEMBERS

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

        //public class PrefabCollection : Dictionary<int, Prefab>
        //{
        //    public void Add(params Point[] points)
        //    {
        //        int key = this.Keys.Count > 0 ? this.Keys.Max() + 1 : 0;
        //        this.Add(key, new Prefab(points));
        //    }
        //}

        public class PlayerDicrionary : Dictionary<int, Player>
        {
            internal void InitPrefabs(Dictionary<int, int> prefabsCountAllow)
            {
                foreach (var player in Values)
                {
                    player.PrefabCountAllow.Clear();
                    foreach (var prefab in prefabsCountAllow)
                    {
                        player.PrefabCountAllow.Add(prefab.Key, prefab.Value);
                    }
                }
            }

            public void Set(IDictionary<int, Player> values)
            {
                this.Clear();
                foreach (var item in values)
                {
                    this.Add(item.Key, item.Value);
                }
            }

            //public void Add(int key, PlayerCellBase playerCell)
            //{
            //    base.Add(key, new Player(playerCell));
            //}

            public void Add(int key, Color playerCell)
            {
                base.Add(key, new Player(playerCell));
            }
        }

        public class EventArgsPrefab : EventArgs
        {
            public EventArgsPrefab(int prefabID)
            {
                PrefabID = prefabID;
            }
            public EventArgsPrefab(Point[] points) : base()
            {
                Points = points;
                //Rectangle = rectangle;
            }

            public EventArgsPrefab(Point[] points, int prefabID, Point location , Prefab.Rotate rotate, int playerID) : base()
            {
                Points = points;
                //Rectangle = rectangle;
                //Prefab = prefab;
                PrefabID = prefabID;
                Location = location;
                Rotate = rotate;
                PlayerID = playerID;
            }
            /// <summary>
            /// Координаты раставленных точек
            /// </summary>
            public Point[] Points { get; }
            /// <summary>
            /// Прямоугольник 
            /// TODO: Непомню зачем, надо вспомнить
            /// </summary>
            public Rectangle Rectangle { get; }
            /// <summary>
            /// Фигура с учетом поворотов
            /// </summary>
            public Prefab Prefab { get; }
            /// <summary>
            /// ID оригинальной фигуры
            /// </summary>
            public int PrefabID { get; }
            public Point Location { get; }

            /// <summary>
            /// Повороты
            /// </summary>
            public Prefab.Rotate Rotate { get; }
            public int PlayerID { get; }
        }

        //public class EventArgsInt : EventArgs
        //{
        //    public EventArgsInt(int integer) : base()
        //    {
        //        Integer = integer;
        //    }

        //    public int Integer { get; }
        //}
    }
}

