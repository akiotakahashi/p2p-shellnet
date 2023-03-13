using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;

using System.Threading;
using System.Runtime.Remoting.Lifetime;

namespace Shellnet {
	class Peer : MarshalByRefObject, IPeer, IDisposable, IDrawingEntity {

		public override object InitializeLifetimeService() {
			return null;
		}

		static readonly PeerManager PeerManager = new PeerManager();
		static int nextpeerid = 0;

		private readonly int peerId;
		private int processId;
		public readonly InternalHost Host;
		private int socketId = 0;
		public readonly ArrayList Sockets = new ArrayList();
	
		internal ManualResetEvent DisposingSignal = new ManualResetEvent(false);
		
		public Peer(InternalHost host, int pid, string keepaliveMutexName) {
			peerId = Interlocked.Increment(ref nextpeerid)-1;
			processId = pid;
			Host = host;
			Host.Register(this);
			PeerManager.Register(this,keepaliveMutexName);
		}

		~Peer() {
		}

		public bool Disposed = false;
		public void Dispose() {
			if(Disposed) return;
			Disposed = true;
			if(OnDispose!=null) OnDispose(this,null);
			Console.WriteLine("disposing peer#{0}",PeerId);
			DisposingSignal.Set();
			ICollection sockets;
			lock(Sockets.SyncRoot) {
				sockets = (ICollection)Sockets.Clone();
			}
			foreach(Socket socket in sockets) {
				socket.Dispose();
			}
			Host.Unregister(this);
		}

		internal void Register(Socket socket) {
			if(socket==null) throw new ArgumentNullException();
			lock(Sockets.SyncRoot) {
				Sockets.Add(socket);
			}
		}

		internal void Unregister(Socket socket) {
			Host.Unregister(socket);
			lock(Sockets.SyncRoot) {
				Sockets.Remove(socket);
			}
		}

		public int ProcessId {
			get {
				return this.processId;
			}
		}

		#region IPeer ÉÅÉìÉo

		public int PeerId {
			get {
				return peerId;
			}
		}

		IHost IPeer.Host {
			get {
				return Host;
			}
		}

		public int Cleanup() {
			Console.WriteLine("Cleanup shellnet peer");
			return 0;
		}

		public System.IO.TextWriter Log {
			get {
				return Console.Out;
			}
		}

		public ISocket CreateSocket(AddressFamily af, SocketType st, ProtocolType pt) {
			if(this.subsystemdown) return null;
			return new InternalSocket(this,af,st,pt,socketId++);
		}

		public int Select(ref ISocket[] read, ref ISocket[] write, ref ISocket[] except, int microSeconds) {
			if(this.subsystemdown) return WSAE.NETDOWN;
			int cr=0, cw=0, ce=0;
			if(read!=null) cr=read.Length;
			if(write!=null) cw=write.Length;
			if(except!=null) ce=except.Length;
			WaitHandle[] signals = new WaitHandle[cr+cw+ce];
			for(int i=0; i<cr; ++i) {
				if(read[i]!=null) {
					signals[i] = ((InternalSocket)read[i]).ReadSignal;
				}
			}
			for(int i=0; i<cw; ++i) {
				if(write[i]!=null) {
					signals[cr+i] = ((InternalSocket)write[i]).WriteSignal;
				}
			}
			for(int i=0; i<ce; ++i) {
				if(except[i]!=null) {
					signals[cr+cw+i] = ((InternalSocket)except[i]).ExceptSignal;
				}
			}

			ArrayList dummysignals = new ArrayList();
			for(int i=0; i<signals.Length; ++i) {
				if(signals[i]==null) {
					signals[i] = new ManualResetEvent(false);
					dummysignals.Add(signals[i]);
				}
			}
			
			WaitHandle.WaitAny(signals,TimeSpan.FromMilliseconds(microSeconds/1000.0),false);

			int miss = 0;

			for(int i=0; i<cr; ++i) {
				if(signals[i]==null || !signals[i].WaitOne(0,false)) {
					read[i] = null;
					++miss;
				}
			}
			for(int i=0; i<cw; ++i) {
				if(signals[i]==null || !signals[cr+i].WaitOne(0,false)) {
					write[i] = null;
					++miss;
				}
			}
			for(int i=0; i<ce; ++i) {
				if(signals[i]==null || !signals[cr+cw+i].WaitOne(0,false)) {
					except[i] = null;
					++miss;
				}
			}

			foreach(WaitHandle evt in dummysignals) {
				evt.Close();
			}

			return cr+cw+ce-miss;
		}

		#endregion

		internal void CloseSocket(Socket socket) {
			;
		}
	
		#region IDrawingEntity ÉÅÉìÉo

		public event EventHandler OnDispose;

		#endregion

		bool subsystemdown = false;
		public void MakeNetSubsystemDown() {
			subsystemdown = true;
		}

		public bool SubsystemDown {
			get {
				return this.subsystemdown;
			}
		}

		int nNongraceful = 0;

		public void IncrementNongraceful() {
			nNongraceful++;
		}

		public int NumberOfNongraceful {
			get {
				return nNongraceful;
			}
		}

	}
}
