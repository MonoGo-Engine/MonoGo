using Microsoft.Xna.Framework;
using System;

namespace MonoGo.Engine
{
	/// <summary>
	/// Represents an angle measured in degrees. 
	/// NOTE: Angle will auto-wrap in 0..359 range.
	/// </summary>
	public struct Angle : IEquatable<Angle>
	{
		public static readonly Angle Up = new Angle(270);
		public static readonly Angle Down = new Angle(90);
		public static readonly Angle Left = new Angle(180);
		public static readonly Angle Right = new Angle(0);

		public double Degrees 
		{
			get => _degrees; 
			set
			{
				_degrees = value;
				Normalize();
			}
		}

		public double Radians 
		{
			get => (_degrees / 360.0) * Math.PI * 2.0;
			set 
			{
				Degrees = (value / (Math.PI * 2.0)) * 360.0;
			}
		}

		public float DegreesF => (float)Degrees;
		public float RadiansF => (float)Radians;		

		private double _degrees;

		public Angle(double degrees)
		{
			_degrees = degrees;
			Normalize();
		}

		public Angle(Vector2 vector)
		{
			_degrees = (Math.Atan2(vector.Y, vector.X) * 360.0) / (Math.PI * 2.0);
			Normalize();
		}

		public Angle(Vector2 point1, Vector2 point2) : this(point2 - point1) {}

		/// <summary>
		/// Creates Angle object from an angle measured in radians.
		/// </summary>
		public static Angle FromRadians(double radians)
		{
			var angle = new Angle();
			angle.Radians = radians;
			return angle;
		}

		/// <summary>
		/// Returns the difference between two angles in the range of -180..180.
		/// Negative angle represents going counter-clockwise, 
		/// positive - clockwise.
		/// </summary>
		public double Difference(Angle other) =>
			((Degrees - other.Degrees + 180.0) % 360.0 + 360.0) % 360.0 - 180.0;

		/// <summary>
		/// Checks if this angle is within a range around another angle.
		/// </summary>
		/// <param name="targetAngle">The target angle to check against</param>
		/// <param name="tolerance">The tolerance in degrees (half-width of the range)</param>
		/// <returns>True if this angle is within the tolerance range of the target angle</returns>
		public bool IsWithinRange(Angle targetAngle, double tolerance) =>
			Math.Abs(Difference(targetAngle)) <= tolerance;

		/// <summary>
		/// Checks if this angle is approximately pointing Right (0°).
		/// Default range: 315° to 45° (90° total range, ±45° from 0°)
		/// </summary>
		/// <param name="tolerance">The tolerance in degrees from the Right direction (default: 45°)</param>
		/// <returns>True if the angle is approximately pointing Right</returns>
		public bool IsApproximatelyRight(double tolerance = 45.0) =>
			IsWithinRange(Right, tolerance);

		/// <summary>
		/// Checks if this angle is approximately pointing Down (90°).
		/// Default range: 45° to 135° (90° total range, ±45° from 90°)
		/// </summary>
		/// <param name="tolerance">The tolerance in degrees from the Down direction (default: 45°)</param>
		/// <returns>True if the angle is approximately pointing Down</returns>
		public bool IsApproximatelyDown(double tolerance = 45.0) =>
			IsWithinRange(Down, tolerance);

		/// <summary>
		/// Checks if this angle is approximately pointing Left (180°).
		/// Default range: 135° to 225° (90° total range, ±45° from 180°)
		/// </summary>
		/// <param name="tolerance">The tolerance in degrees from the Left direction (default: 45°)</param>
		/// <returns>True if the angle is approximately pointing Left</returns>
		public bool IsApproximatelyLeft(double tolerance = 45.0) =>
			IsWithinRange(Left, tolerance);

		/// <summary>
		/// Checks if this angle is approximately pointing Up (270°).
		/// Default range: 225° to 315° (90° total range, ±45° from 270°)
		/// </summary>
		/// <param name="tolerance">The tolerance in degrees from the Up direction (default: 45°)</param>
		/// <returns>True if the angle is approximately pointing Up</returns>
		public bool IsApproximatelyUp(double tolerance = 45.0) =>
			IsWithinRange(Up, tolerance);
				
		public Vector2 ToVector2() =>
			new Vector2((float)Math.Cos(Radians), (float)Math.Sin(Radians));
	
