using System;
using System.Net;
using System.Net.Sockets;

namespace Shellnet {
	public interface IHost {
		short hton(short hostshort);
		short ntoh(short netshort);
		ushort hton(ushort hostshort);
		ushort ntoh(ushort netshort);
		int hton(int hostint);
		int ntoh(int netint);
		uint hton(uint hostint);
		uint ntoh(uint netint);
		long hton(long hostlong);
		long ntoh(long netlong);
		ulong hton(ulong hostlong);
		ulong ntoh(ulong netlong);
		string HostName {get;}
		IPAddress GetIPAddress(long address);
		IPAddress GetIPAddress(byte[] address, long scopeid);
		IPAddress StringToAddress(string str, AddressFamily af);
		string AddressToString(IPAddress addr);
		IPHostEntry GetHostByName(string hostName);
		IPHostEntry GetHostByAddress(IPAddress address);
	}
}
