using System;
using System.Net;

namespace Shellnet {
	class LoopbackDevice : Device {
		public LoopbackDevice(Host host) : base(host,null) {
			Addresses.Add(IPAddress.Loopback);
			//Addresses.Add(IPAddress.IPv6Loopback);
		}
		public override bool External {
			get {
				return false;
			}
		}

	}
}
