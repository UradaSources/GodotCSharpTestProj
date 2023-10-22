namespace urd
{
	public struct vec2
	{
		public static readonly vec2 up = new vec2(0, 1);
		public static readonly vec2 down = new vec2(0, -1);
		public static readonly vec2 left = new vec2(-1, 0);
		public static readonly vec2 right = new vec2(1, 0);
		public static readonly vec2 zero = new vec2(0, 0);
		public static readonly vec2 one = new vec2(1, 1);

		public static float Dot(vec2 l, vec2 r)
		{
			return l.x * r.x + l.y * r.y;
		}

		public static float Angle(vec2 from, vec2 to)
		{
			float denominator = Mathf.Sqrt(from.sqrMagnitude() * to.sqrMagnitude());
			if (denominator < Mathf.EpsilonNormalSqrt)
				return 0;

			float dot = Mathf.Min(Mathf.Max(Dot(from, to) / denominator, -1.0f), 1.0f);
			return Mathf.Acos(dot) * Mathf.Rad2Deg;
		}

		public static float SignedAngle(vec2 from, vec2 to)
		{
			float unsignedAngle = Angle(from, to);
			float sign = (float)(from.x * to.y - from.y * to.x) > 0 ? 1 : -1;
			return unsignedAngle * sign;
		}

		public static float Distance(vec2 a, vec2 b)
		{
			float diff_x = a.x - b.x;
			float diff_y = a.y - b.y;
			return Mathf.Sqrt(diff_x * diff_x + diff_y * diff_y);
		}

		public static vec2 Rotate(vec2 v, float angle, vec2? pivot = null)
		{
			var _pivot = pivot ?? vec2.zero;

			float radians = angle * 3.1415926f / 180.0f;
			float c = Mathf.Cos(radians);
			float s = Mathf.Sin(radians);

			float x = c * (v.x - _pivot.x) - s * (v.y - _pivot.y) + _pivot.x;
			float y = s * (v.x - _pivot.x) + c * (v.y - _pivot.y) + _pivot.y;

			return new vec2(x, y);
		}

		public static vec2 Min(vec2 lhs, vec2 rhs)
		{
			return new vec2(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y));
		}
		public static vec2 Max(vec2 lhs, vec2 rhs)
		{
			return new vec2(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y));
		}

		public static vec2 Lerp(vec2 a, vec2 b, float t) { return (b - a) * t + a; }

		public static vec2 MoveTowards(vec2 current, vec2 target, float maxDistanceDelta)
		{
			float toX = target.x - current.x;
			float toY = target.y - current.y;

			float sqDist = toX * toX + toY * toY;

			if (sqDist == 0 || (maxDistanceDelta >= 0 && sqDist <= maxDistanceDelta * maxDistanceDelta))
				return target;

			float dist = Mathf.Sqrt(sqDist);
			return new vec2(current.x + toX / dist * maxDistanceDelta,
				current.y + toY / dist * maxDistanceDelta);
		}

		public override string ToString()
		{
			return $"vec2{{{this.x:0.00},{this.y:0.00}}}";
		}

		public float x, y;

		public float sqrMagnitude() { return x * x + y * y; }

		public void set(float x, float y) { this.x = x; this.y = y; }

		public void normalize()
		{
			float len = magnitude();
			if (len != 0.0f) this /= len;
		}
		public vec2 normalized()
		{
			float len = magnitude();
			if (len != 0.0f) return this / len;
			else return this;
		}

		public float magnitude() { return Mathf.Sqrt(x * x + y * y); }

		public vec2(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public static explicit operator vec2i(vec2 self)
		{
			return new vec2i((int)self.x, (int)self.y);
		}

		public static implicit operator vec2(System.ValueTuple<float, float> tuple)
		{
			return new vec2(tuple.Item1, tuple.Item2);
		}
		public static implicit operator System.ValueTuple<float, float>(vec2 v)
		{
			return new(v.x, v.y);
		}

		public static vec2 operator +(vec2 a, vec2 b) { return new vec2(a.x + b.x, a.y + b.y); }
		public static vec2 operator -(vec2 a, vec2 b) { return new vec2(a.x - b.x, a.y - b.y); }
		public static vec2 operator *(vec2 a, vec2 b) { return new vec2(a.x * b.x, a.y * b.y); }
		public static vec2 operator /(vec2 a, vec2 b) { return new vec2(a.x / b.x, a.y / b.y); }

		public static vec2 operator +(float a, vec2 b) { return new vec2(a + b.x, a + b.y); }
		public static vec2 operator -(float a, vec2 b) { return new vec2(a - b.x, a - b.y); }
		public static vec2 operator *(float a, vec2 b) { return new vec2(a * b.x, a * b.y); }
		public static vec2 operator /(float a, vec2 b) { return new vec2(a / b.x, a / b.y); }

		public static vec2 operator +(vec2 a, float b) { return new vec2(a.x + b, a.y + b); }
		public static vec2 operator -(vec2 a, float b) { return new vec2(a.x - b, a.y - b); }
		public static vec2 operator *(vec2 a, float b) { return new vec2(a.x * b, a.y * b); }
		public static vec2 operator /(vec2 a, float b) { return new vec2(a.x / b, a.y / b); }

		public static bool operator ==(vec2 a, vec2 b)
		{
			return a.x == b.x && a.y == b.y;
		}
		public static bool operator !=(vec2 a, vec2 b)
		{
			return a.x != b.x || a.y != b.y;
		}

		public override bool Equals(object obj)
		{
			return obj is vec2 vec &&
				x == vec.x &&
				y == vec.y;
		}
		public override int GetHashCode()
		{
			return System.HashCode.Combine(x, y);
		}
	}
}