using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace urd
{
	public class Navigation : Component
	{
		private PathGenerator m_pathfind;

		[BindComponent] private Entity m_entity = null;
		[BindComponent] private Movement m_motion = null;

		private vec2i? m_target;
		private List<TileCell> m_pathNodeList;

		public vec2i? target => m_target;
		public int pathNodeCount => m_pathNodeList.Count;

		public TileCell getPathNode(int index)
		{
			Debug.Assert(index >= 0 && index < m_pathNodeList.Count, $"invaild pathnode index: {index}");
			return m_pathNodeList[index];
		}

		public void setTarget(vec2i target)
		{
			if (m_target != target)
			{
				// 清除旧目标
				this.clearData();

				m_target = target;
				m_pathfind.generatePath(m_entity.coord, target, ref m_pathNodeList, StandardPathfindCost.Default);

				// 立即设置前进方向
				if (m_pathNodeList.Count > 0)
				{
					var nextTile = m_pathNodeList[m_pathNodeList.Count - 1];
					// 计算当前路径前往目标节点的方向
					var moveDirect = new vec2i(nextTile.x, nextTile.y) - m_entity.coord;
					m_motion.direct = moveDirect;
					m_pathNodeList.RemoveAt(m_pathNodeList.Count - 1);
				}
			}
		}
		public void clearData()
		{
			m_target = null;
			m_pathNodeList.Clear();
		}

		public override void _update(float delta)
		{
			// 等待当前运动完成
			if (m_motion.processing) return;

			// 设置下一个目的地
			if (m_pathNodeList.Count > 0)
			{
				var nextTile = m_pathNodeList[m_pathNodeList.Count - 1];
				if (nextTile.type.cost < 0) // 若下一个目标路点无效了, 则清除当前路径和移动方向
				{
					Debug.WriteLine($"next tile({nextTile.x},{nextTile.y}) is unreachable, clear path.", m_entity.name);

					this.clearData();
					m_motion.direct = vec2i.zero;

					// 重新计算路径
					// ...

					return;
				}

				// 计算当前路径前往目标节点的方向
				var moveDirect = new vec2i(nextTile.x, nextTile.y) - m_entity.coord;
				m_motion.direct = moveDirect;
				
				m_pathNodeList.RemoveAt(m_pathNodeList.Count - 1);
			}
			else
			{
				// 在路点全部完成后, 清除移动方向
				m_motion.direct = vec2i.zero;
			}
		}

		public Navigation(PathGenerator pathfind)
		{
			m_pathfind = pathfind;
			m_pathNodeList = new List<TileCell>();
		}
	}
}