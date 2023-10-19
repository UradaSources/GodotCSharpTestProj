using Godot;

namespace urd
{
	// rpg样式的控制器
	public class RpgControl : BasicMoveControl
	{
		public override void _update(float dt)
		{
			if (!this.motion.processing)
			{
				var dir = vec2i.zero;
				if (Input.IsActionPressed("ui_down"))
					dir = vec2i.up;
				else if (Input.IsActionPressed("ui_up"))
					dir = vec2i.down;
				else if (Input.IsActionPressed("ui_left"))
					dir = vec2i.left;
				else if (Input.IsActionPressed("ui_right"))
					dir = vec2i.right;
				this.motion.moveDirect = dir;
			}
		}

		public RpgControl(EntityMotion motion) :
			base(motion)
		{
		}
	}
}
