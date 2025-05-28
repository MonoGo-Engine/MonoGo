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
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGo.Engine.ViewportAdapters
{
    public class WindowViewportAdapter : ViewportAdapter
    {
        public WindowViewportAdapter() : base()
        {
            GameMgr.WindowManager.Window.ClientSizeChanged += OnClientSizeChanged;
        }

        public override int ViewportWidth => GameMgr.WindowManager.Window.ClientBounds.Width;
        public override int ViewportHeight => GameMgr.WindowManager.Window.ClientBounds.Height;
        public override int VirtualWidth => GameMgr.WindowManager.Window.ClientBounds.Width;
        public override int VirtualHeight => GameMgr.WindowManager.Window.ClientBounds.Height;

        public override Matrix GetScaleMatrix()
        {
            return Matrix.Identity;
        }

        private void OnClientSizeChanged(object sender, EventArgs eventArgs)
        {
            var x = GameMgr.WindowManager.Window.ClientBounds.Width;
            var y = GameMgr.WindowManager.Window.ClientBounds.Height;

            GameMgr.WindowManager.GraphicsDevice.Viewport = new Viewport(0, 0, x, y);
        }

        public override void Dispose()
        {
            GameMgr.WindowManager.Window.ClientSizeChanged -= OnClientSizeChanged;
            base.Dispose();
        }
    }
}
