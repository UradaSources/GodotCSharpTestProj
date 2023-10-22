using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace urd
{
	public class TileCell
	{
		public readonly int id;
		public readonly int x, y;

		public TileType type { set; get; }

		public TileCell(int id, int x, int y)
		{
			this.id = id;
			this.x = x;
			this.y = y;
		}
	}
}