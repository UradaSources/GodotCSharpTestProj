using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace urd
{
	public abstract class Resource : Object
	{
		private static Dictionary<System.Type, LinkedList<Resource>> _InstanceRecord
			= new Dictionary<System.Type, LinkedList<Resource>>();

		public static IEnumerable<T> Get<T>()
			where T : Resource
		{
			var key = typeof(T);
			Debug.Assert(_InstanceRecord.TryGetValue(key, out var record));

			for (var it = record.First; it != null; it = it.Next)
				yield return it.Value as T;
		}
		public static void Remove(Resource obj)
		{
			Debug.Assert(obj.__itor != null, "try to unrecord an unrecorded object");
			_InstanceRecord[obj.GetType()].Remove(obj.__itor);
		}

		private LinkedListNode<Resource> __itor;

		private string m_name;

		public string name { set => m_name = value; get => m_name; }

		public Resource(string name) : base()
		{
			m_name = name;

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

	public class DestroyedObjectException : System.Exception { }

	public abstract class Object
	{
		private bool m_destroyed;

		public readonly string mid;

		public bool destroyed => m_destroyed;

		public abstract Object copy();
		protected abstract void _onDestroy();

		public void destroy()
		{
			if (m_destroyed) throw new DestroyedObjectException();

			m_destroyed = true;
			this._onDestroy();
		}

		public override bool Equals(object obj)
		{
			return obj is Object o && mid == o.mid;
		}
		public override int GetHashCode()
		{
			return System.HashCode.Combine(mid);
		}

		protected Object()
		{
			mid = System.Convert.ToBase64String(System.Guid.NewGuid().ToByteArray());
		}
	}
}
