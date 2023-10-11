using System.Collections;
using System.Collections.Generic;
using Godot;

namespace urd
{
	public partial class World : Node2D
	{
		public static Rect2I GetSpriteRect(char c)
		{
			const int SpriteLineCount = 8;
			const int SpriteUnitSize = 8;

			int index = c - ' ';

			int x = index % SpriteLineCount;
			int y = index / SpriteLineCount;

			return new Rect2I(new Vector2I(x, y) * SpriteUnitSize, Vector2I.One * SpriteUnitSize);
		}

		[ExportGroup("grid params")]
		[Export] private int m_width;
		[Export] private int m_height;

		[ExportGroup("view params")]
		[Export] private Texture2D m_tex;
		[Export] private float m_tileSize;

		private WorldGrid m_grid;

		public WorldGrid grid => m_grid;

		public Vector2 CoordMapToPosition(int x, int y)
		{
			return new Vector2(x, y) * m_tileSize;
		}

		public void DrawSprite(int x, int y, char c, CanvasItem canvas)
		{
			var pos = CoordMapToPosition(x, y);

			var target = new Rect2(pos, Vector2.One * m_tileSize);
			var source = GetSpriteRect(c);

			canvas.DrawTextureRectRegion(m_tex, target, source);
		}
		public void DrawSprite(Vector2 pos, char c, CanvasItem canvas)
		{
			var target = new Rect2(pos, Vector2.One * m_tileSize);
			var source = GetSpriteRect(c);

			canvas.DrawTextureRectRegion(m_tex, target, source);
		}

		public override void _Ready()
		{
			var rng = new RandomNumberGenerator();

			m_grid = new WorldGrid(m_width, m_height);
			for (int i = 0; i < m_grid.width() * m_grid.height(); i++)
			{
				m_grid.rawGetTile(i).pass = rng.Randf() > 0.1f;
			}
		}

		public override void _Process(double delta)
		{
			this.QueueRedraw();
		}

		public override void _Draw()
		{
			for (int i = 0; i < m_grid.width() * m_grid.height(); i++)
			{
				var tile = m_grid.rawGetTile(i);
				this.DrawSprite(tile.x, tile.y, tile.pass ? '.' : '#', this);
			}
		}
	}
}
