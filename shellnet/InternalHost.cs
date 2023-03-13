using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;

using System.Threading;

namespace Shellnet {
	class InternalHost : Host {

		public readonly Device LoopbackDevice;

		private readonly Hashtable portmap = new Hashtable();

		public InternalHost(Shellnet shellnet) : base(shellnet) {
			LoopbackDevice = new LoopbackDevice(this);
			LoopbackDevice.Register();
		}

		internal void Unregister(Socket socket) {
			if(socket.LocalEndPoint==null || socket.LocalEndPoint.Port==0) return;
			lock(portmap.SyncRoot) {
				if(!portmap.ContainsKey(socket.LocalEndPoint.Port)) return;
				PORTDESC pd = (PORTDESC)portmap[socket.LocalEndPoint.Port];
				if(pd.sockets!=null) {
					lock(pd.sockets.SyncRoot) {
						pd.sockets.Remove(socket);
						if(pd.sockets.Count==0) {
							pd.sockets = null;
						}
					}
				}
				if(pd.listening==socket) {
					pd.listening = null;
					pd.backlog = null;
					pd.signal = null;
				}
				portmap[socket.LocalEndPoint.Port] = pd;
			}
		}

		public Device GetBindingDevice(IPAddress address) {
			foreach(Device device in Devices) {
				if(device.Addresses.Contains(address)) {
					return device;
				}
			}
			return null;
		}

		int portseq = 1024;

		/// <summary>
		/// listenまたはconnect前のソケットとローカルアドレスを関連付けます。
		/// </summary>
		/// <param name="peer"></param>
		/// <param name="socket"></param>
		/// <param name="ep"></param>
		/// <returns></returns>
		/*
		public void BindSocket(InternalSocket socket, IPEndPoint ep) {
			int port = ep.Port;
			if(port==0) {
				port = portseq;
				while(portmap.ContainsKey(port)) {
					++port;
					if(port>5000) port=1024;
					if(port==portseq) {
						throw new OverflowException("空きポートがありません。");
					}
				}
				portseq = port+1;
			}
			PORTDESC pd;
			lock(portmap.SyncRoot) {
				if(portmap.ContainsKey(port)) {
					pd = (PORTDESC)portmap[port];
				} else {
					pd.listening = null;
					pd.backlog = null;
					pd.signal = null;
					pd.sockets = null;
				}
				if(pd.sockets==null) {
					pd.sockets = new ArrayList();
					portmap[port] = pd;
				}
			}
			pd.sockets.Add(socket);
			socket.SetLocalEndPoint(ep.Address,port);
		}
		*/
		public IPEndPoint AssociateEndPointWithSocket(IPEndPoint ep, Socket socket) {
			//TODO: 挙動違う。。。
			bool portallocation = ep.Port==0;
			int port = ep.Port;
			if(port==0) {
				port = portseq;
				while(portmap.ContainsKey(port)) {
					++port;
					if(port>5000) port=1024;
					if(port==portseq) {
						throw new SocketException(WSAE.ADDRINUSE);
					}
				}
				portseq = port+1;
			}
			if(!portallocation) {
				PORTDESC pd;
				lock(portmap.SyncRoot) {
					if(portmap.ContainsKey(port)) {
						pd = (PORTDESC)portmap[port];
					} else {
						pd.listening = null;
						pd.backlog = null;
						pd.signal = null;
						pd.sockets = null;
					}
					if(pd.sockets==null) {
						pd.sockets = new ArrayList();
						portmap[port] = pd;
					}
				}
				pd.sockets.Add(socket);
			}
			return new IPEndPoint(ep.Address,port);
		}
		public void ReleaseEndPoint(IPEndPoint ep) {
			lock(portmap.SyncRoot) {
				if(portmap.ContainsKey(ep.Port)) {
					portmap.Remove(ep.Port);
				}
			}
		}

		public void ListenSocket(Socket socket, int backlog) {
			if(socket.LocalEndPoint==null) throw new Exception("待機しようとするソケットは結合されていません。");
			int port = socket.LocalEndPoint.Port;
			lock(portmap.SyncRoot) {
				PORTDESC pd;
				if(portmap.ContainsKey(port)) {
					pd = (PORTDESC)portmap[port];
				} else {
					pd.listening = null;
					pd.backlog = null;
					pd.signal = null;
					pd.sockets = null;
				}
				if(pd.listening!=null) {
					throw new Exception("指定されたポートで待機するソケットが存在します。");
				} else {
					pd.listening = socket;
					pd.backlog = new ArrayList();
					pd.signal = new AutoResetEvent(false);
					pd.sockets = new ArrayList();
					portmap[port] = pd;
				}
			}
		}

		public SocketBase EstablishRoute(Socket socket, IPEndPoint ep) {
			Device device = DetermineDevice(ep.Address);
			Device device2 = device.Segment.QueryDevice(ep.Address);
			if(device2==null) throw new Exception("指定されたホストは存在しません");
			SocketBase socket2 = device.Segment.TranslateSocket(socket,ep);
			return device2.Segment.AcceptSocket(socket2,ep);
		}

		public Socket ConnectSocket(InternalSocket socket, IPEndPoint ep) {
			if(socket.LocalEndPoint==null) {
				socket.Bind(new IPEndPoint(GetAccessibleIPAddress(ep.Address),0));
			}
			SocketBase visitor = EstablishRoute(socket,ep);
			Device remoteDevice = Shellnet.GetDeviceByAddress(ep.Address);
			if(remoteDevice==null) throw new Exception("指定されたホストは存在しません");
			PORTDESC pd;
			lock(portmap.SyncRoot) {
				if(portmap.ContainsKey(socket.LocalEndPoint.Port)) {
					pd = (PORTDESC)portmap[socket.LocalEndPoint.Port];
				} else {
					pd.listening = null;
					pd.backlog = null;
					pd.signal = null;
					pd.sockets = null;
				}
				if(pd.sockets==null) {
					pd.sockets = new ArrayList();
					portmap[socket.LocalEndPoint.Port] = pd;
				}
			}
			Socket connected = remoteDevice.RequestConnect(ep,socket);
			lock(pd.sockets.SyncRoot) pd.sockets.Add(socket);
			return connected;
		}

		public override Socket RequestConnect(IPEndPoint ep, Socket socket) {
			PORTDESC pd;
			lock(portmap.SyncRoot) {
				if(!portmap.ContainsKey(ep.Port)) {
					throw new Exception("指定されたポートでは待機していません。");
				} else {
					pd = (PORTDESC)portmap[ep.Port];
				}
			}
			if(pd.listening==null) {
				throw new Exception("指定されたポートのソケットは待機していません。");
			} else {
				return pd.listening.RequestConnect(socket);
			}
		}

		public Device CreateDevice(Segment segment, IPAddress[] addresses) {
			Device device = new InternalDevice(this,segment);
			device.Addresses.AddRange(addresses);
			device.Register();
			return device;
		}

		private struct PORTDESC {
			public Socket listening;
			public ArrayList backlog;
			public AutoResetEvent signal;
			public ArrayList sockets;
		}

		int nUnclosed = 0;

		public void IncrementUnclosed() {
			nUnclosed++;
		}

		public int NumberOfUnclosed {
			get {
				return nUnclosed;
			}
		}

	}
}