		public static Angle Lerp(Angle angle1, Angle angle2, double value)
		{
			var diff = angle1.Difference(angle2);
			return angle1 - diff * value;
		}

		public static double ToDegrees(double radians) =>
			(radians / (Math.PI * 2.0)) * 360.0;

		public static double ToRadians(double degrees) =>
			(degrees / 360.0) * Math.PI * 2.0;

		public bool Equals(Angle other) =>
			_degrees == other._degrees;

		public override bool Equals(object obj)
		{
			if (obj is Angle)
			{
				return Equals(this);
			}
			return false;
		}

		#region Operators.

		public static Angle operator +(Angle a1, Angle a2) =>
			new Angle(a1._degrees + a2._degrees);

		public static Angle operator +(Angle a, double num) =>
			new Angle(a._degrees + num);

		public static Angle operator +(Angle a, float num) =>
			new Angle(a._degrees + num);

		public static Angle operator +(Angle a, int num) =>
			new Angle(a._degrees + num);

		public static Angle operator +(double num, Angle a) =>
			new Angle(a._degrees + num);

		public static Angle operator +(float num, Angle a) =>
			new Angle(a._degrees + num);

		public static Angle operator +(int num, Angle a) =>
			new Angle(a._degrees + num);

		public static Angle operator -(Angle a1, Angle a2) =>
			new Angle(a1._degrees - a2._degrees);

		public static Angle operator -(Angle a, double num) =>
			new Angle(a._degrees - num);

		public static Angle operator -(Angle a, float num) =>
			new Angle(a._degrees - num);

		public static Angle operator -(Angle a, int num) =>
			new Angle(a._degrees - num);

		public static Angle operator -(double num, Angle a) =>
			new Angle(num - a._degrees);

		public static Angle operator -(float num, Angle a) =>
			new Angle(num - a._degrees);

		public static Angle operator -(int num, Angle a) =>
			new Angle(num - a._degrees);

		public static Angle operator *(Angle a, double num) =>
			new Angle(a._degrees * num);

		public static Angle operator *(Angle a, float num) =>
			new Angle(a._degrees * num);

		public static Angle operator *(Angle a, int num) =>
			new Angle(a._degrees * num);

		public static Angle operator *(double num, Angle a) =>
			new Angle(a._degrees * num);

		public static Angle operator *(float num, Angle a) =>
			new Angle(a._degrees * num);

		public static Angle operator *(int num, Angle a) =>
			new Angle(a._degrees * num);

		public static Angle operator /(Angle a, double num) =>
			new Angle(a._degrees / num);

		public static Angle operator /(Angle a, float num) =>
			new Angle(a._degrees / num);

		public static Angle operator /(Angle a, int num) =>
			new Angle(a._degrees / num);

		public static Angle operator /(double num, Angle a) =>
			new Angle(num / a._degrees);

		public static Angle operator /(float num, Angle a) =>
			new Angle(num / a._degrees);

		public static Angle operator /(int num, Angle a) =>
			new Angle(num / a._degrees);

		public static bool operator >(Angle a1, Angle a2) =>
			a1._degrees > a2._degrees;

		public static bool operator <(Angle a1, Angle a2) =>
			a1._degrees < a2._degrees;

		public static bool operator >=(Angle a1, Angle a2) =>
			a1._degrees >= a2._degrees;

		public static bool operator <=(Angle a1, Angle a2) =>
			a1._degrees <= a2._degrees;

		public static bool operator ==(Angle a1, Angle a2) =>
			a1._degrees == a2._degrees;

		public static bool operator !=(Angle a1, Angle a2) =>
			a1._degrees != a2._degrees;

        public static implicit operator Angle(Vector2 value) => new Angle(value);
        public static implicit operator Angle((float x, float y) value) => new Angle(new Vector2(value.x, value.y));
        public static implicit operator Angle(double degrees) => new Angle(degrees);

        public static explicit operator Vector2(Angle angle) => angle.ToVector2();

        #endregion Operators.

        public override int GetHashCode() =>
			_degrees.GetHashCode();

		public override string ToString() =>
			_degrees.ToString();
		
		void Normalize() =>
			_degrees = (_degrees % 360.0 + 360.0) % 360.0;
	}
}
