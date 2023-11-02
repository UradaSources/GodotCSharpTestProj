using System.Collections.Generic;
using System.Diagnostics;

namespace urd
{
	public abstract class Component
	{
		private ComponentContainer m_container;
		public ComponentContainer container => m_container;

		public virtual void _enable(ComponentContainer container)
		{
			Debug.Assert(m_container == null && container != null);
			m_container = container;
		}
		public virtual void _disable()
		{
			Debug.Assert(m_container != null);
			m_container = null;
		}
	}

	public abstract class BehaviorComponent : Component
	{
		private bool m_process = true;
		public bool process { set => m_process = value; get => m_process; }

		public abstract void _update(float delta);
		public abstract void _lateUpdate(float delta);
	}
	public abstract class RenderComponent : Component
	{
		private bool m_process = true;
		public bool process { set => m_process = value; get => m_process; }

		public abstract void _draw();
	}
}