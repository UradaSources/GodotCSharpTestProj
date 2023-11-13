using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace urd
{
	public class WorldSeek
	{
		public readonly static vec2i[] NearOffset = new vec2i[] { 
			vec2i.up, vec2i.down, vec2i.left, vec2i.right };

		public static bool InCircle(vec2i target, vec2i start, float radius)
		{
			return (target - start).magnitude() - 1.0f <= radius;
		}

		private readonly WorldGrid m_world;

		private HashSet<TileCell> m_close = new HashSet<TileCell>();
		private Queue<TileCell> m_open = new Queue<TileCell>();

		public IEnumerable<TileCell> bfs(vec2i start, System.Func<TileCell, bool> condition)
		{
			m_open.Clear();
			m_close.Clear();

			if (m_world.tryGetTile(start.x, start.y, out var tile))
				m_open.Enqueue(tile);
			else
				yield break;

			do
			{
				var cur = m_open.Dequeue();
				m_close.Add(cur);

				yield return cur;

				foreach (var coordOffset in NearOffset)
				{
					var nearCoord = cur.coord + coordOffset;
					if (m_world.tryGetTile(nearCoord.x, nearCoord.y, out var near))
					{
						if (!m_close.Contains(near) && condition.Invoke(near))
							m_open.Enqueue(near);
					}
				}
			} while (m_open.Count != 0);
		}
		public IEnumerable<TileCell> bfsCircle(vec2i start, float radius)
		{
			float sqrtRadius = radius * radius;
			var condition = (TileCell cell) => (cell.coord - start).sqrMagnitude() - 1.0f <= sqrtRadius;
			foreach (var i in bfs(start, condition))
				yield return i;
		}
		public IEnumerable<TileCell> bfsRectangle(vec2i start, vec2i size, vec2i offset)
		{
			var center = start + offset;
			var halfSize = size / 2;

			var max = center + halfSize;
			var min = center - halfSize;

			var condition = (TileCell cell)=> cell.coord <= max && cell.coord >= min;
			foreach (var i in bfs(start, condition))
				yield return i;
		}

		public WorldSeek(WorldGrid world)
		{ 
			m_world = world;
		}
	}
}