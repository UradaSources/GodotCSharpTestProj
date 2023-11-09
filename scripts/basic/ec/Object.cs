using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace urd
{
	public abstract class Object
	{
		private struct Record
		{
			public IList<Object> instances;
			public Dictionary<string, object> index;


		}

		private static Dictionary<System.Type, LinkedList<Object>> _InstanceRecord 
			= new Dictionary<System.Type, LinkedList<Object>>();

		private static int _IdAllot = 0;

		public static IEnumerable<T> IterateInstances<T>()
			where T : Object
		{
			var key = typeof(T);
			if (_InstanceRecord.TryGetValue(key, out var record))
			{
				for (var it = record.First; it != null; it = it.Next)
					yield return it.Value as T;
			}
		}

		private readonly int m_instanceId;
		private readonly string m_name;

		public int instanceId => m_instanceId;
		public string name => m_name;

		protected Object(string name)
		{
			m_instanceId = ++Object._IdAllot;
			m_name = name;

			// 添加记录
			var type = this.GetType();
			var recordOption = type.GetCustomAttribute<RecordObjectAttribute>();
            if (recordOption != null)
            {
				var recordKey = recordOption.overrideRecordKey ?? type;
				Debug.Assert(recordKey.IsInstanceOfType(this));

				LinkedList<Object> record;
				if (!_InstanceRecord.TryGetValue(recordKey, out record))
				{
					record = new LinkedList<Object>();
					_InstanceRecord.Add(recordKey, record);
				}

				record.AddLast(this);
            }
        }
	}
}
