namespace urd
{
	public enum KeyCode
	{
		Apostrophe,   // Key: '
		Comma,        // Key: ,
		Minus,        // Key: -
		Period,       // Key: .
		Slash,        // Key: /
		Key0,         // Key: 0
		Key1,         // Key: 1
		Key2,         // Key: 2
		Key3,         // Key: 3
		Key4,         // Key: 4
		Key5,         // Key: 5
		Key6,         // Key: 6
		Key7,         // Key: 7
		Key8,         // Key: 8
		Key9,         // Key: 9
		Semicolon,    // Key: ;
		Equal,        // Key: =
		A,            // Key: A | a
		B,            // Key: B | b
		C,            // Key: C | c
		D,            // Key: D | d
		E,            // Key: E | e
		F,            // Key: F | f
		G,            // Key: G | g
		H,            // Key: H | h
		I,            // Key: I | i
		J,            // Key: J | j
		K,            // Key: K | k
		L,            // Key: L | l
		M,            // Key: M | m
		N,            // Key: N | n
		O,            // Key: O | o
		P,            // Key: P | p
		Q,            // Key: Q | q
		R,            // Key: R | r
		S,            // Key: S | s
		T,            // Key: T | t
		U,            // Key: U | u
		V,            // Key: V | v
		W,            // Key: W | w
		X,            // Key: X | x
		Y,            // Key: Y | y
		Z,            // Key: Z | z
		Bracketleft,  // Key: [
		Backslash,    // Key: '\'
		Bracketright, // Key: ]
	}
	public enum MouseCode
	{
		Left,
		Right,
		Middle
	}

	public interface InputService
	{
		bool getKey(KeyCode key);
		bool getKeyDown(KeyCode key);
		bool getKeyUp(KeyCode key);
		bool getMouseKey(MouseCode key);
		bool getMouseKeyDown(MouseCode key);
		bool getMouseKeyUp(MouseCode key);
		vec2 getMouseScreenPosition();
	}
}

