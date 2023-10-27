using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace urd
{
	public class Entity : Component
	{
		private static LinkedList<Entity> _InstanceIndex = new LinkedList<Entity>();
		private static Dictionary<TileCell, LinkedList<Entity>> _SpaceIndex;

		public static IEnumerable<Entity> IterateInstance()
		{
			for (var it = _InstanceIndex.First; it != null; it = it.Next)
				yield return it.Value;
		}
		public static IEnumerable<Entity> GetInstanceInTile(TileCell tile)
		{
			if (_SpaceIndex.TryGetValue(tile, out var set))
			{ 
				foreach(var en in set)
					yield return en;
			}
		}

		private LinkedListNode<Entity> _itor = null;
		private LinkedListNode<Entity> _spaceItor = null;

		private string m_name;

		private WorldGrid m_world;
		
		public string name { set => m_name = value; get => m_name; }
		public WorldGrid world { get => m_world; }

		private vec2i _coord;
		public vec2i coord { 
			get => _coord; 
			set 
			{
				if (value != _coord)
				{
					// 删除旧位置的空间索引
					if (_spaceItor != null)
						_SpaceIndex[this.currentTile].Remove(_spaceItor);

					// 设置新位置的空间索引
					_coord = value;

					LinkedList<Entity> enSet;
					if (!_SpaceIndex.TryGetValue(this.currentTile, out enSet))
					{ 
						enSet = new LinkedList<Entity>();
						_SpaceIndex[this.currentTile] = enSet;
					}
					_spaceItor = enSet.AddLast(this);
				}
			}
		}
		
		public bool block { private set; get; }

		// 当前所在的瓦片
		public TileCell currentTile
		{
			get
			{
				this.world.tryGetTile(_coord.x, _coord.y, out var tile);
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
