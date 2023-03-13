using System;
using System.Net;
using System.Net.Sockets;

namespace Shellnet {
	public interface ISocket {
		int SocketId {get;}
		AddressFamily AddressFamily {get;}
		ProtocolType ProtocolType {get;}
		SocketType SocketType {get;}
		IPEndPoint LocalEndPoint {get;}
		IPEndPoint RemoteEndPoint {get;}
		int Available {get;}
		bool Blocking {get;}
		bool Connected {get;}
		void Close();
		ISocket Accept();
		bool Bind(IPEndPoint ep, ref int errcode);
		bool Connect(IPEndPoint ep, ref int errcode);
		bool Listen(int backlog, ref int errcode);
		int Send(byte[] buf);
		byte[] Receive(int bufsize);
		void Shutdown(SocketShutdown how);
		bool AsyncSelect(IntPtr hWnd, uint wMsg, int lEvent, IntPtr fpPostMsg, IntPtr s, ref int errno);
	}
}
