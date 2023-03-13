using System;
using System.Net;
using System.Collections;

namespace Shellnet {
	using RealSocket = System.Net.Sockets.Socket;
	class ExternalHost : Host {

		public readonly IPAddress ExternalAddress;
		public readonly Hashtable sockets = new Hashtable();

		public ExternalHost(Shellnet shellnet, IPAddress extip) : base(shellnet) {
			ExternalAddress = extip;
		}

		public Device CreateDevice(Segment segment, IPAddress[] addresses) {
			Device device = new ExternalDevice(this,segment);
			device.Addresses.AddRange(addresses);
			device.Register();
			return device;
		}

		public override Socket RequestConnect(System.Net.IPEndPoint ep, Socket socket) {
			RealSocket rs = new RealSocket(socket.AddressFamily,socket.SocketType,socket.ProtocolType);
			rs.Connect(new IPEndPoint(ExternalAddress,ep.Port));
			ExternalSocket es = new ExternalSocket(this
				,socket
				,rs
				,ep,socket.LocalEndPoint
				,this.GetDeviceByAddress(ep.Address)
				,socket.LocalDevice);
			sockets.Add(es,es);
			return es;
		}

	}
}
