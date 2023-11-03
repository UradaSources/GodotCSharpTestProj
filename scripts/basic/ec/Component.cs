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
}