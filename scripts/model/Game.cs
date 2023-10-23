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

	private WorldGrid m_world;
	private Pathfind m_pathfind;

	private TileCell m_selectedTile;

	private ComponentContainer m_player;

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
		TileType treeTile = TileType.Create('T', color.FromHex(0x8AB969), 1.5f);
		TileType wallTile = TileType.Create('X', color.FromHex(0xA2DCC7), -1.0f);
		TileType doorTile = TileType.Create('D', color.FromHex(0x819796), 3.0f);
		TileType groundTile = TileType.Create('.', color.FromHex(0xA77B5B), 1.0f);
		TileType floorTile = TileType.Create('_', color.FromHex(0xA77B5B), 3.0f);

		m_world = new WorldGrid(20, 20, groundTile);
		m_pathfind = new Pathfind(m_world);

		m_player = new ComponentContainer();

		m_player.addComponent(new Entity(m_world, vec2i.zero));
		m_player.addComponent(new EntityMotion(3.0f, vec2i.zero));
		m_player.addComponent(new EntityMoveToward(m_pathfind));

		m_player.addComponent(new RandomWalkControl());

		m_player._init();

		RandomNumberGenerator rng = new RandomNumberGenerator();
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

	public override void _Process(double delta)
	{
		m_player._update((float)delta);

		var mousePos = this.GetLocalMousePosition();
		this.TrySelectTile(mousePos);

		// 切换控制器
		//if (Input.IsActionJustPressed("ui_select"))
		//{
		//	m_controllerIndex = (m_controllerIndex + 1) % m_controllerSet.Count;
		//	m_character.moveControl = m_controllerSet[m_controllerIndex];
		//}

		// 设置目的地
		if (Input.IsActionJustPressed("mouse_right"))
		{
			if (m_selectedTile != null && m_player.getComponent(typeof(BasicMotionControl)) == null)
			{
				var navigation = m_player.getComponent<EntityMoveToward>();
				navigation.setTarget(new vec2i(m_selectedTile.x, m_selectedTile.y));
			}
		}
		
		// 设置图块
		if (Input.IsActionJustPressed("mouse_left"))
		{
			if (m_selectedTile != null)
			{
				int typeid = m_selectedTile.type.id;
				typeid = (typeid + 1) % TileType.TypeCount;
				m_selectedTile.type = TileType.GetFromId(typeid);
			}
		}

		this.QueueRedraw();
	}

	public override void _Draw()
	{
		// 绘制地图
		for (int i = 0; i < m_world.tileCount; i++)
		{
			var tile = m_world.rawGetTile(i);
			var color = ToGDColor(tile.type.c);

			var graph = tile.type.graph;

			this.DrawCharacterSprite(tile.x, tile.y, graph, color);
		}

		// 绘制角色本身
		var entity = m_player.getComponent<Entity>();
		this.DrawCharacterSprite(entity.coord.x, entity.coord.y, 'P');

		// 绘制目标格子
		var motion = m_player.getComponent<EntityMotion>();
		if (motion.processing)
		{
			var target = motion.targetCoord;
			this.DrawCharacterSprite(target.x, target.y, 'x', new Color(0, 0.5f, 0, 0.5f));
		}

		// 绘制寻路路径
		var navigation = m_player.getComponent<EntityMoveToward>();
		for (int i = 0; i < navigation.pathNodeCount; i++)
		{
			var node = navigation.getPathNode(i);
			this.DrawCharacterSprite(node.x, node.y, 'x', new Color(1,1,1,0.5f));
		}

		if (m_selectedTile != null)
			this.DrawSelectBox(m_selectedTile.x, m_selectedTile.y, Colors.Red);
	}
}