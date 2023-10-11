using System.Collections;
using Godot;

public partial class cFunc : Node2D
{
	[Export(hintString: "rect")] private Texture2D m_tex;
	[Export(hintString: "tex rect")] private Rect2 m_texRect;

	[Export(hintString: "size")] private Vector2 m_size = new Vector2(0.5f, 0.5f);

	[Export] private Vector2 m_pivot;

	private Transform2D m_mat;

	public void printHello(string msg)
	{
		GD.Print(msg);
	}

	public override void _Ready()
	{
		m_mat = Transform2D.Identity;
		m_mat.ScaledLocal(new Vector2(1, -1));
	}

	public override void _Process(double delta)
	{
		this.QueueRedraw();
	}

	public override void _Draw()
	{
		if (m_tex != null)
		{
			DrawSetTransformMatrix(m_mat);

			var center = m_size * m_pivot;
			Rect2 pos = new Rect2(-center, m_size);

			this.DrawTextureRectRegion(m_tex, pos, m_texRect);
		}
	}
}
