using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Shellnet {
	public interface IShellnet {
		IPeer Startup(int pid, string keepaliveMutexName);
		long inet_addr(string address);
		int _EnumProtocolsA(int[] iProtocols, byte[] protocolBuffer, ref uint pLength);
		int _EnumProtocolsW(int[] iProtocols, byte[] protocolBuffer, ref uint pLength);
	}
}
