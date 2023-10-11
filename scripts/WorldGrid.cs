using System.Diagnostics;

namespace urd
{
	public class Tile
	{
		public int id;
		public int x, y;

		public bool pass = false;

		public Tile(int id, int x, int y)
		{
			this.id = id;
			this.x = x;
			this.y = y;
		}
	}

	public class WorldGrid
	{
		private int m_width;
		private int m_height;

		private Tile[] m_tileArray;

		public int width() { return m_width; }
		public int height() { return m_height; }

		public bool vaildCoord(int x, int y)
		{
			return x >= 0 && x < m_width && y >= 0 && y < m_height;
		}
		public int toIndex(int x, int y) { return y * m_width + x; }

		public Tile rawGetTile(int index)
		{
			return m_tileArray[index];
		}
		public Tile getTile(int x, int y)
		{
			Debug.Assert(vaildCoord(x, y));
			return m_tileArray[toIndex(x, y)];
		}
		public bool tryGetTile(int x, int y, out Tile tile)
		{
			if (vaildCoord(x, y))
			{
				tile = m_tileArray[toIndex(x, y)];
				return true;
			}
			tile = null;
			return false;
		}

		public WorldGrid(int w, int h)
		{
			m_width = w;
			m_height = h;

			m_tileArray = new Tile[w * h];

			int i = 0;
			for (int y = 0; y < m_height; y++)
			{
				for (int x = 0; x < m_width; x++)
				{
					Tile t = new Tile(i, x, y);
					t.pass = true;

					m_tileArray[i++] = t;
				}
			}
		}
	};
}