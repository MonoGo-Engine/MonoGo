using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace MonoGo.Engine.Utils
{
	/// <summary>
	/// Contains useful math stuff. 
	/// </summary>
	public static class GameMath
	{
		#region Math.

		public static double Lerp(double a, double b, double value) =>
			a + (b - a) * value;

		#endregion Math.
		#region Distance.

		/// <summary>
		/// Calculates distance between two points.
		/// </summary>
		public static float Distance(Vector2 p1, Vector2 p2) =>
			(p2 - p1).Length();

		#endregion Distance.
		#region Intersestions.

		/// <summary>
		/// Checks if a point lies within a rectangle.
		/// </summary>
		public static bool PointInRectangle(Vector2 point, Vector2 rectPoint1, Vector2 rectPoint2) =>
			point.X >= rectPoint1.X && point.X <= rectPoint2.X && point.Y >= rectPoint1.Y && point.Y <= rectPoint2.Y;

		/// <summary>
		/// Checks if a point lies within a rectangle.
		/// </summary>
		public static bool PointInRectangleBySize(Vector2 point, Vector2 rectCenter, Vector2 rectSize)
		{
			var rectHalfSize = rectSize / 2f;
			var pt1 = rectCenter - rectHalfSize;
			var pt2 = rectCenter + rectHalfSize;
			return point.X >= pt1.X && point.X <= pt2.X && point.Y >= pt1.Y && point.Y <= pt2.Y;
		}

		/// <summary>
		/// Checks if a point lies within a triangle.
		/// </summary>
		public static bool PointInTriangle(Vector2 point, Vector2 triPoint1, Vector2 triPoint2, Vector2 triPoint3)
		{
			Vector2 p = point - triPoint1;
			Vector2 v2 = triPoint2 - triPoint1;
			Vector2 v3 = triPoint3 - triPoint1;

			float w1 = (triPoint1.X * v3.Y - point.X * v3.Y + p.Y * v3.X) / (v2.Y * v3.X - v2.X * v3.Y);
			float w2 = (point.Y - triPoint1.Y - w1 * v2.Y) / v3.Y;

			return (w1 >= 0 && w2 >= 0 && ((w1 + w2) <= 1));
		}

		/// <summary>
		/// Determines whether a specified point lies within a polygon defined by a list of vertices.
		/// </summary>
		/// <remarks>The polygon is assumed to be non-self-intersecting and defined by its vertices in clockwise or
		/// counterclockwise order. This method uses the ray-casting algorithm to determine whether the point is inside the
		/// polygon.</remarks>
		/// <param name="point">The point to test, represented as a <see cref="Vector2"/>.</param>
		/// <param name="polygonPoints">A list of <see cref="Vector2"/> representing the vertices of the polygon in order.</param>
		/// <returns><see langword="true"/> if the point is inside the polygon; otherwise, <see langword="false"/>.</returns>
		public static bool PointInPolygon(Vector2 point, List<Vector2> polygonPoints)
		{
			bool inside = false;
			int count = polygonPoints.Count;

			for (int i = 0, j = count - 1; i < count; j = i++)
			{
				Vector2 pi = polygonPoints[i];
				Vector2 pj = polygonPoints[j];

				// Check if the horizontal line of a point crosses an edge of the polygon.
				bool intersects = ((pi.Y > point.Y) != (pj.Y > point.Y)) &&
								  (point.X < (pj.X - pi.X) * (point.Y - pi.Y) / (pj.Y - pi.Y + float.Epsilon) + pi.X);

				if (intersects) inside = !inside;
			}
			return inside;
		}

		/// <summary>
		/// Checks if two rectangles intersect.
		/// </summary>
		public static bool RectangleInRectangle(Vector2 rect1Pt1, Vector2 rect1Pt2, Vector2 rect2Pt1, Vector2 rect2Pt2) =>
			rect1Pt1.X <= rect2Pt2.X && rect1Pt2.X >= rect2Pt1.X && rect1Pt1.Y <= rect2Pt2.Y && rect1Pt2.Y >= rect2Pt1.Y;

		/// <summary>
		/// Checks if two rectangles intersect.
		/// </summary>
		public static bool RectangleInRectangleBySize(Vector2 rect1Center, Vector2 rect1Size, Vector2 rect2Center, Vector2 rect2Size)
		{
			var delta = rect2Center - rect1Center;
			var size = (rect2Size + rect1Size) / 2f;

			return Math.Abs(delta.X) <= size.X && Math.Abs(delta.Y) <= size.Y;
		}

		/// <summary>
		/// Calculates on which side point is from a line. 
		/// 1 - left
		/// -1 - right
		/// 0 - on the line
		/// </summary>
		public static int PointSide(Vector2 point, Vector2 linePt1, Vector2 linePt2)
		{
			var v = new Vector2(linePt2.Y - linePt1.Y, linePt1.X - linePt2.X);

			return Math.Sign(Vector2.Dot(point - linePt1, v));
		}

		/// <summary>
		/// Determines the relative position of a point with respect to a line segment.
		/// </summary>
		/// <param name="objectPosition">The position of the point to evaluate.</param>
		/// <param name="linePt1">The starting point of the line segment.</param>
		/// <param name="linePt2">The ending point of the line segment.</param>
		/// <returns>A value between 0 and 1 indicating the relative position of <paramref name="objectPosition"/>  with respect to the
		/// line segment defined by <paramref name="linePt1"/> and <paramref name="linePt2"/>. A return value of 1 indicates
		/// that the point is outside the line segment in the direction of the line, while a value closer to 0 indicates
		/// proximity to the line segment.</returns>
        public static float PointOutside(Vector2 objectPosition, Vector2 linePt1, Vector2 linePt2)
        {
            // Check if objectPosition is outside of pos2.
            Vector2 lineVector = linePt2 - linePt1;
            Vector2 objectVector = objectPosition - linePt2;

            // If dot-product is positive, objectPosition is outside (in direction of the line).
            if (Vector2.Dot(lineVector, objectVector) > 0)
                return 1f;

            // Max distance between the two points.
            float distanceMax = Vector2.Distance(linePt1, linePt2);

            // Distance from pos2 to the objectPosition.
            float distance = Vector2.Distance(linePt2, objectPosition);

            // Normalize distance (0.0 bis 1.0).
            float normalized = distance / distanceMax;

            // Clamp to [0,1] and invert the result (1.0 - Value).
            return 1f - Math.Clamp(normalized, 0f, 1f);
        }

        /// <summary>
        /// Checks if two lines cross. Returns 1 if lines cross, 0 if not and 2 if lines overlap.
        /// </summary>
        public static int LinesCross(Vector2 line1Pt1, Vector2 line1Pt2, Vector2 line2Pt1, Vector2 line2Pt2)
		{
			var line1 = new Vector2(line1Pt2.Y - line1Pt1.Y, line1Pt1.X - line1Pt2.X);
			var line2 = new Vector2(line2Pt2.Y - line2Pt1.Y, line2Pt1.X - line2Pt2.X);

			var side1 = Math.Sign(Vector2.Dot(line2Pt1 - line1Pt1, line1));
			var side2 = Math.Sign(Vector2.Dot(line2Pt2 - line1Pt1, line1));
			var side3 = Math.Sign(Vector2.Dot(line1Pt1 - line2Pt1, line2));
			var side4 = Math.Sign(Vector2.Dot(line1Pt2 - line2Pt1, line2));

			if (side1 != side2 && side3 != side4)
			{
				return 1;
			}

			if (side1 == 0 && side2 == 0)
			{
				return 2;
			}

			return 0;
		}

		/// <summary>
		/// Checks if two linew cross. Returns 1 if lines cross, 0 if not and 2 if lines overlap.
		/// Also calculates intersection point.
		/// </summary>
		public static int LinesCross(Vector2 line1Pt1, Vector2 line1Pt2, Vector2 line2Pt1, Vector2 line2Pt2, ref Vector2 collisionPt)
		{
			int result = LinesCross(line1Pt1, line1Pt2, line2Pt1, line2Pt2);
			if (result == 1)
			{
				Vector2 e1 = line1Pt2 - line1Pt1;
				e1.Normalize();
				Vector2 e2 = line2Pt2 - line2Pt1;
				e2.Normalize();

				// A bit of unpleasant math.
				// The base idea is:
				// e1 * l1 + v1 = e2 * l2 + v2
				// So we need to calculate l1 or l2 and plop it into vector function.

				// Formula will probably break with parallel lines.
				// (result == 1) prevents it, but just keep this in mind.
				float l = (e1.X * (line1Pt1.Y - line2Pt1.Y) + e1.Y * (line2Pt1.X - line1Pt1.X)) / (e1.X * e2.Y - e2.X * e1.Y);

				collisionPt = line2Pt1 + e2 * l;
			}
			return result;
		}

		#endregion Intersections.

		/// <summary>
		/// Indicates if the vertices are in clockwise order.
		/// Warning: If the area of the polygon is 0, it is unable to determine the winding.
		/// </summary>
		public static bool IsClockWise(List<Vector2> vertices)
		{
			//The simplest polygon which can exist in the Euclidean plane has 3 sides.
			if (vertices.Count < 3)
				return false;

			return (GetSignedArea(vertices) > 0.0f);
		}

		/// <summary>
		/// Gets the signed area.
		/// If the area is less than 0, it indicates that the polygon is couter clockwise winded.
		/// </summary>
		/// <returns>The signed area</returns>
		public static float GetSignedArea(List<Vector2> vertices)
		{
			//The simplest polygon which can exist in the Euclidean plane has 3 sides.
			if (vertices.Count < 3)
				return 0;

			int i;
			float area = 0;

			for (i = 0; i < vertices.Count; i++)
			{
				int j = (i + 1) % vertices.Count;

				Vector2 vi = vertices[i];
				Vector2 vj = vertices[j];

				area += vi.X * vj.Y;
				area -= vi.Y * vj.X;
			}
			area /= 2.0f;
			return area;
		}

		/// <summary>
		/// Gets the area.
		/// </summary>
		/// <returns></returns>
		public static float GetArea(List<Vector2> vertices)
		{
			float area = GetSignedArea(vertices);
			return (area < 0 ? -area : area);
		}
	}
}