using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

	private List<Message> m_recordData = new List<Message>(); 
	private Dictionary<string, int> m_indexMap = new Dictionary<string, int>();

	public int recordCount => m_recordData.Count;

	public void Out(string title, string message,
		[CallerLineNumber] int lineNumber = 0,
		[CallerFilePath] string filePath = null)
	{
		string key = $"at line {lineNumber} in {filePath}";
		RawOut(key, title, message);
	}
	public void RawOut(string key, string title, string msg)
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

	public void ClearRecordAll()
	{
		m_indexMap.Clear();
		m_recordData.Clear();
	}

	public Message GetRecord(int index)
	{ 
		return m_recordData[index];
	}
}