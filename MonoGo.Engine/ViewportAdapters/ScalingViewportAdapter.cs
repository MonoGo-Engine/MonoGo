/*MIT License

Copyright (c) 2020 Craftwork Games

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using Microsoft.Xna.Framework;

namespace MonoGo.Engine.ViewportAdapters
{
    public class ScalingViewportAdapter : ViewportAdapter
    {
        public ScalingViewportAdapter(int virtualWidth, int virtualHeight) : base()
        {
            VirtualWidth = virtualWidth;
            VirtualHeight = virtualHeight;
        }

        public override int VirtualWidth { get; }
        public override int VirtualHeight { get; }
        public override int ViewportWidth => GameMgr.WindowManager.GraphicsDevice.Viewport.Width;
        public override int ViewportHeight => GameMgr.WindowManager.GraphicsDevice.Viewport.Height;

        public override Matrix GetScaleMatrix()
        {
            var scaleX = (float)ViewportWidth / VirtualWidth;
            var scaleY = (float)ViewportHeight / VirtualHeight;
            return Matrix.CreateScale(scaleX, scaleY, 1.0f);
        }
    }
}
