using Godot;

namespace urd
{
	public static class MiscUtils
	{
		public static Color FromHex(int hex)
		{
			float red = ((hex >> 16) & 0xFF) / 255.0f;
			float green = ((hex >> 8) & 0xFF) / 255.0f;
			float blue = (hex & 0xFF) / 255.0f;
			return new Color(red, green, blue, 255);
		}
		public static Color FromHexIncludeAlpha(int hex)
		{
			float red = ((hex >> 16) & 0xFF) / 255.0f;
			float green = ((hex >> 8) & 0xFF) / 255.0f;
			float blue = (hex & 0xFF) / 255.0f;
			float alpha = (hex & 0xFF) / 255.0f;
			return new Color(red, green, blue, alpha);
		}

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

		public static Vector2 Max(Vector2 a, Vector2 b)
			=> new Vector2(Mathf.Max(a.X, b.X), Mathf.Max(a.Y, b.Y));
		public static Vector2 Min(Vector2 a, Vector2 b)
			=> new Vector2(Mathf.Min(a.X, b.X), Mathf.Min(a.Y, b.Y));
	}
}
