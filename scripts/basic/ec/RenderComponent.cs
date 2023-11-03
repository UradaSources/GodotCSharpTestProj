namespace urd
{
	public abstract class RenderComponent : Component
	{
		private bool m_process = true;
		public bool process { set => m_process = value; get => m_process; }

		public virtual void _draw() { }
	}
}