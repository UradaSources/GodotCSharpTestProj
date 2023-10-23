namespace urd
{
	public abstract class BasicMotionControl : Component
	{
		private EntityMotion m_motion;

		protected EntityMotion motion => m_motion;

		public override void _init()
		{
			m_motion = this.container.getComponent<EntityMotion>();
		}
	}
}
