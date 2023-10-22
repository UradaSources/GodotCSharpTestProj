using System.Collections;
using System.Collections.Generic;

namespace urd
{
	public class Entity
	{
		private WorldGrid m_world;
		private vec2i m_coord;

		private bool m_shouldBeDestroyed;
		private bool m_isDestroyed;

		public WorldGrid world { get => m_world; }
		public vec2i coord { get => m_coord; set => m_coord = value; }

		public TileCell tile
		{
			get
			{
				if (world.tryGetTile(m_coord.x, m_coord.y, out var tile))
					return tile;
				else
					return null;
			}
		}

		public Entity(WorldGrid world, vec2i coord)
		{
			this.m_world = world;
			this.m_coord = coord;
		}
	}
}
