#define test

using System.Drawing;

namespace GameCore
{
    public abstract class Player
    {
        public virtual Color CellColor { get; set; }
        public virtual Color BorderColor { get; set; }
        //public virtual Color InnerCellColor { get; set; }
        public virtual Color SelectedCellColor { get; set; }
        public void DrawCell(Graphics graphics, Rectangle rectangleBase, Rectangle rectangleUp, ViewMode view, bool selected)
        {
            if (view.HasFlag(ViewMode.Wall)) FillWall(graphics, rectangleBase, rectangleUp);
            if (view.HasFlag(ViewMode.Rectangle))
            {
                if (selected) FillSelectedRectangle(graphics, rectangleBase, rectangleUp);
                else FillUpRectangle(graphics, rectangleBase, rectangleUp);
            }
            if (view.HasFlag(ViewMode.Border)) DrawBorder(graphics, rectangleBase, rectangleUp);
        }
        public abstract void FillWall(Graphics graphics, Rectangle rectangleBase, Rectangle rectangleUp);
        public abstract void FillUpRectangle(Graphics graphics, Rectangle rectangleBase, Rectangle rectangleUp);
        public abstract void DrawBorder(Graphics graphics, Rectangle rectangleBase, Rectangle rectangleUp);
        public abstract void FillSelectedRectangle(Graphics graphics, Rectangle rectangleBase, Rectangle rectangleUp);


        public Image GetImage(Size size, int heigth)
        {
            Bitmap bitmap = new Bitmap(size.Width, size.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            Rectangle rectangleBase = new Rectangle(new Point(heigth, heigth), new Size(size.Width - heigth, size.Height - heigth));
            Rectangle rectangleUp = new Rectangle(Point.Empty, rectangleBase.Size);
            DrawCell(graphics, rectangleBase, rectangleUp, ViewMode.All, false);
            return bitmap;
        }
    }
}

