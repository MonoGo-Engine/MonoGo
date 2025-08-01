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
    public enum BoxingMode
    {
        None,
        Letterbox,
        Pillarbox
    }

    public class BoxingViewportAdapter : ScalingViewportAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoxingViewportAdapter" />.
        /// </summary>
        public BoxingViewportAdapter(int virtualWidth, int virtualHeight, int horizontalBleed = 0, int verticalBleed = 0)
            : base(virtualWidth, virtualHeight)
        {
            GameMgr.WindowManager.Window.ClientSizeChanged += OnClientSizeChanged;
            HorizontalBleed = horizontalBleed;
            VerticalBleed = verticalBleed;
        }

        public override void Dispose()
        {
            GameMgr.WindowManager.Window.ClientSizeChanged -= OnClientSizeChanged;
            base.Dispose();
        }

        /// <summary>
        ///     Size of horizontal bleed areas (from left and right edges) which can be safely cut off
        /// </summary>
        public int HorizontalBleed { get; }

        /// <summary>
        ///     Size of vertical bleed areas (from top and bottom edges) which can be safely cut off
        /// </summary>
        public int VerticalBleed { get; }

        public BoxingMode BoxingMode { get; private set; }

        private void OnClientSizeChanged(object sender, EventArgs eventArgs)
        {
            var clientBounds = GameMgr.WindowManager.Window.ClientBounds;

            var worldScaleX = (float)clientBounds.Width / VirtualWidth;
            var worldScaleY = (float)clientBounds.Height / VirtualHeight;

            var safeScaleX = (float)clientBounds.Width / (VirtualWidth - HorizontalBleed);
            var safeScaleY = (float)clientBounds.Height / (VirtualHeight - VerticalBleed);

            var worldScale = MathHelper.Max(worldScaleX, worldScaleY);
            var safeScale = MathHelper.Min(safeScaleX, safeScaleY);
            var scale = MathHelper.Min(worldScale, safeScale);

            var width = (int)(scale * VirtualWidth + 0.5f);
            var height = (int)(scale * VirtualHeight + 0.5f);

            if (height >= clientBounds.Height && width < clientBounds.Width)
                BoxingMode = BoxingMode.Pillarbox;
            else
            {
                if (width >= clientBounds.Height && height <= clientBounds.Height)
                    BoxingMode = BoxingMode.Letterbox;
                else
                    BoxingMode = BoxingMode.None;
            }

            var x = clientBounds.Width / 2 - width / 2;
            var y = clientBounds.Height / 2 - height / 2;
            GameMgr.WindowManager.GraphicsDevice.Viewport = new Viewport(x, y, width, height);
        }

        public override void Reset()
        {
            base.Reset();
            OnClientSizeChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Transforms world coordinates to viewport coordinates taking into account boxing offsets
        /// </summary>
        public override Vector2 WorldToViewport(Vector2 worldPosition)
        {
            // First apply the base scaling transformation
            var scaledPosition = base.WorldToViewport(worldPosition);
            
            // Then apply viewport offset for boxing
            var viewport = GameMgr.WindowManager.GraphicsDevice.Viewport;
            return new Vector2(scaledPosition.X + viewport.X, scaledPosition.Y + viewport.Y);
        }

        /// <summary>
        /// Transforms viewport coordinates to world coordinates taking into account boxing offsets
        /// </summary>
        public override Vector2 ViewportToWorld(Vector2 viewportPosition)
        {
            // First remove viewport offset for boxing
            var viewport = GameMgr.WindowManager.GraphicsDevice.Viewport;
            var offsetPosition = new Vector2(viewportPosition.X - viewport.X, viewportPosition.Y - viewport.Y);
            
            // Then apply the base inverse scaling transformation
            return base.ViewportToWorld(offsetPosition);
        }
    }
}
