using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Diagnostics;

public class DebugWatch
{
	public struct RecordMesg
	{
		public string key;
		public string tag;
		public string constnet;
		public string time;
	}

	private static DebugWatch _Main;
	public static DebugWatch Main
	{
		get
		{
			if (_Main == null) _Main = new DebugWatch();
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

	private List<RecordMesg> m_recordData = new List<RecordMesg>(); 
	private Dictionary<string, int> m_indexMap = new Dictionary<string, int>();

	public int recordCount => m_recordData.Count;

	[Conditional("DEBUG")]
	public void watchObject(object obj, string tag = null, bool includeProperty = false,
		[CallerLineNumber] int lineNumber = 0,
		[CallerFilePath] string filePath = null)
	{
		string key = $"{filePath}:{lineNumber}";
		string category = $"{obj.GetType().Name}.Info";
		
		string fieldString = "[b]field:[/b]\n";
		foreach (var (name, value) in GetFields(obj))
			fieldString += $"{name,-18} |{value}\n";

		string propertyString = "";
		if (includeProperty)
		{
			propertyString = "[b]property:[/b]\n";
			foreach (var (name, value) in GetProperties(obj))
				propertyString += $"{name,-18} |{value}\n";
		}

		string content = $"{fieldString}{propertyString}";
		rawOut(key, $"{category}:{tag}", content);
	}

	[Conditional("DEBUG")]
	public void watchValue(string tag, string message,
		[CallerLineNumber] int lineNumber = 0,
		[CallerFilePath] string filePath = null)
	{
		string key = $"{filePath}:{lineNumber}";
		rawOut(key, tag, message);
	}

	[Conditional("DEBUG")]
	public void rawOut(string key, string tag, string msg)
	{
		if (m_indexMap.TryGetValue(key + tag, out int index))
		{
			var record = m_recordData[index];

			record.tag = tag;
			record.constnet = msg;
			record.time = System.DateTime.Now.ToString("HH:mm:ss");

			m_recordData[index] = record;
		}
		else
		{
			var record = new RecordMesg
			{
				tag = tag,
				constnet = msg,
				time = System.DateTime.Now.ToString("HH:mm:ss")
			};

			m_indexMap[key + tag] = m_indexMap.Count;
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

	public RecordMesg getRecord(int index)
	{ 
		return m_recordData[index];
	}
}