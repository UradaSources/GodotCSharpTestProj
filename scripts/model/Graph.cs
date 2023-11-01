using Godot;
using urd;

public class Graph : Component
{
	public char graph { set; get; }
	public Color color { set; get; }

	public Graph(char graph, Color color)
	{
		this.graph = graph;
		this.color = color;
	}
}
