using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Godot;
using urd;

public abstract class Job : Object
{
	private Entity m_handler;

	private bool _completed;
	public bool completed 
	{
		protected set => _completed = value; 
		get => _completed;
	}

	public abstract float progressPer { get; }

	public Entity handler => m_handler;

	public void enter(Entity handler)
	{
		Debug.Assert(m_handler == null);
		m_handler = handler;
	}
	public void exit()
	{
		Debug.Assert(m_handler != null);
		m_handler = null;
	}

	void update(float dt);
};

public class Cultivate : Job
{
	private float m_totalProgress;
	private float m_progress;

	private bool m_completed;
	
	private Entity m_handler;

	private TileCell m_target;

	public float progressPer => mathf.clamp01(m_progress / m_totalProgress);
	public bool completed => m_completed;

	public Entity handler => m_handler;


	public void update(float dt)
	{
		m_progress += dt;
		if (this.progressPer > 1.0f)
			
	}

	public Cultivate(TileCell target)
	{
		m_target = target;

		m_totalProgress = target.tile.cost;
		m_progress = 0;
	}
}

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

	public void createTileTypes()
	{
		m_tileTypeList.Add(new TileType("ground", new Sprite("ground", '.', byteColor.FromHex(0xA77B5B)), 1, (ulong)TileType.BuiltinTags.Ground));
		m_tileTypeList.Add(new TileType("grass", new Sprite("grass", '.', byteColor.FromHex(0x567b79)), 1.1f, (ulong)TileType.BuiltinTags.Ground));
		m_tileTypeList.Add(new TileType("rock", new Sprite("rock", 'R', byteColor.FromHex(0x45444f)), -1, (ulong)TileType.BuiltinTags.Wall));
		m_tileTypeList.Add(new TileType("floor", new Sprite("floor", '-', byteColor.FromHex(0x80493A)), 1, (ulong)TileType.BuiltinTags.Floor));
		m_tileTypeList.Add(new TileType("river", new Sprite("river", '`', byteColor.FromHex(0x4B80CA)), 5, (ulong)TileType.BuiltinTags.Water));
		m_tileTypeList.Add(new TileType("deep_river", new Sprite("deep_river", '`', byteColor.FromHex(0x3a3858)), -1, (ulong)TileType.BuiltinTags.Water));
		m_tileTypeList.Add(new TileType("gravel", new Sprite("gravel", '.', byteColor.FromHex(0x45444f)), 2.0f, (ulong)TileType.BuiltinTags.Ground));
		m_tileTypeList.Add(new TileType("cropland", new Sprite("cropland", '+', byteColor.FromHex(0xa77b5b)), 2.0f, (ulong)TileType.BuiltinTags.Ground));
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

		var treeNoise = new FastNoiseLite();
		treeNoise.Seed = 11111;
		treeNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;

		var sampleNoise = new FastNoiseLite();
		sampleNoise.Seed = 54321;
		sampleNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;

		for (int i = 0; i < m_mainWorld.tileCount; i++)
		{
			var tile = m_mainWorld.rawGetTile(i);

			var nosicCoord = new Vector2(tile.x, tile.y) * nosicScale + Vector2.One * nosicOffset;
			var tr = mathf.mapTo01(noise.GetNoise2Dv(nosicCoord), 0.5f, -0.5f);

			if (tr < 0.15f)
				tile.tile = m_tileTypeList[5];
			else if (tr < 0.3f)
				tile.tile = m_tileTypeList[4];
			else if (tr < 0.4f)
				tile.tile = m_tileTypeList[1];
			else if (tr > 0.9f)
				tile.tile = m_tileTypeList[2];
			else if (tr > 0.8f)
				tile.tile = m_tileTypeList[6];
			else
				tile.tile = m_tileTypeList[0];

			if ((tile.tile.tags & (ulong)TileType.BuiltinTags.Ground) > 0)
			{
				// 创建树
				var sampleCoord = new Vector2(tile.x, tile.y) * 20.0f;
				var sample = mathf.mapTo01(sampleNoise.GetNoise2Dv(sampleCoord), 1.0f, -1.0f);

				if (mathf.mapTo01(treeNoise.GetNoise2D(tile.x * 3, tile.y * 3), 1, -1) * sample > 0.45f)
				{
					var tree = new Entity("tree");
					tree.add(new WorldEntity(m_mainWorld, new vec2i(tile.x, tile.y)));

					m_entityList.Add(tree);
				}
			}
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

		Stopwatch sw = new Stopwatch();

		sw.Start();
		this.createTileTypes();
		sw.Stop();
		Debug.WriteLine("createTileTypes: " + sw.ElapsedMilliseconds);

		sw.Restart();
		this.createWorld(80, 80, 12345);
		sw.Stop();
		Debug.WriteLine("createWorld: " + sw.ElapsedMilliseconds);

		sw.Restart();
		this.initEntity();
		sw.Stop();
		Debug.WriteLine("initEntity: " + sw.ElapsedMilliseconds);

		m_entitySprite = new Sprite("entity", 'C', byteColor.white);
		m_targetCellSprite = new Sprite("target_cell", 'x', new byteColor(0, 126, 0, 126));

		m_treeSprite = new Sprite("tree", 'T', byteColor.FromHex(0x75A743));
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

		//for (int y = 0; y < sample.GetLength(1); y++)
		//{
		//	for (int x = 0; x < sample.GetLength(0); x++)
		//	{
		//		var c = (byte)(sample[x, y] * 255);
		//		context.drawBox(new vec2i(x, y), new byteColor(c, c, c), true);
		//		context.drawString(new vec2i(x, y), ((int)(sample[x, y] * 10)).ToString(), 12);
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