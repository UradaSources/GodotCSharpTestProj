#if false && GODOT4_1_OR_GREATER
using System.Collections;
using System.Collections.Generic;
using Godot;

// 需要确保该对象位于节点树的较低层
public partial class GodotInputCache : Node2D, urd.IInputCache
{
	private readonly static Key[] _KeycodeEnumMap = new Key[] {
		Key.Apostrophe,   // Key: '
		Key.Comma,        // Key: ,
		Key.Minus,        // Key: -
		Key.Period,       // Key: .
		Key.Slash,        // Key: /
		Key.Key0,         // Key: 0
		Key.Key1,         // Key: 1
		Key.Key2,         // Key: 2
		Key.Key3,         // Key: 3
		Key.Key4,         // Key: 4
		Key.Key5,         // Key: 5
		Key.Key6,         // Key: 6
		Key.Key7,         // Key: 7
		Key.Key8,         // Key: 8
		Key.Key9,         // Key: 9
		Key.Semicolon,    // Key: ;
		Key.Equal,        // Key: =
		Key.A,            // Key: A | a
		Key.B,            // Key: B | b
		Key.C,            // Key: C | c
		Key.D,            // Key: D | d
		Key.E,            // Key: E | e
		Key.F,            // Key: F | f
		Key.G,            // Key: G | g
		Key.H,            // Key: H | h
		Key.I,            // Key: I | i
		Key.J,            // Key: J | j
		Key.K,            // Key: K | k
		Key.L,            // Key: L | l
		Key.M,            // Key: M | m
		Key.N,            // Key: N | n
		Key.O,            // Key: O | o
		Key.P,            // Key: P | p
		Key.Q,            // Key: Q | q
		Key.R,            // Key: R | r
		Key.S,            // Key: S | s
		Key.T,            // Key: T | t
		Key.U,            // Key: U | u
		Key.V,            // Key: V | v
		Key.W,            // Key: W | w
		Key.X,            // Key: X | x
		Key.Y,            // Key: Y | y
		Key.Z,            // Key: Z | z
		Key.Bracketleft,  // Key: [
		Key.Backslash,    // Key: '\'
		Key.Bracketright, // Key: ]
	};
	private readonly static MouseButton[] _MouseButtonEnumMap = new MouseButton[]
	{
		MouseButton.Left,
		MouseButton.Right,
		MouseButton.Middle
	};

	private static Key _GetGodotKeycode(urd.KeyCode key)
	{
		return _KeycodeEnumMap[(int)key];
	}
	private static MouseButton _GetGodotMouseButton(urd.MouseButton key)
	{
		return _MouseButtonEnumMap[(int)key];
	}

	private urd.vec2 m_mouseScreenPos = default;

	private HashSet<Key> m_pressedKey = new HashSet<Key>();
	private HashSet<Key> m_releasedKey = new HashSet<Key>();

	private HashSet<MouseButton> m_pressedMouseButton = new HashSet<MouseButton>();
	private HashSet<MouseButton> m_releasedMouseButton = new HashSet<MouseButton>();

	public bool getKey(urd.KeyCode key)
	{
		return Input.IsPhysicalKeyPressed(_GetGodotKeycode(key));
	}

	public bool getKeyDown(urd.KeyCode key)
	{
		return m_pressedKey.Contains(_GetGodotKeycode(key));
	}
	public bool getKeyUp(urd.KeyCode key)
	{
		return m_releasedKey.Contains(_GetGodotKeycode(key));
	}

	public bool getMouseKey(urd.MouseButton key)
	{
		return Input.IsMouseButtonPressed(_GetGodotMouseButton(key));
	}

	public bool getMouseKeyDown(urd.MouseButton key)
	{
		return m_pressedMouseButton.Contains(_GetGodotMouseButton(key));
	}
	public bool getMouseKeyUp(urd.MouseButton key)
	{
		return m_releasedMouseButton.Contains(_GetGodotMouseButton(key));
	}

	public urd.vec2 getMouseScreenPosition()
	{
		var pos = this.GetViewport().GetMousePosition();
		return new urd.vec2 { x = pos.X, y = pos.Y };
	}

	public override void _Process(double _)
	{
		m_pressedKey.Clear();
		m_releasedKey.Clear();
		m_pressedMouseButton.Clear();
		m_releasedMouseButton.Clear();

		var pos = this.GetViewport().GetMousePosition();
		m_mouseScreenPos.x = pos.X;
		m_mouseScreenPos.y = pos.Y;
	}

	public override void _Input(InputEvent e)
	{
		if (e is InputEventKey keyInp)
		{
			if (keyInp.IsPressed() && !keyInp.IsEcho())
				m_pressedKey.Add(keyInp.PhysicalKeycode);
			else if (keyInp.IsReleased())
				m_releasedKey.Add(keyInp.PhysicalKeycode);
		}
		else if (e is InputEventMouseButton mouseInp)
		{
			if (mouseInp.IsPressed() && !mouseInp.IsEcho())
				m_pressedMouseButton.Add(mouseInp.ButtonIndex);
			else if (mouseInp.IsReleased())
				m_releasedMouseButton.Add(mouseInp.ButtonIndex);
		}
	}
}
#endif
