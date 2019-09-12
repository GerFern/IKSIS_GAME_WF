using Cairo;
using GameCore;
using Gdk;
using Gtk;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Border = GameCore.Border;
using Color = Cairo.Color;
using Point = Cairo.Point;
using Rectangle = Cairo.Rectangle;

namespace gtk_test
{
    class GameWidgetOver
    {
        public string message = "Ёё!&;Hello, РщЪъцЦ<>q#@";
        public DrawingArea DrawingArea { get; }
        public PointD Cursor { get; private set; }
        public PointD Location { get; private set; }
        public bool Is3D
        {
            get => is3D;
            set
            {
                is3D = value;
                DrawingArea.QueueDraw();
            }
        }
        public int HeigthCell
        {
            get => heigthCell;
            set
            {
                heigthCell = value;
                DrawingArea.QueueDraw();
            }
        }
        public GameCore.Game Game
        {
            get => game;
            set
            {
                game = value;
                game.GameOver += Game_GameOver;
            }
        }

        Cairo.Color? gameWinner;

        private void Game_GameOver(object sender, EventArgs e)
        {
            gameWinner = null;
            if (game.WinnerPlayers.Count == 0 || game.WinnerPlayers.Count >= game.Players.Count) message = "Ничья";
            else if (game.WinnerPlayers.Count == 1)
            {
                var player = ClientManager.Players.Where(a => a.Value.Index == game.WinnerPlayers[0]).First().Value;
                if (game.PlayerIndex == game.WinnerPlayers[0]) message = "Вы выиграли";
                else message = $"{player.Name} выиграл";

                gameWinner = player.Color.ToCairoColor(0.7);
            }

            else
            {
                var names = ClientManager.Players.Where(a => game.WinnerPlayers.Contains(a.Value.Index)).Select(a => a.Value.Name);
                foreach (var item in names.Take(names.Count() - 1))
                {
                    message += item + ", ";
                }
                message += names.Last() + " выиграли";
            }
        }

        public GameCore.Interfaces.ClientManager ClientManager { get; private set; }
        public int SelectedPrefabID
        {
            get => _selectedPrefabID;
            set
            {
                SelectedPrefab = Game.Prefabs[value].Prefab;
                Rotate = Prefab.Rotate.r0;
                //RotatedPrefab = SelectedPrefab.GetRotatedPrefab();
                _selectedPrefabID = value;
            }
        }
        public Prefab SelectedPrefab { get; private set; }
        public Prefab RotatedPrefab { get; private set; }
        public Prefab.Rotate Rotate
        {
            get => _rotate;
            set
            {
                _rotate = value;
                if (SelectedPrefab != null)
                    RotatedPrefab = SelectedPrefab.GetRotatedPrefab(value);
            }
        }
        public double Scale
        {
            get => scale;
            set
            {
                var oldval = scale;
                scale = value;
                if (oldval != value)
                    OnScaleChanged();
                LimitTranslate();
                DrawingArea.QueueDraw();
            }
        }

        public void OnScaleChanged() { ScaleChanged?.Invoke(this, EventArgs.Empty); }
        public event EventHandler ScaleChanged;

        public double TranslateX { get; set; } = 10;
        public double TranslateY { get; set; } = 10;
        public Cairo.Rectangle ViewRect { get; private set; }
        public PointD Center { get; private set; }
        bool move = false;
        double oldX;
        double oldY;

        public bool Control { get; set; } = false;
        public List<Cairo.Point> SelectedPoints { get; } = new List<Cairo.Point>();
        Color _backgroundColor = new Color(0.2, 0.9, 0.5, 1);
        private Prefab.Rotate _rotate;
        private int _selectedPrefabID;
        private double scale = 1;
        private Game game;
        private bool is3D;
        private int heigthCell;

