using System;
using System.Drawing;

namespace Shellnet {
	struct Boundary {
		public Vector Location;
		public Vector Size;
		public Boundary(Vector location, Vector size) {
			this.Location = location;
			this.Size = size;
		}
		public Vector Center {
			get {
				return Location+(Size/2);
			}
		}
		public bool Contains(Vector point) {
			return Location.x<=point.x
				&& Location.y<=point.y
				&& point.x<Location.x+Size.x
				&& point.y<Location.y+Size.y;
		}
		public Boundary Offset(Vector offset) {
			return new Boundary(Location+offset,Size);
		}
		public static implicit operator Rectangle(Boundary b) {
			return new Rectangle(b.Location,b.Size);
		}
	}
}
