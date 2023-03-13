using System;
using System.Net;
using System.Net.Sockets;

namespace Shellnet {
	public interface IPeer {
		int PeerId {get;}
		IHost Host {get;}
		int Cleanup();
		System.IO.TextWriter Log {get;}
		ISocket CreateSocket(AddressFamily af, SocketType st, ProtocolType pt);
		int Select(ref ISocket[] read, ref ISocket[] write, ref ISocket[] except, int microSeconds);
	}
}
