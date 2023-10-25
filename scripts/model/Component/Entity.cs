using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace urd
{
	public class Entity : Component
	{
		private static LinkedList<Entity> _InstanceIndex = new LinkedList<Entity>();

		public static IEnumerable<Entity> IterateInstance()
		{
			for (var it = _InstanceIndex.First; it != null; it = it.Next)
				yield return it.Value;
		}

		private LinkedListNode<Entity> _itor = null;

		private WorldGrid m_world;
		private vec2i m_coord;

		public WorldGrid world { get => m_world; }
		public vec2i coord { get => m_coord; set => m_coord = value; }

		// 当前所在的瓦片
		public TileCell currentTile
		{
			get
			{
				this.world.tryGetTile(m_coord.x, m_coord.y, out var tile);
				return tile;
			}
		}

		// 获取附近的瓦片
		public TileCell getNearTile(vec2i offset, bool loop = false)
		{
			var coord = m_coord + offset;
			if (loop)
			{
				coord.x = Mathf.LoopIndex(coord.x, m_world.width);
				coord.y = Mathf.LoopIndex(coord.y, m_world.height);
			}

			this.world.tryGetTile(coord.x, coord.y, out var tile);
			return tile;
		}

		public Entity(WorldGrid world, vec2i coord)
		{
			this.m_world = world;
			this.m_coord = coord;

			_itor = Entity._InstanceIndex.AddLast(this);
		}
		~Entity()
		{
			Entity._InstanceIndex.Remove(_itor);
		}
	}
}
