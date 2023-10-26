using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

namespace urd
{
	public partial class Game : Node2D
	{
		private const string WorldDataFilePath = "./save/world.json";
	
		public struct WorldData
		{
			[JsonInclude] public int width, height;
			[JsonInclude] public int[] tile;
		}

		public static string ToJson(WorldGrid world)
		{
			var data = new WorldData();
			data.width = world.width;
			data.height = world.height;

			data.tile = new int[data.width * data.height];
			for (int i = 0; i < world.tileCount; i++)
				data.tile[i] = world.rawGetTile(i).type.id;

			return JsonSerializer.Serialize(data);
		}
		public static WorldGrid FromJson(string json)
		{
			var data = JsonSerializer.Deserialize<WorldData>(json);
			WorldGrid world = new WorldGrid(data.width, data.height, TileType.GetFromId(0));

			for (int i = 0; i < world.tileCount; i++)
			{
				int typeId = data.tile[i];
				world.rawGetTile(i).type = TileType.GetFromId(typeId);
			}

			return world;
		}

		public static Color ToGDColor(color c)
		{
			return new Color((float)c.r / 255, (float)c.g / 255, (float)c.b / 255, (float)c.a / 255);
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
		private PathGenerator m_pathfind;

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

		private bool m_runingLoop = true;

		public override void _Ready()
		{
			TileType treeTile = TileType.Create('T', color.FromHex(0x8AB969), 1.5f);
			TileType wallTile = TileType.Create('X', color.FromHex(0x3A3858), -1.0f);
			TileType doorTile = TileType.Create('D', color.FromHex(0x819796), 3.0f);
			TileType groundTile = TileType.Create('.', color.FromHex(0xA77B5B), 1.0f);
			TileType floorTile = TileType.Create('_', color.FromHex(0x89493A), 0.8f);
			TileType.Create('~', color.FromHex(0x4B80CA), 3.0f);

			// 随机化地图
			RandomNumberGenerator rng = new RandomNumberGenerator();
			rng.Seed = m_seed;

			if (File.Exists(WorldDataFilePath))
			{
				var json = File.ReadAllText(WorldDataFilePath);
				m_world = FromJson(json);

				Debug.WriteLine("load world data from json");
			}
			else
			{ 
				m_world = new WorldGrid(20, 20, groundTile);

				for (int i = 1; i < m_world.tileCount - 1; i++)
				{
					TileType type;
					var v = rng.Randf();

					if (v < 0.1f) type = treeTile;
					else type = groundTile;

					m_world.rawGetTile(i).type = type;
				}
			}

			m_pathfind = new PathGenerator(m_world);

			m_player = new ComponentContainer();

			m_player.addComponent(new Entity("Player", m_world, vec2i.zero));
			m_player.addComponent(new Movement(3.0f, vec2i.zero));
			m_player.addComponent(new Navigation(m_pathfind));

			m_player.addComponent(new RandomWalkControl());

			m_player._init();

			for (int i = 0; i < 4; i++)
			{
				var enemy = new ComponentContainer();

				int x = rng.RandiRange(0, m_world.width - 1);
				int y = rng.RandiRange(0, m_world.height - 1);

				enemy.addComponent(new Entity("Enemy", m_world, new vec2i(x, y)));
				enemy.addComponent(new Movement(2.8f, vec2i.zero));
				enemy.addComponent(new Navigation(m_pathfind));

				enemy.addComponent(new FollowWalkControl()).target = m_player.getComponent<Entity>();

				enemy._init();
			}
		}

		public override void _Process(double delta)
		{
			if (m_runingLoop)
			{
				foreach (var entity in Entity.IterateInstance())
					entity.container._update((float)delta);
			}

			var mousePos = this.GetLocalMousePosition();
			this.TrySelectTile(mousePos);

			// 设置目的地
			if (Input.IsActionJustPressed("mouse_right"))
			{
				if (m_selectedTile != null && m_player.getComponent(typeof(BasicMotionControl)) == null)
				{
					var navigation = m_player.getComponent<Navigation>();
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

			// 储存地图数据
			if (Input.IsActionJustPressed("ui_save"))
			{
				var json = ToJson(m_world);
				File.WriteAllText(WorldDataFilePath, json);
				Debug.WriteLine($"save world data");
			}

			// 暂停或是运行游戏
			if (Input.IsActionJustPressed("ui_stop"))
				m_runingLoop = !m_runingLoop;

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

			foreach (var entity in Entity.IterateInstance())
			{
				// 绘制角色本身
				this.DrawCharacterSprite(entity.coord.x, entity.coord.y, entity.name[0]);

				// 绘制目标格子
				var motion = entity.container.getComponent<Movement>();
				if (motion.processing)
				{
					var target = entity.getNearTile(motion.currentDirect);
					this.DrawCharacterSprite(target.x, target.y, 'x', new Color(0, 0.5f, 0, 0.5f));

					//// 绘制寻路路径
					//var navigation = entity.container.getComponent<Navigation>();
					//for (int i = 0; i < navigation.pathNodeCount; i++)
					//{
					//	var node = navigation.getPathNode(i);
					//	this.DrawCharacterSprite(node.x, node.y, 'x', new Color(1, 1, 1, 0.5f));
					//}
				}
			}

			if (m_selectedTile != null)
				this.DrawSelectBox(m_selectedTile.x, m_selectedTile.y, Colors.Red);
		}
	}
}
/*
后续:
网格储存实体
ai行为树
 */
