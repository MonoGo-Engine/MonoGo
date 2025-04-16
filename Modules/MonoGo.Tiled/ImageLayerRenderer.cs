using Microsoft.Xna.Framework;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.EC;
using MonoGo.Engine.SceneSystem;

namespace MonoGo.Engine.Utils.Tilemaps
{
    /// <summary>
    /// Represents a component for rendering Tiled image layers.
    /// </summary>
    public class ImageLayerRenderer : Entity
    {
        /// <summary>
        /// Gets or sets the frame to be rendered.
        /// </summary>
        public Frame Frame { get; set; }

        /// <summary>
        /// Gets or sets the offset for rendering the image layer.
        /// </summary>
        public Vector2 Offset { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageLayerRenderer"/> class.
        /// </summary>
        /// <param name="layer">The layer to which this renderer belongs.</param>
        /// <param name="offset">The offset for rendering the image layer.</param>
        /// <param name="frame">The frame to be rendered.</param>
        public ImageLayerRenderer(Layer layer, Vector2 offset, Frame frame) : base(layer)
        {
            Visible = true;
            Frame = frame;
            Offset = offset;
        }

        /// <summary>
        /// Draws the image layer using the specified frame and offset.
        /// </summary>
        public override void Draw()
        {
            Frame.Draw(Offset, Vector2.Zero);
        }
    }
}
