//using Godot;

//public partial class InputTest : Node2D
//{
//	[Export] private GodotInputCache m_inp;

//	public override void _Process(double delta)
//	{
//		if (m_inp.getKeyDown(urd.KeyCode.A))
//			GD.Print("A down");

//		if (m_inp.getKeyUp(urd.KeyCode.A))
//			GD.Print("A up");

//		if (m_inp.getKey(urd.KeyCode.A))
//			GD.Print("A ing");
//	}

//	public override void _Input(InputEvent @event)
//	{
//		if (@event is InputEventKey keyInp)
//		{
//			//GD.Print($"{keyInp.Keycode}, down: {keyInp.IsPressed()}, up: {keyInp.IsReleased()}, echo: {keyInp.IsEcho()}");
//		}
//	}
//}
