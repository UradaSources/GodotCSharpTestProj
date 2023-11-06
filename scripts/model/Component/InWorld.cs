using System.Diagnostics;

namespace urd
{
	public class InWorld : Component
	{
		public struct ChangeWorldEventArgs
		{
			public WorldGrid oldWorld;
			public vec2i oldCoord;

			public WorldGrid world;
			public vec2i coord;
		}

		public delegate void WorldChangedEventHandler(ChangeWorldEventArgs args);
		public delegate void CoordChangedEventHandler(vec2i oldCoord, vec2i coord);

		public event WorldChangedEventHandler onWorldChanged;
		public event CoordChangedEventHandler onCoordChanged;

		private WorldGrid m_world;
		private float m_cost;

		public WorldGrid world => m_world;

		private vec2i _coord;
		public vec2i coord
		{
			set
			{
				if (_coord == value) return;

				var origCoord = _coord;
				_coord = value;

				this.onCoordChanged?.Invoke(origCoord, _coord);
			}
			get => _coord;
		}

		public float cost { set => m_cost = value; get => m_cost; }

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
			Debug.Assert(world != null, "world cannot be null");
			Debug.Assert(world.vaildCoord(coord.x, coord.y), "inviald init coord");

			var origWorld = m_world;
			var origCoord = _coord;

			m_world = world;
			_coord = coord;

			this.onWorldChanged.Invoke(new ChangeWorldEventArgs
			{
				oldWorld = origWorld,
				oldCoord = origCoord,

				world = world,
				coord = coord
			});
		}

		// 获取附近的瓦片
		public TileCell getNearTile(vec2i offset, bool loop = false)
		{
			var coord = _coord + offset;
			if (loop)
			{
				coord.x = mathf.loopIndex(coord.x, m_world.width);
				coord.y = mathf.loopIndex(coord.y, m_world.height);
			}

			this.world.tryGetTile(coord.x, coord.y, out var tile);
			return tile;
		}

		public InWorld(WorldGrid world, vec2i coord, float cost = 0)
			: base("InWorld")
		{
			this.setWorld(world, coord);
			this.cost = cost;
		}
	}
}
