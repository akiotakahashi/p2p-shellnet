using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;

namespace Shellnet {
	class InternalSocket : Socket, ISocket {

		static private int nextSocketId = 1;
		public readonly int socketId = -1;
		private readonly StreamBuffer buffer = new StreamBuffer();

		public readonly Peer Peer;

		public InternalSocket(Peer peer, AddressFamily af, SocketType st, ProtocolType pt, int id) : base(af,st,pt) {
			Peer = peer;
			socketId = nextSocketId++;
			Peer.Register(this);
		}

		public InternalSocket(InternalSocket listening, Socket incoming) : base(listening.AddressFamily,listening.SocketType,listening.ProtocolType) {
			Peer = listening.Peer;
			this.lep = new IPEndPoint(Peer.Host.GetAccessibleIPAddress(listening.LocalEndPoint.Address),listening.LocalEndPoint.Port);
			this.ldev = Peer.Host.GetDeviceByAddress(lep.Address);
			this.symmetricSocket = incoming;
			this.rep = incoming.LocalEndPoint;
			this.rdev = incoming.LocalDevice;
			socketId = nextSocketId++;
			state = SocketState.CONNECT;
			session = incoming.Session;
			Peer.Register(this);
		}

		public override string ToString() {
			return "is#"+socketId;
		}

		#region IDisposable メンバ

		protected override void Disposing() {
			if(!this.shutdownWriting || !this.shutdownReading) {
				Peer.IncrementNongraceful();
			}
			if(state!=SocketState.CLOSED) {
				Peer.Host.IncrementUnclosed();
				Close();
			}
			Peer.Unregister(this);
			base.Disposing();
		}

		#endregion
	
		#region ISocket メンバ

		public int SocketId {
			get {
				return socketId;
			}
		}

		private ArrayList waitForAccept = new ArrayList();
		private ArrayList waitForConnect = new ArrayList();

		class HandshakeStatus {
			public AutoResetEvent ReadyToAccept = new AutoResetEvent(false);
			public AutoResetEvent ReadyToConnect = new AutoResetEvent(false);
			public volatile Socket ConnectingSocket = null;
			public volatile InternalSocket AcceptingSocket = null;
		}

		public ISocket Accept() {
			if(Peer.SubsystemDown) return null;
			if(this.connectionreset) return null;
			try {
				HandshakeStatus status;
				lock(this) {
					if(waitForAccept.Count>0) {
						status = (HandshakeStatus)waitForAccept[0];
						waitForAccept.RemoveAt(0);
					} else {
						status = new HandshakeStatus();
						waitForConnect.Add(status);
					}
				}
				status.ReadyToAccept.Set();
				status.ReadyToConnect.WaitOne();
				status.AcceptingSocket = new InternalSocket(this,status.ConnectingSocket);
				status.ReadyToAccept.Set();
				status.ReadyToConnect.WaitOne();
				Log("accept");
				return status.AcceptingSocket;
			} catch(Exception ex) {
				Console.WriteLine(ex);
				return null;
			}
		}

		public void Bind(IPEndPoint ep) {
			if(Peer.SubsystemDown) throw new SocketException(WSAE.NETDOWN);
			if(state!=SocketState.CREATED) throw new SocketException(WSAE.INVAL);
			Log("bind");
			lep = Peer.Host.AssociateEndPointWithSocket(ep,this);
			ldev = Peer.Host.GetDeviceByAddress(lep.Address);
			state = SocketState.BINDED;
		}

		bool ISocket.Bind(IPEndPoint ep, ref int errno) {
			try {
				Bind(ep);
				return true;
			} catch(SocketException ex) {
				errno = ex.ErrorCode;
				return false;
			} catch {
				errno = WSAE.FAULT;
				return false;
			}
		}

		public override bool Connected {
			get {
				return state==SocketState.CONNECT;
			}
		}

		public void Listen(int backlog) {
			if(Peer.SubsystemDown) throw new SocketException(WSAE.NETDOWN);
			if(state!=SocketState.BINDED) throw new SocketException(WSAE.INVAL);
			Log("listen");
			Peer.Host.ListenSocket(this,backlog);
			state = SocketState.LISTEN;
		}

		bool ISocket.Listen(int backlog, ref int errno) {
			try {
				Listen(backlog);
				return true;
			} catch(SocketException ex) {
				errno = ex.ErrorCode;
				return false;
			} catch {
				errno = WSAE.FAULT;
				return false;
			}
		}

		public int Available {
			get {
				return buffer.Available;
			}
		}

