using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace urd
{
	[RecordObject]
	public class Entity : Object, IEnumerable<Component>
	{
		private LinkedList<Component> m_components;
		private Dictionary<System.Type, LinkedListNode<Component>> m_index;

		private bool m_active = true;
		private ulong m_tags = 0;

		public bool active { set => m_active = value; get => m_active; }
		public ulong tags { set => m_tags = value; get => m_tags; }

		private void componentInjection<T>(T com)
			where T : Component
		{
			var flag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			var fields = typeof(T).GetFields(flag);

			// 遍历字段并查找具有CustomAttribute的字段
			foreach (var field in fields)
			{
				var bindOptions = field.GetCustomAttribute<BindComponentAttribute>();
				if (bindOptions != null)
				{
					if (!this.find(field.FieldType, out var dependent) && bindOptions.require)
					{
						Debug.Fail($"Unable to add component ({typeof(T).Name}) to the entity, " +
							$"its dependent component ({field.FieldType.Name}) does not exist");
					}

					if (dependent != null) field.SetValue(com, dependent);
				}
			}
		}
		private void bindEvent<T>(T com)
		{
			var type=  typeof(T);

			var flag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			var members = type.GetMembers(flag);

			// 遍历字段并查找具有CustomAttribute的字段
			foreach (var member in members)
			{
				var bindOptions = member.GetCustomAttribute<BindEventAttribute>();
				if (bindOptions != null)
				{
					var handler = System.Delegate.CreateDelegate(
						bindOptions.eventInfo.EventHandlerType, 
						com,
						member as MethodInfo,
						true);
					bindOptions.eventInfo.AddEventHandler(com, handler);
				}
			}
		}

		public T get<T>() where T : Component
		{
			if (m_index.TryGetValue(typeof(T), out var it))
				return it.Value as T;
			else
				return null;
		}
		public bool find(System.Type type, out Component com)
		{
			LinkedListNode<Component> itor;
			if (m_index.TryGetValue(type, out itor))
			{
				com = itor.Value;
				return true;
			}

			for (itor = m_components.First; itor != null; itor = itor.Next)
			{
				if (type.IsInstanceOfType(itor.Value))
				{
					com = itor.Value;
					return true;
				}
			}

			com = null;
			return false;
		}

		public bool has<T>() where T : Component
		{
			return m_index.ContainsKey(typeof(T));
		}

		public T add<T>(T com, bool autoBind = true) where T : Component
		{
			if (this.has<T>())
			{ 
				Debug.Fail($"repeat adding component {typeof(T).Name}", $"{this.GetType().Name}.Fail");
				return null;
			}

			// 使用反射绑定需要的组件
			if (autoBind)
			{
				this.componentInjection(com);
				this.bindEvent(com);
			}

			var it = m_components.AddLast(com);
			m_index.Add(typeof(T), it);
			
			com._init(this);

			return com;
		}
		public T remove<T>() where T : Component
		{
			if (m_index.TryGetValue(typeof(T), out var it))
			{
				var com = it.Value as T;
				com._clear();

				m_components.Remove(it);

				return com;
			}
			return null;
		}

		public void update(float delta)
		{
			for (var it = m_components.First; it != null; it = it.Next)
			{
				var com = it.Value;
				if (com is IComponentBehavior behavior && behavior.enable)
					behavior._update(delta);
			}
		}
		public void lateUpdate(float delta)
		{
			for (var it = m_components.First; it != null; it = it.Next)
			{
				var com = it.Value;
				if (com is IComponentBehavior behavior && behavior.enable)
					behavior._lateUpdate(delta);
			}
		}

		//public void render()
		//{
		//	for (var it = m_components.First; it != null; it = it.Next)
		//	{
		//		var com = it.Value;
		//		if (com is IRenderComponent renderer && renderer.rendering)
		//			renderer._draw();
		//	}
		//}

		public IEnumerator<Component> GetEnumerator() { return m_components.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

		public Entity(string name) : base(name)
		{
			m_components = new LinkedList<Component>();
			m_index = new Dictionary<System.Type, LinkedListNode<Component>>();
		}

		public override string ToString()
		{
			return $"Entity {this.name} ({m_components.Count})";
		}
	}
}