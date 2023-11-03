namespace urd
{
	public abstract class BehaviorComponent : Component
	{
		private bool m_process = true;
		public bool process { set => m_process = value; get => m_process; }

		public virtual void _update(float delta) { }
		public virtual void _lateUpdate(float delta) { }
	}
}