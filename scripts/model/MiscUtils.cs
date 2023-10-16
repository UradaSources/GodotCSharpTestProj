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
	}
}
