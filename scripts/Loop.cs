using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using urd;

public partial class Game : Node2D
{
	// 16, 8
	public static Rect2I GetCharacterSpriteRect(int lineCount, int size, char c)
	{
		int index = c - ' ';

		int x = index % lineCount;
		int y = index / lineCount;

		return new Rect2I(new Vector2I(x * size, y * size), new Vector2I(size, size));
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

	[Export] private Texture2D m_characterSheet;
	[Export] private float m_tileSize = 20;

	private WorldGrid m_world;

	private Character m_character;

	public void DrawCharacterSprite(int x, int y, char c, Color? color = null)
	{
		color ??= Colors.White;

		var pos = new Vector2(x, -y) * m_tileSize;
		var target = new Rect2(pos, Vector2.One * m_tileSize);

		var source = GetCharacterSpriteRect(18, 8, c);

		this.DrawTextureRectRegion(m_characterSheet, target, source, color);
	}
	public void DrawSelectBox(int x, int y, Color? color = null)
	{
		color ??= Colors.White;

		var pos = new Vector2(x, -y) * m_tileSize;

		var target = new Rect2(pos, Vector2.One * m_tileSize);
		this.DrawRect(target, color.Value, false);
	}

	public override void _Ready()
	{
		System.IO.File.ReadAllText("");

		m_world = new WorldGrid(m_w, m_h);

		m_character = new Character(m_world, vec2i.zero, 1.0f, vec2i.zero);
		m_character.moveControl = new PathfindTestControl(m_character.motion);

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
			this.DrawCharacterSprite(tile.x, tile.y, tile.pass ? '.' : 'G', tile.pass ? null : new Color("#6A536E"));
		}

		// draw char
		// bool drawSprite = Metronome(m_character.motion.moveProcessing ? 2 : 1);
		// if (drawSprite)
		{
			this.DrawCharacterSprite(m_character.entity.coord.x, m_character.entity.coord.y, 'P');
		}

		var mousePos = this.GetLocalMousePosition();
		int x = (int)(mousePos.X / m_tileSize);
		int y = (int)(mousePos.Y / m_tileSize);
		GD.Print($"{x},{y} : {mousePos}");
		if (m_world.vaildCoord(x, y))
		{
			var pos = new Vector2(x, y) * m_tileSize;
			this.DrawRect(new Rect2(pos, Vector2.One * m_tileSize), Colors.Red, false);
		}
	}
}