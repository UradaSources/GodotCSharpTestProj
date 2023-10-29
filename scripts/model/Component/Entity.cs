using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace urd
{
	public class Entity : Component
	{
		private static LinkedList<Entity> _Instances = new LinkedList<Entity>();
		public static IEnumerable<Entity> IterateInstance()
		{
			for (var it = _Instances.First; it != null; it = it.Next)
				yield return it.Value;
		}

		private readonly LinkedListNode<Entity> _itor = null;

		private WorldGrid m_grid;
		private vec2i m_coord;

		private bool m_block;

		public WorldGrid grid { set => m_grid = value; get => m_grid; }
		public vec2i coord { set => m_coord = value; get => m_coord; }

		public bool block { set => m_block = value; get => m_block; }

		// 当前所在的瓦片
		public TileCell currentTile
		{
			get
			{
				this.grid.tryGetTile(this.coord.x, this.coord.y, out var tile);
				return tile;
			}
		}

		// 获取附近的瓦片
		public TileCell getNearTile(vec2i offset, bool loop = false)
		{
			var coord = m_coord + offset;
			if (loop)
			{
				coord.x = mathf.loopIndex(coord.x, m_grid.width);
				coord.y = mathf.loopIndex(coord.y, m_grid.height);
			}

			this.grid.tryGetTile(coord.x, coord.y, out var tile);
			return tile;
		}

		public Entity(WorldGrid world, vec2i coord, bool block = true)
		{
			m_grid = world;
			
			this.coord = coord;
			this.block = block;

			Debug.WriteLine($"Entity {this.name} created", $"{this.GetType().Name}.Info");
			_itor = Entity._Instances.AddLast(this);
		}
		~Entity()
		{
			Entity._Instances.Remove(_itor);
		}
	}
}
