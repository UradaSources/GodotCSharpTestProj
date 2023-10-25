using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

namespace urd
{
	public class FollowWalkControl : BasicMotionControl
	{
		[BindComponent] private Entity m_entity;
		[BindComponent] private Navigation m_moveToward;

		public Entity target { set; get; }

		private Godot.RandomNumberGenerator m_rng;
		private float m_timer = 0;

		public override void _update(float dt)
		{
			if (m_motion.processing) return;
			if (this.target != null)
			{
				if (m_timer > 0)
				{
					m_timer -= dt;
					return;
				}

				m_moveToward.setTarget(this.target.coord);
				
				m_timer = m_rng.RandfRange(0.5f, 1.0f);
			}
		}

		public FollowWalkControl()
		{
			m_rng = new Godot.RandomNumberGenerator();
		}
	}
}