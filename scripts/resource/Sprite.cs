namespace urd
{
	public class Sprite : Object
	{
		public readonly char graph;
		public readonly byteColor color;

		public Sprite(string name, char graph)
			: this(name, graph, byteColor.white) { }
		public Sprite(string name, char graph, byteColor color)
			: base(name)
		{
			this.graph = graph;
			this.color = color;
		}
	}
}


