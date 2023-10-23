using System.Diagnostics;

namespace urd
{
	public class ConfigRequiredComponent : ConfigComponent
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

		public ConfigRequiredComponent(params System.Type[] require)
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
}