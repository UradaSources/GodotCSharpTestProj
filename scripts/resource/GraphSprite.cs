namespace urd
{
	public class GraphSprite : Object
	{
		public readonly char graph;
		public readonly byteColor color;

		public GraphSprite(string name, char graph, byteColor color)
			: base(name)
		{
			this.graph = graph;
			this.color = color;
		}
	}

}


