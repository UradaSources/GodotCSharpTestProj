namespace urd
{
	public abstract class BasicMotionControl : Component
	{
		private Movement m_motion;

		protected Movement motion => m_motion;

		public override void _init()
		{
			m_motion = this.container.getComponent<Movement>();
		}
	}
}
