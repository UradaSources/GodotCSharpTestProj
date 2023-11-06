using System.Diagnostics;
using Godot;

public partial class GodotTextRenderTest : Node2D
{
	[Export] private Font m_realFont;

	private TextServer m_textServer;
	
	private Godot.Collections.Array<Rid> m_fonts;
	private Rid m_text;

	public override void _ExitTree()
	{
		base._ExitTree();

		m_textServer.FreeRid(m_text);
	}

	public override void _Ready()
	{
		base._Ready();

		m_textServer = TextServerManager.GetPrimaryInterface();

		m_fonts = m_realFont.GetRids(); // m_textServer.CreateFont();
		m_text = m_textServer.CreateShapedText();

		Debug.WriteLine($"create done, {m_fonts.Count}, {m_text}");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		m_textServer.FontSetEmbolden(m_fonts[0], 1);
		m_textServer.ShapedTextAddString(m_text, "Hello, world", m_fonts, 12);

		this.QueueRedraw();

		m_textServer.FontSetEmbolden(m_fonts[0], 2);
		m_textServer.ShapedTextAddString(m_text, "BHello, world", m_fonts, 12);

		this.QueueRedraw();

		m_textServer.FontSetEmbolden(m_fonts[0], 0);
		m_textServer.ShapedTextAddString(m_text, "Hello, world", m_fonts, 12);

		this.QueueRedraw();
	}

	public override void _Draw()
	{
		base._Draw();
		m_textServer.ShapedTextDraw(m_text, this.GetCanvasItem(), new Vector2(100, 100));
	}
}
