using System.Collections;
using System.Collections.Generic;

namespace urd
{
	public abstract class Object
	{
		private static int _IdAllot = 0;

		private readonly int m_instanceId;
		private readonly string m_name;

		public int instanceId => m_instanceId;
		public string name => m_name;

		protected Object(string name)
		{
			m_instanceId = ++Object._IdAllot;
			m_name = name;
		}
	}

}
