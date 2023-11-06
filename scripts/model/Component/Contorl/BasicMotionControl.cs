namespace urd
{
	public abstract class BasicMotionControl : Component, IBehavior
	{
		[BindComponent] protected Movement m_motion;

		public bool actived { set; get; } = true;

		public virtual void _update(float delta) { }
		public virtual void _lateUpdate(float delta) { }

		protected BasicMotionControl(string name) : base(name)
		{
		}
	}
}
