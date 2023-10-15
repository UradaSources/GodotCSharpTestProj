using System.Collections;
using System.Collections.Generic;
using Godot;
using urd;

public partial class PathfindTest : Node2D, IDebugDrawing
{
	private WorldGrid m_world;
	private SystemFont m_font;

	private float m_tileSize;
	private int m_selectedX;
	private int m_selectedY;

	public override void _Ready()
	{
		m_font = new SystemFont();

		var rng = new RandomNumberGenerator();
		GD.Print($"seed: {rng.Seed}");

		m_world = new WorldGrid(5, 5);
		for (int i = 0; i < m_world.tileCount; i++)
		{
			m_world.rawGetTile(i).pass = rng.Randf() > 0.1f;
		}

		Pathfind pathfind = new Pathfind(m_world);
		foreach (var _ in pathfind.getPath(m_world.rawGetTile(0), m_world.rawGetTile(m_world.tileCount - 1))) ;
	}

	public override void _Process(double delta)
	{
		return;

		base._Process(delta);

		var mousePos = this.GetLocalMousePosition() + new Vector2(10, 10);
		m_selectedX = (int)(mousePos.X / m_tileSize);
		m_selectedY = (int)(mousePos.Y / m_tileSize);

		this.QueueRedraw();
	}

	public override void _Draw()
	{
		var size = 40;

		for (int i = 0; i < m_world.tileCount; i++)
		{
			var tile = m_world.rawGetTile(i);

			var pos = new vec2(tile.x, tile.y) * size;

			this.DrawString(tile.id.ToString(), pos);
			this.DrawBox(pos, vec2.one * size);
		}
	}

	public void DrawString(string msg, vec2 pos)
	{
		this.DrawString(m_font, new Vector2(pos.x, pos.y), msg, HorizontalAlignment.Center, -1, 6);
	}
	public void DrawLine(vec2 a, vec2 b, color? c = null, float width = -1)
	{
		color _c = c ?? color.white;
		this.DrawLine(new Vector2(a.x, a.y), new Vector2(b.x, b.y), new Color(_c.r / 255.0f, _c.g / 255.0f, _c.b / 255.0f), width);
	}
}
