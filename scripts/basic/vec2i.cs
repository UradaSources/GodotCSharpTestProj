namespace urd
{
	public struct vec2i
	{
		public static readonly vec2i up = new vec2i(0, 1);
		public static readonly vec2i down = new vec2i(0, -1);
		public static readonly vec2i left = new vec2i(-1, 0);
		public static readonly vec2i right = new vec2i(1, 0);
		public static readonly vec2i zero = new vec2i(0, 0);
		public static readonly vec2i one = new vec2i(1, 1);

		public static float Distance(vec2i a, vec2i b)
		{
			int diff_x = a.x - b.x;
			int diff_y = a.y - b.y;
			return mathf.sqrt(diff_x * diff_x + diff_y * diff_y);
		}

		public static vec2i Min(vec2i lhs, vec2i rhs)
		{
			return new vec2i(mathf.min(lhs.x, rhs.x), mathf.min(lhs.y, rhs.y));
		}
		public static vec2i Max(vec2i lhs, vec2i rhs)
		{
			return new vec2i(mathf.max(lhs.x, rhs.x), mathf.max(lhs.y, rhs.y));
		}

		public override string ToString()
		{
			return $"vec2i{{{this.x},{this.y}}}";
		}

		public int x, y;

		public float sqrMagnitude() { return x * x + y * y; }

		public void set(int x, int y) { this.x = x; this.y = y; }

		public float magnitude() { return mathf.sqrt(x * x + y * y); }

		public vec2i(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public static implicit operator vec2(vec2i self)
		{
			return new vec2(self.x, self.y);
		}

		public static implicit operator vec2i(System.ValueTuple<int, int> tuple)
		{
			return new vec2i(tuple.Item1, tuple.Item2);
		}
		public static implicit operator System.ValueTuple<int, int>(vec2i v)
		{
			return new(v.x, v.y);
		}

		public static vec2i operator +(vec2i a, vec2i b) { return new vec2i(a.x + b.x, a.y + b.y); }
		public static vec2i operator -(vec2i a, vec2i b) { return new vec2i(a.x - b.x, a.y - b.y); }
		public static vec2i operator *(vec2i a, vec2i b) { return new vec2i(a.x * b.x, a.y * b.y); }
		public static vec2i operator /(vec2i a, vec2i b) { return new vec2i(a.x / b.x, a.y / b.y); }
		public static vec2i operator %(vec2i a, vec2i b) { return new vec2i(a.x / b.x, a.y / b.y); }

		public static bool operator ==(vec2i a, vec2i b)
		{
			return a.x == b.x && a.y == b.y;
		}
		public static bool operator !=(vec2i a, vec2i b)
		{
			return a.x != b.x || a.y != b.y;
		}

		public override bool Equals(object obj)
		{
			return obj is vec2i vec &&
				x == vec.x &&
				y == vec.y;
		}
		public override int GetHashCode()
		{
			return System.HashCode.Combine(x, y);
		}
	}
}