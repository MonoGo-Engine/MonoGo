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
    public abstract class ViewportAdapter : IDisposable
    {
        public virtual void Dispose()
        {
        }

        public Viewport Viewport => GameMgr.WindowManager.GraphicsDevice.Viewport;

        public abstract int VirtualWidth { get; }
        public abstract int VirtualHeight { get; }
        public abstract int ViewportWidth { get; }
        public abstract int ViewportHeight { get; }

        public Vector2 VirtualSize => new Vector2(VirtualWidth, VirtualHeight);
        public Rectangle BoundingRectangle => new Rectangle(0, 0, VirtualWidth, VirtualHeight);
        public Vector2 Center => new Vector2(BoundingRectangle.Center.X, BoundingRectangle.Center.Y);
        public abstract Matrix GetScaleMatrix();

        /// <summary>
        /// Transforms world coordinates to viewport coordinates
        /// </summary>
        public virtual Vector2 WorldToViewport(Vector2 worldPosition)
        {
            var scaleMatrix = GetScaleMatrix();
            return Vector2.Transform(worldPosition, scaleMatrix);
        }

        /// <summary>
        /// Transforms viewport coordinates to world coordinates  
        /// </summary>
        public virtual Vector2 ViewportToWorld(Vector2 viewportPosition)
        {
            var scaleMatrix = GetScaleMatrix();
            var inverseMatrix = Matrix.Invert(scaleMatrix);
            return Vector2.Transform(viewportPosition, inverseMatrix);
        }

        public virtual void Reset()
        {
        }
    }
}
