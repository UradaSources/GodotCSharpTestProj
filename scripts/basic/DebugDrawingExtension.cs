using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using Godot;

namespace urd
{
	public static class DebugDrawingExtension
	{
		public static void DrawLine(this CanvasItem self, vec2 a, vec2 b, color? c = null, float width = -1)
		{
			var _c = c ?? color.white;
			self.DrawLine(new Vector2(a.x, a.y), new Vector2(b.x, b.y), new Color((float)_c.r/255, (float)_c.g/255, (float)_c.b/255, (float)_c.a/255));
		}

		public static void DrawMark(CanvasItem self, vec2 pos, float size = 10.0f, color? c = null, float width = -1)
		{
			vec2 xOffset = size * vec2.right * 0.5f;
			vec2 yOffset = size * vec2.up * 0.5f;

			self.DrawLine(pos - xOffset, pos + xOffset, c, width);
			self.DrawLine(pos - yOffset, pos + yOffset, c, width);
		}

		//public static void DrawArrow(ILineDrawing self, vec2 pos, vec2 vec, color? c = null, float width = -1)
		//{
		//	const float ArrowHeadAngle = 15.0f;

		//	DebugDrawingExtension.DrawRay(self, pos, vec, c, width);

		//	var left = (vec * -1).normalized;
		//	var right = (vec * -1).normalized;

		//	left = Quaternion.AngleAxis(Vector2.Angle(vec, Vector2.zero) + ArrowHeadAngle, Vector3.forward) * left;
		//	right = Quaternion.AngleAxis(Vector2.Angle(vec, Vector2.zero) - ArrowHeadAngle, Vector3.forward) * right;

		//	var scale = MiscUtils.EditorGizmoScale(pos);
		//	param.size *= scale;

		//	DebugUtils.DrawRay(pos + vec, left * param.size, args);
		//	DebugUtils.DrawRay(pos + vec, right * param.size, args);
		//}

		//public static void DrawArrowBetween(ILineDrawing self, vec2 start, vec2 target, color? c = null, float width = -1)
		//{
		//}

		public static void DrawRay(CanvasItem self, vec2 pos, vec2 dir, color? c = null, float width = -1)
		{
			self.DrawLine(pos, pos + dir, c: c, width: width);
		}

		public static void DrawLines(CanvasItem self, IEnumerable<vec2> points, bool isClosed = false, bool markPoint = false, color? c = null, float width = -1)
		{
			bool recordFirst = false;
			vec2 first = default;

			vec2 previous = default;

			foreach (var cur in points)
			{
				if (markPoint)
					DebugDrawingExtension.DrawMark(self, cur, c: c, width: width);

				if (!recordFirst)
				{
					first = cur;
					recordFirst = true;
				}
				else
					self.DrawLine(previous, cur, c: c, width: width);

				previous = cur;
			}

			if (isClosed)
				self.DrawLine(previous, first, c: c, width: width);
		}

		public static void DrawBox(CanvasItem self, vec2 pos, vec2 size, color? c = null, float width = -1)
		{
			float w = size.x * 0.5f;
			float h = size.y * 0.5f;

			var p1 = pos + new vec2(-w, h);
			var p2 = pos + new vec2(w, h);
			var p3 = pos + new vec2(w, -h);
			var p4 = pos + new vec2(-w, -h);

			self.DrawLine(p1, p2, c: c, width: width);
			self.DrawLine(p2, p3, c: c, width: width);
			self.DrawLine(p3, p4, c: c, width: width);
			self.DrawLine(p4, p1, c: c, width: width);
		}

		public static void DrawRange(CanvasItem self, vec2 min, vec2 max, color? c = null, float width = -1)
		{
			DebugDrawingExtension.DrawBox(self, vec2.Lerp(min, max, 0.5f), (max - min), c: c, width: width);

			DebugDrawingExtension.DrawMark(self, min, c: c, width: width);
			DebugDrawingExtension.DrawMark(self, max, c: c, width: width);
		}

		public static void DrawCircle(CanvasItem self, vec2 centre, float r, int sample = 32, color? c = null, float width = -1)
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
					self.DrawLine(first, previous, c: c, width: width);
				else
					self.DrawLine(previous, cur, c: c, width: width);

				previous = cur;
			}
		}

		public static void DrawCurve(CanvasItem self, System.Func<float, vec2> curve, int sample, bool isClosed = false, color? c = null, float width = -1)
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
					self.DrawLine(previous, first, c: c, width: width);
				else
					self.DrawLine(previous, cur, c: c, width: width);

				previous = cur;
			}
		}
	}
}