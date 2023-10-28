using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using Godot;

namespace urd
{
	public interface BasicLineRenderer
	{
		void renderLine(vec2 a, vec2 b, color? c = null, float width = -1);
	}

	public static class DebugDrawingExtension
	{
		public static BasicLineRenderer LineRenderer { set; get; }

		public static void DrawLine(this vec2 a, vec2 b, color? c = null, float width = -1)
		{
			LineRenderer.renderLine(a, b, c, width);
		}

		public static void DrawMark(vec2 pos, float size = 10.0f, color? c = null, float width = -1)
		{
			vec2 xOffset = size * vec2.right * 0.5f;
			vec2 yOffset = size * vec2.up * 0.5f;

			LineRenderer.renderLine(pos - xOffset, pos + xOffset, c, width);
			LineRenderer.renderLine(pos - yOffset, pos + yOffset, c, width);
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

		public static void DrawRay(vec2 pos, vec2 dir, color? c = null, float width = -1)
		{
			LineRenderer.renderLine(pos, pos + dir, c: c, width: width);
		}

		public static void DrawLines(IEnumerable<vec2> points, bool isClosed = false, bool markPoint = false, color? c = null, float width = -1)
		{
			bool recordFirst = false;
			vec2 first = default;

			vec2 previous = default;

			foreach (var cur in points)
			{
				if (markPoint)
					DebugDrawingExtension.DrawMark(cur, c: c, width: width);

				if (!recordFirst)
				{
					first = cur;
					recordFirst = true;
				}
				else
					LineRenderer.renderLine(previous, cur, c: c, width: width);

				previous = cur;
			}

			if (isClosed) LineRenderer.renderLine(previous, first, c: c, width: width);
		}

		public static void DrawBox(vec2 pos, vec2 size, color? c = null, float width = -1)
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

		public static void DrawRange(vec2 min, vec2 max, color? c = null, float width = -1)
		{
			DebugDrawingExtension.DrawBox(self, vec2.Lerp(min, max, 0.5f), (max - min), c: c, width: width);

			DebugDrawingExtension.DrawMark(self, min, c: c, width: width);
			DebugDrawingExtension.DrawMark(self, max, c: c, width: width);
		}

		public static void DrawCircle(vec2 centre, float r, int sample = 32, color? c = null, float width = -1)
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

		public static void DrawCurve(System.Func<float, vec2> curve, int sample, bool isClosed = false, color? c = null, float width = -1)
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