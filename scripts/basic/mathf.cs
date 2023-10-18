namespace urd
{
	public static class Mathf
	{
		public const float Epsilon = 0.00001f;
		public const float EpsilonNormalSqrt = 1e-15f;
		public const float Rad2Deg = 360.0f / (3.1415926f * 2.0f);
		public const float PI = (float)System.Math.PI;

		public static float Abs(float x) { return System.Math.Abs(x); }

		public static float Min(float a, float b) { return System.Math.Min(a, b); }
		public static int Min(int a, int b) { return System.Math.Min(a, b); }
		public static float Max(float a, float b) { return System.Math.Max(a, b); }
		public static int Max(int a, int b) { return System.Math.Max(a, b); }

		public static float Sqrt(float v) { return (float)System.Math.Sqrt(v); }

		public static float Sin(float v) { return (float)System.Math.Sin(v); }
		public static float Cos(float v) { return (float)System.Math.Cos(v); }
		public static float Acos(float v) { return (float)System.Math.Acos(v); }

		public static float MoveTowards(float from, float target, float maxDelta)
		{
			if (Mathf.Abs(target - from) > maxDelta)
				return target > from ? from + maxDelta : from - maxDelta;

			return target;
		}

		public static float Clamp(float v, float min, float max)
		{
			return v > max ? max : v < min ? v : v;
		}
		public static float Clamp01(float v)
		{
			return Mathf.Clamp(v, 0, 1);
		}

		public static bool Approximately(float a, float b)
		{
			return Mathf.Abs(b - a) < Mathf.Max(0.000001f * Mathf.Max(Mathf.Abs(a), Mathf.Abs(b)), Mathf.Epsilon * 8);
		}

		public static float LoopValue(float v, float max, float min)
		{
			if (v < min)
			{
				float dist = Mathf.Abs(v - min) % (max - min);
				return max - dist;
			}
			else if (v > max)
			{
				float dist = Mathf.Abs(max - v) % (max - min);
				return min + dist;
			}
			else return v;
		}
		public static float LoopValue(float v, float max)
			=> LoopValue(v, max, 0);

		public static int LoopIndex(int i, int length)
		{
			if (i < 0) i = length + (i % length);
			return i % length;
		}
	}
}