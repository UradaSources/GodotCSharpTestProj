﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace urd
{
	public class StandardPathfindCost : IPathfindCost
	{
		public void init(WorldGrid world, vec2i start, vec2 end) { }

		// 用于计算h值的曼哈顿距离
		public static float ManhattanDistance(TileCell a, TileCell b)
			=> (new vec2(b.x, b.y) - new vec2(a.x, a.y)).magnitude() * mathf.random(0.8f, 1.2f);
			//=> mathf.abs(a.x - b.x) + mathf.abs(a.y - b.y);

		public readonly static StandardPathfindCost Default = new StandardPathfindCost();

		public float tileCost(TileCell cur, TileCell target)
		{
			float cost = target.tile.cost;

			if (cost >= 0)
			{
				foreach (var obj in Object.Get<WorldEntity>()
					.Where(t => t.currentTile == target))
				{
					if (obj.cost < 0) return -1;
					else return obj.cost + cost;
				}
			}
			return cost;
		}

		public float hValue(TileCell cur, TileCell target)
		{
			return StandardPathfindCost.ManhattanDistance(target, cur);
		}
	}
}