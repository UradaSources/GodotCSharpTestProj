﻿namespace urd
{
	public class TileCell
	{
		public readonly int id;
		public readonly int x, y;

		public vec2i coord => new vec2i(x, y);

		public TileType tile { set; get; }

		public TileCell(int id, int x, int y)
		{
			this.id = id;
			this.x = x;
			this.y = y;
		}
	}
}