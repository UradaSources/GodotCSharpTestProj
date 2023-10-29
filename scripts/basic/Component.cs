using System.Collections.Generic;
using System.Diagnostics;

namespace urd
{
	public abstract class Component
	{
		private ComponentContainer m_container;
		public ComponentContainer container => m_container;

		public virtual void _onAddToContainer(ComponentContainer container)
		{
			Debug.Assert(m_container == null);
			Debug.Assert(container != null);

			m_container = container;
		}
		public virtual void _onRemoveFromContainer()
		{
			Debug.Assert(m_container != null);
			m_container = null;
		}

		public virtual void _update(float delta) { }
	}
}