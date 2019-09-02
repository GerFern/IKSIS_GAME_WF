#define test

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using GameCore.Interfaces;

namespace GameCore
{
    public class PrefabLimit
    {
        public PrefabLimit(Interfaces.PrefabLimit value)
        {
            Count = value.count;
            Prefab = new Prefab(value.prefab.points);
        }

        public PrefabLimit(int count, Prefab prefab)
        {
            Count = count;
            Prefab = prefab ?? throw new ArgumentNullException(nameof(prefab));
        }

        public int Count { get; }
        public Prefab Prefab { get; }
    }

    public class Prefab
    {
        PointWithBorder[,] cells;
        public List<Point> AnglePoints { get; } = new List<Point>();

        //KeyValuePair<Point, Border>[] pointsB;
        Size size;

        public PointWithBorder[] Points { get; }

        public PointWithBorder[] GetPointsCursors(Point plusPoint)
        {
            PointWithBorder[] points = new PointWithBorder[this.Points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                PointWithBorder item = this.Points[i];
                points[i] = new PointWithBorder { Point = new Point(item.Point.X + plusPoint.X, item.Point.Y + plusPoint.Y), Border = item.Border };
            }
            return points;
        }

        public Size Size => size;

        public Prefab(params Point[] points)
        {
            if (points == null || points.Length == 0) throw new Exception();
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
            var orderedPoints = points.Distinct().OrderBy(a => a.X).OrderBy(a => a.Y).GetEnumerator();
            this.Points = new PointWithBorder[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                orderedPoints.MoveNext();
                this.Points[i] = new PointWithBorder { Point = orderedPoints.Current };
            }
            //this.points = points.OrderBy(a => a.X).OrderBy(a => a.Y).ToArray();
            Point[] fixPoints = new Point[this.Points.Length];

            size = new Size(Xmax - Xmin + 1, Ymax - Ymin + 1);
            cells = new PointWithBorder[size.Width + 2, size.Height + 2];

            //pointsB = new KeyValuePair<Point, Border>[points.Length];
            for (int i = 0; i < this.Points.Length; i++)
            {
                fixPoints[i] = this.Points[i].Point.DownRight();
                //Point source = this.points[i];
                //fixPoints[i] = new Point(source.X + 1, source.Y + 1);
                //Для границ
            }

            int index = 0;
            foreach (var item in fixPoints)
            {
                cells[item.X, item.Y] = new PointWithBorder { Point = item, Index = index++ };
            }

            var sPoint = this.Points.Select(a => a.Point).GetEnumerator();
            foreach (var item in fixPoints)
            {
                sPoint.MoveNext();
                Point currentPoint = (Point)sPoint.Current;
                Point tempPoint;
                Point up = item.Up();
                Point down = item.Down();
                Point left = item.Left();
                Point right = item.Right();
                PointWithBorder pointWithBorder = cells[item.X, item.Y];
                bool bUp = cells[up.X, up.Y] == null;
                bool bDown = cells[down.X, down.Y] == null;
                bool bLeft = cells[left.X, left.Y] == null;
                bool bRight = cells[right.X, right.Y] == null;
                if (bUp)
                {
                    pointWithBorder.Border |= Border.Top;
                    if (bLeft)
                    {
                        tempPoint = item.UpLeft();
                        if (!AnglePoints.Contains(tempPoint))
                            AnglePoints.Add(tempPoint);
                    }
                    if (bRight)
                    {
                        tempPoint = item.UpRight();
                        if (!AnglePoints.Contains(tempPoint))
                            AnglePoints.Add(tempPoint);
                    }
                }
                if (bDown)
                {
                    pointWithBorder.Border |= Border.Down;
                    if (bLeft)
                    {
                        tempPoint = item.DownLeft();
                        if (!AnglePoints.Contains(tempPoint))
                            AnglePoints.Add(tempPoint);
                    }
                    if (bRight)
                    {
                        tempPoint = item.DownRight();
                        if (!AnglePoints.Contains(tempPoint))
                            AnglePoints.Add(tempPoint);
                    }
                }
                if (bLeft) pointWithBorder.Border |= Border.Left;
                if (bRight) pointWithBorder.Border |= Border.Rigth;
                this.Points[pointWithBorder.Index].Border = pointWithBorder.Border;
            }

            List<Point> removeAnglePoints = new List<Point>();
            foreach (var item in AnglePoints)
            {
                Point up = item.Up();
                Point down = item.Down();
                Point left = item.Left();
                Point right = item.Right();
                bool bUp;
                if (up.X < 0 || up.X > size.Width || up.Y < 0 || up.Y > size.Height) bUp = false;
                else bUp = cells[up.X, up.Y] != null;
                if (bUp)
                {
                    removeAnglePoints.Add(item);
                    continue;
                }
                bool bDown;
                if (down.X < 0 || down.X > size.Width || down.Y < 0 || down.Y > size.Height) bDown = false;
                else bDown = cells[down.X, down.Y] != null;
                if (bDown)
                {
                    removeAnglePoints.Add(item);
                    continue;
                }
                bool bLeft;
                if (left.X < 0 || left.X > size.Width || left.Y < 0 || left.Y > size.Height) bLeft = false;
                else bLeft = cells[left.X, left.Y] != null;
                if (bLeft)
                {
                    removeAnglePoints.Add(item);
                    continue;
                }
                bool bRight;
                if (right.X < 0 || right.X > size.Width || right.Y < 0 || right.Y > size.Height) bRight = false;
                else bRight = cells[right.X, right.Y] != null;
                if (bRight)
                    removeAnglePoints.Add(item);
            }
            AnglePoints = AnglePoints.Except(removeAnglePoints).ToList();
            for (int i = 0; i < AnglePoints.Count; i++)
            {
                AnglePoints[i] = AnglePoints[i].UpLeft();
            }

            //foreach (var item in points)
            //{
            //    Point t;
            //    bool tUp = !points.Contains(item.Up());
            //    bool tDown = !points.Contains(item.Down());
            //    bool tLeft = !points.Contains(item.Left());
            //    bool tRigth = !points.Contains(item.Right());
            //    if(tUp)
            //    {
            //        if (tLeft)
            //        {
            //            t = item.UpLeft();
            //            if (!points.Contains(t.Left()) && !points.Contains(t.Up()))
            //                anglePoints.Add(t);
            //        }
            //        if (tRigth)
            //        {
            //            t = item.UpRight();
            //            if (!points.Contains(t.Right()) && !points.Contains(t.Up()))
            //                anglePoints.Add(t);
            //        }
            //    }
            //    if(tDown)
            //    {
            //        if (tLeft)
            //        {
            //            t = item.DownLeft();
            //            if (!points.Contains(t.Left()) && !points.Contains(t.Down()))
            //                anglePoints.Add(t);
            //        }
            //        if (tRigth)
            //        {
            //            t = item.DownRight();
            //            if (!points.Contains(t.Right()) && !points.Contains(t.Down()))
            //                anglePoints.Add(t);
            //        }
            //    }
            //}

        }

