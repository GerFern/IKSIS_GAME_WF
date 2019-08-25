#define test

using System.Drawing;

namespace GameCore
{
    public class PlayerCell : PlayerCellBase
    {
        Color cellColor;
        Color selectedCellColor;
        Color borderColor;
        //Color innerCellColor;

        //GameControl GameControlParent { get; }

        public override Color CellColor
        {
            get => cellColor;
            set
            {
                cellColor = value;
            }
        }
        public override Color BorderColor
        {
            get => borderColor;
            set
            {
                borderColor = value;
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
            }
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
            //cellBrush = new SolidBrush(cellColor);
            //borderPen = new Pen(borderColor);
            ////innerCellPen = new Pen(innerCellColor);
            //selectedCellBrush = new HatchBrush(HatchStyle.WideDownwardDiagonal, selectedCellColor, cellColor);
        }

        public void SetColors(Color cellColor, Color borderColor, Color selectedCellColor)
        {
            this.cellColor = cellColor;
            this.borderColor = borderColor;
            this.selectedCellColor = selectedCellColor;
        }

    }

    public class EmptyCell : PlayerCellBase
    {
        //GameControl GameControlParent { get; }
        Color cellColor;
        Color selectedCellColor;

        public override Color CellColor
        {
            get => cellColor;
            set
            {
                cellColor = value;
            }
        }

        public override Color SelectedCellColor
        {
            get => selectedCellColor;
            set
            {
                selectedCellColor = value;
            }
        }


        //public EmptyCell(Color cellColor, Color selectedCellColor)
        //{
        //    this.cellColor = cellColor;
        //    this.selectedCellColor = selectedCellColor;
        //}

        //GameControl IPlayerCell.GameControlParent { get; }
    }
}

