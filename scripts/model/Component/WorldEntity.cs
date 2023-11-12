using System.Diagnostics;

namespace urd
{
	[RecordObject]
	public class WorldEntity : Component
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

		private WorldGrid _world;
		public WorldGrid world => _world;

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

		private float _cost;
		public float cost { set => _cost = value; get => _cost; }

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

			var origWorld = this.world;
			var origCoord = this.coord;

			_world = world;
			_coord = coord;

			this.onWorldChanged?.Invoke(new ChangeWorldEventArgs
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
			var coord = this.coord + offset;
			if (loop)
			{
				coord.x = mathf.loopIndex(coord.x, this.world.width);
				coord.y = mathf.loopIndex(coord.y, this.world.height);
			}

			this.world.tryGetTile(coord.x, coord.y, out var tile);
			return tile;
		}

		public WorldEntity(WorldGrid world, vec2i coord, float cost = 0)
			: base("WorldEntity")
		{
			this.setWorld(world, coord);
			this.cost = cost;
		}
	}
}
