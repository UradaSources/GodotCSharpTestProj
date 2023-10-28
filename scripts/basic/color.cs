namespace urd
{
	public struct Color
	{
		public readonly static Color clear = new Color(0, 0, 0, 0);
		public readonly static Color white = new Color(255, 255, 255, 255);
		public readonly static Color black = new Color(0, 0, 0, 255);
		public readonly static Color red = new Color(255, 0, 0, 255);
		public readonly static Color green = new Color(0, 255, 0, 255);
		public readonly static Color blue = new Color(0, 0, 255, 255);
		public readonly static Color cyan = new Color(0, 255, 255, 255);
		public readonly static Color gray = new Color(125, 125, 125, 255);
		public readonly static Color magenta = new Color(255, 0, 255, 255);
		public readonly static Color yellow = new Color(255, 234, 4, 255);

		public static Color FromHex(int hex)
		{
			byte red = (byte)((hex >> 16) & 0xFF);
			byte green = (byte)((hex >> 8) & 0xFF);
			byte blue = (byte)(hex & 0xFF);

			return new Color(red, green, blue, 255);
		}
		public static Color FromHexIncludeAlpha(int hex)
		{
			byte red = (byte)((hex >> 24) & 0xFF);
			byte green = (byte)((hex >> 16) & 0xFF);
			byte blue = (byte)((hex >> 8) & 0xFF);
			byte alpha = (byte)(hex & 0xFF);

			return new Color(red, green, blue, alpha);
		}

		public byte r, g, b, a;

		public void set(byte r, byte g, byte b, byte a = 255)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		public int toHex()
		{
			return (this.r << 24) | (this.g << 16) | (this.b << 8) | this.a;
		}

		public Color(byte r, byte g, byte b, byte a = 255)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		public static bool operator ==(Color l, Color r)
		{
			return l.toHex() == r.toHex();
		}
		public static bool operator !=(Color l, Color r)
		{
			return l.toHex() != r.toHex();
		}

		public override bool Equals(object obj)
		{
			return obj is Color color &&
				this.toHex() == color.toHex();
		}
		public override int GetHashCode()
		{
			return this.toHex();
		}
	}
} // namespace urd