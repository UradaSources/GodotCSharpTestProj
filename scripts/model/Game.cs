using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Godot;
using urd;

public class GameLoop
{
	public static GameLoop Instance { private set; get; }

	private bool m_mainLoop = true;

	private WorldGrid m_mainWorld;
	private PathGenerator m_pathGenerator;

	private List<TileType> m_tileTypeList = new List<TileType>();
	private List<Entity> m_entityList = new List<Entity>();

	private Sprite m_entitySprite;
	private Sprite m_targetCellSprite;

	public void createTileTypes()
	{
		m_tileTypeList.Add(new TileType("ground", new Sprite("ground", '.', byteColor.FromHex(0xA77B5B)), 1, (ulong)TileType.BuiltinTags.Ground));
		m_tileTypeList.Add(new TileType("wall", new Sprite("wall", '#', byteColor.FromHex(0xF2F015)), -1, (ulong)TileType.BuiltinTags.Wall));
		m_tileTypeList.Add(new TileType("floor", new Sprite("floor", '-', byteColor.FromHex(0x80493A)), 1, (ulong)TileType.BuiltinTags.Floor));
		m_tileTypeList.Add(new TileType("river", new Sprite("river", '`', byteColor.FromHex(0x4B80CA)), 5, (ulong)TileType.BuiltinTags.Water));
	}

	public void createWorld(int w, int h, int seed)
	{
		m_mainWorld = new WorldGrid("main world", w, h);
		m_pathGenerator = new PathGenerator(m_mainWorld);

		for (int i = 0; i < m_mainWorld.tileCount; i++)
			m_mainWorld.rawGetTile(i).tile = m_tileTypeList[0];

		// 生成河流
		var riverStart = new vec2i(mathf.random(1, m_mainWorld.width / 2), 0);
		var riverEnd = m_mainWorld.size - vec2i.one;

		var riverPath = new List<TileCell>();
		m_pathGenerator.generatePath(riverStart, riverEnd, ref riverPath, StandardPathfindCost.Default);

		Debug.WriteLine($"生成河流从{riverStart}到{riverEnd}, 共{riverPath.Count}");

		foreach (var cell in riverPath) 
			cell.tile = m_tileTypeList[3];

		// 随机创建房屋
		//for (int roomId = 0; roomId < 2;)
		//{
		//	vec2i roomSize = new vec2i(random.Next(4, 6), random.Next(4, 6));
		//	vec2i roomMin = new vec2i(random.Next(m_mainWorld.width - roomSize.x),
		//		random.Next(m_mainWorld.height - roomSize.y));

		//	// 检查时多向外检查一圈
		//	for (int y = roomMin.y - 1; y <= roomMin.y + roomSize.y; y++)
		//	{
		//		for (int x = roomMin.x - 1; x <= roomMin.x + roomSize.x; x++)
		//		{
		//			if (!m_mainWorld.vaildCoord(x, y)) continue;

		//			int index = m_mainWorld.toIndex(x, y);
		//			var cell = m_mainWorld.rawGetTile(index);

		//			// 必须为地面
		//			if ((cell.tile.tags | (ulong)TileType.BuiltinTags.Ground) == 0)
		//			{
		//				// 重新生成
		//				continue;
		//			}
		//		}
		//	}

		//	// 设置墙
		//	for (int x = roomMin.x; x < roomMin.x + roomSize.x; x++)
		//	{
		//		var cell = m_mainWorld.rawGetTile(m_mainWorld.toIndex(x, roomMin.y));
		//		cell.tile = m_tileTypeList[1];

		//		cell = m_mainWorld.rawGetTile(m_mainWorld.toIndex(x, roomMin.y + roomSize.y - 1));
		//		cell.tile = m_tileTypeList[1];
		//	}
		//	for (int y = roomMin.y; y < roomMin.y + roomSize.y; y++)
		//	{
		//		var cell = m_mainWorld.rawGetTile(m_mainWorld.toIndex(x, roomMin.y));
		//		cell.tile = m_tileTypeList[1];

		//		cell = m_mainWorld.rawGetTile(m_mainWorld.toIndex(x, roomMin.y + roomSize.y - 1));
		//		cell.tile = m_tileTypeList[1];
		//	}
		//}
	}

	public void initEntity()
	{
		var player = new Entity("player");

		player.add(new WorldEntity(m_mainWorld, vec2i.zero));
		player.add(new Movement(3.0f, vec2i.zero));
		player.add(new Navigation(m_pathGenerator));
		player.add(new RandomWalkControl());

		m_entityList.Add(player);

		var rng = new RandomNumberGenerator();
		for (int i = 0; i < 1; i++)
		{
			int x = rng.RandiRange(0, m_mainWorld.width - 1);
			int y = rng.RandiRange(0, m_mainWorld.height - 1);

			var enemy = new Entity("enemy");

			enemy.add(new WorldEntity(m_mainWorld, new vec2i(x, y)));
			enemy.add(new Movement(2.0f, vec2i.zero));
			enemy.add(new Navigation(m_pathGenerator));
			enemy.add(new FollowWalkControl()).target = player.get<WorldEntity>();

			m_entityList.Add(enemy);
		}
	}

	public void init()
	{
		Instance = this;

		this.createTileTypes();
		this.createWorld(20, 20, 1234);
		this.initEntity();

		m_entitySprite = new Sprite("entity", 'C', byteColor.white);
		m_targetCellSprite = new Sprite("target_cell", 'x', new byteColor(0, 126, 0, 126));
	}

	public void update(float delta)
	{
		if (m_mainLoop)
		{
			foreach (var en in m_entityList
				.Where((Entity e) => e.active))
				en.update(delta);

			foreach (var en in m_entityList
				.Where((Entity e) => e.active))
				en.lateUpdate(delta);
		}
	}

	public void draw()
	{
		var context = RenderServer.Instance;

		for (int i = 0; i < m_mainWorld.tileCount; i++)
		{
			var cell = m_mainWorld.rawGetTile(i);
			context.drawCharSprite(new vec2i(cell.x, cell.y), cell.tile.sprite);
		}

		foreach (var entity in m_entityList)
		{
		 	var worldEn = entity.get<WorldEntity>();
			if (worldEn != null)
			{
				// 绘制角色本身
				context.drawCharSprite(worldEn.coord, m_entitySprite);

				// 绘制目标格子
				var motion = entity.get<Movement>();
				if (motion != null && motion.processing)
				{
					var target = worldEn.getNearTile(motion.currentDirect);
					context.drawCharSprite(new vec2i(target.x, target.y), m_targetCellSprite);

					//// 绘制寻路路径
					//var navigation = entity.container.getComponent<Navigation>();
					//for (int i = 0; i < navigation.pathNodeCount; i++)
					//{
					//	var node = navigation.getPathNode(i);
					//	this.DrawCharacterSprite(node.x, node.y, 'x', new Color(1, 1, 1, 0.5f));
					//}
				}
			}
		}
	}
}

public partial class Game : Node2D
{
	private GameLoop m_loop;

	public override void _Ready()
	{
		m_loop = new GameLoop();
		m_loop.init();
	}

	public override void _Process(double delta)
	{
		m_loop.update((float)delta);

		// var mousePos = this.GetLocalMousePosition();
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

		// 暂停或是运行游戏
		//if (Input.IsActionJustPressed("ui_stop"))
		//	m_mainLoop = !m_mainLoop;
	}
}