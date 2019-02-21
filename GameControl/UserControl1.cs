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

namespace GameControl
{
    public partial class GameControl : UserControl
    {
        #region TestDebug
#if test
        private int _tCurPref;
        [Category("Test")]
        [Description("Текущий игрок")]
        public int TCurPlayer
        {
            get => currentPlayer;
            set => currentPlayer = value;
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
                    currentPrefab = Prefabs[value];
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
        public Cell[][] TCells => cells;

        [Category("Test")]
        [Description("Список ячеек для заполнения")]
        public List<CellPoint> TCellsList { get; } = new List<CellPoint>();

        public class CellPoint : Cell
        {
            public Point Point { get; set; }
        }
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
            public PaintEventArgsWithPoint(Graphics graphics, Rectangle clipRect, Point point, bool preview = false):base(graphics,clipRect)
            {
                Point = point;
                Preview = true;
            }
        }

        Point previewPrefab = Point.Empty;
        List<Prefab> prefabs;
        List<Point> previewOld = new List<Point>();
        List<Point> previewNew = new List<Point>();
        List<Point> previewUpdate = new List<Point>();
        public List<Prefab> Prefabs => prefabs;
        Pen _border = new Pen(Color.Black, 3);
        Pen _borderYellow = new Pen(Color.Yellow, 3);
        Pen _borderRed = new Pen(Color.DarkRed, 3);
        Color background = Color.Gray;
        Prefab currentPrefab;
        int currentPlayer;
        Dictionary<int, List<Point>> allowPointsForPlayer;

        public Dictionary<int, Brush> Players { get; } = new Dictionary<int, Brush>();
        public Queue<Point> RePaintPoints { get; } = new Queue<Point>();
        public Cell[][] cells;
        public bool[][] prePaste;
        public int Rows
        {
            get => _rows;
            private set
            {
                _rows = value;
                DRowCalc();
            }
        }
        public int Columns
        {
            get => _columns;
            private set
            {
                _columns = value;
                DColumnCalc();
            }
        }
        int dRow = 1, dColumn = 1;

        private void DRowCalc()
        {
            try { dRow = _rows == 0 ? 0 : (Height - Height % (Height / _rows)) / _rows; }
            catch { dRow = 1; }
            if (dRow == 0) dRow = 1;
        }

        private void DColumnCalc()
        {
            try { dColumn = _columns == 0 ? 0 : (Width - Width % (Width / _columns)) / _columns; }
            catch { dColumn = 1; }
            if (dColumn == 0) dColumn = 1;
        }

        private int _rows;
        private int _columns;

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
            cells = new Cell[columns][];
            prePaste = new bool[columns][];
            for (int i = 0; i < columns; i++)
            {
                Cell[] tmp1 = new Cell[Rows];
                bool[] tmp2 = new bool[Rows];
                for (int j = 0; j < rows; j++)
                {
                    tmp1[j] = new Cell();
                    tmp2[j] = false;
                }
                cells[i] = tmp1;
                prePaste[i] = tmp2;
            }
            RepaintAll();
        }

        public Cell this[int column, int row]
        {
            get => cells[column][row];
            set
            {
                cells[column][row] = value;
            }
        }

        public Cell this[Point point]
        {
            get => cells[point.X][point.Y];
            set
            {
                cells[point.X][point.Y] = value;
            }
        }

        private bool repaint = false;

