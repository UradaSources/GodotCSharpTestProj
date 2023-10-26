using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

namespace urd
{
	public class FollowWalkControl : BasicMotionControl
	{
		[BindComponent] private Navigation m_navigation = null;

		private vec2i m_targetLastCoord;

		public Entity target { set; get; }

		public override void _update(float dt)
		{
			if (m_motion.processing) return;

			if (this.target != null && m_targetLastCoord != this.target.coord)
			{
				Debug.WriteLine($"entity is stop ({this.container.getComponent<Entity>().coord}) and update target coord to {m_targetLastCoord}");

				m_navigation.setTarget(this.target.coord + vec2i.right);
				m_targetLastCoord = this.target.coord;
			}
		}
	}
}