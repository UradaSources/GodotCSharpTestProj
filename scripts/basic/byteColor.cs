using System;

namespace urd
{
	public struct byteColor
	{
		public readonly static byteColor clear = new byteColor(0, 0, 0, 0);
		public readonly static byteColor white = new byteColor(255, 255, 255, 255);
		public readonly static byteColor black = new byteColor(0, 0, 0, 255);
		public readonly static byteColor red = new byteColor(255, 0, 0, 255);
		public readonly static byteColor green = new byteColor(0, 255, 0, 255);
		public readonly static byteColor blue = new byteColor(0, 0, 255, 255);
		public readonly static byteColor cyan = new byteColor(0, 255, 255, 255);
		public readonly static byteColor gray = new byteColor(125, 125, 125, 255);
		public readonly static byteColor magenta = new byteColor(255, 0, 255, 255);
		public readonly static byteColor yellow = new byteColor(255, 234, 4, 255);

		public static byteColor FromHex(int hex)
		{
			byte red = (byte)((hex >> 16) & 0xFF);
			byte green = (byte)((hex >> 8) & 0xFF);
			byte blue = (byte)(hex & 0xFF);

			return new byteColor(red, green, blue, 255);
		}
		public static byteColor FromHexIncludeAlpha(int hex)
		{
			byte red = (byte)((hex >> 24) & 0xFF);
			byte green = (byte)((hex >> 16) & 0xFF);
			byte blue = (byte)((hex >> 8) & 0xFF);
			byte alpha = (byte)(hex & 0xFF);

			return new byteColor(red, green, blue, alpha);
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

		public byteColor(byte r, byte g, byte b, byte a = 255)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		public static bool operator ==(byteColor l, byteColor r)
		{
			return l.toHex() == r.toHex();
		}
		public static bool operator !=(byteColor l, byteColor r)
		{
			return l.toHex() != r.toHex();
		}

		public override bool Equals(object obj)
		{
			return obj is byteColor color &&
				this.toHex() == color.toHex();
		}
		public override int GetHashCode()
		{
			return this.toHex();
		}
	}
} // namespace urd