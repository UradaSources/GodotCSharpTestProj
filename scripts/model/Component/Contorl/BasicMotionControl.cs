namespace urd
{
	public abstract class BasicMotionControl : Component, IComponentBehavior
	{
		[RequireComponent] protected Movement m_motion;

		public bool enable { set; get; } = true;

		public virtual void _update(float delta) { }
		public virtual void _lateUpdate(float delta) { }

		protected BasicMotionControl(string name) : base(name)
		{
		}
	}
}
