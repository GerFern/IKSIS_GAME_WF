using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace SkiaSharp.Controls
{
    class RectPanel:Control
    {
        SKPath SKPath;
        public float XRadius { get; set; }
        public float YRadius { get; set; }
        public override void Paint()
        {
            base.Paint();
            SKPath = new SKPath();
            SKRoundRect sKRoundRect = new SKRoundRect(Rectangle, XRadius, YRadius);
            SKPath.AddRoundRect(sKRoundRect);
            Parent.DrawPath(SKPath, SKPaint);
        }
    }
}
