using System.Collections.Generic;
using System.Diagnostics;

namespace urd
{
	public abstract class Component
	{
		private static Dictionary<System.Type, ConfigComponent> _Config
			= new Dictionary<System.Type, ConfigComponent>();

		public static void BindTypeConfig<T>(ConfigComponent config)
			where T : Component
		{
			var key = typeof(T);

			if (config != null) _Config[key] = config;
			else _Config.Remove(key);
		}
		public static ConfigComponent GetTypeConfig<T>()
			where T : Component
		{
			return _Config.TryGetValue(typeof(T), out var config) ? config : null;
		}

		private ComponentContainer m_container;
		public ComponentContainer container => m_container;

		public virtual void _onAddToContainer(ComponentContainer container, int index)
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