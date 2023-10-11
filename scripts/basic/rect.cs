namespace urd // Note: actual namespace depends on the project name.
{
	public struct rect
	{
		public static rect MaxMin(vec2 max, vec2 min)
		{
			vec2 center = vec2.Lerp(min, max, 0.5f);
			return new rect(center, max - min);
		}
		public static rect Overlaps(rect l, rect r)
		{
			vec2 overlapsMin = vec2.Max(l.min(), r.min());
			vec2 overlapsMax = vec2.Min(l.max(), r.max());
			return rect.MaxMin(overlapsMax, overlapsMin);
		}

		public vec2 center;
		public vec2 size;

		public vec2 haflSize() { return this.size * 0.5f; }

		public float top() { return center.y + size.y * 0.5f; }
		public float bottom() { return center.y - size.y * 0.5f; }
		public float right() { return center.x + size.x * 0.5f; }
		public float left() { return center.x - size.x * 0.5f; }

		public vec2 min() { return center - size * 0.5f; }
		public vec2 max() { return center + size * 0.5f; }

		public float x() { return center.x; }
		public float y() { return center.y; }
		public float width() { return size.x; }
		public float height() { return size.y; }

		public rect(vec2 center, vec2 size)
		{
			this.center = center;
			this.size = size;
		}
		public rect(float x, float y, float w, float h)
		{
			this.center = new vec2(x, y);
			this.size = new vec2(w, h);
		}
	};
}