using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace urd
{
	public class Character: IComponent
	{
		private WorldGrid m_world;

		private vec2i m_coord;
		private vec2 m_position;

		private float m_moveSpeed;
		private vec2i m_moveDirect;

		private bool m_moveProcessing;

		public WorldGrid world => m_world;
		
		public vec2i coord => m_coord;
		public vec2 position => m_position;

		public float moveSpeed
		{ 
			set => m_moveSpeed = value;
			get => m_moveSpeed;
		}
		public vec2i moveDirect
		{
			set => m_moveDirect = value;
			get => m_moveDirect;
		}

		public bool moveProcessing => m_moveProcessing;

		public void _update(float delta)
		{
			// 若当前正在移动中, 则更新位置
			if (m_moveProcessing)
			{
				var targetPos = new vec2(m_coord.x, m_coord.y);
				var posDelta = m_moveSpeed * delta;

				m_position = vec2.MoveTowards(m_position, targetPos, posDelta);

				// 到达指定位置重置processing标志
				if (m_position == targetPos)
					m_moveProcessing = false;
			}
			else // 若当前没有在移动
			{
				// 继续移动
				if(m_moveDirect != vec2i.zero)
				{
					vec2i targetCoord = m_coord + m_moveDirect;
					if (m_world.tryGetTile(targetCoord.x, targetCoord.y, out var tile) && tile.pass)
					{
						m_coord = targetCoord;
						m_moveProcessing = true;
					}
					else
					{
						// 停止移动
						m_moveDirect = vec2i.zero;
					}
				}
			}
		}

		public Character(WorldGrid world, vec2i coord, float moveSpeed, vec2i moveDirect)
		{
			m_world = world;

			Debug.Assert(m_world.vaildCoord(coord.x, coord.y));
			m_coord = coord;
			m_position = new vec2(m_coord.x, m_coord.y);

			Debug.Assert(moveSpeed >= 0);
			m_moveSpeed = moveSpeed;
			m_moveDirect = moveDirect;
		}
	}
}
