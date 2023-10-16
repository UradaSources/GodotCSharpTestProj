using System.Collections;
using System.Collections.Generic;
using Godot;

namespace urd
{
	public class PlayerControlInput : BasicMoveControl
	{
		private vec2i m_cacheMoveDirect;

		public override void _update(float dt)
		{
			var inp = ServiceManager.Input;

			// 缓存移动方向的输入
			if (inp.getKeyDown(KeyCode.W))
				m_cacheMoveDirect = vec2i.up;
			else if (inp.getKeyDown(KeyCode.S))
				m_cacheMoveDirect = vec2i.down;
			else if (inp.getKeyDown(KeyCode.A))
				m_cacheMoveDirect = vec2i.left;
			else if (inp.getKeyDown(KeyCode.D))
				m_cacheMoveDirect = vec2i.right;

			if (!this.motion.moveProcessing)
			{
				GD.Print($"get inp, now dir is {m_cacheMoveDirect}");
				this.motion.moveDirect = m_cacheMoveDirect;
			}
		}

		public PlayerControlInput(EntityMotion motion) :
			base(motion)
		{
		}
	}
}