        [Flags]
        public enum Rotate : byte
        {
            r0 = 0x0,
            r90 = 0x1,
            r180 = 0x2,
            r270 = 0x3,
            vertical = 0x4,
            horizontal = 0x8
        }

        public Prefab GetRotatedPrefab(Rotate rotate)
        {
            Point[] points = new Point[this.Points.Length];
            Point source;
            int Width = size.Width - 1;
            int Height = size.Height - 1;
            const Rotate hor_ver = Rotate.horizontal | Rotate.vertical;
            if (rotate.HasFlag(hor_ver))
            {
                rotate ^= hor_ver;
                if (rotate.HasFlag(Rotate.r180)) rotate ^= Rotate.r180;
                else rotate |= Rotate.r180;
            }
            switch (rotate)
            {
                case Rotate.r90:
                    for (int i = 0; i < points.Length; i++)
                    {
                        source = this.Points[i].Point;
                        points[i] = new Point(Height - source.Y, source.X);
                    }
                    break;
                case Rotate.r180:
                    for (int i = 0; i < points.Length; i++)
                    {
                        source = this.Points[i].Point;
                        points[i] = new Point(Width - source.X, Height - source.Y);
                    }
                    break;
                case Rotate.r270:
                    for (int i = 0; i < points.Length; i++)
                    {
                        source = this.Points[i].Point;
                        points[i] = new Point(source.Y, Width - source.X);
                    }
                    break;
                case Rotate.vertical:
                    for (int i = 0; i < points.Length; i++)
                    {
                        source = this.Points[i].Point;
                        points[i] = new Point(source.X, Height - source.Y);
                    }
                    break;
                case Rotate.horizontal:
                    for (int i = 0; i < points.Length; i++)
                    {
                        source = this.Points[i].Point;
                        points[i] = new Point(Width - source.X, source.Y);
                    }
                    break;
                case Rotate.r90 | Rotate.vertical:
                    for (int i = 0; i < points.Length; i++)
                    {
                        source = this.Points[i].Point;
                        points[i] = new Point(source.Y, source.X);
                    }
                    break;
                case Rotate.r180 | Rotate.vertical:
                    for (int i = 0; i < points.Length; i++)
                    {
                        source = this.Points[i].Point;
                        points[i] = new Point(Width - source.X, source.Y);
                    }
                    break;
                case Rotate.r270 | Rotate.vertical:
                    for (int i = 0; i < points.Length; i++)
                    {
                        source = this.Points[i].Point;
                        points[i] = new Point(Height - source.Y, Width - source.X);
                    }
                    break;
                case Rotate.r90 | Rotate.horizontal:
                    for (int i = 0; i < points.Length; i++)
                    {
                        source = this.Points[i].Point;
                        points[i] = new Point(Height - source.Y, Width - source.X);
                    }
                    break;
                case Rotate.r180 | Rotate.horizontal:
                    for (int i = 0; i < points.Length; i++)
                    {
                        source = this.Points[i].Point;
                        points[i] = new Point(source.X, Height - source.Y);
                    }
                    break;
                case Rotate.r270 | Rotate.horizontal:
                    for (int i = 0; i < points.Length; i++)
                    {
                        source = this.Points[i].Point;
                        points[i] = new Point(source.Y, source.X);
                    }
                    break;
                default:
                    points = this.Points.Select(a => a.Point).ToArray();//.CopyTo(points, 0);
                    break;
            }
            return new Prefab(points);
        }
    }
}

