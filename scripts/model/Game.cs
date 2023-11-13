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

// 开垦农田
public class ChangeTile : Job
{
	private float m_totalProgress;
	private float m_progress;

	private bool m_completed;

	private Entity m_handler;

	private TileCell m_target;
	private TileType m_type;

	private float m_workToMake;

	public void update(float dt)
	{

	}

	public ChangeTile(TileCell target, TileType type, float workToMake)
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
	private WorldPath m_pathGenerator;
	private WorldSeek m_seek;

	private List<TileType> m_tileTypeList = new List<TileType>();

	public void assets()
	{
		m_tileTypeList.Add(new TileType("ground", new Sprite("ground", '.', rgba.Byte(128, 73, 58)), 1, (ulong)TileType.BuiltinTags.Ground));
		m_tileTypeList.Add(new TileType("grass", new Sprite("grass", '.', rgba.Hex(0x567b79)), 1.1f, (ulong)TileType.BuiltinTags.Ground));
		m_tileTypeList.Add(new TileType("rock", new Sprite("rock", 'R', rgba.Hex(0x45444f)), -1, (ulong)TileType.BuiltinTags.Wall));
		m_tileTypeList.Add(new TileType("floor", new Sprite("floor", '-', rgba.Hex(0x80493A)), 1, (ulong)TileType.BuiltinTags.Floor));
		m_tileTypeList.Add(new TileType("river", new Sprite("river", '`', rgba.Hex(0x4B80CA)), 5, (ulong)TileType.BuiltinTags.Water));
		m_tileTypeList.Add(new TileType("deep_river", new Sprite("deep_river", '`', rgba.Hex(0x3a3858)), -1, (ulong)TileType.BuiltinTags.Water));
		m_tileTypeList.Add(new TileType("gravel", new Sprite("gravel", '.', rgba.Hex(0x45444f)), 2.0f, (ulong)TileType.BuiltinTags.Ground));
		m_tileTypeList.Add(new TileType("cropland", new Sprite("cropland", '+', rgba.Hex(0xa77b5b)), 2.0f, (ulong)TileType.BuiltinTags.Ground));

		new Sprite("player", 'P', rgba.Byte(184, 181, 185));
		new Sprite("enemy", 'E', rgba.Byte(184, 181, 185));
		new Sprite("tree", 'T', rgba.Byte(123, 114, 67));
		new Sprite("bush", '`', rgba.Byte(138, 176, 96));

		new Sprite("target_cell", 'x', new rgba(0, 126, 0, 126));

		m_mainWorld = new WorldGrid("main world", 40, 40);
		m_pathGenerator = new WorldPath(m_mainWorld);
		m_seek = new WorldSeek(m_mainWorld);
	}

