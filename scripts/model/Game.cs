using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using urd;

public partial class Game : Node2D
{
	public static Color ToGDColor(color c)
	{
		return new Color((float)c.r/255, (float)c.g/255, (float)c.b/255, (float)c.a/255);
	}
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
	
	[Export] private ulong m_seed;

	[Export] private Texture2D m_characterSheet;
	[Export] private float m_tileSize = 20;

	private SystemFont m_font;

	private WorldGrid m_world;
	private Character m_character;

	private int m_controllerIndex = 0;
	private List<BasicMoveControl> m_controllerSet;

	private TileCell m_selectedTile;

	public void DrawCharacterSprite(int x, int y, char c, Color? color = null)
	{
		color ??= Colors.White;

		var pos = new Vector2(x, y) * m_tileSize;
		var target = new Rect2(pos, Vector2.One * m_tileSize);

		var source = GetCharacterSpriteRect(16, 8, c);

		this.DrawTextureRectRegion(m_characterSheet, target, source, color);
	}
	public void DrawSelectBox(int x, int y, Color? color = null)
	{
		color ??= Colors.White;

		var pos = new Vector2(x, y) * m_tileSize;

		var target = new Rect2(pos, Vector2.One * m_tileSize);
		this.DrawRect(target, color.Value, false);
	}

	private void TrySelectTile(Vector2 pos)
	{
		int x = (int)(pos.X / m_tileSize);
		int y = (int)(pos.Y / m_tileSize);
		m_world.tryGetTile(x, y, out m_selectedTile);
	}

	public override void _Ready()
	{
		TileType treeTile = TileType.Create("Tree", 'T', color.FromHex(0x8AB969), 1.5f);
		TileType wallTile = TileType.Create("Wall", 'X', color.FromHex(0x411d31), -1.0f);
		TileType doorTile = TileType.Create("Door", 'D', color.FromHex(0x819796), 1.1f);
		TileType groundTile = TileType.Create("Ground", '.', color.FromHex(0xA77B5B), 1.0f);

		//var path = ProjectSettings.GlobalizePath("user://data/tile_types.json");
		//if (System.IO.File.Exists(path))
		//{
		//	var jsonData = System.IO.File.ReadAllText(path);
		//	int count = TileType.AttachFromJson(jsonData);
		//	GD.Print($"加载瓦片数据{count}个");
		//}
		//else
		//{
		//	treeTile = TileType.Create("Tree", 'T', color.FromHex(1), -1);
		//	groundTile = TileType.Create("Ground", '.', color.FromHex(1), -1);
		//	wallTile = TileType.Create("Wall", 'W', color.FromHex(1), -1);
		//	doorTile = TileType.Create("Door", 'D', color.FromHex(1), -1);

		//	var jsonData = TileType.ToJson();
		//	System.IO.File.AppendAllText(path, jsonData);
		//}

		m_font = new SystemFont();

		m_world = new WorldGrid(10, 10, groundTile);

		m_character = new Character(m_world, vec2i.zero, 3.0f, vec2i.zero);

		// 初始化并选择控制器
		m_controllerSet = new List<BasicMoveControl>
		{
			new ClassicArcadeControl(m_character.motion),
			new RpgControl(m_character.motion),
			new RandomWalkAIControl(m_character.motion)
		};
		m_character.moveControl = m_controllerSet[m_controllerIndex];

		var rng = new RandomNumberGenerator();
		rng.Seed = m_seed;

		for (int i = 1; i < m_world.tileCount-1; i++)
		{
			TileType type;
			var v = rng.Randf();

			if (v < 0.1f) type = treeTile;
			else type = groundTile;

			m_world.rawGetTile(i).type = type;
		}
	}

	private float m_delta;

	public override void _Process(double delta)
	{
		m_delta = (float)delta;

		m_character._update((float)delta);

		var mousePos = this.GetLocalMousePosition();
		this.TrySelectTile(mousePos);

		if (Input.IsActionJustPressed("ui_select"))
		{
			m_controllerIndex = (m_controllerIndex + 1) % m_controllerSet.Count;
			m_character.moveControl = m_controllerSet[m_controllerIndex];
		}
		if (Input.IsMouseButtonPressed(MouseButton.Left))
		{
			if (m_selectedTile != null)
			{
				m_selectedTile.type = TileType.GetType('W');	
			}
		}

		this.QueueRedraw();
	}

	public override void _Draw()
	{
		this.DrawString(m_font, new Vector2(0, -24), $"fps: {(1 / m_delta).ToString("0.0")}");
		this.DrawString(m_font, new Vector2(0, -12), $"control mode: {m_controllerIndex}");

		for (int i = 0; i < m_world.tileCount; i++)
		{
			var tile = m_world.rawGetTile(i);
			var color = ToGDColor(tile.type.color);

			this.DrawCharacterSprite(tile.x, tile.y, tile.type.graph, color);
		}
		
		this.DrawCharacterSprite(m_character.entity.coord.x, m_character.entity.coord.y, 'P');

		if (m_selectedTile != null)
			this.DrawSelectBox(m_selectedTile.x, m_selectedTile.y, Colors.Red);
	}
}