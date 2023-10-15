using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using urd;

public partial class Loop : Node2D
{
	public static Rect2I GetSpriteRect(char c)
	{
		const int SpriteLineCount = 16;
		const int SpriteUnitSize = 8;

		int index = c - ' ';

		int x = index % SpriteLineCount;
		int y = index / SpriteLineCount;

		return new Rect2I(new Vector2I(x, y) * SpriteUnitSize, Vector2I.One * SpriteUnitSize);
	}
	public static bool Metronome(int frequency, float offset = 0)
	{
		switch (frequency)
		{
		case -1: return true;
		case 0: return false;
		}

		float t = offset + Time.GetTicksMsec() * 0.001f;
		return (int)(t * 2 * frequency) % 2 == 0;
	}

	[ExportGroup("grid params")]
	[Export] private ulong m_seed;
	[Export] private int m_w = 100;
	[Export] private int m_h = 100;

	[ExportGroup("view params")]
	[Export] private Texture2D m_tex;
	[Export] private float m_tileSize = 20;

	private WorldGrid m_world;

	private Character m_character;

	private PlayerControlInput m_player;
	private PathfindTestControl m_ai;

	private Transform2D m_drawMat;

	public void DrawSprite(int x, int y, char c, Color? color = null)
	{
		color ??= Colors.White;

		var pos = new Vector2(x, y) * m_tileSize * m_drawMat;

		var target = new Rect2(pos, Vector2.One * m_tileSize);
		var source = GetSpriteRect(c);

		this.DrawTextureRectRegion(m_tex, target, source, color);
	}
	public void DrawSelectBox(int x, int y, Color? color = null)
	{
		color ??= Colors.White;

		var pos = new Vector2(x, y) * m_tileSize * m_drawMat;

		var target = new Rect2(pos, Vector2.One * m_tileSize);
		this.DrawRect(target, color.Value);
	}

	public override void _Ready()
	{
		m_drawMat = Transform2D.Identity;
		m_drawMat.Scaled(new Vector2(1, -1));

		m_world = new WorldGrid(m_w, m_h);

		m_character = new Character(m_world, vec2i.zero, 1.0f, vec2i.zero);

		m_player = new PlayerControlInput(m_character.motion);
		m_ai = new PathfindTestControl(m_character.motion);

		m_character.moveControl = m_ai;

		var rng = new RandomNumberGenerator();
		// rng.Seed = m_seed;

		for (int i = 0; i < m_world.tileCount; i++)
		{
			m_world.rawGetTile(i).pass = rng.Randf() > 0.1f;
		}
	}

	public override void _Process(double delta)
	{
		m_character._update((float)delta);
		this.QueueRedraw();
	}

	public override void _Draw()
	{
		// draw world
		for (int i = 0; i < m_world.tileCount; i++)
		{
			var tile = m_world.rawGetTile(i);
			this.DrawSprite(tile.x, tile.y, tile.pass ? '.' : 'T', tile.pass ? null : new Color("#BAB060"));
		}

		// draw char
		// bool drawSprite = Metronome(m_character.motion.moveProcessing ? 2 : 1);
		// if (drawSprite)
		{
			this.DrawSprite(m_character.entity.coord.x, m_character.entity.coord.y, 'P');
		}
	}
}