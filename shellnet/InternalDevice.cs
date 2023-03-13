using System;

namespace Shellnet {
	class InternalDevice : Device {
		public InternalDevice(InternalHost host, Segment segment) : base(host,segment) {
		}
	}
}
