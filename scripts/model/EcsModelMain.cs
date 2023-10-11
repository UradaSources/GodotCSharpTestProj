using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Arch.Core;
using Arch.Core.Utils;
using Godot;

namespace urd.model
{
	public partial class EcsModelMain : Node2D
	{
		public static Rect2I GetSpriteRect(char c)
		{
			const int SpriteLineCount = 8;
			const int SpriteUnitSize = 8;

			int index = c - ' ';

			int x = index % SpriteLineCount;
			int y = index / SpriteLineCount;

			return new Rect2I(new Vector2I(x, y) * SpriteUnitSize, Vector2I.One * SpriteUnitSize);
		}

		[ExportGroup("grid params")]
		[Export] private int m_width;
		[Export] private int m_height;

		[ExportGroup("view params")]
		[Export] private Texture2D m_tex;
		[Export] private float m_tileSize;

		private WorldGrid m_world;
		private Arch.Core.World m_registry;

		private Entity m_player;

		//private sys_updateMoveProcess p1 = default;
		//private sys_updateMoveDirect p2 = default;
		//private sys_startMoveProcess p3 = default;
		//private sys_updateMoveControlInput p4 = default;

		public Vector2 CoordMapToPosition(int x, int y)
		{
			return new Vector2(x, y) * m_tileSize;
		}

		public void DrawSprite(int x, int y, char c, CanvasItem canvas)
		{
			var pos = CoordMapToPosition(x, y);

			var target = new Rect2(pos, Vector2.One * m_tileSize);
			var source = GetSpriteRect(c);

			canvas.DrawTextureRectRegion(m_tex, target, source);
		}
		public void DrawSprite(Vector2 pos, char c, CanvasItem canvas)
		{
			var target = new Rect2(pos, Vector2.One * m_tileSize);
			var source = GetSpriteRect(c);

			canvas.DrawTextureRectRegion(m_tex, target, source);
		}

		public override void _Ready()
		{
			Debug.Assert(m_width > 0 && m_height > 0);

			var rng = new RandomNumberGenerator();

			// 初始化网格地图
			m_world = new WorldGrid(m_width, m_height);
			for (int i = 0; i < m_world.tileCount(); i++)
				m_world.rawGetTile(i).pass = rng.Randf() > 0.1f;

			m_world.rawGetTile(0).pass = true;

			// 初始化实体
			m_registry = Arch.Core.World.Create();

			var playerComs = new ComponentType[] {
				typeof(com_worldRef),
				typeof(com_coord),
				typeof(com_moveProcess),
				typeof(com_moveDirect),
				typeof(com_moveSpeed) };

			m_player = m_registry.Create(playerComs);
			m_registry.Set(m_player, new com_worldRef { world = m_world });
			m_registry.Set(m_player, new com_coord { x = 1, y = 0 });
			m_registry.Set(m_player, new com_moveSpeed { speed = 2.0f });
		}

		public override void _Process(double delta)
		{
			Time.deltaTime = (float)delta;

			var q = new QueryDescription();

			m_registry.InlineEntityQuery<sys_updateMoveProcess, 
				com_moveProcess, com_coord, com_moveSpeed>(in q);
			m_registry.InlineEntityQuery<sys_updateMoveDirect,
				com_cacheMoveControlInput, com_worldRef, com_coord, com_moveDirect>(in q);

			m_registry.InlineEntityQuery<sys_startMoveProcess,
				com_moveProcess, com_worldRef, com_coord, com_moveDirect>(in q);
			m_registry.InlineEntityQuery<sys_updateMoveControlInput,
				com_cacheMoveControlInput>(in q);
		}

		public override void _Draw()
		{
			for (int i = 0; i < m_world.tileCount(); i++)
			{
				var tile = m_world.rawGetTile(i);
				this.DrawSprite(tile.x, tile.y, tile.pass ? '.' : '#', this);
			}

			//var q = new QueryDescription();

			//m_registry.Query(in q, (in Entity en, ref com_coord coord, ref com_moveProcess process) => {
			//	float t = Godot.Time.GetTicksMsec() % 1;
			//	if (!process.processing || t < 0.6f)
			//	{
			//		this.DrawSprite(coord.x, coord.y, '@', this);
			//	}
			//});
		}
	}
}