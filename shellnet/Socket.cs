using System;
using System.Net;
using System.Net.Sockets;

namespace Shellnet {

	abstract class SocketBase : MarshalByRefObject, IDisposable {

		public override object InitializeLifetimeService() {
			return null;
		}
	
		#region IDisposable メンバ

		protected bool Disposed = false;
		public virtual void Dispose() {
			if(Disposed) return;
			Disposed = true;
			Disposing();
		}

		protected virtual void Disposing() {
		}

		#endregion

		public void Log(string format, params object[] args) {
			lock(Console.Out) {
				Console.Out.Write("[{0,3}] ",this.ToString());
				Console.Out.WriteLine(format,args);
			}
		}

		public abstract System.Net.IPEndPoint LocalEndPoint {get;}
		public abstract System.Net.IPEndPoint RemoteEndPoint {get;}

	}

	enum SocketState {
		CREATED,		//作成済み
		BINDED,			//結合済み
		LISTEN,			//接続待機
		CONNECT,		//接続済み
		CLOSED,			//使用済み
		DISCONNECTED,	//切断済み
		DISPOSED,		//破棄済み
	}

	abstract class Socket : SocketBase,IDrawingEntity {

		private AddressFamily	af;
		private SocketType		st;
		private ProtocolType	pt;

		private bool blocking = true;

		protected IPEndPoint lep;
		protected IPEndPoint rep;
		protected Device ldev;
		protected Device rdev;

		protected Socket symmetricSocket;
		protected IDrawingEntity session = null;

		public Socket PairSocket {
			get {
				return this.symmetricSocket;
			}
		}

		public IDrawingEntity Session {
			get {
				return session;
			}
			set {
				session = value;
			}
		}
		
		protected SocketState state;

		public System.IO.MemoryStream History = new System.IO.MemoryStream();

		public Socket(AddressFamily af, SocketType st, ProtocolType pt) {
			this.af = af;
			this.st = st;
			this.pt = pt;
			state = SocketState.CREATED;
		}
	
		#region IDisposable メンバ

		public override void Dispose() {
			if(!this.Disposed) {
			}
			base.Dispose();
		}

		protected override void Disposing() {
			state = SocketState.DISPOSED;
			base.Disposing();
		}

		#endregion

		#region IDrawingEntity メンバ

		public event System.EventHandler OnDispose;

		#endregion

		public SocketState State {
			get {
				return state;
			}
		}

		public System.Net.Sockets.AddressFamily AddressFamily {
			get {
				return af;
			}
		}

		public System.Net.Sockets.ProtocolType ProtocolType {
			get {
				return ProtocolType.Tcp;
			}
		}

		public System.Net.Sockets.SocketType SocketType {
			get {
				return st;
			}
		}

		public override System.Net.IPEndPoint LocalEndPoint {
			get {
				return lep;
			}
		}

		public override System.Net.IPEndPoint RemoteEndPoint {
			get {
				return rep;
			}
		}

		public Device LocalDevice {
			get {
				return ldev;
			}
		}

		public Device RemoteDevice {
			get {
				return rdev;
			}
		}

		public bool Blocking {
			get {
				return blocking;
			}
			set {
				blocking = value;
			}
		}

		public abstract bool Connected {get;}
		public abstract IDrawingEntity Owner {get;}

		public virtual void Close() {
			state = SocketState.CLOSED;
			Console.WriteLine("socket is closed");
			if(OnDispose!=null) OnDispose(this,null);
		}

		public abstract Socket RequestConnect(Socket socket);
		public virtual void NotifyShutdown() {
		}
		public virtual void NotifyClose() {
			switch(state) {
			case SocketState.CONNECT:
				break;
			case SocketState.CLOSED:
				this.symmetricSocket.Disconnect();
				Disconnect();
				break;
			default:
				throw new InvalidOperationException("不正なクローズが通知されました");
			}
		}
		public virtual void Disconnect() {
			this.symmetricSocket = null;
			this.Dispose();
		}
		public abstract int Write(byte[] buf, int offset, int size);
		public abstract void MakeConnectionDown();
		public abstract int SendSize {get;}
		public abstract int RecvSize {get;}

	}
}
