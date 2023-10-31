using Godot;

namespace urd
{
	public static class MiscUtils
	{
		public static T CreateRootNode<T>(string name)
			where T : Node, new()
		{
			var node = new T();
			node.Name = name;

			var tree = Engine.GetMainLoop() as SceneTree;
			tree.Root.CallDeferred("add_child", node);

			return node;
		}

		public static bool Metronome(int frequency, float offset = 0)
		{
			switch (frequency)
			{
				case -1: return true;
				case 0: return false;
			}

			float t = offset + Time.GetTicksMsec() * 0.001f;
			return (int)(t * 2 * frequency) % 2 == 0;
		}
	}
}
