namespace urd
{
	// rpg样式的控制器
	public class RpgControl : BasicMotionControl
	{
		public override void _update(float dt)
		{
			base._update(dt);

			if (!m_motion.processing)
			{
				var dir = vec2i.zero;
				if (Godot.Input.IsActionPressed("ui_down"))
					dir = vec2i.up;
				else if (Godot.Input.IsActionPressed("ui_up"))
					dir = vec2i.down;
				else if (Godot.Input.IsActionPressed("ui_left"))
					dir = vec2i.left;
				else if (Godot.Input.IsActionPressed("ui_right"))
					dir = vec2i.right;

				m_motion.direct = dir;
			}
		}

		public RpgControl() : base("RpgControl") { }
	}
}
