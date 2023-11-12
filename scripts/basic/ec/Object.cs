using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace urd
{
	public abstract class Object
	{
		private static Dictionary<System.Type, LinkedList<Object>> _InstanceRecord 
			= new Dictionary<System.Type, LinkedList<Object>>();

		private static int _IdAllot = 0;

		public static IEnumerable<T> Get<T>()
			where T : Object
		{
			var key = typeof(T);
			Debug.Assert(_InstanceRecord.TryGetValue(key, out var record));

			for (var it = record.First; it != null; it = it.Next)
				yield return it.Value as T;
		}
		public static void RemoveRecord(Object obj)
		{
			Debug.Assert(obj.__itor != null, "try to unrecord an unrecorded object");
			_InstanceRecord[obj.GetType()].Remove(obj.__itor);
		}

		private LinkedListNode<Object> __itor;

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
