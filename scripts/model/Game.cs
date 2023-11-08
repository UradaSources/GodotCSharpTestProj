using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
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

	private Sprite m_treeSprite;
	private Sprite m_treeSprite2;

	private float[,] m_noiseValue;

	public void createTileTypes()
	{
		m_tileTypeList.Add(new TileType("ground", new Sprite("ground", '.', byteColor.FromHex(0xA77B5B)), 1, (ulong)TileType.BuiltinTags.Ground));
		m_tileTypeList.Add(new TileType("grass", new Sprite("grass", '.', byteColor.FromHex(0x567b79)), 1.1f, (ulong)TileType.BuiltinTags.Ground));
		m_tileTypeList.Add(new TileType("wall", new Sprite("wall", 'X', byteColor.FromHex(0x45444f)), -1, (ulong)TileType.BuiltinTags.Wall));
		m_tileTypeList.Add(new TileType("floor", new Sprite("floor", '-', byteColor.FromHex(0x80493A)), 1, (ulong)TileType.BuiltinTags.Floor));
		m_tileTypeList.Add(new TileType("river", new Sprite("river", '`', byteColor.FromHex(0x4B80CA)), 5, (ulong)TileType.BuiltinTags.Water));
		m_tileTypeList.Add(new TileType("deep_river", new Sprite("deep_river", '`', byteColor.FromHex(0x3a3858)), -1, (ulong)TileType.BuiltinTags.Water));
	}

	// 地形生成
	public void terrain()
	{
		// 生成河流
		var riverStart = new vec2i(mathf.random(1, m_mainWorld.width / 2), 0);
		var riverEnd = m_mainWorld.size - vec2i.one;

		var riverPath = new List<TileCell>();
		m_pathGenerator.generatePath(riverStart, riverEnd, ref riverPath, StandardPathfindCost.Default);

		foreach (var cell in riverPath)
			cell.tile = m_tileTypeList[3];

		for (int i = 0; i < mathf.random(0, m_mainWorld.tileCount / 3); i++)
		{
			int x, y;
			do
			{
				x = mathf.random(0, m_mainWorld.width - 1);
				y = mathf.random(0, m_mainWorld.height - 1);
			}
			while (m_mainWorld.getTile(x, y).tile.tags != (ulong)TileType.BuiltinTags.Ground);

			var tree = new Entity("tree");
			tree.add(new WorldEntity(m_mainWorld, new vec2i(x, y)));

			m_entityList.Add(tree);
		}
	}

	public void createWorld(int w, int h, int seed)
	{
		mathf.randomSeed(seed);

		m_mainWorld = new WorldGrid("main world", w, h);
		m_pathGenerator = new PathGenerator(m_mainWorld);

		var noise = new FastNoiseLite();
		noise.Seed = 12345;
		noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;

		float nosicScale = 3.0f;
		float nosicOffset = 20.0f;

		for (int i = 0; i < m_mainWorld.tileCount; i++)
		{
			var tile = m_mainWorld.rawGetTile(i);

			var nosicCoord = new Vector2(tile.x, tile.y) * nosicScale + Vector2.One * nosicOffset;
			var tr = noise.GetNoise2Dv(nosicCoord);

			if (tr < 0.005f)
				tile.tile = m_tileTypeList[5];
			else if (tr < 0.1f)
				tile.tile = m_tileTypeList[4];
			else if (tr < 0.2f)
				tile.tile = m_tileTypeList[1];
			else if (tr > 0.4f)
				tile.tile = m_tileTypeList[2];
			else
				tile.tile = m_tileTypeList[0];
		}

		for (int i = 0; i < m_mainWorld.tileCount / 15; i++)
		{
			int x, y;
			do
			{
				x = mathf.random(0, m_mainWorld.width - 1);
				y = mathf.random(0, m_mainWorld.height - 1);
			}
			while (m_mainWorld.getTile(x, y).tile.tags != (ulong)TileType.BuiltinTags.Ground);

			var tree = new Entity("tree");
			tree.add(new WorldEntity(m_mainWorld, new vec2i(x, y)));

			m_entityList.Add(tree);
		}
	}

	public void initEntity()
	{
		var player = new Entity("player");

		player.add(new WorldEntity(m_mainWorld, vec2i.zero));
		player.add(new Movement(3.0f, vec2i.zero));
		player.add(new Navigation(m_pathGenerator));
		player.add(new RandomWalkControl());

		m_entityList.Add(player);

		for (int i = 0; i < 1; i++)
		{
			int x = mathf.random(0, m_mainWorld.width - 1);
			int y = mathf.random(0, m_mainWorld.height - 1);

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
		this.createWorld(40, 40, 12345);
		this.initEntity();

		m_entitySprite = new Sprite("entity", 'C', byteColor.white);
		m_targetCellSprite = new Sprite("target_cell", 'x', new byteColor(0, 126, 0, 126));

		m_treeSprite = new Sprite("tree", 'T', byteColor.FromHex(0x75A743));
		m_treeSprite2 = new Sprite("tree", 't', byteColor.FromHex(0x75A743));
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
				if (entity.name == "tree")
				{ 
					context.drawCharSprite(worldEn.coord, m_treeSprite);
				}
				else
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

		//for (int y = 0; y < m_noiseValue.GetLength(1); y++)
		//{
		//	for (int x = 0; x < m_noiseValue.GetLength(0); x++)
		//	{
		//		var c = (byte)(m_noiseValue[x, y] * 255);
		//		context.drawBox(new vec2i(x, y), new byteColor(c, c, c), true);
		//		context.drawString(new vec2i(x, y), ((int)(m_noiseValue[x, y] *10)).ToString(), 12);
		//	}
		//}
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