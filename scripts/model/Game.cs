using System.IO;
using System.Collections;
using System.Diagnostics;
using Godot;

namespace urd
{
	public partial class Game : Node2D
	{
		private const string WorldDataFilePath = "./save/world.json";
		private const string TileTypeDataFilePath = "./save/tileTypes.json";

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

		private WorldGrid m_world;
		private PathGenerator m_pathfind;

		private ComponentContainer m_player;

		private bool m_runingLoop = true;

		public override void _Ready()
		{
			TileType.Create('T', Color.FromHex(0x8AB969), 1.5f);
			TileType.Create('#', Color.FromHex(0x3A3858), -1.0f);
			TileType.Create('D', Color.FromHex(0x819796), 3.0f);
			TileType.Create('.', Color.FromHex(0xA77B5B), 1.0f);
			TileType.Create('_', Color.FromHex(0x89493A), 0.8f);
			TileType.Create('`', Color.FromHex(0x68C2D3), 3.0f);
	
			if (File.Exists(WorldDataFilePath))
			{
				var json = File.ReadAllText(WorldDataFilePath);
				m_world = WorldGridUtils.FromJson(json);

				Debug.WriteLine("load world data from json");
			}

			m_pathfind = new PathGenerator(m_world);

			m_player = new ComponentContainer();

			m_player.addComponent(new Entity(m_world, vec2i.zero));
			m_player.addComponent(new Movement(3.0f, vec2i.zero));
			m_player.addComponent(new Navigation(m_pathfind));
			m_player.addComponent(new RandomWalkControl());
			//m_player.addComponent(new CharacterView('P', false));

			m_player._init();

			var rng = new RandomNumberGenerator();

			for (int i = 0; i < 3; i++)
			{
				var enemy = new ComponentContainer();

				int x = rng.RandiRange(0, m_world.width - 1);
				int y = rng.RandiRange(0, m_world.height - 1);

				enemy.addComponent(new Entity(m_world, new vec2i(x, y)));
				enemy.addComponent(new Movement(2.0f, vec2i.zero));
				enemy.addComponent(new Navigation(m_pathfind));
				enemy.addComponent(new FollowWalkControl()).target = m_player.getComponent<Entity>();
				//enemy.addComponent(new CharacterView('E', true));

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
			// this.TrySelectTile(mousePos);

			//// 设置目的地
			//if (Input.IsActionJustPressed("mouse_right"))
			//{
			//	if (m_selectedTile != null && m_player.findComponent(typeof(BasicMotionControl)) == null)
			//	{
			//		var navigation = m_player.getComponent<Navigation>();
			//		navigation.setTarget(new vec2i(m_selectedTile.x, m_selectedTile.y));
			//	}
			//}

			//// 设置图块
			//if (Input.IsActionJustPressed("mouse_left"))
			//{
			//	if (m_selectedTile != null)
			//	{
			//		int typeid = m_selectedTile.type.id;
			//		typeid = (typeid + 1) % TileType.TypeCount;
			//		m_selectedTile.type = TileType.Get(typeid);
			//	}
			//}

			// 储存地图数据
			if (Input.IsActionJustPressed("ui_save"))
			{
				var json = WorldGridUtils.ToJson(m_world);
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
			RenderFunc.Canvas = this;

			// 绘制地图
			for (int i = 0; i < m_world.tileCount; i++)
			{
				var tile = m_world.rawGetTile(i);
				RenderFunc.drawCharSprite(tile.x, tile.y, tile.type.graph, tile.type.c);
			}

			//if (m_runingLoop)
			//{
			//	foreach (var renderer in Renderer.IterateInstance())
			//		renderer._draw();
			//}

			//foreach (var entity in Entity.IterateInstance())
			//{
			//	// 绘制角色本身
			//	this.DrawCharacterSprite(entity.coord.x, entity.coord.y, entity.name[0]);

			//	// 绘制目标格子
			//	var motion = entity.container.getComponent<Movement>();
			//	if (motion.processing)
			//	{
			//		var target = entity.getNearTile(motion.currentDirect);
			//		this.DrawCharacterSprite(target.x, target.y, 'x', new Godot.Color(0, 0.5f, 0, 0.5f));

			//		//// 绘制寻路路径
			//		//var navigation = entity.container.getComponent<Navigation>();
			//		//for (int i = 0; i < navigation.pathNodeCount; i++)
			//		//{
			//		//	var node = navigation.getPathNode(i);
			//		//	this.DrawCharacterSprite(node.x, node.y, 'x', new Color(1, 1, 1, 0.5f));
			//		//}
			//	}
			//}

			//if (m_selectedTile != null)
			//	this.DrawSelectBox(m_selectedTile.x, m_selectedTile.y, Colors.Red);
		}
	}
}