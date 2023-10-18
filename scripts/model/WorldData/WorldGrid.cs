using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace urd
{
	public class WorldGrid
	{
		private int m_width;
		private int m_height;

		private TileCell[] m_tileArray;

		public int width => m_width;
		public int height => m_height;

		public int tileCount => m_width * m_height;

		public bool vaildCoord(int x, int y)
		{
			return x >= 0 && x < m_width && y >= 0 && y < m_height;
		}
		public int toIndex(int x, int y) { return y * m_width + x; }

		public TileCell rawGetTile(int index)
		{
			return m_tileArray[index];
		}
		public TileCell getTile(int x, int y)
		{
			Debug.Assert(vaildCoord(x, y));
			return m_tileArray[toIndex(x, y)];
		}
		public bool tryGetTile(int x, int y, out TileCell tile)
		{
			if (vaildCoord(x, y))
			{
				tile = m_tileArray[toIndex(x, y)];
				return true;
			}
			tile = null;
			return false;
		}

		public WorldGrid(int w, int h, TileType fill)
		{
			Debug.Assert(w > 0 && h > 0);
			Debug.Assert(fill != null);

			m_width = w;
			m_height = h;

			m_tileArray = new TileCell[w * h];

			int i = 0;
			for (int y = 0; y < m_height; y++)
			{
				for (int x = 0; x < m_width; x++)
				{
					TileCell t = new TileCell(i, x, y);
					t.type = fill;

					m_tileArray[i++] = t;
				}
			}
		}
	};
}