using Godot;

namespace urd
{
	public partial class RenderFunc : Node2D
	{
		private static Godot.Color GDColor(Color c)
		{
			return new Godot.Color((float)c.r / 255, (float)c.g / 255, (float)c.b / 255, (float)c.a / 255);
		}
		private static Rect2I GetCharSpriteRect(int lineCount, int size, char c)
		{
			int index = c - ' ';

			int x = index % lineCount;
			int y = index / lineCount;

			return new Rect2I(new Vector2I(x * size, y * size), new Vector2I(size, size));
		}

		[Export] private Texture2D m_charSheet;
		[Export] private float m_tileSize = 20;

		public void drawCharSprite(int x, int y, char c, Color? color = null)
		{
			var pos = new Vector2(x, y) * m_tileSize;
			var target = new Rect2(pos, Vector2.One * m_tileSize);

			var source = GetCharSpriteRect(16, 8, c);
			this.DrawTextureRectRegion(m_charSheet, target, source, GDColor(color ?? Color.white));
		}
		public void drawBox(int x, int y, Color? color = null, bool fill = false)
		{
			var pos = new Vector2(x, y) * m_tileSize;

			var target = new Rect2(pos, Vector2.One * m_tileSize);
			this.DrawRect(target, GDColor(color ?? Color.white), fill);
		}

		public vec2i mapToCoord(vec2 pos)
		{
			int x = (int)(pos.x / m_tileSize);
			int y = (int)(pos.y / m_tileSize);
			return new vec2i(x, y);
		}
		public vec2 getPosition(vec2i coord)
		{
			return (vec2)coord * m_tileSize;
		}

		public vec2 mousePosition()
		{
			var pos = this.GetLocalMousePosition();
			return new vec2(pos.X, pos.Y);
		}

		public override void _Ready()
		{
			RenderFunc.Canvas = this;
		}
	}
	
}
