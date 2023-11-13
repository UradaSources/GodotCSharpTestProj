using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Linq;

namespace urd
{
	public class FollowWalkControl : BasicMotionControl
	{
		[BindComponent] private Navigation m_navigation = null;

		private vec2i m_targetLastCoord;

		public WorldEntity target { set; get; }

		public override void _update(float dt)
		{
			if (m_motion.processing) return;

			if (this.target != null && m_targetLastCoord != this.target.coord)
			{
				if (WorldSeek.InCircle(this.target.coord, m_navigation.worldEntity.coord, 3.0f))
				{
					m_navigation.setTarget(this.target.coord + vec2i.right);
					m_targetLastCoord = this.target.coord;
				}
				else
				{
					int x = mathf.random(-4, 4);
					int y = mathf.random(-4, 4);
					m_navigation.setTarget(this.target.coord + new vec2i(x, y));

					Debug.WriteLine("random target");
				}
			}
		}

		public FollowWalkControl() : base("FollowWalkControl") { }
	}
}