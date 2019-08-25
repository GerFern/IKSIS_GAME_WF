using System;
using System.Collections.Generic;
using System.Text;

namespace SkiaSharp.Controls
{
    class Button : Control
    {
        public string Text { get; set; }
        public override void Paint()
        {
            base.Paint();
            Parent.drawText
        }
    }
}
