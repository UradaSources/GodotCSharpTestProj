namespace urd
{
	public abstract class BasicMoveControl : BasicComponent
	{
		private EntityMotion m_motion;
		public EntityMotion motion => m_motion;

		public BasicMoveControl(EntityMotion move)
		{
			m_motion = move;
		}
	}
}