	public void createWorld(int seed)
	{
		mathf.randomSeed(seed);

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
				var sample = mathf.mapTo01(sampleNoise.GetNoise2Dv(sampleCoord), 0.5f, -0.5f);

				float br = mathf.mapTo01(treeNoise.GetNoise2D(tile.x * 3, tile.y * 3), 0.5f, -0.5f) * sample;
				if (tr < 0.4f || tr > 0.8f) br -= 0.2f;
				if (br > 0.5f)
				{
					var tree = new Entity("tree");
					tree.add(new WorldEntity(m_mainWorld, new vec2i(tile.x, tile.y), 2.0f));
					tree.add(new DrawSprite(Object.Get<Sprite>().First(t => t.name == "tree")));
				}
				else if (br > 0.2f)
				{
					var bush = new Entity("bush");
					bush.add(new WorldEntity(m_mainWorld, new vec2i(tile.x, tile.y), 1.5f));
					bush.add(new DrawSprite(Object.Get<Sprite>().First(t=>t.name == "bush")));
				}
			}
		}
	}

	public void initEntity()
	{
		var playSp = Sprite.Get<Sprite>().First(t => t.name == "player");

		var player = new Entity("player");
		player.add(new WorldEntity(m_mainWorld, vec2i.zero, 5.0f));
		player.add(new Movement(3.0f, vec2i.zero));
		player.add(new Navigation(m_pathGenerator));
		player.add(new RandomWalkControl());
		player.add(new DrawSprite(playSp));

		var enemySp = Sprite.Get<Sprite>().First(t => t.name == "enemy");
		for (int i = 0; i < 5; i++)
		{
			int x = mathf.random(0, m_mainWorld.width - 1);
			int y = mathf.random(0, m_mainWorld.height - 1);

			var enemy = new Entity("enemy");
			enemy.add(new WorldEntity(m_mainWorld, new vec2i(x, y), 5.0f));
			enemy.add(new Movement(2.0f, vec2i.zero));
			enemy.add(new Navigation(m_pathGenerator));
			enemy.add(new FollowWalkControl()).target = player.get<WorldEntity>();
			enemy.add(new DrawSprite(enemySp));
		}
	}

	private Foo[] m_foo;

	public void init()
	{
		Instance = this;

		this.assets();
		this.createWorld(12345);
		this.initEntity();

		m_foo = new Foo[m_mainWorld.tileCount];
	}

	public void update(float delta)
	{
		if (m_mainLoop)
		{
			foreach (var en in Object.Get<Entity>()
				.Where(e => e.active))
				en.update(delta);

			foreach (var en in Object.Get<Entity>()
				.Where(e => e.active))
				en.lateUpdate(delta);

			for (int i = 0; i < m_foo.Length; i++)
				m_foo[i].value = 1.0f;

			foreach (var en in Object.Get<WorldEntity>().Where(t => t.entity.name == "player" || t.entity.name == "enemy"))
			{
				foreach (var cell in m_seek.bfsCircle(en.coord, 3))
				{
					m_foo[m_mainWorld.toIndex(cell.x, cell.y)].value -= 1f;
				}
			}
		}
	}

	private struct Foo
	{
		public TileCell cell;
		public vec2i coord;
		public float value;
	}

	public void draw()
	{
		var context = RenderServer.Instance;

		for (int i = 0; i < m_mainWorld.tileCount; i++)
		{
			var cell = m_mainWorld.rawGetTile(i);
			context.drawCharSprite(new vec2i(cell.x, cell.y), cell.tile.sprite);
		}

		var targetCellSp = Object.Get<Sprite>().First(e => e.name == "target_cell");
		foreach (var entity in Object.Get<Entity>().Where(e => e.has<DrawSprite>()))
		{
			var drawSprite = entity.get<DrawSprite>();
		 	var worldEn = entity.get<WorldEntity>();
			if (worldEn != null)
			{
				context.drawCharSprite(worldEn.coord, drawSprite.sprite);

				// 绘制目标格子
				var motion = entity.get<Movement>();
				if (motion != null && motion.processing)
				{
					var target = worldEn.getNearTile(motion.currentDirect);
					context.drawCharSprite(new vec2i(target.x, target.y), targetCellSp);

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

		for (int i = 0; i < m_mainWorld.tileCount; i++)
		{
			var cell = m_mainWorld.rawGetTile(i);
			if (m_foo[i].value > 0)
				context.drawBox(new vec2i(cell.x, cell.y), rgba.Float(0,0,0, 0.5f * m_foo[i].value), true);
		}

		//for (int i = 0; i < m_mainWorld.tileCount; i++)
		//{
		//	var cell = m_mainWorld.rawGetTile(i);
		//	context.drawBox(new vec2i(cell.x, cell.y), rgba.Float(0,0,0, foo[i].value));
		//}

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
	private Stopwatch m_timer;
	private GameLoop m_loop;

	public override void _Ready()
	{
		m_timer = new Stopwatch();

		m_loop = new GameLoop();
		m_loop.init();
	}

	public override void _Process(double delta)
	{
		m_timer.Restart();
		m_loop.update((float)delta);
		m_timer.Stop();

		DebugWatch.Main.watchValue("fps", $"time: {m_timer.ElapsedMilliseconds}: {1000.0f/60}");

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