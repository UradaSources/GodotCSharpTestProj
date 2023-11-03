using System;

namespace urd
{
	public struct colorb
	{
		public readonly static colorb clear = new colorb(0, 0, 0, 0);
		public readonly static colorb white = new colorb(255, 255, 255, 255);
		public readonly static colorb black = new colorb(0, 0, 0, 255);
		public readonly static colorb red = new colorb(255, 0, 0, 255);
		public readonly static colorb green = new colorb(0, 255, 0, 255);
		public readonly static colorb blue = new colorb(0, 0, 255, 255);
		public readonly static colorb cyan = new colorb(0, 255, 255, 255);
		public readonly static colorb gray = new colorb(125, 125, 125, 255);
		public readonly static colorb magenta = new colorb(255, 0, 255, 255);
		public readonly static colorb yellow = new colorb(255, 234, 4, 255);

		public static colorb FromHex(int hex)
		{
			byte red = (byte)((hex >> 16) & 0xFF);
			byte green = (byte)((hex >> 8) & 0xFF);
			byte blue = (byte)(hex & 0xFF);

			return new colorb(red, green, blue, 255);
		}
		public static colorb FromHexIncludeAlpha(int hex)
		{
			byte red = (byte)((hex >> 24) & 0xFF);
			byte green = (byte)((hex >> 16) & 0xFF);
			byte blue = (byte)((hex >> 8) & 0xFF);
			byte alpha = (byte)(hex & 0xFF);

			return new colorb(red, green, blue, alpha);
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

		public colorb(byte r, byte g, byte b, byte a = 255)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		public static bool operator ==(colorb l, colorb r)
		{
			return l.toHex() == r.toHex();
		}
		public static bool operator !=(colorb l, colorb r)
		{
			return l.toHex() != r.toHex();
		}

		public override bool Equals(object obj)
		{
			return obj is colorb color &&
				this.toHex() == color.toHex();
		}
		public override int GetHashCode()
		{
			return this.toHex();
		}
	}
} // namespace urd