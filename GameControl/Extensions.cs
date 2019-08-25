using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCore
{
    public static class Extensions
    {
        public static Point Up(this Point point) => new Point(point.X, point.Y - 1);
        public static Point UpLeft(this Point point) => new Point(point.X - 1, point.Y - 1);
        public static Point UpRight(this Point point) => new Point(point.X + 1, point.Y - 1);
        public static Point Down(this Point point) => new Point(point.X, point.Y + 1);
        public static Point DownLeft(this Point point) => new Point(point.X - 1, point.Y + 1);
        public static Point DownRight(this Point point) => new Point(point.X + 1, point.Y + 1);
        public static Point Left(this Point point) => new Point(point.X - 1, point.Y);
        public static Point Right(this Point point) => new Point(point.X + 1, point.Y);

        public static Point Down(this Point point, int y) => new Point(point.X, point.Y + y);
        public static Point Right(this Point point, int x) => new Point(point.X + x, point.Y);
        public static Point DownRight(this Point point, int x, int y) => new Point(point.X + x, point.Y + y);

        public static Point UpLeft(this Rectangle rectangle) => rectangle.Location;
        public static Point UpRight(this Rectangle rectangle) => new Point(rectangle.X + rectangle.Width, rectangle.Y);
        public static Point DownLeft(this Rectangle rectangle) => new Point(rectangle.X, rectangle.Y + rectangle.Height);
        public static Point DownRight(this Rectangle rectangle) => new Point(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);

        public static Rectangle Move(this Rectangle rectangle, int x, int y) => new Rectangle(rectangle.Location.DownRight(x, y), rectangle.Size);


        public static Prefab.Rotate AddRotate(this Prefab.Rotate rotate, Prefab.Rotate add)
        {
            byte b1 = (byte)rotate;
            byte b2 = (byte)add;
            byte b3 = (byte)((b1 + b2) % 4);
            return (Prefab.Rotate)b3 | ((rotate ^ add) & (Prefab.Rotate.horizontal | Prefab.Rotate.vertical));
        }
    }
}