using System.Collections;
using System.Collections.Generic;

namespace urd
{
	public abstract class Object
	{
		private static Dictionary<System.Type, LinkedList<Object>> _Ref;
		private static int _IdAllot = 0;

		public static IEnumerable<T> IterateObject<T>()
			where T : Object
		{
			if (_Ref.TryGetValue(typeof(T), out var list))
			{
				for (var it = list.First; it != null; it = it.Next)
					yield return it.Value as T;
			}
		}

		private readonly int m_id;
		private readonly string m_name;

		public int id => m_id;
		public string name => m_name;

		protected Object(string name)
		{
			m_id = ++Object._IdAllot;
			m_name = name;

			var realType = this.GetType();
			if (!Object._Ref.TryGetValue(realType, out var list))
			{
				list = new LinkedList<Object>();
				Object._Ref.Add(realType, list);
			}
			list.AddLast(this);
		}
	}

}
