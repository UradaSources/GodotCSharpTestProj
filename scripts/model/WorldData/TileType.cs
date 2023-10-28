using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using urd;

public class TileType
{
	public static readonly TileType Default = new TileType(-1, ' ', Color.white, -1);
	private static List<TileType> _Types = new List<TileType>();

	public static int TypeCount => _Types.Count;

	public static TileType Create(char graph, Color color, float cost)
	{
		int id = _Types.Count;
		var type = new TileType(id, graph, color, cost);
		_Types.Add(type);
		return type;
	}
	public static TileType Get(int id)
	{
		Debug.Assert(id >= 0 && id < _Types.Count, $"tile type({id}) does not exist");
		return _Types[id];
	}

	public static void InitFromJson(string jsonData)
	{
		Debug.Assert(_Types.Count == 0, "cannot init tiletype form json.");
		_Types = JsonSerializer.Deserialize<List<TileType>>(jsonData);
	}
	public static string ToJson()
	{
		return JsonSerializer.Serialize(_Types);
	}

	public readonly int id;
	public readonly float cost;

	public readonly char graph;
	public readonly Color c;

	private TileType(int id, char graph, Color c, float cost)
	{
		this.id = id;
		this.cost = cost;

		this.graph = graph;
		this.c = c;
	}
}
