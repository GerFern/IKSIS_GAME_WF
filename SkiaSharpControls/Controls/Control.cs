using System;
using System.Drawing;
using SkiaSharp;

namespace SkiaSharp.Controls
{
    public class Control : IDisposable
    {
        protected SKPaint SKPaint { get; set; }
        public SKCanvas Parent;
        public bool IsDisposed { get; private set; }
        public bool Antialias
        {
            get => SKPaint.IsAntialias;
            set
            {
                SKPaint.IsAntialias = value;
                AntialiasChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public SKPoint Location { get; set; }
        public SKSize Size { get; set; }
        public SKRect Rectangle => new SKRect(Location.X, Location.Y, Size.Width + Location.X, Size.Height + Location.Y);
        public int ZOrder { get; set; }
        public object Tag { get; set; }
        public SKColor BackgroundColor { get; set; }
        public SKColor ForeColor { get; set; }
        public virtual void Paint()
        {
            //Parent.Clear(BackgroundColor);
            
        }

        public virtual void MouseClick()
        {
            SKPaint.
        }

        public void Dispose()
        {
            IsDisposed = true;
            SKPaint.Dispose();
        }

        #region Events
        public event EventHandler AntialiasChanged;
        public event EventHandler LocationChanged;
        public event EventHandler SizeChanged;
        public event EventHandler ZOrderChanged;
        public event EventHandler TagChanged;
        public event EventHandler BackgroundColorChanged;
        public event EventHandler ForeColorChanged;
        #endregion Events
    }
}
