using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using GLFW;
using SkiaSharp;

namespace GLFW_TEST
{
    public class GameWindow : NativeWindow
    {
        SKSurface SKSurface;
        GRContext GRContext;
        SKCanvas SKCanvas;
        Random Random = new Random();
        public GameWindow() : base()
        {
            GRContext = GenerateSkiaContext(this);
            SKSurface = GenerateSkiaSurface(GRContext, this.ClientSize);
            SKCanvas = SKSurface.Canvas;
        }

        public void Render()
        {
            SKCanvas.Clear(new SKColor((uint)Random.Next()));
            var headerPaint = new SKPaint { Color = SKColor.Parse("#333333"), TextSize = 50, IsAntialias = true };
            SKCanvas.DrawText("Hello from GLFW.NET + SkiaSharp!", 10, 60, headerPaint);
            var inputInfoPaint = new SKPaint { Color = SKColor.Parse("#F34336"), TextSize = 18, IsAntialias = true };
            //SKCanvas.DrawText($"Last key pressed: {Program.lastKeyPressed}", 10, 90, inputInfoPaint);
            //SKCanvas.DrawText($"Last mouse position: {Program.lastMousePosition}", 10, 120, inputInfoPaint);

            var exitInfoPaint = new SKPaint { Color = SKColor.Parse("#3F51B5"), TextSize = 18, IsAntialias = true };
            SKCanvas.DrawText("Press Enter to Exit.", 10, 160, exitInfoPaint);

            SKCanvas.Flush();
            this.SwapBuffers();
        }



        private static SKSurface GenerateSkiaSurface(GRContext skiaContext, Size surfaceSize)
        {
            var frameBufferInfo = new GRGlFramebufferInfo((uint)new UIntPtr(0), GRPixelConfig.Rgba8888.ToGlSizedFormat());
            var backendRenderTarget = new GRBackendRenderTarget(surfaceSize.Width,
                                                                surfaceSize.Height,
                                                                0,
                                                                8,
                                                                frameBufferInfo);
            return SKSurface.Create(skiaContext, backendRenderTarget, GRSurfaceOrigin.BottomLeft, SKImageInfo.PlatformColorType);
        }

        private static GRContext GenerateSkiaContext(NativeWindow nativeWindow)
        {
            var nativeContext = GetNativeContext(nativeWindow);
            var glInterface = GRGlInterface.AssembleGlInterface(nativeContext, (contextHandle, name) => GLFW.Glfw.GetProcAddress(name));
            return GRContext.Create(GRBackend.OpenGL, glInterface);
        }

        private static object GetNativeContext(NativeWindow nativeWindow)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Native.GetWglContext(nativeWindow);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // XServer
                return Native.GetGLXContext(nativeWindow);
                // Wayland
                //return Native.GetEglContext(nativeWindow);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Native.GetNSGLContext(nativeWindow);
            }
            throw new PlatformNotSupportedException();
        }
    }
}
