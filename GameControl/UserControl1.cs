#define test

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace GameCore
{
    public partial class GameControl : UserControl
    {
        #region TestDebug
#if test
        private int _tCurPref;
        [Category("Test")]
        [Description("Режим отображения")]
        public ViewMode TView { get => view; set => view = value; }
        [Category("Test")]
        [Description("Высота фигур")]
        public int THeightCell { get => HeightCell; set => HeightCell = value; }
        [Category("Test")]
        [Description("Цвета игроков")]
        public Dictionary<int, Player> TPlayers
        {
            get => Game.Players;
        }

        [Category("Test")]
        [Description("Текущий игрок")]
        public int TCurPlayer
        {
            get => Game.CurrentPlayer;
            set => Game.CurrentPlayer = value;
        }
        [Category("Test")]
        [Description("Индекс текущего префаба")]
        public int TCurPref
        {
            get => _tCurPref;
            set
            {
                try
                {
                    currentPrefab = Game.Prefabs[value];
                    _tCurPref = value;
                }
                catch
                {
                    MessageBox.Show("Вне диапозона");
                }
            }
        }
        //[Category("Test")]
        //[Description("Строки")]
        //public int TRows { get; set; }
        //[Category("Test")]
        //[Description("Столбцы")]
        //public int TColumns { get; set; }

        [Category("Test")]
        [Description("Количество клеток поля")]
        public Size TSize { get; set; }

        [Category("Test")]
        [Description("Создание сетки и начало игры")]
        public bool TStart
        {
            get => false;
            set
            {
                if (value)
                {
                    Start(TSize.Width, TSize.Height);
                    TCurPlayer = 1;
                }
            }
        }

        [Category("Test")]
        [Description("Перерисовать")]
        public bool TRePaint
        {
            get => false;
            set
            {
                if (value) RepaintAll();
            }
        }

        [Category("Test")]
        [Description("Разместить фигуры на поле")]
        public bool TPaint
        {
            get => false;
            set
            {
                if (value)
                {
                    foreach (var item in TCellsList)
                    {
                        this[item.Point] = item;
                        RePaintPoints.Enqueue(item.Point);
                    }
                    TCellsList.Clear();
                    UpdateCellView();
                    //if (value) OnPaint(new PaintEventArgsWithPoint(,) { });
                }
            }
        }

        [Category("Test")]
        [Description("Игровое поле")]
        public Cell[,] TCells => Game.Cells;

        [Category("Test")]
        [Description("Список ячеек для заполнения")]
        public List<Cell> TCellsList { get; } = new List<Cell>();

        //public class CellAngle : Cell
        //{
        //    public int PlayerAllow { get; set; }
        //}

        //public class CellPoint : Cell
        //{
        //    public Point Point { get; set; }
        //}
#endif
        #endregion

        public class PaintEventArgsWithPoint : PaintEventArgs
        {
            public bool Preview { get; set; }
            public Point Point { get; set; }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="graphics"></param>
            /// <param name="clipRect"></param>
            /// <param name="point"></param>
            /// <param name="previewBrush">Если null, то не используется</param>
            public PaintEventArgsWithPoint(Graphics graphics, Rectangle clipRect, Point point, bool preview = false) : base(graphics, clipRect)
            {
                Point = point;
                Preview = preview;
            }
        }
        public Game Game { get; private set; }
#if test
        = new Game(
            new Player[]
            {
                new PlayerCell(Color.Blue, Color.DeepSkyBlue, Color.AliceBlue),
                new PlayerCell(Color.Red,  Color.IndianRed,   Color.AliceBlue)
            },
            new Game.PrefabCollection
            {
                { new Point(0, 0)                                                                     },
                { new Point(0, 0), new Point(0, 1)                                                    },
                { new Point(0, 0), new Point(0, 1), new Point(1, 0)                                   },
                { new Point(0, 0), new Point(0, 1), new Point(1, 0), new Point(1, 1)                  },
                { new Point(0, 0), new Point(0, 1), new Point(0, 2)                                   },
                { new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(1, 0)                  },
                { new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(1, 1)                  },
                { new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(0, 3)                  },
                { new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(1, 0), new Point(2, 0) },
                { new Point(0, 0), new Point(0, 2), new Point(2, 0), new Point(2, 2)                  },
                { new Point(0, 0), new Point(0, 2), new Point(2, 0), new Point(2, 2), new Point(2, 1) }
            });
#endif
        
        public int HeightCell
        {
            get => heightCell;
            set
            {
                heightCell = value;
                RepaintAll();
            }
        }
        ViewMode view = ViewMode.All;
        /// <summary>
        /// Высота фигур
        /// </summary>
        int heightCell;
        Point previewPrefab = Point.Empty;
        List<Point> previewOld = new List<Point>();
        List<Point> previewNew = new List<Point>();
        List<Point> previewUpdate = new List<Point>();
        //public List<Prefab> Prefabs { get; }
        Pen _border;
        Pen _borderYellow = new Pen(Color.Yellow, 3);
        Pen _borderRed = new Pen(Color.DarkRed, 3);
        Color background = Color.Gray;
        //Prefab currentPrefab;
        
        Dictionary<int, List<Point>> allowPointsForPlayer;

        ///// <summary>
        ///// Список игроков (в том числе неиграбельные)
        ///// </summary>
        //public Dictionary<int, IPlayerCell> Players { get; } = new Dictionary<int, IPlayerCell>();
        //public Dictionary<int, ColorSolidBrushControl> PlayersBack { get; } = new Dictionary<int, ColorSolidBrushControl>();
        //public Dictionary<int, ColorHatchBrushControl> PlayersSolid { get; } = new Dictionary<int, ColorHatchBrushControl>();
        public Queue<Point> RePaintPoints { get; } = new Queue<Point>();
        ///// <summary>
        ///// Игровое поле
        ///// </summary>
        //public Cell[][] cells;
        /// <summary>
        /// Предпросмотр
        /// </summary>
        public bool[][] prePaste;
        /// <summary>
        /// Строки
        /// </summary>
        public int Rows
        {
            get => Game.Row;
            private set
            {
                Game.Row = value;
                DRowCalc();
            }
        }
        /// <summary>
        /// Столбцы
        /// </summary>
        public int Columns
        {
            get => Game.Columns;
            private set
            {
                Game.Columns = value;
                DColumnCalc();
            }
        }
        int dRow = 1, dColumn = 1;

        private void DRowCalc()
        {
            try { dRow = Rows == 0 ? 0 : (Height - Height % (Height / Rows)) / Rows; }
            catch { dRow = 1; }
            if (dRow <= 0) dRow = 1;
        }

        private void DColumnCalc()
        {
            try { dColumn = Columns == 0 ? 0 : (Width - Width % (Width / Columns)) / Columns; }
            catch { dColumn = 1; }
            if (dColumn <= 0) dColumn = 1;
        }

        //private int _rows;
        //private int _columns;

        public void UpdateCellView()
        {
            Graphics graphics = CreateGraphics();
            while (RePaintPoints.Count != 0)
            {
                Point point = RePaintPoints.Dequeue();
                OnPaint(new PaintEventArgsWithPoint(CreateGraphics(), Rectangle.Inflate(GetRectangle(point), 2, 2), point));
                //cell = this[point];
                //rectangle = GetRectangle(point);
                //graphics.FillRectangle(Players[cell.Owner], MargineRectangle(rectangle, 1));
                ////Border border = cell.Border;
                //DrawBorders(graphics, rectangle, cell.Border);
            }
        }


        public void Start(int columns, int rows)
        {
            this.Rows = rows;
            this.Columns = columns;
            Game.Init(columns, rows);
            //cells = new Cell[columns][];
            prePaste = new bool[columns][];
            for (int i = 0; i < columns; i++)
            {
                //Cell[] tmp1 = new Cell[Rows];
                bool[] tmp2 = new bool[Rows];
                for (int j = 0; j < rows; j++)
                {
                    //tmp1[j] = new Cell();
                    tmp2[j] = false;
                }
                //cells[i] = tmp1;
                prePaste[i] = tmp2;
            }
            RepaintAll();
            allowPointsForPlayer.Clear();
        }

        public Cell this[int column, int row]
        {
            get => Game[column, row];
#if test
            set => Game[column, row] = value;
#endif
        }

        public Cell this[Point point]
        {
            get => Game[point];
#if test
            set => Game[point] = value;
#endif
        }

        private bool repaint = false;
        private Prefab currentPrefab;

        public GameControl()
        {
            InitializeComponent();
            allowPointsForPlayer = new Dictionary<int, List<Point>>();
            //Players.Add(-2, Color.Azure);
            //Players.Add(-1, Color.AliceBlue);
            
            _borderYellow = new Pen(Color.Crimson);
            _border = new Pen(this.ForeColor, 3);
            Rows = Columns = 0;
#if test
            TCurPref = new Random().Next(8);
#endif
            //Prefabs = new List<Prefab>();
            //Prefabs.Add(new Prefab(new Point(0, 0)));
            //Prefabs.Add(new Prefab(new Point(0, 0), new Point(0, 1)));
            //Prefabs.Add(new Prefab(new Point(0, 0), new Point(0, 1), new Point(1, 0)));
            //Prefabs.Add(new Prefab(new Point(0, 0), new Point(0, 1), new Point(1, 0), new Point(1, 1)));
            //Prefabs.Add(new Prefab(new Point(0, 0), new Point(0, 1), new Point(0, 2)));
            //Prefabs.Add(new Prefab(new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(1, 0)));
            //Prefabs.Add(new Prefab(new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(1, 1)));
            ////Prefabs.Add(new Prefab(new Point(1, 0), new Point(1, 1), new Point(1, 2), new Point(0, 0)));
            //Prefabs.Add(new Prefab(new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(0, 3)));
            //Prefabs.Add(new Prefab(new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(1, 0), new Point(2, 0)));
            //Prefabs.Add(new Prefab(new Point(0, 0), new Point(0, 2), new Point(2, 0), new Point(2, 2)));
            //Prefabs.Add(new Prefab(new Point(0, 0), new Point(0, 2), new Point(2, 0), new Point(2, 2), new Point(2, 1)));
            //Point[] p = new Point[64];
            //for (int i = 0; i < 8; i++)
            //{
            //    for (int j = 0; j < 8; j++)
            //    {
            //        p[i * 8 + j] = new Point(i, j);
            //    }
            //}
            //Prefabs.Add(new Prefab(p));
            //currentPrefab = Prefabs.First();
        }
       

        private void RepaintAll()
        {
            repaint = true;
            Update();
            //OnPaint(new PaintEventArgs(this.CreateGraphics(), this.ClientRectangle));
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            e.Control.Dispose();
            foreach (Control item in Controls) { item.Dispose(); }
        }

#region OnPaintMethods

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            if (this.DesignMode)
            {
                //int k = 0;
                //int t = 5 / k;
                //graphics.Clear(BackColor);
                Rectangle rectangle = RectangleToScreen(ClientRectangle) ;
                if (Parent != null)
                    rectangle.Intersect(Parent.RectangleToScreen(Parent.DisplayRectangle));
                rectangle = RectangleToClient(rectangle);
                graphics.FillRectangle(new System.Drawing.Drawing2D.LinearGradientBrush(rectangle.UpLeft(), rectangle.DownLeft(), Color.Blue, Color.Red),rectangle);
                graphics.DrawString($"{this.Name}", new Font(this.Font.FontFamily, 30), new SolidBrush(ForeColor), rectangle, new StringFormat());
                //if(new Random().Next()%25==0)
                //throw new Exception("Test");
            }
            else
            {
                Rectangle rectangleDown;
                Rectangle rectangleUp;
                Cell cell;
                Point point;
                if (repaint)//Перерисовать все
                {
                    graphics.Clear(BackColor);
                    for (int i = 0; i <= Columns; i++)
                    {
                        int x = i * dColumn;
                        graphics.DrawLine(_border, x, 0, x, Height);
                    }
                    for (int i = 0; i <= Rows; i++)
                    {
                        int y = i * dRow;
                        graphics.DrawLine(_border, 0, y, Width, y);
                    }
                    for (int i = 0; i < Columns; i++)
                    {
                        for (int j = 0; j < Rows; j++)
                        {
                            cell = this[i, j];
                            if (cell.PlayerOwner != 0)
                            {
                                rectangleDown = GetRectangle(i, j);
                                rectangleUp = rectangleDown.Move(-heightCell, -heightCell);
                                Game.Players[cell.PlayerOwner].DrawCell(graphics, rectangleDown, rectangleUp, view, false);
                            }
                        }
                    }

                    RePaintPoints.Clear();
                    repaint = false;
                }
                else if (e is PaintEventArgsWithPoint paintEventArgsWithPoint)
                {
                    point = paintEventArgsWithPoint.Point;
                    cell = this[point];
                    //rectangleClip = GetRectangle(point);
                    ////Point p = rectangleClip.Location;
                    ////p.X -= 2;
                    ////p.Y -= 2;
                    ////rectangleClip.Location = p;
                    //rectangleCell = Rectangle.Inflate(rectangleClip, -3, -3);
                    //rectangleBorder = Rectangle.Inflate(rectangleClip, -1, -1);
                    SetClip(graphics, out rectangleDown, out rectangleUp, point);
                    if (paintEventArgsWithPoint.Preview)
                        Game.Players[cell.PlayerOwner].DrawCell(graphics, rectangleDown, rectangleUp, view, true);
                    else Game.Players[cell.PlayerOwner].DrawCell(graphics, rectangleDown, rectangleUp, view, false);
                    //if (previewNew.Contains(paintEventArgsWithPoint.Point))
                    //{

                    //    //Game.Players[cell.PlayerOwner].FillSelectedRectangle(graphics, rectangleDown, rectangleUp);
                    //    //else Game.Players[cell.PlayerOwner].FillUpRectangle(graphics, rectangleDown, rectangleUp);
                    //}
                    //if (paintEventArgsWithPoint.Preview && previewNew.Contains(paintEventArgsWithPoint.Point))
                    //{
                    //    Players[cell.Owner].FillSelectedRectangle(graphics, point, cell);
                    //    //if( cell.Owner!=0)
                    //    //    graphics.FillRectangle(PlayersSolid[cell.Owner], rectangleCell);
                    //    //else
                    //    //graphics.FillRectangle(Players[-1], rectangleCell);
                    //}
                    //else
                    //{
                    //    Players[cell.Owner].FillUpRectangle(graphics, point, cell);
                    //    //graphics.FillRectangle(Players[cell.Owner], rectangleCell);
                    //    //DrawBorders(graphics, rectangleBorder, _borderYellow, cell.Border);
                    //}
                }
                else if (Columns != 0 || Rows != 0)
                {
                    //Если нужно обновить определенную область (перерисовка окна)
                    Point first = GetPointCellFromReal(e.ClipRectangle.Location);
                    Point second = GetPointCellFromReal(new Point(e.ClipRectangle.X + e.ClipRectangle.Width, e.ClipRectangle.Y + e.ClipRectangle.Height));
                    for (int i = first.X; i <= second.X + 1; i++)//Обновление фоновых границ
                    {
                        int x = i * dColumn;
                        graphics.DrawLine(_border, x, e.ClipRectangle.Y, x, e.ClipRectangle.Y + Height);
                    }


                    for (int i = first.Y; i <= second.Y + 1; i++)//Обновление фоновых границ
                    {
                        int y = i * dRow;
                        graphics.DrawLine(_border, e.ClipRectangle.X, y, e.ClipRectangle.X + Width, y);
                    }

                    //for (int i = first.X; i < second.X; i++)
                    //{//Восстанавливаем границы ячеек, которые могли быть случайно стерты в предыдущем цикле операцией
                    //    for (int j = first.Y; j < second.Y; j++)
                    //    {
                    //        rectangleBorder = GetRectangle(i, j);
                    //        if (this[i, j].Border.HasFlag(Border.Rigth))
                    //        {
                    //            int x1 = rectangleBorder.X + rectangleBorder.Width -2;
                    //            int y1 = rectangleBorder.Y;
                    //            int y2 = rectangleBorder.Y + rectangleBorder.Height ;
                    //            graphics.DrawLine(_borderYellow, x1, y1, x1, y2);
                    //        }
                    //    }
                    //}

                    //for (int j = first.Y; j < second.Y; j++)
                    //{//Восстанавливаем границы ячеек, которые могли быть случайно стерты в предыдущем цикле операцией
                    //    for (int i = first.X; i < second.X; i++)
                    //    {
                    //        rectangleBorder = GetRectangle(i, j);
                    //        if (this[i, j].Border.HasFlag(Border.Down))
                    //        {
                    //            int x1 = rectangleBorder.X + rectangleBorder.Width;
                    //            int x2 = rectangleBorder.X + rectangleBorder.Width;
                    //            int y1 = rectangleBorder.Y + rectangleBorder.Height - 1;
                    //            graphics.DrawLine(_borderYellow, x1, y1, x2, y1);
                    //        }
                    //    }
                    //}

                    for (int i = first.X; i <= second.X; i++)
                    {
                        for (int j = first.Y; j <= second.Y; j++)
                        {
                            cell = this[i, j];
                            //Point p = rectangleClip.Location;
                            //p.X -= 2;
                            //p.Y -= 2;
                            //rectangleClip.Location = p;
                            //rectangleCell = Rectangle.Inflate(rectangleDown, -3, -3);
                            //rectangleBorder = Rectangle.Inflate(rectangleDown, -1, -1);
                            //rectangleDown = GetRectangle(i,j);
                            //rectangleUp = rectangleDown.Move(-heightCell, -heightCell);
                            SetClip(graphics, out rectangleDown, out rectangleUp, new Point(i, j));
                            Game.Players[cell.PlayerOwner].DrawCell(graphics, rectangleDown, rectangleUp, view, false);
                            //Players[cell.Owner].DrawRectangle(graphics, rectangleClip, cell);
                            //graphics.FillRectangle(Players[cell.Owner], rectangleCell);
                            //DrawBorders(graphics, rectangleBorder, _borderYellow, cell.Border);
                        }
                    }
                }

                //while (RePaintPoints.Count != 0)
                //{
                //    point = RePaintPoints.Dequeue();
                //    cell = this[point];
                //    rectangle = GetRectangle(point);
                //    graphics.FillRectangle(Players[cell.Owner], MargineRectangle(rectangle, 1));
                //    //Border border = cell.Border;
                //    DrawBorders(graphics, rectangle, cell.Border);
                //}
                base.OnPaint(e);
            }
        }

        private void SetClip(Graphics graphics, out Rectangle rectangleDown, out Rectangle rectangleUp, Point point)
        {
            rectangleDown = GetRectangle(point);
            rectangleUp = rectangleDown.Move(-heightCell, -heightCell);
            Region region = rectangleDown.GetRegion(rectangleUp);
            if (point.Y + 1 < Rows)
            {
                try
                {
                    Point p = point.Down();
                    if (this[p].PlayerOwner > 0)
                    {
                        Rectangle rectangleDownE = GetRectangle(p);
                        Rectangle rectangleUpE = rectangleDownE.Move(-heightCell, -heightCell);
                        region.Exclude(rectangleDownE.GetRegion(rectangleUpE));
                    }
                }
                catch { }
                if (point.X + 1 < Columns)
                {
                    try
                    {
                        Point p = point.DownRight();
                        if (this[p].PlayerOwner > 0)
                        {
                            Rectangle rectangleDownE = GetRectangle(p);
                            Rectangle rectangleUpE = rectangleDownE.Move(-heightCell, -heightCell);
                            region.Exclude(rectangleDownE.GetRegion(rectangleUpE));
                        }
                    }
                    catch { }
                }
            }
            if (point.X + 1 < Columns)
            {
                try
                {
                    Point p = point.Right();
                    if (this[p].PlayerOwner > 0)
                    {
                        Rectangle rectangleDownE = GetRectangle(p);
                        Rectangle rectangleUpE = rectangleDownE.Move(-heightCell, -heightCell);
                        region.Exclude(rectangleDownE.GetRegion(rectangleUpE));
                    }
                }
                catch { }
            }
            graphics.Clip = region;
        }

        //OldMethods
        //private void DrawCell(Graphics graphics, Rectangle rectangleClip, Cell cell)
        //{
        //    Rectangle rectangleCell = Rectangle.Inflate(rectangleClip, -3, -3);
        //    Rectangle rectangleBorder = Rectangle.Inflate(rectangleClip, -1, -1);
        //    if (cell.Owner > 0)
        //    {
        //        rectangleCell.X -= 1;
        //        rectangleCell.Y -= 1;
        //        rectangleBorder.X -= 1;
        //        rectangleBorder.Y -= 1;
        //    }
        //    Players[cell.Owner].DrawBorder(graphics, rectangleClip, cell);
        //    //graphics.FillRectangle(Players[cell.Owner], rectangleCell);
        //    //DrawBorders(graphics, rectangleBorder, _borderYellow, cell.Border);
        //}

        //private void DrawBorders(Graphics graphics, Rectangle rectangle, Pen penBorder, Border border)
        //{
        //    int x1 = rectangle.X, y1 = rectangle.Y;
        //    int x2 = rectangle.X + rectangle.Width - 1, y2 = rectangle.Y + rectangle.Height - 1;
        //    int xd1 = x1 - 1, yd1 = y1 - 1;
        //    int xd2 = x2 + 2, yd2 = y2 + 2;
        //    if (border.HasFlag(Border.Left)) graphics.DrawLine(penBorder, x1, yd1, x1, yd2);
        //    if (border.HasFlag(Border.Top)) graphics.DrawLine(penBorder, xd1, y1, xd2, y1);
        //    if (border.HasFlag(Border.Rigth)) graphics.DrawLine(penBorder, x2, yd1, x2, yd2);
        //    if (border.HasFlag(Border.Down)) graphics.DrawLine(penBorder, xd1, y2, xd2, y2);
        //}

        #endregion

        private Point GetPointCellFromReal(Point point)
        {
            int x = point.X;
            int y = point.Y;
            Point npoint = new Point(x / dColumn, y / dRow);
            if (npoint.X < 0) npoint.X = 0;
            if (npoint.X >= Columns) npoint.X = Columns - 1;
            if (npoint.Y < 0) npoint.Y = 0;
            if (npoint.Y >= Rows) npoint.Y = Rows - 1;
            return npoint;
        }

        public Rectangle GetRectangle(Point point)
        {
            //int dmargin = margin + margin;
            return new Rectangle(point.X * dColumn + 2, point.Y * dRow + 2, dColumn - 3, dRow - 3);
        }

        public Rectangle GetRectangle(int x, int y)
        {
            //int dmargin = margin + margin;
            return new Rectangle(x * dColumn + 2, y * dRow + 2, dColumn - 3, dRow - 3);
        }


        protected override void OnResize(EventArgs e)
        {
            DColumnCalc();
            DRowCalc();
            RepaintAll();
            base.OnResize(e);
        }

#region Mouse

        protected override void OnMouseClick(MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    //Point point = GetPointCellFromReal(e.Location);
                    //Cell cell = this[point];
                    //foreach (var item in currentPrefab.Points)
                    //{

                    //}
                    //if (cell.Owner == 0) cell.Owner = 1;
                    //else if (cell.Owner == 1) cell.Owner = 2;
                    //else cell.Owner = 0;
                    //bool succes1 = true;
                    //bool succes2 = true;
                    //List<Point> allow; // Точки, где разрешено размещение
                    //List<Point> remoweAllow = new List<Point>(); // Точки, где запрещено размещение
                    //Point plusPoint = FixPointCursor(e.Location); // Кликнутая ячейка
                    //var places = currentPrefab.GetPointsCursors(plusPoint); // Получить координаты всех ячеек фигуры
                    //foreach (var item in places) // Для всех координат проверка
                    //{
                    //    if (this[item.Point].PlayerOwner != 0) // Если координата занята другой фигурой
                    //    {
                    //        succes1 = false;
                    //        break;
                    //    }
                    //    try
                    //    {
                    //        if (this[item.Point.Up()].PlayerOwner == Game.CurrentPlayer)
                    //        {
                    //            succes1 = false;
                    //            break;
                    //        }
                    //    }
                    //    catch { }
                    //    try
                    //    {
                    //        if (this[item.Point.Down()].PlayerOwner == currentPlayer)
                    //        {
                    //            succes1 = false;
                    //            break;
                    //        }
                    //    }
                    //    catch { }
                    //    try
                    //    {
                    //        if (this[item.Point.Left()].PlayerOwner == currentPlayer)
                    //        {
                    //            succes1 = false;
                    //            break;
                    //        }
                    //    }
                    //    catch { }
                    //    try
                    //    {
                    //        if (this[item.Point.Right()].PlayerOwner == currentPlayer)
                    //        {
                    //            succes1 = false;
                    //            break;
                    //        }
                    //    }
                    //    catch { }
                    //}
                    //if (succes1)
                    //{
                    //    if (!allowPointsForPlayer.ContainsKey(currentPlayer))
                    //    {
                    //        allow = new List<Point>();
                    //        allowPointsForPlayer.Add(currentPlayer, allow);
                    //    }
                    //    else
                    //    {
                    //        succes2 = false;
                    //        allow = allowPointsForPlayer[currentPlayer];
                    //        foreach (var item in places)
                    //        {
                    //            if (allow.Contains(item.Point))
                    //            {
                    //                succes2 = true;
                    //                remoweAllow.Add(item.Point);
                    //                //break;
                    //            }
                    //        }
                    //    }


                    //    if (succes2)
                    //    {
                    //        foreach (var item in places)
                    //        {
                    //            //Point t = item.Up();
                    //            //if(!places.Contains(t)&&this[item].Owner!=currentPlayer)
                    //            //{
                    //            //    Point t2 = item.Left();
                    //            //    if(!places.Contains(t)&&this[item].Owner!=currentPlayer)
                    //            //}
                    //            Cell cell = this[item.Point];
                    //            cell.PlayerOwner = currentPlayer;
                    //            cell.Border = item.Border;
                    //            RePaintPoints.Enqueue(item.Point);
                    //        }
                    //        foreach (var item in currentPrefab.AnglePoints)
                    //        {
                    //            Point fixPoint = new Point(item.X + plusPoint.X, item.Y + plusPoint.Y);
                    //            if (!allow.Contains(fixPoint)) allow.Add(fixPoint);
                    //        }
                    //        //foreach (var item in remoweAllow)
                    //        //{
                    //        //    allow.Remove(item);
                    //        //}
                    PointWithBorder[] points;
                    if (Game.SetPrefab(currentPrefab, FixPointCursor(e.Location), out points))
                    {
                        foreach (var item in points)
                        {
                            RePaintPoints.Enqueue(item.Point);
                        }
                        UpdateCellView();
                    }
#if test
                    TCurPref = new Random().Next(8);
#endif
                    break;
                case MouseButtons.None:
                    break;
                case MouseButtons.Right:
                    currentPrefab = currentPrefab.GetRotatedPrefab(Prefab.Rotate.horizontal);
                    break;
                case MouseButtons.Middle:
                    break;
                case MouseButtons.XButton1:
                    break;
                case MouseButtons.XButton2:
                    break;
                default:
                    break;
            }
            base.OnMouseClick(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            VisualPreview(e);
            base.OnMouseMove(e);
        }

        private void VisualPreview(MouseEventArgs e)
        {
            if (currentPrefab != null && (Rows != 0 || Columns != 0))
            {
                //Point centre = FixPointCursor(e.Location);
                previewNew = new List<Point>();
                //foreach (var item in currentPrefab.Points)
                //{
                //    previewNew.Add(new Point(item.X + centre.X, item.Y + centre.Y));
                //}
                previewNew.AddRange(currentPrefab.GetPointsCursors(FixPointCursor(e.Location)).Select(a => a.Point));
                //previewUpdate = new List<Point>();
                Graphics graphics = CreateGraphics();
                foreach (var item in previewNew)
                {
                    if (!previewOld.Contains(item))
                        OnPaint(new PaintEventArgsWithPoint(graphics, GetRectangle(item), item, true));
                }
                foreach (var item in previewOld)
                {
                    if (!previewNew.Contains(item))
                        OnPaint(new PaintEventArgsWithPoint(graphics, GetRectangle(item), item, false));
                }

                previewOld = previewNew;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (currentPrefab != null && (Rows != 0 || Columns != 0))
            {
                //Point centre = FixPointCursor(e.Location);
                previewNew = new List<Point>();
                //foreach (var item in currentPrefab.Points)
                //{
                //    previewNew.Add(new Point(item.X + centre.X, item.Y + centre.Y));
                //}
                //previewNew.AddRange(currentPrefab.GetPointsCursors(FixPointCursor(e.Location)));


                previewUpdate.Clear();
                previewUpdate.AddRange(previewOld);
                //foreach (var item in previewNew)
                //{
                //    if (!previewUpdate.Contains(item)) previewUpdate.Add(item);
                //}

                Graphics graphics = CreateGraphics();
                foreach (var item in previewUpdate)
                {
                    OnPaint(new PaintEventArgsWithPoint(graphics, GetRectangle(item), item, false));
                }
                previewOld.Clear();
            }
            base.OnMouseLeave(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (e.Delta < 0)
                currentPrefab = currentPrefab.GetRotatedPrefab(Prefab.Rotate.r90);
            //else if(e.Delta<0)
            else
                currentPrefab = currentPrefab.GetRotatedPrefab(Prefab.Rotate.r270);
            VisualPreview(e);
            base.OnMouseWheel(e);
        }
#endregion

        protected override void OnBackColorChanged(EventArgs e)
        {
            dynamic emptyCell = Game.Players[0];
            emptyCell.CellColor = BackColor;
            //Players[0].Dispose();
            //Players[0] = new SolidBrush(this.BackColor);
            base.OnBackColorChanged(e);
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            if (_border != null) _border.Dispose();
            _border = new Pen(ForeColor, 3);
            base.OnForeColorChanged(e);
        }

        private Point FixPointCursor(Point point)
        {
            Size size = new Size(currentPrefab.Size.Width - 1, currentPrefab.Size.Height - 1);
            point.Y -= (int)(dRow * (size.Height / 2.0f));
            point.X -= (int)(dColumn * (size.Width / 2.0f));
            Point centre = GetPointCellFromReal(point);
            if ((centre.X + size.Width) >= Columns)
            {
                centre.X = Columns - size.Width - 1;
            }
            if ((centre.Y + size.Height) >= Rows)
            {
                centre.Y = Rows - size.Height - 1;
            }
            return centre;
        }
    }

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

    //public class Map
    //{
    //    public class HorizontalNode
    //    {
    //        Node zero;
    //        public Node Zero => zero;
    //        public int Index;
    //        public HorizontalNode Left, Right;
    //    }

    //    public class Node
    //    {
    //        public int Index;
    //        public HorizontalNode Parent;
    //        public Node Up;
    //        public Node Down;
    //    }

    //    Node zero;
    //    public Node Zero => zero;

    //}

    public class PointWithBorder
    {
        public int Index { get; set; }
        public Point Point { get; set; }
        public Border Border { get; set; }
        //public bool IsAllowPoint { get; set; }
    }

    //public class ColorSolidBrushControl
    //{
    //    SolidBrush brush;
    //    public SolidBrush Brush => brush; 
    //    Color color;
    //    public Color Color
    //    {
    //        get => color;
    //        set
    //        {
    //            color = value;
    //            brush.Dispose();
    //            brush = new SolidBrush(value);
    //        }
    //    }
    //    public ColorSolidBrushControl(Color color)
    //    {
    //        this.color = color;
    //        brush = new SolidBrush(color);
    //    }
    //}

    //public class ColorHatchBrushControl
    //{
    //    HatchBrush brush;
    //    public HatchBrush Brush => brush;
    //    Color backColor;
    //    Color foreColor;
    //    public Color BackColor
    //    {
    //        get => backColor;
    //        set
    //        {
    //            backColor = value;
    //            brush.Dispose();
    //            brush = new HatchBrush(HatchStyle.WideUpwardDiagonal, foreColor, backColor);
    //        }
    //    }
    //    public Color ForeColor
    //    {
    //        get => foreColor;
    //        set
    //        {
    //            foreColor = value;
    //            brush.Dispose();
    //            brush = new HatchBrush(HatchStyle.WideUpwardDiagonal, foreColor, backColor);
    //        }
    //    }
    //    public void SetColors(Color foreColor, Color backColor)
    //    {
    //        this.foreColor = foreColor;
    //        this.backColor = backColor;
    //        brush.Dispose();
    //        brush = new HatchBrush(HatchStyle.WideUpwardDiagonal, foreColor, backColor);
    //    }

    //    public ColorHatchBrushControl(Color foreColor, Color backColor)
    //    {
    //        this.foreColor = foreColor;
    //        this.backColor = backColor;
    //        brush = new HatchBrush(HatchStyle.WideUpwardDiagonal, foreColor, backColor);
    //    }
    //}

    //public class ColorPenControl
    //{
    //    int width;
    //    public int Width
    //    {
    //        get => width;
    //        set
    //        {
    //            width = value;
    //            pen.Dispose();
    //            pen = new Pen(color, width);
    //        }
    //    }
    //    Pen pen;
    //    Pen Pen => pen;
    //    Color color;
    //    Color Color
    //    {
    //        get => color;
    //        set
    //        {
    //            color = value;
    //            pen.Dispose();
    //            pen = new Pen(color, width);
    //        }
    //    }

    //    public ColorPenControl(Color color, int width = 1)
    //    {
    //        this.color = color;
    //        this.width = width;
    //        pen = new Pen(color, width);
    //    }
    //}

    [Flags]
    public enum ViewMode
    {
        None = 0x00,
        Border = 0x01,
        Rectangle = 0x02,
        Wall = 0x04,
        Border_Rectangle = Border | Rectangle,
        Border_Wall = Border | Wall,
        Rectangle_Wall = Rectangle | Wall,
        All = Border | Rectangle | Wall
    }

   



    public enum GameCode : byte
    {
        ID = 1,
        GetAll,
        //SetForeColor,
        //SetBackColor, 
        SetImage,
        SetCellColor,
        SetBorderColor,
        AddPrefab,
        RemovePrefab,
        GetPrefabs,
        SetSize,
        GetSize,
        MakeStep,
        Validate,
        StartGame,
        EndGame
    }
    public enum Status : byte
    {
        OK = 1
    }

    public static class ObjectToBinaryWriter
    {
        public static void WriteObject(this BinaryWriter bw, GameCode code, Prefab prefab)
        {
            bw.Write((byte)code);
            var points = prefab.Points;
            bw.Write((short)(points.Length * 2));
            foreach (var item in points)
            {
                Point p = item.Point;
                bw.Write((byte)p.X);
                bw.Write((byte)p.Y);
            }
        }
        public static void WriteObject(this BinaryWriter bw, GameCode code, Color color)
        {
            bw.Write((byte)code);
            bw.Write(4);
            bw.Write(color.ToArgb());
        }
        public static void WriteObject(this BinaryWriter bw, GameCode code, String s)
        {
            bw.Write((byte)code);
            bw.Write(s.Length);
            bw.Write(s.ToCharArray());
        }
    }
}

