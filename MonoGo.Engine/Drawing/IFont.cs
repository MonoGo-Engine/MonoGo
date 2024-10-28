using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MonoGo.Engine.Drawing
{
    /// <summary>
    /// Use <see cref="UI.UISystem"/> for serious graphical user interfaces!
    /// </summary>
    public interface IFont
	{
		Texture2D Texture {get;}
        SpriteFont SpriteFont { get; }
        ReadOnlyCollection<char> Characters {get;}
		char? DefaultCharacter {get; set;}
		int LineSpacing {get; set;}
		float Spacing {get; set;}

		Dictionary<char, SpriteFont.Glyph> GetGlyphs();
		Vector2 MeasureString(string text);
		Vector2 MeasureString(StringBuilder text);
		float MeasureStringWidth(string text);
		float MeasureStringWidth(StringBuilder text);
		float MeasureStringHeight(string text);
		float MeasureStringHeight(StringBuilder text);

		void Draw(string text, Vector2 position, TextAlign halign, TextAlign valign);
	}
}
