using System.Collections;
using System.Collections.Generic;

namespace urd
{
	public class Character : BasicComponent
	{
		private Entity m_entity;
		private EntityMotion m_motion;

		private BasicMoveControl m_moveControl;

		public EntityMotion motion => m_motion;
		public Entity entity => m_entity;
		public BasicMoveControl moveControl { get => m_moveControl; set => m_moveControl = value; }

		public override void _update(float dt)
		{
			m_moveControl._update(dt);
			m_motion._update(dt);
		}

		public Character(WorldGrid world, vec2i initCoord, float speed, vec2i moveDirect)
		{
			m_entity = new Entity(world, initCoord);
			m_motion = new EntityMotion(m_entity, speed, moveDirect);
		}
	}
}
