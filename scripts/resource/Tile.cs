using System.Collections.Generic;
using System.Diagnostics;

namespace urd
{
	public class Tile : Object
	{
		public readonly ulong tags;

		public readonly float cost;
		public readonly GraphSprite sprite;

		public Tile(string name, ulong tags, GraphSprite sprite, float cost) 
			: base(name)
		{
			this.tags = tags;

			this.cost = cost;
			this.sprite = sprite;
		}
	}
}


