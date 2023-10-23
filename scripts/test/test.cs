using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public interface ComponentConfig
{
	bool beforeJoinContainer(Component com, ComponentContainer container);
}

public class RequiredComponentConfig : ComponentConfig
{
	private System.Type[] m_require;

	public bool beforeJoinContainer(Component com, ComponentContainer container)
	{
		bool missRequired = false;
		for (int i = 0; i < m_require.Length; i++)
		{
			var type = m_require[i];
			if (container.getComponent(type) == null)
			{
				Debug.WriteLine($"required prefix type was not found: {type.Name}");
				missRequired = true;
			}
		}
		return !missRequired;
	}

	public RequiredComponentConfig(params System.Type[] require) 
	{
#if DEBUG
		for (int i = 0; i < m_require.Length; i++)
		{
			Debug.Assert(m_require[i].IsInterface || m_require[i].IsSubclassOf(typeof(Component)),
				"invalid type, must be an interface or inherited from component");
		}
#endif
		m_require = require;
	}
}

public abstract class Component
{
	private static Dictionary<System.Type, ComponentConfig> _Config 
		= new Dictionary<System.Type, ComponentConfig>();

	public static void BindConfig<T>(ComponentConfig config)
		where T : Component
	{
		var key = typeof(T);

		if (config != null) _Config[key] = config;
		else _Config.Remove(key);
	}
	public static ComponentConfig GetConfig<T>()
		where T : Component
	{
		return _Config.TryGetValue(typeof(T), out var config) ? config : null;
	}

	private ComponentContainer m_container;
	private int m_index;

	public ComponentContainer container => m_container;
	public int index => m_index;

	public virtual void _onAddToContainer(ComponentContainer container, int index)
	{
		Debug.Assert(m_container == null, $"component {this} is currently in the container {m_container}");
		Debug.Assert(container != null, $"");

		m_container = container;
		m_index = index;
	}
	public virtual void _onRemoveFromContainer()
	{
		Debug.Assert(m_container != null, "");

		m_container = null;
		m_index = -1;
	}

	public virtual void _onEnable() { }
	public virtual void _onDisable() { }

	public virtual void _update(float delta) { }
	public virtual void _lateUpdate(float delta) { }
}

public class ComponentContainer
{
	public enum EnableState
	{ 
		Disabled		= 0x0010,
		Enabled			= 0x0001,

		WaitForDisable	= 0x0101,
		WaitForEnable	= 0x1001,
	}

	private static Queue<ComponentContainer> _WaitForEnable = new Queue<ComponentContainer>();
	private static Queue<ComponentContainer> _WaitForDisable = new Queue<ComponentContainer>();
	
	private static LinkedList<ComponentContainer> _Enabled = new LinkedList<ComponentContainer>();

	public static void Enable(ComponentContainer container)
	{
		Debug.Assert(container.state == EnableState.Enabled);
		
		_WaitForEnable.Enqueue(container);
		container._enabledHandle = _Enabled.AddLast(container);
	}
	public static void Disable(ComponentContainer container)
	{
		Debug.Assert((container.state | EnableState.Enabled) == EnableState.Enabled);

		_WaitForDisable.Enqueue(container);

		_Enabled.Remove(container._enabledHandle);
		container._enabledHandle = null;
		container.m_state = EnableState.WaitForDisable;
	}

	public static void UpdateEnabled(float delta)
	{
		while (_WaitForDisable.Count > 0)
		{
			var c = _WaitForDisable.Dequeue();

			var it = c.m_components.First;
			while (it != null)
			{
				it.Value._onDisable();
				it = it.Next;
			}
		}
		while (_WaitForEnable.Count > 0)
		{
			var c = _WaitForEnable.Dequeue();

			var it = c.m_components.First;
			while (it != null)
			{
				it.Value._onEnable();
				it = it.Next;
			}
		}

		// update
		{
			var cIt = _Enabled.First;
			while (cIt != null)
			{
				var comIt = cIt.Value.m_components.First;
				while (comIt != null)
				{
					comIt.Value._update(delta);
					comIt = comIt.Next;
				}

				cIt = cIt.Next;
			}
		}

		// late update
		{
			var cIt = _Enabled.First;
			while (cIt != null)
			{
				var comIt = cIt.Value.m_components.First;
				while (comIt != null)
				{
					comIt.Value._lateUpdate(delta);
					comIt = comIt.Next;
				}

				cIt = cIt.Next;
			}
		}
	}

	private LinkedListNode<ComponentContainer> _enabledHandle;

	private EnableState m_state = EnableState.Disabled;

	private LinkedList<Component> m_components;
	private Dictionary<System.Type, LinkedListNode<Component>> m_indexMap;

	public EnableState state => m_state;

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

		var it = m_components.First;
		while (it != null)
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
		var config = Component.GetConfig<T>();
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

	public ComponentContainer()
	{
		m_components = new LinkedList<Component>();
		m_indexMap = new Dictionary<System.Type, LinkedListNode<Component>>();
	}
}

public static class Program
{
	public class Com1 : Component
	{
		public override void _onEnable()
		{
			System.Console.WriteLine("Com1 Init");
		}
		public override void _update(float delta)
		{
			System.Console.WriteLine("Com1 Update");
		}
	}
	public class Com2 : Component
	{
		public override void _onEnable()
		{
			System.Console.WriteLine("Com2 Init");
		}
		public override void _update(float delta)
		{
			System.Console.WriteLine("Com2 Update");
		}

		static Com2()
		{
			BindConfig<Com2>(new RequiredComponentConfig(typeof(Com1)));
		}
	}

	public static void Main()
	{
		ConsoleTraceListener consoleTrace = new ConsoleTraceListener();
		Trace.Listeners.Add(consoleTrace);

		var go = new ComponentContainer();
		go.emplaceComponent<Com1>();
		go.emplaceComponent<Com2>();

		ComponentContainer.Enable(go);

		ComponentContainer.UpdateEnabled(0);
		ComponentContainer.UpdateEnabled(0);
		ComponentContainer.UpdateEnabled(0);
		ComponentContainer.UpdateEnabled(0);
		ComponentContainer.UpdateEnabled(0);
	}
}