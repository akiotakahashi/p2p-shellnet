using System;

namespace Shellnet {
	class ExternalDevice : Device {
		public ExternalDevice(ExternalHost host, Segment segment) : base(host,segment) {
		}
	}
}
