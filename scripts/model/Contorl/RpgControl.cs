namespace urd
{
	// rpg样式的控制器
	public class RpgControl : BasicMoveControl
	{
		public override void _update(float dt)
		{
			var inp = ServiceManager.Input;

			if (!this.motion.processing)
			{
				var dir = vec2i.zero;
				if (inp.getKey(KeyCode.S))
					dir = vec2i.up;
				else if (inp.getKey(KeyCode.W))
					dir = vec2i.down;
				else if (inp.getKey(KeyCode.A))
					dir = vec2i.left;
				else if (inp.getKey(KeyCode.D))
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