		public int Send(byte[] buf) {
			if(Peer.SubsystemDown) return WSAE.NETDOWN;
			if(this.connectionreset) return WSAE.CONNRESET;
			switch(state) {
			case SocketState.CREATED:
				throw new SocketException(WSAE.NOTCONN);
			case SocketState.CONNECT:
				try {
					if(shutdownWriting) throw new SocketException(WSAE.SHUTDOWN);
					int iosize = this.symmetricSocket.Write(buf,0,buf.Length);
					wsize += iosize;
					return iosize;
				} catch(Exception ex) {
					Console.WriteLine(ex);
					return -1;
				}
			case SocketState.DISCONNECTED:
				throw new SocketException(WSAE.DISCON);
			default:
				throw new SocketException(WSAE.INVAL);
			}
		}

		bool shutdownWriting = false;

		public void Shutdown(SocketShutdown how) {
			//if(Peer.SubsystemDown) return WSAE.NETDOWN;
			if(how==SocketShutdown.Send || how==SocketShutdown.Both) {
				shutdownWriting = true;
				this.symmetricSocket.NotifyShutdown();
			}
			if(how==SocketShutdown.Receive || how==SocketShutdown.Both) {
				this.buffer.ShutdownReading();
				this.ValidateClose();
			}
		}

		public override void Close() {
			if(state==SocketState.CONNECT) {
				if(this.symmetricSocket!=null) {
					symmetricSocket.NotifyClose();
				}
			}
			base.Close();
		}

		public void ValidateClose() {
			if(!buffer.WriterConnected && buffer.Available==0) {
				this.PostAsyncSelectMsg(FD.CLOSE);
			}
		}

		public byte[] Receive(int bufsize) {
			if(this.connectionreset) return null;
			try {
				switch(state) {
				case SocketState.CREATED:
					throw new SocketException(WSAE.NOTCONN);
				case SocketState.CONNECT:
					byte[] buf = new byte[bufsize];
					int len = this.Read(buf,0,buf.Length);
					ValidateClose();
					byte[] buf2 = new byte[len];
					Array.Copy(buf,0,buf2,0,len);
					rsize += buf2.Length;
					return buf2;
				case SocketState.DISCONNECTED:
					throw new SocketException(WSAE.DISCON);
				default:
					throw new InvalidOperationException("ソケットの状態が不正です");
				}
			} catch(Exception ex) {
				Console.WriteLine(ex);
				return null;
			}
		}

		class DummyEntity : IDrawingEntity {
			#region IDrawingEntity メンバ
			public event System.EventHandler OnDispose;
			#endregion
		}

		public void Connect(IPEndPoint ep) {
			if(Peer.SubsystemDown) throw new SocketException(WSAE.NETDOWN);
			if(this.connectionreset) throw new SocketException(WSAE.CONNRESET);
			if(state!=SocketState.CREATED && state!=SocketState.BINDED) throw new SocketException(WSAE.INVAL);
			Log("connect");
			Session = new DummyEntity();
			signalWrite.Reset();
			signalExcept.Reset();
			try {
				Device remoteDevice = Peer.Host.QueryDevice(ep.Address);
				if(remoteDevice==null) throw new SocketException(WSAE.REFUSED);
				if(LocalEndPoint==null || LocalEndPoint.Port==0) {
					Bind(new IPEndPoint(Peer.Host.GetAccessibleIPAddress(ep.Address),0));
				}
				if(LocalEndPoint.Address.Equals(IPAddress.Any) || LocalEndPoint.Address.Equals(IPAddress.IPv6Any)) {
					LocalEndPoint.Address = Peer.Host.GetAccessibleIPAddress(ep.Address);
					ldev = Peer.Host.GetDeviceByAddress(LocalEndPoint.Address);
				}
				this.symmetricSocket = remoteDevice.RequestConnect(ep,this);
				rep = this.symmetricSocket.LocalEndPoint;
				rdev = this.symmetricSocket.LocalDevice;
				state = SocketState.CONNECT;
				this.signalWrite.Set();
				this.PostAsyncSelectMsg(FD.CONNECT);
			} catch {
				signalExcept.Set();
				throw;
			}
		}

		bool ISocket.Connect(System.Net.IPEndPoint ep, ref int errno) {
			try {
				Connect(ep);
				return true;
			} catch(SocketException ex) {
				errno = ex.ErrorCode;
				return false;
			} catch(Exception ex) {
				Console.WriteLine(ex);
				errno = WSAE.FAULT;
				return false;
			}
		}

		[System.Runtime.InteropServices.DllImport("ssp")]
		extern static bool SNPostMessage(IntPtr fp, IntPtr hWnd, uint Msg, uint wParam, uint lParam);

		uint asyncselectMsg;
		IntPtr asyncselectSocket;
		int asyncselectInterestEvents;
		IntPtr asyncselectTargetWnd;
		IntPtr asyncselectPostMessage;

