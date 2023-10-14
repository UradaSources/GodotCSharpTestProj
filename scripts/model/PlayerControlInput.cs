using System.Collections;
using System.Collections.Generic;
using Godot;

namespace urd
{
	public class PlayerControlInput : BasicMoveControl
	{
		private vec2i m_cacheMoveDirect;

		public override void _init() { }
		public override void _update(float dt)
		{
			// 缓存移动方向的输入
			if (Input.IsActionPressed("ui_up"))
				m_cacheMoveDirect = vec2i.up;
			else if (Input.IsActionPressed("ui_down"))
				m_cacheMoveDirect = vec2i.down;
			else if (Input.IsActionPressed("ui_left"))
				m_cacheMoveDirect = vec2i.left;
			else if (Input.IsActionPressed("ui_right"))
				m_cacheMoveDirect = vec2i.right;

			// 尝试切换移动方向到目标移动方向
			vec2i targetCoord = this.motion.entity.coord + m_cacheMoveDirect;

			var world = this.motion.entity.world;
			if (world.tryGetTile(targetCoord.x, targetCoord.y, out var tile) && tile.pass)
			{
				this.motion.moveDirect = m_cacheMoveDirect;
			}
		}

		public PlayerControlInput(EntityMotion motion) :
			base(motion)
		{
		}
	}
}
