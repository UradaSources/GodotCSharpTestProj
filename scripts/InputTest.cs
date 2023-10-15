using Godot;
using urd;

public partial class ServiceManager : Node 
{
	private IInputService _inputService;
}

public partial class InputTest : Node2D
{
	private IInputService m_inp;

	public override void _Ready()
	{
		base._Ready();

		m_inp = this.GetNodeOrNull<InputCache>("/root/InputService");
		if (m_inp == null)
		{
			var node = new InputCache();
			node.Name = "InputService";

			this.AddChild(node);

			m_inp = node;
		}
	}

	public override void _Process(double delta)
	{
		if (m_inp.getKeyDown(KeyCode.A))
			GD.Print("A down");

		if (m_inp.getKeyUp(KeyCode.A))
			GD.Print("A up");

		if (m_inp.getKey(KeyCode.A))
			GD.Print("A ing");
	}
}