		private bool PostAsyncSelectMsg(int socketevent, int errno) {
			if(0==(asyncselectInterestEvents&socketevent)) return false;
			return SNPostMessage(asyncselectPostMessage,asyncselectTargetWnd,asyncselectMsg
				,(uint)asyncselectSocket.ToInt32(),(uint)((errno<<16)|socketevent));
		}

		private bool PostAsyncSelectMsg(int socketevent) {
			return this.PostAsyncSelectMsg(socketevent,0);
		}

		public bool AsyncSelect(IntPtr hWnd, uint wMsg, int lEvent, IntPtr fpPostMsg, IntPtr s, ref int errno) {
			lock(this) {
				this.asyncselectMsg = wMsg;
				this.asyncselectSocket = s;
				this.asyncselectTargetWnd = hWnd;
				this.asyncselectInterestEvents = lEvent;
				this.asyncselectPostMessage = fpPostMsg;
				this.Blocking = false;
			}
			if(0!=(lEvent&FD.READ)) {
				if(this.Available>0) this.PostAsyncSelectMsg(FD.READ);
			}
			if(0!=(lEvent&FD.WRITE)) {
				if(!this.shutdownWriting) this.PostAsyncSelectMsg(FD.WRITE);
			}
			if(0!=(lEvent&FD.ACCEPT) && this.Listening) {
				lock(this.waitForAccept.SyncRoot) {
					if(this.waitForAccept.Count>0) {
						this.PostAsyncSelectMsg(FD.ACCEPT);
					}
				}
			}
			if(0!=(lEvent&FD.CONNECT)) {
				if(this.Connected) this.PostAsyncSelectMsg(FD.CONNECT);
			}
			if(0!=(lEvent&FD.CLOSE)) {
				this.ValidateClose();
			}
			return true;
		}

		#endregion

		public override IDrawingEntity Owner {
			get {
				return this.Peer;
			}
		}

		ManualResetEvent signalRead = new ManualResetEvent(false);
		ManualResetEvent signalWrite = new ManualResetEvent(false);
		ManualResetEvent signalExcept = new ManualResetEvent(false);

		public bool Listening {
			get {
				return state==SocketState.LISTEN;
			}
		}

		public WaitHandle ReadSignal {
			get {
				return signalRead;
			}
		}

		public WaitHandle WriteSignal {
			get {
				return signalWrite;
			}
		}

		public WaitHandle ExceptSignal {
			get {
				return signalExcept;
			}
		}

		public Socket RemoteSocket {
			get {
				return symmetricSocket;
			}
		}

		public override Socket RequestConnect(Socket socket) {
			HandshakeStatus status;
			lock(this) {
				if(this.waitForConnect.Count>0) {
					status = (HandshakeStatus)this.waitForConnect[0];
					this.waitForConnect.RemoveAt(0);
				} else {
					status = new HandshakeStatus();
					this.waitForAccept.Add(status);
				}
			}
			status.ConnectingSocket = socket;
			status.ReadyToConnect.Set();
			status.ReadyToAccept.WaitOne();
			status.ReadyToAccept.WaitOne();
			Socket connected = status.AcceptingSocket;
			status.ReadyToConnect.Set();
			return connected;
		}

		bool shutdownReading = false;

		public override void NotifyShutdown() {
			this.shutdownReading = true;
			buffer.ShutdownWriting();
			base.NotifyShutdown();
		}

		public override void NotifyClose() {
			this.PostAsyncSelectMsg(FD.CLOSE);
			base.NotifyClose();
		}

		public override int Write(byte[] buf, int offset, int size) {
			if(!buffer.ReaderConnected) throw new SocketException(WSAE.CONNABORTED);
			if(!this.Connected) throw new SocketException(WSAE.DISCON);
			lock(this) {
				int iosize = buffer.Write(buf,offset,size);
				lock(this.History) this.History.Write(buf,offset,iosize);
				if(iosize>0) {
					this.signalRead.Set();
					this.PostAsyncSelectMsg(FD.READ);
				}
				return iosize;
			}
		}

		public int Read(byte[] buf, int offset, int size) {
			int iosize = buffer.Read(buf,offset,size,Blocking);
			if(buffer.Available==0) {
				if(buffer.WriterConnected) {
					this.signalRead.Reset();
				} else {
					this.signalRead.Set();
					this.PostAsyncSelectMsg(FD.CLOSE);
				}
			}
			return iosize;
		}

		bool connectionreset = false;
		public override void MakeConnectionDown() {
			connectionreset = true;
			this.Close();
			this.buffer.Abort();
			this.signalExcept.Set();
		}

		int rsize = 0;
		public override int RecvSize {
			get {
				return rsize;
			}
		}

		int wsize = 0;
		public override int SendSize {
			get {
				return wsize;
			}
		}


	}
}
