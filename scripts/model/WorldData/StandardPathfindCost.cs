using System.Linq;

namespace urd
{
	public class StandardPathfindCost : IPathfindCost
	{
		public void init(WorldGrid world, vec2i start, vec2 end) { }

		// 用于计算h值的曼哈顿距离
		public static float ManhattanDistance(TileCell a, TileCell b)
			=> mathf.abs(a.x - b.x) + mathf.abs(a.y - b.y);

		public readonly static StandardPathfindCost Default = new StandardPathfindCost();

		public float tileCost(TileCell cur, TileCell target)
		{
			float cost = target.type.cost;

			if (cost >= 0)
			{
				foreach (var en in Entity.IterateInstance()
					.Where((Entity e) => e.currentTile == target))
				{
					if (en.block) 
						return -1;
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