        public Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                DrawingArea.QueueDraw();
            }
        }
        public GameWidgetOver(DrawingArea drawingArea)
        {
            DrawingArea = drawingArea;
            drawingArea.AddEvents((int)(EventMask.ScrollMask | EventMask.ButtonMotionMask | EventMask.ButtonPressMask | EventMask.ButtonReleaseMask | EventMask.KeyPressMask | EventMask.KeyReleaseMask | EventMask.PointerMotionMask));
            drawingArea.ScrollEvent += new ScrollEventHandler((o, e) =>
             {
                 if (Game == null) return;
                 if (Control)
                 {
                     if (e.Event.Direction == ScrollDirection.Down)
                         Scale -= 0.1;
                     else if (e.Event.Direction == ScrollDirection.Up)
                         Scale += 0.1;
                     if (Scale < 0.1) Scale = 0.1;
                     if (Scale > 3) Scale = 3;
                     Location = FixPoint(Cursor);
                     UpdateViewRect();
                 }
                 else
                 {
                     if (e.Event.Direction == ScrollDirection.Down)
                     {
                         Rotate = Rotate.AddRotate(Prefab.Rotate.r270);
                     }
                     else if (e.Event.Direction == ScrollDirection.Up)
                     {
                         Rotate = Rotate.AddRotate(Prefab.Rotate.r90);
                     }
                     UpdateSelectionPoints();
                 }
                 UpdateViewRect();
                 LimitTranslate();
                 drawingArea.QueueDraw();
                 System.Diagnostics.Debug.WriteLine(Scale);
             });
            drawingArea.Drawn += new DrawnHandler((o, e) =>
            {
                if (Game == null) return;

                bool test = true;
                Console.WriteLine($"{Center.X} {Center.Y}");
                Context gr = e.Cr;
                drawingArea.CreatePangoContext();
                gr.Scale(Scale, Scale);
                gr.Translate(TranslateX, TranslateY);
                gr.Antialias = Antialias.Subpixel;
                gr.Rectangle(new Cairo.Rectangle(0, 0, Game.Columns * 50, Game.Rows * 50));
                gr.Color = BackgroundColor;
                gr.Color = new Color(0.4, 0.5, 0.8, 1);
                gr.Fill();
                gr.Color = new Color(0, 0, 0, 1);
                gr.LineWidth = 10;
                //gr.LineJoin = LineJoin.Round;
                gr.LineCap = LineCap.Round;
                //DrawGrid(gr, 50, 50, 5, 5);
                //gr.Stroke();
                //DrawCell(gr, 1, 1, 50, 50, 10, new Color(0,0,1, 0.75));
                //DrawCell(gr,0, 2, 50, 50, 10, new Color(1, 0, 0, 0.75));
                //DrawCell(gr, 1, 2, 50, 50, 10, new Color(0, 0, 1, 0.75));

                DrawGrid(gr, 50, 50, Game.Columns, Game.Rows);
                if (is3D)
                {
                    List<(int col, int row, int owner, int dist, Border border, bool selected)> vs = new List<(int, int, int, int, Border, bool)>();
                    Point c = GetPosition(Center);
                    for (int i = 0; i < Game.Columns; i++)
                    {
                        for (int j = 0; j < Game.Rows; j++)
                        {
                            Cell cell = Game[i, j];
                            int p = cell.PlayerOwner;
                            bool selected = SelectedPoints.Contains(new Cairo.Point(i, j));
                            if (p != 0)
                            {
                                vs.Add((i, j, p, Math.Abs(c.X - i) + Math.Abs(c.Y - j), cell.Border, selected));
                            }
                            else if (selected)
                            {
                                DrawEmptySelectedCell(gr, i, j, 50, 50, 0);
                            }
                        }
                    }
                    foreach (var (col, row, owner, dist, border, selected) in vs.OrderByDescending(a => a.dist))
                    {
                        System.Drawing.Color color = Game.Players[owner].PlayerCell;
                        Color color1 = new Color(((double)color.R) / 255, ((double)color.G) / 255, ((double)color.B) / 255, 0.65);
                        DrawCell(gr, col, row, 50, 50, heigthCell, color1, border, selected, ViewRect, Center);
                    }
                }
                else
                {
                    for (int i = 0; i < Game.Columns; i++)
                    {
                        for (int j = 0; j < Game.Rows; j++)
                        {
                            Cell cell = Game[i, j];
                            int p = cell.PlayerOwner;
                            bool selected = SelectedPoints.Contains(new Cairo.Point(i, j));
                            if (p != 0)
                            {
                                System.Drawing.Color color = Game.Players[p].PlayerCell;
                                Color color1 = new Color(((double)color.R) / 255, ((double)color.G) / 255, ((double)color.B) / 255, 0.65);
                                DrawCell(gr, i, j, 50, 50, heigthCell, color1, cell.Border, selected);
                                if (selected)
                                    DrawEmptySelectedCell(gr, i, j, 50, 50, heigthCell);
                            }
                            else if (selected)
                            {
                                DrawEmptySelectedCell(gr, i, j, 50, 50, 0);
                            }
                        }
                    }
                }
                //DrawEmptySelectedCell(gr, 5, 5, 50, 50, 0);
                //TODO: GameLogic

                gr.Translate(-TranslateX, -TranslateY);
                var sc = 1 / Scale;
                gr.Scale(sc, sc);
                //Pango.Renderer renderer = new Pango.Renderer();

                if (!Game.IsGameEnd)
                    DrawGameState(gr, drawingArea.Allocation.Size, 0.3, 0.7, message, Game.Players[Game.CurrentPlayer].PlayerCell.ToCairoColor(0.7));
                else
                {
                    DrawGameState(gr, drawingArea.Allocation.Size, 0.3, 0.7, message, gameWinner);
                }
            });
            drawingArea.MotionNotifyEvent += DrawingArea_MotionNotifyEvent;
            drawingArea.ButtonPressEvent += DrawingArea_ButtonPressEvent;
            drawingArea.ButtonReleaseEvent += DrawingArea_ButtonReleaseEvent;
            UpdateViewRect();
        }

        private void DrawingArea_ButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
        {
            var evnt = args.Event;
            if (evnt.Button == 2) move = false;
        }

        private void DrawingArea_MotionNotifyEvent(object o, MotionNotifyEventArgs args)
        {
            var evnt = args.Event;
            Cursor = new PointD(evnt.X, evnt.Y);
            Location = FixPoint(Cursor);
            UpdateViewRect();
            if (Game == null) return;
            if (move)
            {
                double dX = evnt.X - oldX;
                double dY = evnt.Y - oldY;
                TranslateX += dX / Scale;
                TranslateY += dY / Scale;
                oldX = evnt.X;
                oldY = evnt.Y;
                UpdateViewRect();
                LimitTranslate();
                System.Diagnostics.Debug.WriteLine($"{TranslateX} {TranslateY}");
                DrawingArea.QueueDraw();
            }
            else
            {
                //Prefab prefab = new Prefab(new System.Drawing.Point[]
                //{
                //    new System.Drawing.Point(0,0),
                //    new System.Drawing.Point(0,1),
                //});
                UpdateSelectionPoints();
                DrawingArea.QueueDraw();
            }
        }
        private void DrawingArea_ButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            var evnt = args.Event;
            if (Game == null) return;
            if (!Game.AllowSetPrefab) return;
            if (evnt.Button == 1)
            {
                if (RotatedPrefab == null) return;
                PointD pointD = Location;
                float dx = (float)(RotatedPrefab.Size.Width - 1) / 2;
                float dy = (float)(RotatedPrefab.Size.Height - 1) / 2;
                pointD.X -= dx * 50;
                pointD.Y -= dy * 50;
                Point point = GetPosition(pointD);
                if (point.X >= 0 && point.Y >= 0 && point.X + RotatedPrefab.Size.Width <= Game.Columns && point.Y + RotatedPrefab.Size.Height <= Game.Rows)
                {
                    System.Drawing.Point location = new System.Drawing.Point(point.X, point.Y);

                    if (ClientManager == null)
                    {
                        Game.SetPrefab(SelectedPrefabID, Rotate, location, out _, 0, true, false);
                    }
                    else
                        ClientManager.Server.Place(SelectedPrefabID, location, Rotate);
                }
            }
            else if (evnt.Button == 2)
            {
                move = true;
                oldX = evnt.X;
                oldY = evnt.Y;
            }
            else if (evnt.Button == 3)
            {
                Rotate = Rotate.AddRotate(Prefab.Rotate.vertical);
                UpdateSelectionPoints();
                DrawingArea.QueueDraw();
            }
        }

        //public GameWidget(IntPtr raw):base(raw) { }
        private void DrawEmptySelectedCell(Context context, int column, int row, int dcolumn, int drow, int h)
        {
            double LineWidth = 6 / 2;
            double LineWidth2 = LineWidth + LineWidth;
            Cairo.Rectangle rectangle =
                new Cairo.Rectangle(column * dcolumn + LineWidth - h,
                                    row * drow + LineWidth - h,
                                    dcolumn - LineWidth2,
                                    drow - LineWidth2);
            //LinearGradient linearGradient = new LinearGradient(rectangle.X, rectangle.Y,
            //rectangle.X + rectangle.Width, rectangle.Y + rectangle.Width);
            //SolidPattern solidPattern = new SolidPattern(0.75, 0.75, 0.75, 0.8);
            //Cairo.Rectangle r = new Cairo.Rectangle(0, 0, 100, 100);
            context.Rectangle(rectangle);
            //context.SetSource(solidPattern);
            context.Color = new Color(0.75, 0.75, 0.75, 0.8);
            context.Fill();
            //Color white = new Color(0.5, 0.5, 0.5, 1);
            //Color transparent = new Color(0, 0, 0, 0);
            //linearGradient.AddColorStop(0.1, transparent);
            //linearGradient.AddColorStop(0.2, white);
            //linearGradient.AddColorStop(0.3, transparent);
            //linearGradient.AddColorStop(0.4, white);
            //linearGradient.AddColorStop(0.5, transparent);
            //linearGradient.AddColorStop(0.6, white);
            //linearGradient.AddColorStop(0.7, transparent);
            //linearGradient.AddColorStop(0.8, white);
            //linearGradient.AddColorStop(0.9, transparent);
            //linearGradient.AddColorStop(1, white);
            //context.Rectangle(rectangle);
            //context.SetSource(linearGradient);
            //context.Fill();
        }

        //protected override bool OnMotionNotifyEvent(EventMotion evnt)
        //{
        //    Cursor = new PointD(evnt.X, evnt.Y);
        //    Location = FixPoint(Cursor);
        //    UpdateViewRect();
        //    if (Game == null) base.OnMotionNotifyEvent(evnt);
        //    if (move)
        //    {
        //        double dX = evnt.X - oldX;
        //        double dY = evnt.Y - oldY;
        //        TranslateX += dX / Scale;
        //        TranslateY += dY / Scale;
        //        oldX = evnt.X;
        //        oldY = evnt.Y;
        //        UpdateViewRect();
        //        LimitTranslate();
        //        System.Diagnostics.Debug.WriteLine($"{TranslateX} {TranslateY}");
        //        QueueDraw();
        //    }
        //    else
        //    {
        //        //Prefab prefab = new Prefab(new System.Drawing.Point[]
        //        //{
        //        //    new System.Drawing.Point(0,0),
        //        //    new System.Drawing.Point(0,1),
        //        //});
        //        UpdateSelectionPoints();
        //        this.QueueDraw();
        //    }
        //    return base.OnMotionNotifyEvent(evnt);
        //}
        void UpdateSelectionPoints()
        {
            if (RotatedPrefab != null)
            {

                PointD pointD = Location;
                float dx = (float)(RotatedPrefab.Size.Width - 1) / 2;
                float dy = (float)(RotatedPrefab.Size.Height - 1) / 2;
                pointD.X -= dx * 50;
                pointD.Y -= dy * 50;
                Point point = GetPosition(pointD);

                //if (point.X < 0) point.X = 0;
                //else if (point.X > Game.Columns - prefab.Size.Width)
                //    point.X = Game.Columns - prefab.Size.Width;
                //if (point.Y < 0) point.Y = 0;
                //else if (point.Y > Game.Rows - prefab.Size.Height)
                //    point.Y = Game.Rows - prefab.Size.Height;

                if (point.X < 0
                    || point.Y < 0
                    || point.X > Game.Columns - RotatedPrefab.Size.Width
                    || point.Y > Game.Rows - RotatedPrefab.Size.Height)
                {
                    SelectedPoints.Clear();
                    return;
                }


                SelectedPoints.Clear();
                SelectedPoints.AddRange(RotatedPrefab.GetPointsCursors(new System.Drawing.Point(point.X, point.Y)).Select(a =>
                {
                    return new Point(a.Point.X, a.Point.Y);
                }));
                Console.WriteLine($"{point.X} {point.Y}");
            }
        }
        private void LimitTranslate()
        {
            const int delta = 150;
            double XLimit = -((Game.Rows * 50)) - (delta - DrawingArea.Allocation.Width) / Scale;
            double YLimit = -((Game.Rows * 50)) - (delta - DrawingArea.Allocation.Height) / Scale;
            if (TranslateX < XLimit) TranslateX = XLimit;
            if (TranslateY < YLimit) TranslateY = YLimit;
            if (TranslateX > delta / Scale) TranslateX = delta / Scale;
            if (TranslateY > delta / Scale) TranslateY = delta / Scale;
        }

        private void UpdateViewRect()
        {
            var alloc = DrawingArea.Allocation;
            ViewRect = new Cairo.Rectangle(-TranslateX, -TranslateY, alloc.Width / Scale, alloc.Height / Scale);
            Center = new PointD((ViewRect.X + ViewRect.X + ViewRect.Width) / 2, (ViewRect.Y + ViewRect.Y + ViewRect.Height) / 2);
            //Center = Location;
        }

        public void SetClientManager(GameCore.Interfaces.ClientManager clientManager)
        {
            ClientManager = clientManager;
            Game = ClientManager.Game;
            Game.PrefabPlaced += Game_PrefabPlaced;
            ClientManager.EventChangePlayer += ClientManager_EventChangePlayer;
            ClientManager_EventChangePlayer(Game.CurrentPlayer);
            ClientManager.EventPlace += ClientManager_EventPlace;
        }

        private void ClientManager_EventChangePlayer(int obj)
        {
            if (Game.PlayerIndex == obj) message = "Ваш ход";
            else message = $"Ход игрока {ClientManager.Players.Where(a => a.Value.Index == obj).First().Value.Name}";
        }

        private void Game_PrefabPlaced(object sender, Game.EventArgsPrefab e)
        {
            Application.Invoke(new EventHandler((o, _e) =>
            {
                DrawingArea.QueueDraw();
            }));
        }

        private void ClientManager_EventPlace(int arg1, int arg2, System.Drawing.Point arg3, Prefab.Rotate arg4)
        {
        }

        //protected override bool OnButtonPressEvent(EventButton evnt)
        //{
        //    if (Game == null) return base.OnButtonPressEvent(evnt);
        //    if(evnt.Button == 1)
        //    { 
        //        if(RotatedPrefab==null) return base.OnButtonPressEvent(evnt);
        //        PointD pointD = Location;
        //        float dx = (float)(RotatedPrefab.Size.Width - 1) / 2;
        //        float dy = (float)(RotatedPrefab.Size.Height - 1) / 2;
        //        pointD.X -= dx * 50;
        //        pointD.Y -= dy * 50;
        //        Point point = GetPosition(pointD);
        //        if(point.X>=0&&point.Y>=0&&point.X+RotatedPrefab.Size.Width<=Game.Columns&&point.Y + RotatedPrefab.Size.Height<=Game.Rows)
        //        Game.SetPrefab(SelectedPrefabID, Rotate, new System.Drawing.Point(point.X, point.Y), out _, 1, true, false);
        //    }
        //    else if (evnt.Button == 2)
        //    {
        //        move = true;
        //        oldX = evnt.X;
        //        oldY = evnt.Y;
        //    }
        //    else if(evnt.Button == 3)
        //    {
        //        Rotate = Rotate.AddRotate(Prefab.Rotate.vertical);
        //        UpdateSelectionPoints();
        //        QueueDraw();
        //    }
        //    return base.OnButtonPressEvent(evnt);
        //}

        //protected override bool OnButtonReleaseEvent(EventButton evnt)
        //{
        //    if (evnt.Button == 2) move = false;
        //    return base.OnButtonReleaseEvent(evnt);
        //}


        //protected override bool OnScrollEvent(EventScroll evnt)
        //{
        //    if (evnt.Direction == ScrollDirection.Down)
        //        Scale -= 0.1;
        //    else if (evnt.Direction == ScrollDirection.Up)
        //        Scale += 0.1;
        //    QueueDraw();
        //    //return true;
        //    return base.OnScrollEvent(evnt);
        //}
        public static void DrawGameState(Context context, Size size, double aspect, double opacity, string text, Color? cell)
        {
            //text = "w";
            context.SelectFontFace("Sans", FontSlant.Normal, FontWeight.Normal);
            FontFace ffSans = context.GetContextFontFace();
            Matrix fm = new Matrix(/*font size*/ 30.0, 0.0, 0.0, /*font size*/ 30.0,
                                            /*translationX*/ 0.0, /*translationY*/ 0.0);
            Matrix tm = new Matrix(1, 0.0, 0.0, 1.0, 0.0, 0.0);
            FontOptions fo = new FontOptions();
            ScaledFont sfSans = new ScaledFont(ffSans, fm, tm, fo);
            context.SetScaledFont(sfSans);

            var te = context.TextExtents(text);


            double x = (size.Width - te.Width - 20) / 2;
            double y = size.Height - te.Height - 50;
            double width = te.Width + 20;
            double height = 30 + 10;
            const double degrees = Math.PI / 180;
            double corner_radius = height / 10;
            double radius = corner_radius / aspect;

            if (cell.HasValue) width += 50;


            context.NewSubPath();
            context.Arc(x + width - radius, y + radius, radius, -90 * degrees, 0);
            context.Arc(x + width - radius, y + height - radius, radius, 0 * degrees, 90 * degrees);
            context.Arc(x + radius, y + height - radius, radius, 90 * degrees, 180 * degrees);
            context.Arc(x + radius, y + radius, radius, 180 * degrees, 270 * degrees);
            context.ClosePath();

            context.SetSourceColor(new Color(0.2, 0.2, 0.2, opacity));
            context.FillPreserve();

            //var layout =
            //new Pango.Layout(pango_c);
            //Pango.FontDescription fontDescription = Pango.FontDescription.FromString("Sans Bold 27");
            //layout.FontDescription = fontDescription;
            //layout.SetText(text);


            context.SetSourceColor(new Color(1, 1, 1, opacity));
            context.MoveTo(x + 10, y + 30 );
            context.ShowText(text);


            ffSans.Dispose();
            sfSans.Dispose();

            if (cell.HasValue)
            {
                double tx = x + width - 45, ty = y + height - 32;
                context.Translate(tx, ty);
                //DrawGrid(context, 40, 40, 1, 1);
                DrawCell(context, 0, 0, 30, 30, 5, cell.Value, Border.All, false);
                context.Translate(-tx, -ty);
            }
            //Cairo.Glyph[] glyphs;
            //context.
            // .ScaledFontTextToGlyphs(sfSans,
            //    "the self-provided converter: μ-∑-√-‡-€-™", 15, 295, out glyphs);
            //// Draw the text from converted glyphs.
            //Cairo.CairoWrapper.ShowGlyphs(context, glyphs);
            ////context.ShowText("AAA");
        }

        public static void DrawGrid(Context context, int dcolumn, int drow, int columns, int rows)
        {
            int heigth, weight;
            heigth = drow * rows;
            weight = dcolumn * columns;
            context.LineWidth = 6;
            for (int i = 0; i <= columns; i++)
            {
                context.MoveTo(i * dcolumn, 0);
                context.LineTo(i * dcolumn, heigth);
            }
            for (int i = 0; i <= rows; i++)
            {
                context.MoveTo(0, i * drow);
                context.LineTo(weight, i * drow);
            }
            context.Stroke();
        }

        public static void DrawCell(Context context, int column, int row, int dcolumn, int drow, int h, Color color, Border border, bool selected)
        {
            double LineWidth = 6 / 2;
            double LineWidth2 = LineWidth + LineWidth;
            Cairo.Rectangle rectangle =
                new Cairo.Rectangle(column * dcolumn + LineWidth,
                                    row * drow + LineWidth,
                                    dcolumn - LineWidth2,
                                    drow - LineWidth2);
            Cairo.Rectangle rectangleUp =
                new Cairo.Rectangle(rectangle.X - h,
                                    rectangle.Y - h,
                                    rectangle.Width,
                                    rectangle.Height);
            DrawCell(context, rectangle, rectangleUp, color, border, selected);
        }

        public static void DrawCell(Context context, int column, int row, int dcolumn, int drow, int h, Color color, Border border, bool selected, Rectangle viewRect, PointD center)
        {
            double LineWidth = 6 / 2;
            double LineWidth2 = LineWidth + LineWidth;
            Rectangle rectangle =
                new Rectangle(column * dcolumn + LineWidth,
                                    row * drow + LineWidth,
                                    dcolumn - LineWidth2,
                                    drow - LineWidth2);
            double x1, x2, y1, y2, /*xv1, xv2, yv1, yv2*/ ww, hh, tx1, ty1, tx2, ty2;
            x1 = rectangle.X;
            x2 = x1 + rectangle.Width;
            y1 = rectangle.Y;
            y2 = y1 + rectangle.Height;
            //xv1 = viewRect.X;
            //xv2 = xv1 + viewRect.Width;
            //yv1 = viewRect.Y;
            //yv2 = yv1 + viewRect.Y;

            ww = center.X;
            hh = center.Y;

            tx1 = (x1 - ww) / Math.Abs(ww);
            ty1 = (y1 - hh) / Math.Abs(hh);
            tx2 = (x2 - ww) / Math.Abs(ww);
            ty2 = (y2 - hh) / Math.Abs(hh);
            Rectangle rectangleUp =
                new Rectangle(x1 + h * tx1,
                              y1 + h * ty1,
                               (x2 + tx2 * h) - (x1 + h * tx1),
                              (y2 + ty2 * h) - (y1 + h * ty1));
            DrawCell(context, rectangle, rectangleUp, color, border, selected);
        }

        public static void DrawCell(Context context, Cairo.Rectangle rectangle, Cairo.Rectangle rectangleUp, Color color, Border border, bool selected)
        {
            context.Antialias = Antialias.Subpixel;
            context.LineJoin = LineJoin.Round;
            context.LineWidth = 1.5;
            Color gray = new Color(0.2, 0.2, 0.2, 0.95);
            context.SetSourceColor(gray);

            context.Rectangle(rectangle);
            context.Stroke();

            Ang ang = Ang.None;
            Ang sep = Ang.None;
            if (border.HasFlag(Border.Rigth))
            {
                sep |= Ang.LeftUp | Ang.LeftDown;
            }
            if (border.HasFlag(Border.All))
            {
                sep |= Ang.RightDown | Ang.LeftDown;
            }
            if (border.HasFlag(Border.Left))
            {
                sep |= Ang.RightDown | Ang.RightUp;
            }
            if (border.HasFlag(Border.Down))
            {
                sep |= Ang.LeftUp | Ang.RightUp;
            }

            if (rectangle.X > rectangleUp.X)
            {
                if (rectangle.Y > rectangleUp.Y)
                {
                    ang |= Ang.LeftUp;
                    //context.MoveTo(rectangle.LeftUp());
                    //context.LineTo(rectangleUp.LeftUp());
                    //context.Stroke();
                }
                if (rectangle.Y + rectangle.Height < rectangleUp.Y + rectangleUp.Height)
                {
                    ang |= Ang.LeftDown;
                    //context.MoveTo(rectangle.LeftDown());
                    //context.LineTo(rectangleUp.LeftDown());
                    //context.Stroke();
                }
            }
            if (rectangle.X + rectangle.Width < rectangleUp.X + rectangleUp.Width)
            {
                if (rectangle.Y > rectangleUp.Y)
                {
                    ang |= Ang.RightUp;
                    //context.MoveTo(rectangle.RightUp());
                    //context.LineTo(rectangleUp.RightUp());
                    //context.Stroke();
                }
                if (rectangle.Y + rectangle.Height < rectangleUp.Y + rectangleUp.Height)
                {
                    ang |= Ang.RightDown;
                    //context.MoveTo(rectangle.RightDown());
                    //context.LineTo(rectangleUp.RightDown());
                    //context.Stroke();
                }
            }

            const double H = 1;
            DrawCellLine(context, rectangle, rectangleUp, color, ang, sep, H);

            Storona s = Storona.None;

            if (!ang.HasFlag(Ang.LeftUp))
            {
                if (!ang.HasFlag(Ang.LeftDown))
                {
                    s |= Storona.Left;
                }
                if (!ang.HasFlag(Ang.RightUp))
                {
                    s |= Storona.Up;
                }
            }
            if (!ang.HasFlag(Ang.RightDown))
            {
                if (!ang.HasFlag(Ang.LeftDown))
                {
                    s |= Storona.Down;
                }
                if (!ang.HasFlag(Ang.RightUp))
                {
                    s |= Storona.Right;
                }
            }
            if (s.HasFlag(Storona.Left))
            {
                context.MoveTo(rectangle.LeftUp());
                context.LineTo(rectangle.LeftDown());
                context.LineTo(rectangleUp.LeftDown());
                context.LineTo(rectangleUp.LeftUp());
                context.ClosePath();
            }
            if (s.HasFlag(Storona.Up))
            {
                context.MoveTo(rectangle.LeftUp());
                context.LineTo(rectangle.RightUp());
                context.LineTo(rectangleUp.RightUp());
                context.LineTo(rectangleUp.LeftUp());
                context.ClosePath();
            }
            if (s.HasFlag(Storona.Right))
            {
                context.MoveTo(rectangle.RightUp());
                context.LineTo(rectangle.RightDown());
                context.LineTo(rectangleUp.RightDown());
                context.LineTo(rectangleUp.RightUp());
                context.ClosePath();
            }
            if (s.HasFlag(Storona.Down))
            {
                context.MoveTo(rectangle.RightDown());
                context.LineTo(rectangle.LeftDown());
                context.LineTo(rectangleUp.LeftDown());
                context.LineTo(rectangleUp.RightDown());
                context.ClosePath();
            }
            context.Rectangle(rectangleUp);
            context.Color = color;
            context.Fill();
            if (selected)
            {
                context.Rectangle(rectangleUp);
                context.Color = new Color(0.75, 0.75, 0.75, 0.5);
                context.Fill();
            }
            context.Color = gray;
            context.Rectangle(rectangleUp);
            context.Stroke();
            ang ^= Ang.All;
            DrawCellLine(context, rectangle, rectangleUp, color, ang, sep, H);
            FillCellPath(context, rectangle, rectangleUp, color, border, H);
            //if (!ang.HasFlag(Ang.LeftUp))
            //{
            //    context.MoveTo(rectangle.LeftUp());
            //    context.LineTo(rectangleUp.LeftUp());
            //}
            //if (!ang.HasFlag(Ang.LeftDown))
            //{
            //    context.MoveTo(rectangle.LeftDown());
            //    context.LineTo(rectangleUp.LeftDown());
            //}
            //if (!ang.HasFlag(Ang.RightUp))
            //{
            //    context.MoveTo(rectangle.RightUp());
            //    context.LineTo(rectangleUp.RightUp());
            //}
            //if (!ang.HasFlag(Ang.RightDown))
            //{
            //    context.MoveTo(rectangle.RightDown());
            //    context.LineTo(rectangleUp.RightDown());
            //}
            context.Stroke();

            //context.MoveTo(rectangleUp.LeftUp());
            //context.LineTo(rectangle.LeftUp());
            //context.MoveTo(rectangle.LeftDown());
            //context.LineTo(rectangle.LeftUp());
            //context.LineTo(rectangle.RightUp());
            //context.Stroke();

            //context.SetSourceColor(color);
            //context.MoveTo(rectangleUp.LeftDown());
            //context.LineTo(rectangleUp.RightDown());
            //context.LineTo(rectangleUp.RightUp());
            //context.LineTo(rectangle.RightUp());
            //context.LineTo(rectangle.RightDown());
            //context.LineTo(rectangle.LeftDown());
            //context.ClosePath();

            //context.Rectangle(rectangle);
            ////context.Fill();

            //context.Rectangle(rectangleUp);
            //context.Fill();


            ////context.Rectangle(rectangle);




            //context.SetSourceColor(new Color(0.2, 0.2, 0.2, 0.95));
            //context.MoveTo(rectangleUp.LeftDown());
            //context.LineTo(rectangle.LeftDown());
            //context.MoveTo(rectangleUp.RightDown());
            //context.LineTo(rectangle.RightDown());
            //context.MoveTo(rectangleUp.RightUp());
            //context.LineTo(rectangle.RightUp());
            //context.Rectangle(rectangleUp);
            //context.MoveTo(rectangle.LeftDown());
            //context.LineTo(rectangle.RightDown());
            //context.LineTo(rectangle.RightUp());
            //context.Stroke();
        }

        public static void FillCellPath(Context context, Rectangle rectangle, Rectangle rectangleUp, Color color, Border border, double h)
        {
            context.Color = color;
            if (border.HasFlag(Border.Rigth))
            {
                if (border.HasFlag(Border.Down))
                {

                }
            }
        }

        public static void DrawCellLine(Context context, Cairo.Rectangle rectangle, Cairo.Rectangle rectangleUp, Color color, Ang linedraw, Ang linesep, double h)
        {
            PointD t;
            if (linedraw.HasFlag(Ang.LeftUp))
            {
                if (linesep.HasFlag(Ang.LeftUp))
                {
                    t = rectangleUp.LeftUp();
                    context.MoveTo(t);
                    context.LineTo(t.Middle(rectangle.LeftUp(), h));
                }
                else
                {
                    context.MoveTo(rectangle.LeftUp());
                    context.LineTo(rectangleUp.LeftUp());
                }
            }
            if (linedraw.HasFlag(Ang.LeftDown))
            {
                if (linesep.HasFlag(Ang.LeftDown))
                {
                    t = rectangleUp.LeftDown();
                    context.MoveTo(t);
                    context.LineTo(t.Middle(rectangle.LeftDown(), h));
                }
                else
                {
                    context.MoveTo(rectangle.LeftDown());
                    context.LineTo(rectangleUp.LeftDown());
                }
            }
            if (linedraw.HasFlag(Ang.RightUp))
            {
                if (linesep.HasFlag(Ang.RightUp))
                {
                    t = rectangleUp.RightUp();
                    context.MoveTo(t);
                    context.LineTo(t.Middle(rectangle.RightUp(), h));
                }
                else
                {
                    context.MoveTo(rectangle.RightUp());
                    context.LineTo(rectangleUp.RightUp());
                }
            }
            if (linedraw.HasFlag(Ang.RightDown))
            {
                if (linesep.HasFlag(Ang.RightDown))
                {
                    t = rectangleUp.RightDown();
                    context.MoveTo(t);
                    context.LineTo(t.Middle(rectangle.RightDown(), h));
                }
                else
                {
                    context.MoveTo(rectangle.RightDown());
                    context.LineTo(rectangleUp.RightDown());
                }
            }
            context.Stroke();
        }

        public Cairo.PointD FixPoint(PointD point)
        {
            return new PointD(point.X / Scale - TranslateX, point.Y / Scale - TranslateY);
        }

        public Cairo.Point GetPosition(PointD pointD)
        {
            return new Cairo.Point((int)pointD.X / 50, (int)pointD.Y / 50);
        }


    }

    [Flags]
    public enum Storona
    {
        None = 0,
        Left = 0x1,
        Up = 0x2,
        Right = 0x4,
        Down = 0x8
    }

    [Flags]
    public enum Ang
    {
        None = 0,
        LeftUp = 0x1,
        LeftDown = 0x2,
        RightUp = 0x4,
        RightDown = 0x8,
        All = LeftUp|LeftDown|RightUp|RightDown
    }

    public static class Extensions
    {
        public static PointD LeftUp(this Cairo.Rectangle rectangle) => 
            new PointD(rectangle.X, rectangle.Y);
        public static PointD LeftDown(this Cairo.Rectangle rectangle) => 
            new PointD(rectangle.X, rectangle.Y + rectangle.Height);
        public static PointD RightUp(this Cairo.Rectangle rectangle) => 
            new PointD(rectangle.X + rectangle.Width, rectangle.Y);
        public static PointD RightDown(this Cairo.Rectangle rectangle) =>
            new PointD(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);
        public static PointD Middle(this Cairo.PointD point, PointD other, double h)
        {
            double x = (other.X - point.X) * h + point.X;
            double y = (other.Y - point.Y) * h + point.Y;
            return new PointD(x, y);
        }

        public static Gdk.Color ToGdkColor(this System.Drawing.Color color) =>
            new Gdk.Color(color.R, color.G, color.B);

        public static Cairo.Color ToCairoColor(this System.Drawing.Color color) =>
           new Cairo.Color((double)color.R/256, (double)color.G/256, (double)color.B/256);

        public static Cairo.Color ToCairoColor(this System.Drawing.Color color, double alpha) =>
          new Cairo.Color((double)color.R / 256, (double)color.G / 256, (double)color.B / 256, alpha);


        public static System.Drawing.Color ToSystemColor(this Gdk.Color color) =>
            System.Drawing.Color.FromArgb(color.Red/256, color.Green/256, color.Blue/256);
    }
}
