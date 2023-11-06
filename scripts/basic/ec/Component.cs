using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace urd
{
	public interface IBehavior
	{
		public bool actived { get; }
		void _update(float delta);
		void _lateUpdate(float delta);
	}

	public interface IRender
	{
		bool rendering { get; }
		void _draw();
	}

	public abstract class Component : Object
	{
		private Entity m_entity = null;

		public Entity entity => m_entity;

		public virtual void _init(Entity entity)
		{
			Debug.Assert(m_entity == null && entity != null);
			m_entity = entity;
		}
		public virtual void _clear() 
		{
			Debug.Assert(m_entity != null);
			m_entity = null;
		}

		protected Component(string name) : base(name) { }
	}
}