using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace urd
{
	public class Entity : Object, IEnumerable<Component>
	{
		private LinkedList<Component> m_components;
		private Dictionary<System.Type, LinkedListNode<Component>> m_index;

		private string m_name;
		private bool m_active = true;
		private ulong m_tags = 0;

		public string name { set => m_name = value; get => m_name; }
		public bool active { set => m_active = value; get =>  m_active; }
		public ulong tags { set => m_tags = value; get => m_tags; }

		private void componentInjection<T>(T com)
			where T : Component
		{
			var flag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			var fields = typeof(T).GetFields(flag);

			// 遍历字段并查找具有CustomAttribute的字段
			foreach (var field in fields)
			{
				var bindOptions = field.GetCustomAttribute<RequireComponentAttribute>();
				if (bindOptions != null)
				{
					if (!this.find(field.FieldType, out var dependent) && bindOptions.necessary)
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

		private void destroyComponent<T>(T com) 
			where T : Component
		{
			com._clear();

			m_components.Remove(it);
			com.destroy();
		}
		private void addComponent<T>(T com)
			where T : Component
		{
			// 使用反射绑定需要的组件
			this.componentInjection(com);
			this.bindEvent(com);

			var it = m_components.AddLast(com);
			m_index.Add(typeof(T), it);
		}

		public bool hasTag(ulong tag)
		{
			if (this.destroyed) throw new DestroyedObjectException();
			return (tags & tag) != 0;
		}

		public T get<T>() where T : Component
		{
			if (this.destroyed) throw new DestroyedObjectException();

			if (m_index.TryGetValue(typeof(T), out var it))
				return it.Value as T;
			
			return null;
		}

		public bool tryGet<T>(out T com) where T : Component
		{
			if (this.destroyed) throw new DestroyedObjectException();

			if (m_index.TryGetValue(typeof(T), out var it))
			{
				com = it.Value as T;
				return true;
			}

			com = default;
			return false;
		}
		public void tryGet<T1, T2>(out T1 com1, out T2 com2)
			where T1 : Component
			where T2 : Component
		{
			this.tryGet(out com1);
			this.tryGet(out com2);
		}
		public void tryGet<T1, T2, T3>(out T1 com1, out T2 com2, out T3 com3) 
			where T1 : Component
			where T2 : Component
			where T3 : Component
		{
			this.tryGet(out com1);
			this.tryGet(out com2);
			this.tryGet(out com3);
		}

		public bool find(System.Type type, out Component com)
		{
			if (this.destroyed) throw new DestroyedObjectException();

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
			if (this.destroyed) throw new DestroyedObjectException();
			return m_index.ContainsKey(typeof(T));
		}

		public T emplace<T>() where T : Component
		{
			if (this.destroyed) throw new DestroyedObjectException();

			if (this.has<T>())
			{
				Debug.Fail($"repeat adding component {typeof(T).Name}", $"{this.GetType().Name}.Fail");
				return null;
			}
			else
			{
				T com = new T(this);
				this.addComponent(com);

				return com;
			}
		}
		public bool remove<T>() where T : Component
		{
			if (this.destroyed) throw new DestroyedObjectException();

			if (m_index.TryGetValue(typeof(T), out var it))
			{
				var com = it.Value as T;
				com._clear();

				m_components.Remove(it);
				com.destroy();

				return true;
			}
			return false;
		}

		public void update(float delta)
		{
			if (this.destroyed) throw new DestroyedObjectException();

			for (var it = m_components.First; it != null; it = it.Next)
			{
				var com = it.Value;
				if (com is IComponentBehavior behavior && behavior.enable)
					behavior._update(delta);
			}
		}
		public void render()
		{
			for (var it = m_components.First; it != null; it = it.Next)
			{
				var com = it.Value;
				if (com is IRenderComponent renderer && renderer.rendering)
					renderer._draw();
			}
		}

		public IEnumerator<Component> GetEnumerator() { return m_components.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

		public override string ToString()
		{
			return $"Entity {this.name} ({m_components.Count})";
		}

		public override Object copy()
		{
			var en = new Entity(this.name + " (copy)");
			for (var it = m_components.First; it != null; it = it.Next)
				en.add();

			return en;
		}

		protected override void _onDestroy()
		{
			for (var it = m_components.First; it != null; it = it.Next)
				it.Value.destroy();
		}

		public Entity(string name) : base()
		{
			m_name = name;

			m_components = new LinkedList<Component>();
			m_index = new Dictionary<System.Type, LinkedListNode<Component>>();
		}
	}
}