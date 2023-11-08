using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace urd
{
	public static class WorldGridUtils
	{
		public static IEnumerable<TileCell> IterateArea(WorldGrid grid, vec2i start, vec2i end)
		{
			start = vec2i.Max(start, vec2i.zero);
			end = vec2i.Min(end, grid.size - vec2i.one);

			Debug.Assert(start.x < end.x && start.y < end.y, 
				$"invaild start and end coord {start},{end}");

			for (int y = start.y; y <= end.y; y++)
			{
				for (int x = start.x; x <= end.x; x++)
					yield return grid.rawGetTile(grid.toIndex(x, y));
			}
		}
		public static int OverrideTileType(IEnumerable<TileType> types, IEnumerable<TileCell> target)
		{
			int count = 0;

			var itor = types.GetEnumerator();
			foreach (var cell in target)
			{
				if (itor.MoveNext())
				{
					cell.tile = itor.Current;
					count += 1;
				}
			}
			itor.Dispose();

			return count;
		}
	}
}