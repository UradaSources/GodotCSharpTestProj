using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace urd
{
	public class Entity : Component
	{
		public struct ChangeWorldEventArgs
		{
			public WorldGrid origWorld;
			public vec2i origCoord;

			public WorldGrid world;
			public vec2i coord;
		}

		public delegate void ChangeWorldEventHandler(ChangeWorldEventArgs args);
		public delegate void ChangeCoordEventHandler(vec2i origCoord, vec2i coord);

		public event ChangeWorldEventHandler onChangeWorld;
		public event ChangeCoordEventHandler onChangeCoord;

		private static LinkedList<Entity> _Instances = new LinkedList<Entity>();
		public static IEnumerable<Entity> IterateInstance()
		{
			for (var it = _Instances.First; it != null; it = it.Next)
				yield return it.Value;
		}

		private readonly LinkedListNode<Entity> _itor = null;

		private WorldGrid m_world;
		private vec2i m_coord;

		private bool m_block;

		public WorldGrid world => m_world;
		public vec2i coord 
		{
			set
			{
				if (m_coord == value) return;

				var origCoord = m_coord;
				m_coord = value;

				this.onChangeCoord?.Invoke(origCoord, m_coord);
			}
			get => m_coord; 
		}

		public bool block { set => m_block = value; get => m_block; }

		// 当前所在的瓦片
		public TileCell currentTile
		{
			get
			{
				this.world.tryGetTile(this.coord.x, this.coord.y, out var tile);
				return tile;
			}
		}

		public void setWorld(WorldGrid world, vec2i coord)
		{
			var origWorld = m_world;
			var origCoord = m_coord;

			m_world = world;
			m_coord = coord;

			this.onChangeWorld.Invoke(new ChangeWorldEventArgs
			{
				origWorld = origWorld,
				origCoord = origCoord,
				world = world,
				coord = coord
			});
		}

		// 获取附近的瓦片
		public TileCell getNearTile(vec2i offset, bool loop = false)
		{
			var coord = m_coord + offset;
			if (loop)
			{
				coord.x = mathf.loopIndex(coord.x, m_world.width);
				coord.y = mathf.loopIndex(coord.y, m_world.height);
			}

			this.world.tryGetTile(coord.x, coord.y, out var tile);
			return tile;
		}

		public Entity(WorldGrid world, vec2i coord, bool block = true)
		{
			m_world = world;

			this.coord = coord;
			this.block = block;

			_itor = Entity._Instances.AddLast(this);
		}
		~Entity()
		{
			Entity._Instances.Remove(_itor);
		}

		public override void _update(float delta)
		{
			base._update(delta);

			DebugWatch.Main.watchObject(this);
		}
	}
}
