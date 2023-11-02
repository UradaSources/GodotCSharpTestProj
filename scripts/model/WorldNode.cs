using System.IO;
using System.Linq;
using Godot;
using urd;

public partial class WorldNode : Node2D
{
	private const string WorldSavePath = "./save/world.json";
	private const string TileTypeSavePath = "./save/tileTypes.json";

	private static Rect2I GetCharSpriteRect(int lineCount, int size, char c)
	{
		int index = c - ' ';

		int x = index % lineCount;
		int y = index / lineCount;

		return new Rect2I(new Vector2I(x * size, y * size), new Vector2I(size, size));
	}

	[ExportGroup("params")]
	[Export] private Texture2D m_charSheet;
	[Export] private float m_tileSize = 20;

	public WorldGrid model { private set; get; }
	public PathGenerator pathGenerator { private set; get; }

	public float tileSize => m_tileSize;

	public void initTileType(string path)
	{
		bool loadFromSavedSuccess = false;
		if (File.Exists(path))
		{
			var json = File.ReadAllText(path);
			TileType.InitSetFromJson(json);

			loadFromSavedSuccess = TileType.TypeCount > 0;
		}

		// 加载失败时使用默认值
		if (!loadFromSavedSuccess)
		{
			TileType.Create('T', MiscUtils.FromHex(0x8AB969), 1.5f);
			TileType.Create('#', MiscUtils.FromHex(0x3A3858), -1.0f);
			TileType.Create('D', MiscUtils.FromHex(0x819796), 3.0f);
			TileType.Create('.', MiscUtils.FromHex(0xA77B5B), 1.0f);
			TileType.Create('_', MiscUtils.FromHex(0x89493A), 0.8f);
			TileType.Create('`', MiscUtils.FromHex(0x68C2D3), 3.0f);
		}
	}
	public void initModel(string path)
	{
		bool loadFromSavedSuccess = false;
		if (File.Exists(path))
		{
			var json = File.ReadAllText(path);
			this.model = WorldGridUtils.TryFromJson(json);
			loadFromSavedSuccess = this.model != null && this.model.tileCount > 0;
		}

		// 加载失败时使用默认值
		if (!loadFromSavedSuccess)
			this.model = new WorldGrid(20, 20, TileType.Get(0));

		// 创建导航网格
		this.pathGenerator = new PathGenerator(this.model);
	}

	public void drawCharSprite(CanvasItem canvas, int x, int y, char c, Color? color = null)
	{
		var pos = new Vector2(x, y) * m_tileSize;
		var target = new Rect2(pos, Vector2.One * m_tileSize);

		var source = GetCharSpriteRect(16, 8, c);
		canvas.DrawTextureRectRegion(m_charSheet, target, source, color ?? Colors.White);
	}
	public void drawBox(CanvasItem canvas, int x, int y, Color? color = null, bool fill = false)
	{
		var pos = new Vector2(x, y) * m_tileSize;

		var target = new Rect2(pos, Vector2.One * m_tileSize);
		canvas.DrawRect(target, color ?? Colors.White, fill);
	}

	public Vector2I mapToCoord(Vector2 pos)
	{
		var local = this.ToLocal(pos);
		return (Vector2I)(local / m_tileSize);
	}
	public Vector2 getPosition(Vector2I coord)
	{
		var local = (Vector2)coord * m_tileSize;
		return this.ToGlobal(local);
	}

	public override void _Ready()
	{
		base._Ready();

		this.initTileType(TileTypeSavePath);
		this.initModel(WorldSavePath);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		this.QueueRedraw();

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

		//// 储存地图数据
		//if (Input.IsActionJustPressed("ui_save"))
		//{
		//	var json = WorldGridUtils.ToJson(m_world);
		//	File.WriteAllText(WorldDataFilePath, json);
		//	Debug.WriteLine($"save world data");
		//}
	}

	public override void _Draw()
	{
		base._Draw();

		// 绘制瓦片
		for (int i = 0; i < this.model.tileCount; i++)
		{
			var tile = this.model.rawGetTile(i);
			this.drawCharSprite(this, tile.x, tile.y, tile.type.graph, tile.type.c);
		}

		// 绘制所有处于活动状态且位于本地图上的实体
		foreach (var en in Entity.IterateInstance()
			.Where((Entity e) => e.container != null && e.world == this.model))
		{
			var graph = en.container.getComponent<Graph>();
			if (graph != null) this.drawCharSprite(this, en.coord.x, en.coord.y, graph.graph, graph.color);
		}
	}
}