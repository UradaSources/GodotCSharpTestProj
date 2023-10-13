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
	[Export] private int m_w = 10;
	[Export] private int m_h = 10;

	[ExportGroup("view params")]
	[Export] private Texture2D m_tex;
	[Export] private float m_tileSize = 20;

	private WorldGrid m_world;

	//private MoveControl m_control;
	private AIControl m_control;
	private Character m_character;

	private Pathfind m_pathfind;

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
		m_pathfind = new Pathfind(m_world);

		m_character = new Character(m_world, vec2i.zero, 1.0f, vec2i.zero);
		m_control = new AIControl(m_character, m_pathfind);

		var rng = new RandomNumberGenerator();
		for (int i = 0; i < m_world.tileCount; i++)
		{
			m_world.rawGetTile(i).pass = rng.Randf() > 0.1f;
		}

		m_control.setTarget(new vec2i(9, 9));

		GD.Print("path:");
		foreach (var t in m_pathfind.getPath(m_world.getTile(0, 0), m_world.getTile(9, 9)))
		{
			GD.Print($"{t.x}, {t.y}");
		}
	}

	public override void _Process(double delta)
	{
		m_character._update((float)delta);
		m_control._update((float)delta);

		this.QueueRedraw();
	}

	public override void _Draw()
	{
		// draw world
		for (int i = 0; i < m_world.tileCount; i++)
		{
			var tile = m_world.rawGetTile(i);
			this.DrawSprite(tile.x, tile.y, tile.pass ? '.' : 'T', tile.pass ? null : Colors.Green);
		}

		// if (Metronome(1, 2.123f))
		{
			foreach (var t in m_pathfind.getPath(m_world.getTile(0, 0), m_world.getTile(9, 9)))
			{
				this.DrawSprite(t.x, t.y, ',', Colors.Brown);
			}
		}

		// draw char
		bool drawSprite = Metronome(m_character.moveProcessing ? 3 : 1);
		if (drawSprite)
		{
			this.DrawSprite(m_character.coord.x, m_character.coord.y, '@');
		}
	}
}