using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Linq;

namespace urd
{
	public class FollowWalkControl : BasicMotionControl
	{
		[BindComponent] private Navigation m_navigation = null;

		private WorldSeek m_seek;
		private vec2i m_targetLastCoord;

		public WorldEntity target { set; get; }

		public override void _update(float dt)
		{
			if (m_motion.processing) return;

			if (this.target != null && m_targetLastCoord != this.target.coord)
			{
				//var cells = m_seek.bfsCircle(m_navigation.worldEntity.coord, m_seekRange);
				//var en = WorldSeek.GetWorldEntity(cells).FirstOrDefault(en => en.name == "player");
				//if (en != null)
				{
					m_navigation.setTarget(this.target.coord + vec2i.right);
					m_targetLastCoord = this.target.coord;
				}
			}
		}

		public FollowWalkControl() : base("FollowWalkControl") { }
	}
}