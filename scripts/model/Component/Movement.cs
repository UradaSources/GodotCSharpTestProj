using System.Diagnostics;
using Godot;

namespace urd
{
	public class Movement: Component
	{
		private Entity m_entity;

		private vec2i m_targetCoord;

		private float m_moveSpeed;
		private vec2i m_moveDirect;

		private float m_progress;

		public vec2i targetCoord => m_targetCoord;

		public float moveSpeed { set => m_moveSpeed = value; get => m_moveSpeed; }
		public vec2i moveDirect { set => m_moveDirect = value; get => m_moveDirect; }

		public float progress => m_progress;
		public bool processing => m_progress >= 0;

		public override void _onAddToContainer(ComponentContainer container, int index)
		{
			base._onAddToContainer(container, index);

			m_entity = this.container.getComponent<Entity>();
			Debug.Assert(m_entity != null);
		}
		public override void _update(float delta)
		{
			// 若当前正在移动中, 则更新位置
			if (this.processing)
			{
				var targetCost = m_entity.world.getTile(m_targetCoord.x, m_targetCoord.y).type.cost;
				
				// 若块在移动过程中突然无法通过, 回退到原先的块
				if (targetCost < 0)
				{
					m_progress = -1;
					return;
				}

				var progressDelta = m_moveSpeed * delta / targetCost;
				m_progress = Mathf.MoveTowards(m_progress, 1, progressDelta);

				// 到达指定位置后重置标志
				if (m_progress == 1)
				{
					m_entity.coord = m_targetCoord;
					m_progress = -1;
				}
			}
			else // 若当前没有在移动
			{
				// 继续移动
				if (m_moveDirect != vec2i.zero)
				{
					vec2i targetCoord = m_entity.coord + m_moveDirect;

					// 计算循环的坐标
					targetCoord.x = Mathf.LoopIndex(targetCoord.x, m_entity.world.width);
					targetCoord.y = Mathf.LoopIndex(targetCoord.y, m_entity.world.height);

					var world = m_entity.world;
					if (world.tryGetTile(targetCoord.x, targetCoord.y, out var tile) 
						&& tile.type.cost >= 0)
					{
						m_targetCoord = targetCoord;
						m_progress = 0;
					}
					else
					{
						// 清空移动方向
						m_moveDirect = vec2i.zero;
					}
				}
			}
		}

		public Movement(float moveSpeed, vec2i moveDirect)
		{
			m_moveSpeed = moveSpeed;
			m_moveDirect = moveDirect;
		}
	}
}
