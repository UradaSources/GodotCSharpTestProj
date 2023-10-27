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
		private LinkedListNode<Entity> _spaceItor = null;

		private string m_name;
		private WorldGrid m_world;
		private bool m_block;
		
		public string name { set => m_name = value; get => m_name; }
		public WorldGrid world { get => m_world; }
		public bool block { set => m_block = value; get => m_block; }

		private vec2i _coord;
		public vec2i coord { 
			set => _coord = value;
			get => _coord;
		}

		// 当前所在的瓦片
		public TileCell currentTile
		{
			get
			{
				this.world.tryGetTile(this.coord.x, this.coord.y, out var tile);
				return tile;
			}
		}

		// 获取附近的瓦片
		public TileCell getNearTile(vec2i offset, bool loop = false)
		{
			var coord = _coord + offset;
			if (loop)
			{
				coord.x = Mathf.LoopIndex(coord.x, m_world.width);
				coord.y = Mathf.LoopIndex(coord.y, m_world.height);
			}

			this.world.tryGetTile(coord.x, coord.y, out var tile);
			return tile;
		}

		public override void _update(float delta)
		{
			DebugDisplay.Main.outObject(this.name, this);
		}

		public Entity(string name, WorldGrid world, vec2i coord, bool block = true)
		{
			m_name = name;
			m_world = world;

			this.coord = coord;
			this.block = block;

			_itor = Entity._InstanceIndex.AddLast(this);
		}
		~Entity()
		{
			Entity._InstanceIndex.Remove(_itor);
		}
	}
}
