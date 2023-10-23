using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace urd
{
	public class ComponentContainer
	{
		private LinkedList<Component> m_components;
		private Dictionary<System.Type, LinkedListNode<Component>> m_indexMap;

		public T getComponent<T>()
			where T : Component
		{
			if (m_indexMap.TryGetValue(typeof(T), out var it))
				return it.Value as T;
			else
				return null;
		}
		public Component getComponent(System.Type type)
		{
			Debug.Assert(type.IsInterface || type.IsSubclassOf(typeof(Component)),
				"invalid type, must be an interface or inherited from component");

			for (var it = m_components.First; it != null; it = it.Next)
			{
				if (type.IsInstanceOfType(it.Value))
					return it.Value;
			}
			return null;
		}

		public bool hasComponent<T>()
			where T : Component
		{
			return m_indexMap.ContainsKey(typeof(T));
		}

		public T emplaceComponent<T>()
			where T : Component, new()
		{
			Debug.Assert(!this.hasComponent<T>());

			var com = new T();
			return this.addComponent(com);
		}
		public T addComponent<T>(T com)
			where T : Component
		{
			var config = Component.GetTypeConfig<T>();
			if (config == null || config.beforeJoinContainer(com, this))
			{
				com._onAddToContainer(this, m_components.Count);

				var it = m_components.AddLast(com);
				m_indexMap.Add(typeof(T), it);
				return com;
			}
			else
			{
				Debug.WriteLine($"add compnent {typeof(T).Name} faild", $"{this.GetType().Name}.Warn");
				return null;
			}
		}

		public T removeComponent<T>()
			where T : Component
		{
			if (m_indexMap.TryGetValue(typeof(T), out var it))
			{
				var com = it.Value as T;
				m_components.Remove(it);

				return com;
			}
			return null;
		}

		public void _init()
		{
			for (var it = m_components.First; it != null; it = it.Next)
			{
				var com = it.Value;
				if (com.enable) com._init();
			}
		}
		public void _update(float delta)
		{
			for (var it = m_components.First; it != null; it = it.Next)
			{
				var com = it.Value;
				if (com.enable) com._update(delta);
			}
		}

		public ComponentContainer()
		{
			m_components = new LinkedList<Component>();
			m_indexMap = new Dictionary<System.Type, LinkedListNode<Component>>();
		}
	}
}