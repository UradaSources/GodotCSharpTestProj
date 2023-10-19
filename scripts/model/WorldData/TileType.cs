using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using urd;

public class TileType
{
	private static SortedList<char, TileType> _Types = new SortedList<char, TileType>();

	public static TileType Create(string name, char graph, color color, float cost)
	{
		Debug.Assert(!_Types.ContainsKey(graph));

		var type = new TileType(name, graph, color, cost);
		_Types.Add(graph, type);

		return type;
	}
	public static TileType GetType(char graph)
	{
		if (_Types.TryGetValue(graph, out var type)) return type;
		else return null;
	}
	public static TileType FindType(string name)
	{
		foreach (var pair in _Types)
		{
			if (pair.Value.name == name)
				return pair.Value;
		}
		return null;
	}

	public static string ToJson()
	{
		return JsonSerializer.Serialize(_Types);
	}
	public static int AttachFromJson(string jsonData)
	{
		int count = _Types.Count;
		_Types = JsonSerializer.Deserialize<SortedList<char, TileType>>(jsonData);
		return _Types.Count - count;
	}

	public readonly char graph;
	public readonly color color;

	public readonly string name;
	public readonly float cost;

	private TileType(string name, char graph, color color, float cost)
	{
		this.graph = graph;
		this.color = color;
		this.name = name;
		this.cost = cost;
	}
}
