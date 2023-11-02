using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace urd
{
	public static class WorldGridUtils
	{
		[System.Serializable]
		private struct WorldSerializeData
		{
			[JsonInclude] public int width, height;
			[JsonInclude] public int[] tile;
		}

		public static string ToJson(WorldGrid world)
		{
			var data = new WorldSerializeData();
			data.width = world.width;
			data.height = world.height;

			data.tile = new int[data.width * data.height];
			for (int i = 0; i < world.tileCount; i++)
				data.tile[i] = world.rawGetTile(i).type.id;

			return JsonSerializer.Serialize(data);
		}
		public static WorldGrid TryFromJson(string json, WorldGrid target = null)
		{
			var data = JsonSerializer.Deserialize<WorldSerializeData>(json);
			if (data.width == 0 || data.height == 0)
			{
				Debug.WriteLine($"serialized data corruption",
					$"{typeof(WorldGridUtils).Name}.Error");

				return null;
			}

			if (target != null && (target.width != data.width || target.height != data.height))
			{
				Debug.WriteLine($"serialized data provides {data.width}x{data.height} data, while the target is {target.width}x{target.height}",
					$"{typeof(WorldGridUtils).Name}.Error");

				return null;
			}

			var world = target ?? new WorldGrid(data.width, data.height, TileType.Default);

			for (int i = 0; i < world.tileCount; i++)
			{
				int typeId = data.tile[i];
				world.rawGetTile(i).type = TileType.Get(typeId);
			}

			return world;
		}

		public static IEnumerable<TileCell> IterateTileCells(WorldGrid grid, vec2i start, vec2i end)
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
					cell.type = itor.Current;
					count += 1;
				}
			}
			itor.Dispose();

			return count;
		}
	}
}