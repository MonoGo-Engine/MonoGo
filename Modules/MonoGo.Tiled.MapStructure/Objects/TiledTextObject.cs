using Microsoft.Xna.Framework;

namespace MonoGo.Tiled.MapStructure.Objects
{
    /// <summary>
    /// Represents a text object in a Tiled map.
    /// </summary>
    public class TiledTextObject : TiledObject
    {
        /// <summary>
        /// Gets or sets the text content of the object.
        /// </summary>
        public string Text;

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        public Color Color;

        /// <summary>
        /// Gets or sets a value indicating whether word wrapping is enabled for the text.
        /// </summary>
        public bool WordWrap;

        /// <summary>
        /// Gets or sets the horizontal alignment of the text.
        /// </summary>
        public TiledTextAlign HorAlign;

        /// <summary>
        /// Gets or sets the vertical alignment of the text.
        /// </summary>
        public TiledTextAlign VerAlign;

        /// <summary>
        /// Gets or sets the font used for the text.
        /// </summary>
        public string Font;

        /// <summary>
        /// Gets or sets the font size of the text.
        /// </summary>
        public int FontSize = 12;

        /// <summary>
        /// Gets or sets a value indicating whether the text is underlined.
        /// </summary>
        public bool Underlined;

        /// <summary>
        /// Gets or sets a value indicating whether the text is strikethrough.
        /// </summary>
        public bool StrikedOut;

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledTextObject"/> class.
        /// </summary>
        public TiledTextObject() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledTextObject"/> class by copying another object.
        /// </summary>
        /// <param name="obj">The object to copy.</param>
        public TiledTextObject(TiledObject obj) : base(obj) { }
    }
}