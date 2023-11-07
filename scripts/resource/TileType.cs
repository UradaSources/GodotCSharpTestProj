using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace urd
{
	public class TileType : Object
	{
		public enum BuiltinTags
		{ 
			Ground = 1 >> 0,
			Wall = 1 >> 1,
			Floor = 1 >> 2,
			Water = 1 >> 3,
		}

		public readonly Sprite sprite;
		
		public readonly float cost;
		public readonly ulong tags;

		public TileType(string name, Sprite sprite, float cost = 1, ulong tags = (ulong)BuiltinTags.Ground) 
			: base(name)
		{
			this.sprite = sprite;

			this.cost = cost;
			this.tags = tags;
		}
	}
}


