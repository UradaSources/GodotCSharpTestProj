using System.Collections;
using System.Collections.Generic;

namespace urd
{
	// 经典街机样式的控制器
	public class ClassicArcadeControl : BasicMotionControl
	{
		private vec2i m_cacheMoveDirect;

		public override void _update(float dt)
		{
			base._update(dt);

			if (Godot.Input.IsActionJustPressed("ui_down"))
				m_cacheMoveDirect = vec2i.up;
			else if (Godot.Input.IsActionJustPressed("ui_up"))
				m_cacheMoveDirect = vec2i.down;
			else if (Godot.Input.IsActionJustPressed("ui_left"))
				m_cacheMoveDirect = vec2i.left;
			else if (Godot.Input.IsActionJustPressed("ui_right"))
				m_cacheMoveDirect = vec2i.right;

			if (!m_motion.processing)
			{
				m_motion.direct = m_cacheMoveDirect;
			}
		}
	}
}
