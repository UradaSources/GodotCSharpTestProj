using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

namespace urd
{
	public interface IDebugDrawing
	{
		public void DrawLine(vec2 a, vec2 b, color? c = null, float width = -1);
	}

	public static class DebugDrawingExtension
	{
		public static void DrawMark(this IDebugDrawing drawing, vec2 pos, float size = 10.0f, color? c = null, float width = -1)
		{
			vec2 xOffset = size * vec2.right * 0.5f;
			vec2 yOffset = size * vec2.up * 0.5f;

			drawing.DrawLine(pos - xOffset, pos + xOffset, c, width);
			drawing.DrawLine(pos - yOffset, pos + yOffset, c, width);
		}

		//public static void DrawArrow(this ILineDrawing drawing, vec2 pos, vec2 vec, color? c = null, float width = -1)
		//{
		//	const float ArrowHeadAngle = 15.0f;

		//	DebugDrawingExtension.DrawRay(drawing, pos, vec, c, width);

		//	var left = (vec * -1).normalized;
		//	var right = (vec * -1).normalized;

		//	left = Quaternion.AngleAxis(Vector2.Angle(vec, Vector2.zero) + ArrowHeadAngle, Vector3.forward) * left;
		//	right = Quaternion.AngleAxis(Vector2.Angle(vec, Vector2.zero) - ArrowHeadAngle, Vector3.forward) * right;

		//	var scale = MiscUtils.EditorGizmoScale(pos);
		//	param.size *= scale;

		//	DebugUtils.DrawRay(pos + vec, left * param.size, args);
		//	DebugUtils.DrawRay(pos + vec, right * param.size, args);
		//}

		//public static void DrawArrowBetween(this ILineDrawing drawing, vec2 start, vec2 target, color? c = null, float width = -1)
		//{
		//}

		public static void DrawRay(this IDebugDrawing drawing, vec2 pos, vec2 dir, color? c = null, float width = -1)
		{
			drawing.DrawLine(pos, pos + dir, c: c, width: width);
		}

		public static void DrawLines(this IDebugDrawing drawing, IEnumerable<vec2> points, bool isClosed = false, bool markPoint = false, color? c = null, float width = -1)
		{
			bool recordFirst = false;
			vec2 first = default;

			vec2 previous = default;

			foreach (var cur in points)
			{
				if (markPoint)
					DebugDrawingExtension.DrawMark(drawing, cur, c: c, width: width);

				if (!recordFirst)
				{
					first = cur;
					recordFirst = true;
				}
				else
					drawing.DrawLine(previous, cur, c: c, width: width);

				previous = cur;
			}

			if (isClosed)
				drawing.DrawLine(previous, first, c: c, width: width);
		}

		public static void DrawBox(this IDebugDrawing drawing, vec2 pos, vec2 size, color? c = null, float width = -1)
		{
			float w = size.x * 0.5f;
			float h = size.y * 0.5f;

			var p1 = pos + new vec2(-w, h);
			var p2 = pos + new vec2(w, h);
			var p3 = pos + new vec2(w, -h);
			var p4 = pos + new vec2(-w, -h);

			drawing.DrawLine(p1, p2, c: c, width: width);
			drawing.DrawLine(p2, p3, c: c, width: width);
			drawing.DrawLine(p3, p4, c: c, width: width);
			drawing.DrawLine(p4, p1, c: c, width: width);
		}

		public static void DrawRange(this IDebugDrawing drawing, vec2 min, vec2 max, color? c = null, float width = -1)
		{
			DebugDrawingExtension.DrawBox(drawing, vec2.Lerp(min, max, 0.5f), (max - min), c: c, width: width);

			DebugDrawingExtension.DrawMark(drawing, min, c: c, width: width);
			DebugDrawingExtension.DrawMark(drawing, max, c: c, width: width);
		}

		public static void DrawCircle(this IDebugDrawing drawing, vec2 centre, float r, int sample = 32, color? c = null, float width = -1)
		{
			vec2 first = default;
			vec2 previous = default;

			// [0, 2PI]
			for (int i = 0; i <= sample; i++)
			{
				float radian = (2.0f * Mathf.PI) * (((float)i) / sample);

				vec2 cur = new vec2(
					r * Mathf.Cos(radian),
					r * Mathf.Sin(radian)
				);
				cur += centre;

				if (i == 0)
					first = cur; // 记录第一个采样点
				else if (i == sample) // 最后一次绘制时连接最后一个采样点与第一个采样点
					drawing.DrawLine(first, previous, c: c, width: width);
				else
					drawing.DrawLine(previous, cur, c: c, width: width);

				previous = cur;
			}
		}

		public static void DrawCurve(this IDebugDrawing drawing, System.Func<float, vec2> curve, int sample, bool isClosed = false, color? c = null, float width = -1)
		{
			vec2 first = default;
			vec2 previous = default;

			for (int i = 0; i <= sample; i++)
			{
				float t = Mathf.Clamp01(((float)i) / sample);

				vec2 cur = curve(t);

				if (i == 0)
					first = cur; // 记录第一个采样点
				else if (i == sample && isClosed) // 如果是封闭曲线, 连接末尾点到起点
					drawing.DrawLine(previous, first, c: c, width: width);
				else
					drawing.DrawLine(previous, cur, c: c, width: width);

				previous = cur;
			}
		}
	}
}