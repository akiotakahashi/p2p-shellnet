using System;
using System.Drawing;

namespace Shellnet {
	struct Vector {
		public double x;
		public double y;
		public Vector(double x, double y) {
			this.x = x;
			this.y = y;
		}
		public override bool Equals(object obj) {
			return this==(Vector)obj;
		}
		public override int GetHashCode() {
			return x.GetHashCode()+y.GetHashCode();
		}
		public double GetLength2() {
			return x*x+y*y;
		}
		public double GetLength() {
			return Math.Sqrt(GetLength2());
		}
		public void Normalize() {
			double l = Math.Sqrt(GetLength2());
			if(l==0) return;
			x /= l;
			y /= l;
		}
		public static Vector Zero = new Vector(0,0);
		public static Vector MinValue = new Vector(double.MinValue,double.MinValue);
		public static Vector MaxValue = new Vector(double.MaxValue,double.MaxValue);
		static Random rnd = new Random();
		public static Vector Random() {
			return new Vector(rnd.NextDouble()*2-1,rnd.NextDouble()*2-1);
		}
		public static bool operator ==(Vector v1, Vector v2) {
			return v1.x==v2.x && v1.y==v2.y;
		}
		public static bool operator !=(Vector v1, Vector v2) {
			return v1.x!=v2.x || v1.y!=v2.y;
		}
		public static Vector operator -(Vector v) {
			return new Vector(-v.x,-v.y);
		}
		public static Vector operator +(Vector v1, Vector v2) {
			return new Vector(v1.x+v2.x,v1.y+v2.y);
		}
		public static Vector operator -(Vector v1, Vector v2) {
			return new Vector(v1.x-v2.x,v1.y-v2.y);
		}
		public static double operator *(Vector v1, Vector v2) {
			return v1.x*v2.x + v1.y*v2.y;
		}
		public static Vector operator *(Vector v, double scale) {
			return new Vector(v.x*scale,v.y*scale);
		}
		public static Vector operator /(Vector v, double scale) {
			return new Vector(v.x/scale,v.y/scale);
		}
		public static implicit operator Point(Vector v) {
			return new Point((int)v.x,(int)v.y);
		}
		public static implicit operator PointF(Vector v) {
			return new PointF((float)v.x,(float)v.y);
		}
		public static implicit operator Size(Vector v) {
			return new Size((int)v.x,(int)v.y);
		}
		public static implicit operator SizeF(Vector v) {
			return new SizeF((float)v.x,(float)v.y);
		}
	}
}
