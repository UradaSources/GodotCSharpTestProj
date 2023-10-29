using System.Collections;
using System.Collections.Generic;

namespace urd
{
	public abstract class Renderer : Component
	{
		public static LinkedList<Renderer> _Instances = new LinkedList<Renderer>();
		public static IEnumerable<Renderer> IterateInstance()
		{
			for (var it = _Instances.First; it != null; it = it.Next)
				yield return it.Value;
		}

		private readonly LinkedListNode<Renderer> _itor = null;

		public Renderer()
		{
			_itor = Renderer._Instances.AddLast(this);
		}
		~Renderer()
		{
			Renderer._Instances.Remove(_itor);
		}

		public abstract void _draw();
	}
}