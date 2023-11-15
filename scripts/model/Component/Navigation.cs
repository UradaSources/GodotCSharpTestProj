using System.Collections.Generic;
using System.Diagnostics;

namespace urd
{
	public class Navigation : Component, IComponentBehavior
	{
		public delegate void StartEventHandler(vec2i target);
		public delegate void InterruptEventHandler(vec2i target);
		public delegate void CompletionEventHandler(vec2i target);

		public event StartEventHandler onStart;
		public event CompletionEventHandler onCompletion;
		public event InterruptEventHandler onInterrupt;

		private WorldPath m_pathfind;

		[RequireComponent] private WorldEntity m_worldEntity = null;
		[RequireComponent] private Movement m_movement = null;

		private vec2i? m_target;
		private List<TileCell> m_pathNodeList;

		public WorldEntity worldEntity => m_worldEntity;
		public Movement movement => m_movement;

		public bool enable { set; get; } = true;

		public vec2i? target => m_target;
		public int pathNodeCount => m_pathNodeList.Count;

		public TileCell getPathNode(int index)
		{
			Debug.Assert(index >= 0 && index < m_pathNodeList.Count, 
				$"invaild index: {index}");
			
			return m_pathNodeList[index];
		}

		public void setTarget(vec2i target)
		{
			if (m_target != target)
			{
				// 清除旧目标
				this.clear();

				m_target = target;
				m_pathfind.generatePath(m_worldEntity.coord, target, ref m_pathNodeList, StandardPathfindCost.Default);

				// 立即设置前进方向
				if (m_pathNodeList.Count > 0)
				{
					// 计算当前路径前往目标节点的方向
					var nextTile = m_pathNodeList[m_pathNodeList.Count - 1];
					var moveDirect = new vec2i(nextTile.x, nextTile.y) - m_worldEntity.coord;
					
					m_movement.direct = moveDirect;
					m_pathNodeList.RemoveAt(m_pathNodeList.Count - 1);
					
					this.onStart?.Invoke(m_target.Value);
				}
			}
		}
		public void clear()
		{
			m_target = null;
			m_pathNodeList.Clear();
		}

		public void _update(float delta)
		{
			// 等待当前运动完成
			if (m_movement.processing) return;

			// 设置下一个目的地
			if (m_pathNodeList.Count > 0)
			{
				var nextTile = m_pathNodeList[m_pathNodeList.Count - 1];
				if (nextTile.tile.cost < 0) // 若下一个目标路点无效了, 则清除当前路径和移动方向
				{
					var _target = m_target;

					this.clear();
					m_movement.direct = vec2i.zero;

					this.onStart?.Invoke(_target.Value);

					return;
				}

				// 计算当前路径前往目标节点的方向
				var moveDirect = new vec2i(nextTile.x, nextTile.y) - m_worldEntity.coord;
				m_movement.direct = moveDirect;
				
				m_pathNodeList.RemoveAt(m_pathNodeList.Count - 1);
			}
			else if(m_target.HasValue) // 在路点全部执行完成后进行清理
			{
				// 在路点全部完成后, 清除移动方向
				var _target = m_target;

				m_target = null;
				m_movement.direct = vec2i.zero;

				this.onCompletion?.Invoke(_target.Value);
			}
		}
		public void _lateUpdate(float delta) { }

		public Navigation(WorldPath pathfind)
			: base("Navigation")
		{
			m_pathfind = pathfind;
			m_pathNodeList = new List<TileCell>();
		}
	}
}