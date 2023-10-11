namespace urd
{
	public static class mathf
	{
		public const float Epsilon = 0.00001f;
		public const float EpsilonNormalSqrt = 1e-15f;
		public const float Rad2Deg = 360.0f / (3.1415926f * 2.0f);

		public static float abs(float x) { return System.Math.Abs(x); }

		public static float min(float a, float b) { return System.Math.Min(a, b); }
		public static int min(int a, int b) { return System.Math.Min(a, b); }
		public static float max(float a, float b) { return System.Math.Max(a, b); }
		public static int max(int a, int b) { return System.Math.Max(a, b); }

		public static float sqrt(float v) { return (float)System.Math.Sqrt(v); }

		public static float sin(float v) { return (float)System.Math.Sin(v); }
		public static float cos(float v) { return (float)System.Math.Cos(v); }
		public static float acos(float v) { return (float)System.Math.Acos(v); }

		public static float moveTowards(float from, float target, float maxDelta)
		{
			if (mathf.abs(target - from) > maxDelta)
				return target > from ? from + maxDelta : from - maxDelta;

			return target;
		}

		public static float clamp(float v, float min, float max)
		{
			return v > max ? max : v < min ? v : v;
		}
		public static float clamp01(float v)
		{
			return mathf.clamp(v, 0, 1);
		}
	}
}