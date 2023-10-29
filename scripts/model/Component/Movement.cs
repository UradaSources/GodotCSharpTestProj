using System.Diagnostics;

namespace urd
{
	public class Movement: Component
	{
		[BindComponent] private Entity m_entity = null;

		private vec2i m_currentDirect;

		private float m_moveSpeed;
		private vec2i m_direct;

		private float m_progress;

		public vec2i currentDirect => m_currentDirect;

		public float speed { set => m_moveSpeed = value; get => m_moveSpeed; }
		public vec2i direct { set => m_direct = value; get => m_direct; }

		public float progress => m_progress;
		public bool processing => m_progress >= 0;

		public override void _update(float delta)
		{
			DebugWatch.Main.watchObject(this, m_entity.name);

			// 若当前正在移动中, 则更新位置
			if (this.processing)
			{
				var targetTile = m_entity.getNearTile(this.currentDirect, loop: true);
				var targetCost = targetTile.type.cost;
				
				// 若块在移动过程中突然无法通过, 回退到原先的块
				if (targetCost < 0)
				{
					Debug.WriteLine($"break in tile({m_currentDirect.x},{m_currentDirect.y})");

					m_progress = -1;
					return;
				}

				var progressDelta = m_moveSpeed * delta / targetCost;
				m_progress = mathf.moveTowards(m_progress, 1, progressDelta);

				// 到达指定位置后重置标志
				if (m_progress == 1)
				{
					m_entity.coord = new vec2i(targetTile.x, targetTile.y);
					m_currentDirect = vec2i.zero;

					m_progress = -1;
				}
			}
			// 若当前没有在移动, 则按照既定方向尝试进行移动
			else if(m_direct != vec2i.zero)
			{
				// 尝试检查目标方向的下一个块是否可达
				var nextTile = m_entity.getNearTile(m_direct, loop: true);
				if (nextTile != null && nextTile.type.cost >= 0)
				{
					m_currentDirect = m_direct;
					m_progress = 0;
				}
			}
		}

		public Movement(float moveSpeed, vec2i moveDirect)
		{
			m_moveSpeed = moveSpeed;
			m_direct = moveDirect;
		}
	}
}
