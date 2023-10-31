using System.IO;
using System.Linq;
using Godot;

namespace urd
{
	public partial class World : Node2D
	{
		private const string WorldSavePath = "./save/world.json";
		private const string TileTypeSavePath = "./save/tileTypes.json";

		private static Godot.Color GDColor(Color c)
		{
			return new Godot.Color((float)c.r / 255, (float)c.g / 255, (float)c.b / 255, (float)c.a / 255);
		}
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
				TileType.Create('T', Color.FromHex(0x8AB969), 1.5f);
				TileType.Create('#', Color.FromHex(0x3A3858), -1.0f);
				TileType.Create('D', Color.FromHex(0x819796), 3.0f);
				TileType.Create('.', Color.FromHex(0xA77B5B), 1.0f);
				TileType.Create('_', Color.FromHex(0x89493A), 0.8f);
				TileType.Create('`', Color.FromHex(0x68C2D3), 3.0f);
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

		private void drawCharSprite(int x, int y, char c, Color? color = null)
		{
			var pos = new Vector2(x, y) * m_tileSize;
			var target = new Rect2(pos, Vector2.One * m_tileSize);

			var source = GetCharSpriteRect(16, 8, c);
			this.DrawTextureRectRegion(m_charSheet, target, source, GDColor(color ?? Color.white));
		}
		private void drawBox(int x, int y, Color? color = null, bool fill = false)
		{
			var pos = new Vector2(x, y) * m_tileSize;

			var target = new Rect2(pos, Vector2.One * m_tileSize);
			this.DrawRect(target, GDColor(color ?? Color.white), fill);
		}

		public vec2i mapToCoord(vec2 pos)
		{
			var local = this.ToLocal(new Vector2(pos.x, pos.y));

			int x = (int)(local.X / m_tileSize);
			int y = (int)(local.Y / m_tileSize);
			return new vec2i(x, y);
		}
		public vec2 getPosition(vec2i coord)
		{
			var local = (vec2)coord * m_tileSize;
			var golbal = this.ToGlobal(new Vector2(local.x, local.y));

			return new vec2(golbal.X, golbal.Y);
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
				this.drawCharSprite(tile.x, tile.y, tile.type.graph, tile.type.c);
			}

			// 绘制所有处于活动状态且位于本地图上的实体
			foreach (var en in Entity.IterateInstance()
				.Where((Entity e) => e.container != null && e.world == this.model))
			{
				this.drawCharSprite(en.coord.x, en.coord.y, 'C');
			}
		}
	}
}