        public GameControl()
        {
            Players.Add(-1, Brushes.AliceBlue);
            Players.Add(0, Brushes.Gray);
            Players.Add(1, Brushes.Blue);
            Players.Add(2, Brushes.Red);
            //allowPointsForPlayer.Add(0, new List<Point>());
            //allowPointsForPlayer.Add(1, new List<Point>());
            InitializeComponent();
            Rows = Columns = 0;
            prefabs = new List<Prefab>();
            Prefabs.Add(new Prefab(new Point(0, 0)));
            Prefabs.Add(new Prefab(new Point(0, 0), new Point(0, 1)));
            Prefabs.Add(new Prefab(new Point(0, 0), new Point(0, 1), new Point(1, 0)));
            Prefabs.Add(new Prefab(new Point(0, 0), new Point(0, 1), new Point(1, 0), new Point(1, 1)));
            Prefabs.Add(new Prefab(new Point(0, 0), new Point(0, 1), new Point(0, 2)));
            Prefabs.Add(new Prefab(new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(1, 0)));
            Prefabs.Add(new Prefab(new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(1, 1)));
            Prefabs.Add(new Prefab(new Point(1, 0), new Point(1, 1), new Point(1, 2), new Point(0, 0)));
            Prefabs.Add(new Prefab(new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(0, 3)));
            Point[] p = new Point[64];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    p[i * 8 + j] = new Point(i, j);
                }
            }
            Prefabs.Add(new Prefab(p));
            currentPrefab = prefabs.First();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            //e.Graphics = Graphics.FromImage(bitmap);
            base.OnPaint(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {

        }

        private void RepaintAll()
        {
            repaint = true;
            OnPaint(new PaintEventArgs(this.CreateGraphics(), this.ClientRectangle));
        }

        #region OnPaintMethods
        private void GameControl_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Rectangle rectangleClip;
            Rectangle rectangleBorder;
            Rectangle rectangleCell;
            Cell cell;
            Point point;
            if (repaint)
            {
                graphics.Clear(background);
                for (int i = 0; i <= _columns; i++)
                {
                    int x = i * dColumn;
                    graphics.DrawLine(_border, x, 0, x, Height);
                }
                for (int i = 0; i <= _rows; i++)
                {
                    int y = i * dRow;
                    graphics.DrawLine(_border, 0, y, Width, y);
                }
                for (int i = 0; i < _columns; i++)
                {
                    for (int j = 0; j < _rows; j++)
                    {
                        cell = this[i, j];
                        if (cell.Owner != 0)
                        {
                            rectangleClip = GetRectangle(i, j);
                            //Point p = rectangleClip.Location;
                            //p.X -= 2;
                            //p.Y -= 2;
                            //rectangleClip.Location = p;
                            rectangleCell = Rectangle.Inflate(rectangleClip, -3, -3);
                            rectangleBorder = Rectangle.Inflate(rectangleClip, -1, -1);
                            graphics.FillRectangle(Players[cell.Owner], rectangleCell);
                            DrawBorders(graphics, rectangleBorder, _borderYellow, cell.Border);
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
                rectangleClip = GetRectangle(point);
                //Point p = rectangleClip.Location;
                //p.X -= 2;
                //p.Y -= 2;
                //rectangleClip.Location = p;
                rectangleCell = Rectangle.Inflate(rectangleClip, -3, -3);
                rectangleBorder = Rectangle.Inflate(rectangleClip, -1, -1);
                if (paintEventArgsWithPoint.Preview&&previewNew.Contains(paintEventArgsWithPoint.Point))
                {
                    graphics.FillRectangle(Players[-1], rectangleCell);
                }
                else
                {
                    graphics.FillRectangle(Players[cell.Owner], rectangleCell);
                    DrawBorders(graphics, rectangleBorder, _borderYellow, cell.Border);
                }
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

                for (int i = first.X; i < second.X; i++)
                {//Восстанавливаем границы ячеек, которые могли быть случайно стерты в предыдущем цикле операцией
                    for (int j = first.Y; j < second.Y; j++)
                    {
                        rectangleBorder = GetRectangle(i, j);
                        if (this[i, j].Border.HasFlag(Border.Rigth))
                        {
                            int x1 = rectangleBorder.X + rectangleBorder.Width - 1;
                            int y1 = rectangleBorder.Y;
                            int y2 = rectangleBorder.Y + rectangleBorder.Height;
                            graphics.DrawLine(_borderYellow, x1, y1, x1, y2);
                        }
                    }
                }

                for (int i = first.Y; i <= second.Y + 1; i++)//Обновление фоновых границ
                {
                    int y = i * dRow;
                    graphics.DrawLine(_border, e.ClipRectangle.X, y, e.ClipRectangle.X + Width, y);
                }

                for (int j = first.Y; j < second.Y; j++)
                {//Восстанавливаем границы ячеек, которые могли быть случайно стерты в предыдущем цикле операцией
                    for (int i = first.X; i < second.X; i++)
                    {
                        rectangleBorder = GetRectangle(i, j);
                        if (this[i, j].Border.HasFlag(Border.Down))
                        {
                            int x1 = rectangleBorder.X + rectangleBorder.Width;
                            int x2 = rectangleBorder.X + rectangleBorder.Width;
                            int y1 = rectangleBorder.Y + rectangleBorder.Height - 1;
                            graphics.DrawLine(_borderYellow, x1, y1, x2, y1);
                        }
                    }
                }

                for (int i = first.X; i <= second.X; i++)
                {
                    for (int j = first.Y; j <= second.Y; j++)
                    {
                        cell = this[i, j];
                        rectangleClip = GetRectangle(i, j);
                        //Point p = rectangleClip.Location;
                        //p.X -= 2;
                        //p.Y -= 2;
                        //rectangleClip.Location = p;
                        rectangleCell = Rectangle.Inflate(rectangleClip, -3, -3);
                        rectangleBorder = Rectangle.Inflate(rectangleClip, -1, -1);
                        graphics.FillRectangle(Players[cell.Owner], rectangleCell);
                        DrawBorders(graphics, rectangleBorder, _borderYellow, cell.Border);
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
        }

        private void DrawBorders(Graphics graphics, Rectangle rectangle, Pen penBorder, Border border)
        {
            int x1 = rectangle.X, y1 = rectangle.Y;
            int x2 = rectangle.X + rectangle.Width - 1, y2 = rectangle.Y + rectangle.Height - 1;
            int xd1 = x1 - 1, yd1 = y1 - 1;
            int xd2 = x2 + 2, yd2 = y2 + 2;
            if (border.HasFlag(Border.Left)) graphics.DrawLine(penBorder, x1, yd1, x1, yd2);
            if (border.HasFlag(Border.Top)) graphics.DrawLine(penBorder, xd1, y1, xd2, y1);
            if (border.HasFlag(Border.Rigth)) graphics.DrawLine(penBorder, x2, yd1, x2, yd2);
            if (border.HasFlag(Border.Down)) graphics.DrawLine(penBorder, xd1, y2, xd2, y2);
        }

        #endregion

        private Point GetPointCellFromReal(Point point)
        {
            int x = point.X;
            int y = point.Y;
            Point npoint = new Point(x / dColumn, y / dRow);
            if (npoint.X < 0) npoint.X = 0;
            if (npoint.X >= _columns) npoint.X = _columns -1;
            if (npoint.Y < 0) npoint.Y = 0;
            if (npoint.Y >= _rows) npoint.Y = _rows -1;
            return npoint;
        }

        private Rectangle GetRectangle(Point point)
        {
            //int dmargin = margin + margin;
            return new Rectangle(point.X * dColumn -1, point.Y * dRow -1, dColumn + 3, dRow + 3);
        }

        private Rectangle GetRectangle(int x, int y)
        {
            //int dmargin = margin + margin;
            return new Rectangle(x * dColumn-1, y * dRow-1, dColumn+3, dRow+3);
        }


        private void GameControl_Resize(object sender, EventArgs e)
        {
            DColumnCalc();
            DRowCalc();
            RepaintAll();
        }

        #region Mouse

        private void GameControl_MouseClick(object sender, MouseEventArgs e)
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
                    if (!allowPointsForPlayer.ContainsKey(currentPlayer)) allowPointsForPlayer.Add(currentPlayer, new List<Point>());
                    else
                    {
                        Point[] places = currentPrefab.GetPointsCursors(FixPointCursor(e.Location));
                        List<Point> allow = allowPointsForPlayer[currentPlayer];
                        List<Point> remoweAllow = new List<Point>();
                        bool succes = false;
                        foreach (var item in places)
                        {
                            prePaste[item.X][item.Y] = true;
                            try
                            {
                                if (this[item.Up()].Owner == currentPlayer)
                                {
                                    succes = false;
                                    break;
                                }
                            }
                            catch { }
                            try
                            {
                                if (this[item.Down()].Owner == currentPlayer)
                                {
                                    succes = false;
                                    break;
                                }
                            }
                            catch { }
                            try
                            {
                                if (this[item.Left()].Owner == currentPlayer)
                                {
                                    succes = false;
                                    break;
                                }
                            }
                            catch { }
                            try
                            {
                                if (this[item.Right()].Owner == currentPlayer)
                                {
                                    succes = false;
                                    break;
                                }
                            }
                            catch { }

                            if (allow.Contains(item))
                            {
                                remoweAllow.Add(item);
                                succes = true;
                            }
                        }

                        if (succes)
                        {
                            foreach (var item in places)
                            {
                                //Point t = item.Up();
                                //if(!places.Contains(t)&&this[item].Owner!=currentPlayer)
                                //{
                                //    Point t2 = item.Left();
                                //    if(!places.Contains(t)&&this[item].Owner!=currentPlayer)
                                //}
                                this[item].Owner = currentPlayer;
                                RePaintPoints.Enqueue(item);
                            }
                            foreach (var item in remoweAllow)
                            {
                                allow.Remove(item);
                            }
                        }
                    }
                    //this[point] = cell;
                    //RePaintPoints.Enqueue(point);
                    UpdateCellView();
                    if (currentPlayer == 1) currentPlayer = 2;
                    else if (currentPlayer == 2) currentPlayer = 1;
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
           
        }

        private void GameControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentPrefab != null&&(Rows!=0||Columns!=0))
            {
                //Point centre = FixPointCursor(e.Location);
                previewNew = new List<Point>();
                //foreach (var item in currentPrefab.Points)
                //{
                //    previewNew.Add(new Point(item.X + centre.X, item.Y + centre.Y));
                //}
                previewNew.AddRange(currentPrefab.GetPointsCursors(FixPointCursor(e.Location)));


                previewUpdate.Clear();
                previewUpdate.AddRange(previewOld);
                foreach (var item in previewNew)
                {
                    if (!previewUpdate.Contains(item)) previewUpdate.Add(item);
                }

                Graphics graphics = CreateGraphics();
                foreach (var item in previewUpdate)
                {
                    OnPaint(new PaintEventArgsWithPoint(graphics, GetRectangle(item), item, true));
                }

                previewOld = previewNew;
            }

        }

        private void GameControl_MouseLeave(object sender, EventArgs e)
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
                    OnPaint(new PaintEventArgsWithPoint(graphics, GetRectangle(item), item, true));
                }
                previewOld.Clear();
            }

        }

        private void GameControl_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
                currentPrefab = currentPrefab.GetRotatedPrefab(Prefab.Rotate.r90);
            //else if(e.Delta<0)
            else
                currentPrefab = currentPrefab.GetRotatedPrefab(Prefab.Rotate.r270);
            this.OnMouseMove(e);
        }

        #endregion

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

        //private class RectanglePoint : Rectangle
        //{

        //}
    }

    [Flags]
    public enum Border:byte
    {
        None = 0x00,
        Top = 0x01,
        Left = 0x02,
        Rigth = 0x04,
        Down = 0x08,
        All = Top|Left|Rigth|Down
    }

    public class Cell
    {
        int owner;
        Border border;

        public int Owner
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

        public Cell()
        {
            owner = 0;
            border = Border.None;
        }
    }

    public class Prefab
    {

        Point[] points;
        List<Point> anglePoints = new List<Point>();
        KeyValuePair<Point, Border>[] pointsB;
        Size size;

        public Point[] Points => points;

        public Point[] GetPointsCursors(Point plusPoint)
        {
            Point[] points = new Point[this.points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                Point t = this.points[i];
                points[i] = new Point(t.X + plusPoint.X, t.Y + plusPoint.Y);
            }
            return points;
        }

        public Size Size => size;

        public Prefab(params Point[] points)
        {
            if (points == null || points.Length == 0) throw new Exception();
            pointsB = new KeyValuePair<Point, Border>[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                //Для границ
            }
            this.points = points;
            foreach (var item in points)
            {
                Point t;
                bool tUp = !points.Contains(item.Up());
                bool tDown = !points.Contains(item.Down());
                bool tLeft = !points.Contains(item.Left());
                bool tRigth = !points.Contains(item.Right());
                if(tUp)
                {
                    if (tLeft)
                    {
                        t = item.UpLeft();
                        if (!points.Contains(t.Left()) && !points.Contains(t.Up()))
                            anglePoints.Add(t);
                    }
                    if (tRigth)
                    {
                        t = item.UpRight();
                        if (!points.Contains(t.Right()) && !points.Contains(t.Up()))
                            anglePoints.Add(t);
                    }
                }
                if(tDown)
                {
                    if (tLeft)
                    {
                        t = item.DownLeft();
                        if (!points.Contains(t.Left()) && !points.Contains(t.Down()))
                            anglePoints.Add(t);
                    }
                    if (tRigth)
                    {
                        t = item.DownRight();
                        if (!points.Contains(t.Right()) && !points.Contains(t.Down()))
                            anglePoints.Add(t);
                    }
                }
            }
            int Xmin, Xmax, Ymin, Ymax;
            Point first = points.First();
            Xmin = Xmax = first.X;
            Ymin = Ymax = first.Y;
            foreach (var item in points.Skip(1))
            {
                if (Xmin > item.X) Xmin = item.X;
                else if (Xmax < item.X) Xmax = item.X;
                if (Ymin > item.Y) Ymin = item.Y;
                else if (Ymax < item.Y) Ymax = item.Y;
            }
            size = new Size(Xmax - Xmin + 1, Ymax - Ymin + 1);
        }

        public enum Rotate
        {
            r0,
            r90,
            r180,
            r270,
            vertical,
            horizontal
        }



        public Prefab GetRotatedPrefab(Rotate rotate)
        {
            Point[] points = new Point[this.Points.Length];
            Point source;
            int Width = size.Width - 1;
            int Height = size.Height - 1;
            switch (rotate)
            {
                case Rotate.r90:
                    for (int i = 0; i < points.Length; i++)
                    {
                        source = this.points[i];
                        points[i] = new Point(Height - source.Y, source.X);
                    }
                    break;
                case Rotate.r180:
                    for (int i = 0; i < points.Length; i++)
                    { 
                        source = this.points[i];
                        points[i] = new Point(Width - source.X -1, source.X);
                    }
                    break;
                case Rotate.r270:
                    for (int i = 0; i < points.Length; i++)
                    {
                        source = this.points[i];
                        points[i] = new Point(source.Y, Width - source.X);
                    }
                    break;
                case Rotate.vertical:
                    for (int i = 0; i < points.Length; i++)
                    {
                        source = this.points[i];
                        points[i] = new Point(source.X, Height - source.Y);
                    }
                    break;
                case Rotate.horizontal:
                    for (int i = 0; i < points.Length; i++)
                    {
                        source = this.points[i];
                        points[i] = new Point(Width - source.X, source.Y);
                    }
                    break;
                default:
                    this.points.CopyTo(points, 0);
                    break;
            }
            return new Prefab(points);
        }
    }

    public class Map
    {
        public class HorizontalNode
        {
            Node zero;
            public Node Zero => zero;
            public int Index;
            public HorizontalNode Left, Right;
        }

        public class Node
        {
            public int Index;
            public HorizontalNode Parent;
            public Node Up;
            public Node Down;
        }

        Node zero;
        public Node Zero => zero;
        
    }
}

