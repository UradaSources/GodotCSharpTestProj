using System;

namespace urd
{
	public struct color
	{
		public readonly static color clear = new color(0, 0, 0, 0);
		public readonly static color white = new color(255, 255, 255, 255);
		public readonly static color black = new color(0, 0, 0, 255);
		public readonly static color red = new color(255, 0, 0, 255);
		public readonly static color green = new color(0, 255, 0, 255);
		public readonly static color blue = new color(0, 0, 255, 255);
		public readonly static color cyan = new color(0, 255, 255, 255);
		public readonly static color gray = new color(125, 125, 125, 255);
		public readonly static color magenta = new color(255, 0, 255, 255);
		public readonly static color yellow = new color(255, 234, 4, 255);

		public static color FromHex(int hex)
		{
			byte red = (byte)((hex >> 16) & 0xFF);
			byte green = (byte)((hex >> 8) & 0xFF);
			byte blue = (byte)(hex & 0xFF);

			return new color(red, green, blue, 255);
		}
		public static color FromHexIncludeAlpha(int hex)
		{
			byte red = (byte)((hex >> 24) & 0xFF);
			byte green = (byte)((hex >> 16) & 0xFF);
			byte blue = (byte)((hex >> 8) & 0xFF);
			byte alpha = (byte)(hex & 0xFF);

			return new color(red, green, blue, alpha);
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

		public color(byte r, byte g, byte b, byte a = 255)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		public static bool operator ==(color l, color r)
		{
			return l.toHex() == r.toHex();
		}
		public static bool operator !=(color l, color r)
		{
			return l.toHex() != r.toHex();
		}

		public override bool Equals(object obj)
		{
			return obj is color color &&
				this.toHex() == color.toHex();
		}
		public override int GetHashCode()
		{
			return this.toHex();
		}
	}
} // namespace urd