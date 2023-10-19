using System.Collections;
using System.Collections.Generic;
using Godot;

namespace urd
{
	// 经典街机样式的控制器
	public class ClassicArcadeControl : BasicMoveControl
	{
		private vec2i m_cacheMoveDirect;

		public override void _update(float dt)
		{
			if (Input.IsActionJustPressed("ui_down"))
				m_cacheMoveDirect = vec2i.up;
			else if (Input.IsActionJustPressed("ui_up"))
				m_cacheMoveDirect = vec2i.down;
			else if (Input.IsActionJustPressed("ui_left"))
				m_cacheMoveDirect = vec2i.left;
			else if (Input.IsActionJustPressed("ui_right"))
				m_cacheMoveDirect = vec2i.right;

			if (!this.motion.processing)
			{
				GD.Print($"get inp, now dir is {m_cacheMoveDirect}");
				this.motion.moveDirect = m_cacheMoveDirect;
			}
		}

		public ClassicArcadeControl(EntityMotion motion) :
			base(motion)
		{
		}
	}
}
