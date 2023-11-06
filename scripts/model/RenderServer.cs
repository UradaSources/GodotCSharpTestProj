using Godot;
using urd;

public partial class RenderServer : Node2D
{
	public static RenderServer Instance { private set; get; }

	private static Rect2I GetCharSpriteRect(int lineCount, int size, char c)
	{
		int index = c - ' ';

		int x = index % lineCount;
		int y = index / lineCount;

		return new Rect2I(new Vector2I(x * size, y * size), new Vector2I(size, size));
	}
	private static Color GdColor(byteColor c)
	{
		return new Color((float)c.r / 255, (float)c.g / 255, (float)c.b / 255, (float)c.a / 255);
	}

	[Export] private Texture2D m_charSheet;
	[Export] private float m_tileSize = 20;

	public void drawCharSprite(vec2i coord, char c, byteColor? color = null)
	{
		var pos = (vec2)coord * m_tileSize;
		var target = new Rect2(pos, Vector2.One * m_tileSize);

		var source = GetCharSpriteRect(16, 8, c);
		this.DrawTextureRectRegion(m_charSheet, target, source, GdColor(color ?? byteColor.white));
	}
	public void drawBox(vec2i coord, byteColor? color = null, bool fill = false)
	{
		var pos = (vec2)coord * m_tileSize;

		var target = new Rect2(pos, Vector2.One * m_tileSize);
		this.DrawRect(target, GdColor(color ?? byteColor.white), fill);
	}

	public vec2i globalPositionMapToCoord(vec2 pos)
	{
		var local = this.ToLocal(pos) ;
		var coord = (Vector2I)(local / m_tileSize);
		return new vec2i(coord.X, coord.Y);
	}
	public vec2 getGlobalPosition(vec2i coord)
	{
		var local = (vec2)coord * m_tileSize;
		var pos = this.ToGlobal(local);
		return new vec2(pos.X, pos.Y);
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		Instance = this;
	}

	public override void _Draw()
	{
		base._Draw();

	}
}
