﻿using System.Diagnostics;
using Godot;

namespace urd
{
	public class EntityMotion: BasicComponent
	{
		private Entity m_entity;

		private vec2i m_targetCoord;
		private vec2 m_position;

		private float m_moveSpeed;
		private vec2i m_moveDirect;

		private bool m_moveProcessing;

		public Entity entity => m_entity;

		public vec2i targetCoord => m_targetCoord;
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

		public override void _init() { }
		public override void _update(float delta)
		{
			// 若当前正在移动中, 则更新位置
			if (m_moveProcessing)
			{
				var targetPos = new vec2(m_targetCoord.x, m_targetCoord.y);
				var posDelta = m_moveSpeed * delta;

				m_position = vec2.MoveTowards(m_position, targetPos, posDelta);

				// 到达指定位置重置processing标志
				if (m_position == targetPos)
				{
					GD.Print($"to target {targetPos}");

					this.entity.coord = m_targetCoord;
					m_moveProcessing = false;
				}
			}
			else // 若当前没有在移动
			{
				// 继续移动
				if (m_moveDirect != vec2i.zero)
				{
					vec2i targetCoord = m_entity.coord + m_moveDirect;
					
					var world = m_entity.world;
					if (world.tryGetTile(targetCoord.x, targetCoord.y, out var tile) 
						&& tile.type.cost >= 0)
					{
						m_position = new vec2(this.entity.coord.x, this.entity.coord.y);
						m_targetCoord = targetCoord;

						m_moveProcessing = true;
					}
					else
					{
						// 清空移动方向
						m_moveDirect = vec2i.zero;
					}
				}
			}
		}

		public EntityMotion(Entity entity, float moveSpeed, vec2i moveDirect)
		{
			m_entity = entity;
			m_moveSpeed = moveSpeed;
			m_moveDirect = moveDirect;
		}
	}
}
