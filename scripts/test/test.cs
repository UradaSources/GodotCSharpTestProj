using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public interface ComponentConfig
{
	bool beforeJoinContainer(Component com, ComponentContainer container);
}

public abstract class Component
{
	private static Dictionary<System.Type, ComponentConfig> _Config;

	public static void BindConfig<T>(ComponentConfig config)
		where T : Component
	{
		var key = typeof(T);

		if (config != null)
			_Config[key] = config;
		else
			_Config.Remove(key);
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
		Debug.Assert(m_container == null, "");
		Debug.Assert(container != null, "");

		m_container = container;
		m_index = index;
	}
	public virtual void _onRemoveFromContainer()
	{
		Debug.Assert(m_container != null, "");

		m_container = null;
		m_index = -1;
	}

	public abstract void _onEnable();
	public abstract void _onDisable();

	public abstract void _update(float delta);
	public abstract void _lateUpdate(float delta);
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
		Debug.Assert(type.IsSubclassOf(typeof(Component)));

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
			return null;
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
