namespace urd
{
	public struct rgba
	{
		public readonly static rgba clear = new rgba(0, 0, 0, 0);
		public readonly static rgba white = new rgba(255, 255, 255, 255);
		public readonly static rgba black = new rgba(0, 0, 0, 255);
		public readonly static rgba red = new rgba(255, 0, 0, 255);
		public readonly static rgba green = new rgba(0, 255, 0, 255);
		public readonly static rgba blue = new rgba(0, 0, 255, 255);
		public readonly static rgba cyan = new rgba(0, 255, 255, 255);
		public readonly static rgba gray = new rgba(125, 125, 125, 255);
		public readonly static rgba magenta = new rgba(255, 0, 255, 255);
		public readonly static rgba yellow = new rgba(255, 234, 4, 255);

		public static rgba Hex(int hex)
		{
			byte red = (byte)((hex >> 16) & 0xFF);
			byte green = (byte)((hex >> 8) & 0xFF);
			byte blue = (byte)(hex & 0xFF);

			return new rgba(red, green, blue, 255);
		}
		public static rgba HexAlpha(int hex)
		{
			byte red = (byte)((hex >> 24) & 0xFF);
			byte green = (byte)((hex >> 16) & 0xFF);
			byte blue = (byte)((hex >> 8) & 0xFF);
			byte alpha = (byte)(hex & 0xFF);

			return new rgba(red, green, blue, alpha);
		}
		public static rgba Float(float r, float g, float b, float a = 1.0f)
		{
			return new rgba((byte)(r * 255), (byte)(g * 255), (byte)(b * 255), (byte)(a * 255));
		}
		public static rgba Byte(byte r, byte g, byte b, byte a = 255)
		{
			return new rgba(r, g, b, a);
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

		public rgba(byte r, byte g, byte b, byte a = 255)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		public static bool operator ==(rgba l, rgba r)
		{
			return l.toHex() == r.toHex();
		}
		public static bool operator !=(rgba l, rgba r)
		{
			return l.toHex() != r.toHex();
		}

		public override bool Equals(object obj)
		{
			return obj is rgba color &&
				this.toHex() == color.toHex();
		}
		public override int GetHashCode()
		{
			return this.toHex();
		}
	}
} // namespace urd