using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace urd
{
	public class ComponentContainer
	{
		public static void BindComponent<T>(T com)
			where T : Component
		{
			Debug.Assert(com.container != null);

			var flag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			var fields = typeof(T).GetFields(flag);

			// 遍历字段并查找具有CustomAttribute的字段
			foreach (var field in fields)
			{
				var bindOptions = field.GetCustomAttribute<BindComponentAttribute>();
				if (bindOptions != null)
				{
					var dependent = com.container.findComponent(field.FieldType);
					Debug.Assert(dependent != null || !bindOptions.require,
						$"Unable to add component ({typeof(T).Name}) to the container, its dependent component ({field.FieldType.Name}) does not exist");

					if (dependent != null) field.SetValue(com, dependent);
				}
			}
		}
		public static void BindEvent<T>(T com)
			where T : Component
		{
			var methodFilter = BindingFlags.Public | BindingFlags.NonPublic;
			foreach (var method in typeof(T).GetMethods(methodFilter))
			{
				var bindOptions = method.GetCustomAttribute<BindEventAttribute>();
				if (bindOptions != null)
				{
					var handler = System.Delegate.CreateDelegate(typeof(T), method);
					bindOptions.eventInfo.AddEventHandler(bindOptions.target, handler);
				}
			}
		}

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

		public Component findComponent(System.Type type)
		{
			Debug.Assert(type.IsInterface || type.IsSubclassOf(typeof(Component)),
				"invalid type, must be an interface or inherited from component");

			if (m_indexMap.TryGetValue(type, out var com))
				return com.Value;

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

		public T emplaceComponent<T>(bool autoBind = true)
			where T : Component, new()
		{
			Debug.Assert(!this.hasComponent<T>());

			var com = new T();
			return this.addComponent(com, autoBind);
		}
		public void tryEmplaceComponents(bool autoBind, params System.Type[] types)
		{
			for (int i = 0; i < types.Length; i++)
			{
				if (m_indexMap.ContainsKey(types[i])) continue;

				Debug.Assert(types[i].IsSubclassOf(typeof(Component)));
				var com = (Component)System.Activator.CreateInstance(types[i]);

				this.addComponent(com, autoBind);
			}
		}

		public T addComponent<T>(T com, bool autoBind = true)
			where T : Component
		{
			Debug.Assert(!this.hasComponent<T>());

			// 使用反射绑定需要的组件
			if (autoBind)
			{
				var flag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
				var fields = typeof(T).GetFields(flag);

				// 遍历字段并查找具有CustomAttribute的字段
				foreach (var field in fields)
				{
					var bindOptions = field.GetCustomAttribute<BindComponentAttribute>();
					if (bindOptions != null)
					{
						var dependent = this.findComponent(field.FieldType);
						Debug.Assert(dependent != null || !bindOptions.require,
							$"Unable to add component ({typeof(T).Name}) to the container, its dependent component ({field.FieldType.Name}) does not exist");

						if (dependent != null) field.SetValue(com, dependent);
					}
				}
			}

			var it = m_components.AddLast(com);
			m_indexMap.Add(typeof(T), it);

			com._enable(this);

			return com;
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

		public IEnumerable<Component> iterateComponents()
		{
			for (var it = m_components.First; it != null; it = it.Next)
				yield return it.Value;
		}

		public virtual void _init() { }
		public virtual void _update(float delta)
		{
			for (var it = m_components.First; it != null; it = it.Next)
				it.Value._update(delta);
		}

		public ComponentContainer()
		{
			m_components = new LinkedList<Component>();
			m_indexMap = new Dictionary<System.Type, LinkedListNode<Component>>();
		}
	}
}