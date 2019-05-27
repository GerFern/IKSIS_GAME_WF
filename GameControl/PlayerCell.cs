#define test

using System.Drawing;
using System.Drawing.Drawing2D;

namespace GameCore
{
    public class PlayerCell : Player
    {
        Color cellColor;
        Color selectedCellColor;
        Color borderColor;
        //Color innerCellColor;

        Brush cellBrush;
        Pen borderPen;
        //Pen innerCellPen;
        Brush selectedCellBrush;
        //GameControl GameControlParent { get; }

        public override Color CellColor
        {
            get => cellColor;
            set
            {
                cellColor = value;
                cellBrush.Dispose();
                cellBrush = new SolidBrush(cellColor);
                selectedCellBrush.Dispose();
                selectedCellBrush = new HatchBrush(HatchStyle.WideDownwardDiagonal, selectedCellColor, cellColor);
            }
        }
        public override Color BorderColor
        {
            get => borderColor;
            set
            {
                borderColor = value;
                borderPen.Dispose();
                borderPen = new Pen(borderColor);
            }
        }
        //public override Color InnerCellColor
        //{
        //    get => innerCellColor;
        //    set
        //    {
        //        innerCellColor = value;
        //        innerCellPen.Dispose();
        //        innerCellPen = new Pen(innerCellColor);
        //    }
        //}
        public override Color SelectedCellColor
        {
            get => selectedCellColor;
            set
            {
                selectedCellColor = value;
                selectedCellBrush.Dispose();
                selectedCellBrush = new HatchBrush(HatchStyle.WideDownwardDiagonal, selectedCellColor, cellColor);
            }
        }


        public override void DrawBorder(Graphics graphics, Rectangle rectangleBase, Rectangle rectangleUp)
        {
            //Rectangle rectangleCell = GameControlParent.GetRectangle(point);
            //int h = -(GameControlParent.HeightCell);
            //Rectangle rectangleUp = rectangleBase.Move(-heigth, -heigth);
            Point[] ps = new Point[]
            {
                rectangleBase.DownLeft(),
                rectangleBase.DownRight(),
                rectangleBase.UpRight(),
                rectangleUp.UpRight(),
                rectangleUp.DownRight(),
                rectangleUp.DownLeft()
            };

            graphics.DrawLine(borderPen, ps[0], ps[5]);
            graphics.DrawLine(borderPen, ps[1], ps[4]);
            graphics.DrawLine(borderPen, ps[2], ps[3]);
            graphics.DrawLine(borderPen, ps[0], ps[1]);
            graphics.DrawLine(borderPen, ps[1], ps[2]);
            graphics.DrawRectangle(borderPen, rectangleUp);
        }

        public override void FillUpRectangle(Graphics graphics, Rectangle rectangleBase, Rectangle rectangleUp)
        {
            
            //int h = -(GameControlParent.HeightCell);
            //Rectangle rectangleCell = GameControlParent.GetRectangle(point).Move(h, h);
            //Rectangle rectangleCell = Rectangle.Inflate(rectangle, -3, -3).Move(h, h);
            //Rectangle rectangleUpCell = rectangleCell.Move(h, h);
            //int h = -(GameControlParent.HeightCell + 1);
            //Rectangle rectangleCell = Rectangle.Inflate(rectangle, -4, -4).Move(h,h);
            //Rectangle rectangleUpCell = rectangleCell.Move(-h, -h);
            //rectangleCell.Width -= 1;
            //rectangleCell.Height -= 1;
            graphics.FillRectangle(cellBrush, rectangleUp);
            graphics.DrawRectangle(borderPen, rectangleUp);
        }

        public override void FillWall(Graphics graphics, Rectangle rectangleBase, Rectangle rectangleUp)
        {
            //int h = -(GameControlParent.HeightCell);
            //Rectangle rectangleCell = GameControlParent.GetRectangle(point);
            //Rectangle rectangleCell = Rectangle.Inflate(rectangle, -3, -3).Move(-1, -1);
            Point[] ps = new Point[]
            {
                rectangleBase.DownLeft(),
                rectangleBase.DownRight(),
                rectangleBase.UpRight(),
                rectangleUp.UpRight(),
                rectangleUp.DownRight(),
                rectangleUp.DownLeft()
            };
            graphics.FillPolygon(cellBrush, ps);
        }

        public override void FillSelectedRectangle(Graphics graphics, Rectangle rectangleBase, Rectangle rectangleUp)
        {
            //int h = -(GameControlParent.HeightCell);
            //Rectangle rectangleCell = GameControlParent.GetRectangle(point).Move(h, h);
            //rectangleCell.Inflate(-1, -1);
            //Rectangle rectangleCell = Rectangle.Inflate(rectangle, -3, -3).Move(h, h);
            graphics.FillRectangle(selectedCellBrush, rectangleUp);
            graphics.DrawRectangle(borderPen, rectangleUp);
        }

