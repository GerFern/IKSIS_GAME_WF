using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameControl
{
    public static class Extensions
    {
        public static Point Up(this Point point) => new Point(point.X, point.Y + 1);
        public static Point UpLeft(this Point point) => new Point(point.X - 1, point.Y + 1);
        public static Point UpRight(this Point point) => new Point(point.X + 1, point.Y + 1);
        public static Point Down(this Point point) => new Point(point.X, point.Y - 1);
        public static Point DownLeft(this Point point) => new Point(point.X - 1, point.Y - 1);
        public static Point DownRight(this Point point) => new Point(point.X + 1, point.Y - 1);
        public static Point Left(this Point point) => new Point(point.X - 1, point.Y);
        public static Point Right(this Point point) => new Point(point.X + 1, point.Y);
    }
}