using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Diagnostics;

public class DebugDisplay
{
	public struct Message
	{
		public string key;
		public string title;
		public string msg;
		public System.DateTime time;
	}

	private static DebugDisplay _Main;
	public static DebugDisplay Main
	{
		get
		{
			if (_Main == null) _Main = new DebugDisplay();
			return _Main;
		}
	}

	public static IEnumerable<(string name, object value)> GetFields(object obj)
	{
		System.Type type = obj.GetType();
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		foreach (FieldInfo field in fields)
		{
			if (field.Name[0] == '<') continue;
			yield return (field.Name, field.GetValue(obj));
		}
	}
	public static IEnumerable<(string name, object value)> GetProperties(object obj)
	{
		System.Type type = obj.GetType();
		PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		foreach (PropertyInfo property in properties)
		{
			yield return (property.Name, property.GetValue(obj));
		}
	}

	private List<Message> m_recordData = new List<Message>(); 
	private Dictionary<string, int> m_indexMap = new Dictionary<string, int>();

	public int recordCount => m_recordData.Count;

	[Conditional("DEBUG")]
	public void outObject(object obj, string title = null, bool field = true, bool property = false,
		[CallerLineNumber] int lineNumber = 0,
		[CallerFilePath] string filePath = null)
	{
		string key = $"at line {lineNumber} in {filePath}";
		title ??= obj.GetType().Name;
		
		string fieldString = "";
		if (field)
		{
			fieldString = "Field:\n";
			foreach (var (name, value) in GetFields(obj))
				fieldString += $"{name,-18} |{value}\n";
		}

		string propertyString = "";
		if (property)
		{
			propertyString = "Property:\n";
			foreach (var (name, value) in GetProperties(obj))
				propertyString += $"{name,-18} |{value}\n";
		}


		string mesg = $"{fieldString}{propertyString}";
		rawOut(key, title, mesg);
	}

	[Conditional("DEBUG")]
	public void outString(string title, string message,
		[CallerLineNumber] int lineNumber = 0,
		[CallerFilePath] string filePath = null)
	{
		string key = $"at line {lineNumber} in {filePath}";
		rawOut(key, title, message);
	}

	[Conditional("DEBUG")]
	public void rawOut(string key, string title, string msg)
	{
		if (m_indexMap.TryGetValue(key, out int index))
		{
			var record = m_recordData[index];

			record.title = title;
			record.msg = msg;
			record.time = System.DateTime.Now;

			m_recordData[index] = record;
		}
		else
		{
			var record = new Message
			{
				title = title,
				msg = msg,
				time = System.DateTime.Now
			};

			m_indexMap[key] = m_indexMap.Count;
			m_recordData.Add(record);

			Debug.Assert(m_indexMap.Count <= 200, "Too many data record");
		}
	}

	[Conditional("DEBUG")]
	public void clearRecordAll()
	{
		m_indexMap.Clear();
		m_recordData.Clear();
	}

	public Message getRecord(int index)
	{ 
		return m_recordData[index];
	}
}