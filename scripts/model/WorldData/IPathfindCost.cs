namespace urd
{
	// 路径代价计算器
	// 用于在寻路过程中计算gh值
	public interface IPathfindCost
	{
		void init(WorldGrid world, vec2i start, vec2 end);

		// 计算瓦片的通行代价
		// 返回值应该是1为基准的标准值
		float tileCost(TileCell cur, TileCell target);

		// 计算h值
		float hValue(TileCell cur, TileCell target);
	}

}