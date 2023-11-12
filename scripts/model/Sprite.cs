namespace urd
{
	[RecordObject]
	public class Sprite : Object
	{
		public readonly char graph;
		public readonly rgba color;

		public Sprite(string name, char graph)
			: this(name, graph, rgba.white) { }
		public Sprite(string name, char graph, rgba color)
			: base(name)
		{
			this.graph = graph;
			this.color = color;
		}
	}
}