        //public override void DrawCell(Graphics graphics, Rectangle rectangleBase, Rectangle rectangleUp, ViewMode view, bool selected)
        //{
        //    if (view.HasFlag(ViewMode.Wall)) FillWall(graphics, rectangleBase, rectangleUp);
        //    if (view.HasFlag(ViewMode.Rectangle))
        //    {
        //        if (selected) FillSelectedRectangle(graphics, rectangleBase, rectangleUp);
        //        else FillUpRectangle(graphics, rectangleBase, rectangleUp);
        //    }
        //    if (view.HasFlag(ViewMode.Border)) DrawBorder(graphics, rectangleBase, rectangleUp);
        //}

        public PlayerCell(Color cellColor, Color borderColor, Color selectedCellColor)
        {
            this.cellColor = cellColor;
            this.borderColor = borderColor;
            //this.innerCellColor = innerCellColor;
            this.selectedCellColor = selectedCellColor;
            cellBrush = new SolidBrush(cellColor);
            borderPen = new Pen(borderColor);
            //innerCellPen = new Pen(innerCellColor);
            selectedCellBrush = new HatchBrush(HatchStyle.WideDownwardDiagonal, selectedCellColor, cellColor);
        }

        public void SetColors(Color cellColor, Color borderColor, Color selectedCellColor)
        {
            this.cellColor = cellColor;
            this.borderColor = borderColor;
            this.selectedCellColor = selectedCellColor;
            cellBrush.Dispose();
            borderPen.Dispose();
            selectedCellBrush.Dispose();
            cellBrush = new SolidBrush(cellColor);
            borderPen = new Pen(borderColor);
            selectedCellBrush = new HatchBrush(HatchStyle.WideDownwardDiagonal, selectedCellColor, cellColor);
        }

    }

    public class EmptyCell : Player
    {
        //GameControl GameControlParent { get; }
        Color cellColor;
        Color selectedCellColor;

        Brush cellBrush;
        Brush selectedCellBrush;

        public override Color CellColor
        {
            get => cellColor;
            set
            {
                cellColor = value;
                cellBrush.Dispose();
                cellBrush = new SolidBrush(cellColor);
            }
        }

        public override Color SelectedCellColor
        {
            get => selectedCellColor;
            set
            {
                selectedCellColor = value;
                selectedCellBrush.Dispose();
                selectedCellBrush = new SolidBrush(selectedCellColor);
            }
        }


        public EmptyCell(Color cellColor, Color selectedCellColor)
        {
            this.cellColor = cellColor;
            this.selectedCellColor = selectedCellColor;
            this.cellBrush = new SolidBrush(cellColor);
            this.selectedCellBrush = new SolidBrush(selectedCellColor);
        }

        public override void DrawBorder(Graphics graphics, Rectangle rectangleBase, Rectangle rectangleUp)
        {
            //Rectangle rectangleCell = Rectangle.Inflate(rectangle, -3, -3);
            //Rectangle rectangleBorder = Rectangle.Inflate(rectangle, -1, -1);
            //rectangleCell.X -= 1;
            //rectangleCell.Y -= 1;
            //rectangleBorder.X -= 1;
            //rectangleBorder.Y -= 1;
            graphics.FillRectangle(cellBrush, rectangleBase);
        }

        public override void FillSelectedRectangle(Graphics graphics, Rectangle rectangleBase, Rectangle rectangleUp)
        {
            //Rectangle rectangleCell = GameControlParent.GetRectangle(point);
            //Rectangle rectangleCell = Rectangle.Inflate(rectangle, -3, -3);
            //Rectangle rectangleBorder = Rectangle.Inflate(rectangle, -1, -1);
            //rectangleCell.X -= 1;
            //rectangleCell.Y -= 1;
            //rectangleBorder.X -= 1;
            //rectangleBorder.Y -= 1;
            graphics.FillRectangle(selectedCellBrush, rectangleBase);
        }

        public override void FillUpRectangle(Graphics graphics, Rectangle rectangleBase, Rectangle rectangleUp)
        {
            //Rectangle rectangleCell = GameControlParent.GetRectangle(point);
            //Rectangle rectangleCell = Rectangle.Inflate(rectangle, -3, -3);
            //Rectangle rectangleBorder = Rectangle.Inflate(rectangle, -1, -1);
            graphics.FillRectangle(cellBrush, rectangleBase);
        }

        //public override void DrawCell(Graphics graphics, Rectangle rectangleBase, Rectangle rectangleUp, ViewMode view, bool selected)
        //{
        //    if (view.HasFlag(ViewMode.Border)) FillSelectedRectangle(graphics, rectangleBase, rectangleUp);
        //    //if (view.HasFlag(ViewMode.Wall)) FillWall(graphics, point, cell);
        //    if (view.HasFlag(ViewMode.Rectangle))
        //    {
        //        if (selected) FillSelectedRectangle(graphics, rectangleBase, rectangleUp);
        //        else FillUpRectangle(graphics, rectangleBase, rectangleUp);
        //    }
        //}

        public override void FillWall(Graphics graphics, Rectangle rectangleBase, Rectangle rectangleUp)
        {
        }
        //GameControl IPlayerCell.GameControlParent { get; }
    }
}

