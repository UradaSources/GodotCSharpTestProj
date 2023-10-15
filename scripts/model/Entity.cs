namespace urd
{
	public class Entity
	{
		private WorldGrid m_world;
		private vec2i m_coord;

		public WorldGrid world { get => m_world; }
		public vec2i coord { get => m_coord; set => m_coord = value; }


		public Entity(WorldGrid world, vec2i coord)
		{
			this.m_world = world;
			this.m_coord = coord;
		}
	}
}
