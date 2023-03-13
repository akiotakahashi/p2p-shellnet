using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Shellnet {
	using RealSocket = System.Net.Sockets.Socket;
	class ExternalSocket : Socket {

		public readonly ExternalHost Host;
		RealSocket realSocket = null;
		Thread ReceivingThread = null;

		public ExternalSocket(ExternalHost host, Socket symmetric, RealSocket rs, IPEndPoint lep, IPEndPoint rep, Device ldev, Device rdev) : base(rs.AddressFamily,rs.SocketType,rs.ProtocolType) {
			this.Host = host;
			this.symmetricSocket = symmetric;
			this.Session = symmetric.Session;
			this.realSocket = rs;
			this.lep = lep;
			this.rep = rep;
			this.ldev = ldev;
			this.rdev = rdev;
			this.realSocket.Blocking = true;
			ReceivingThread = new Thread(new ThreadStart(ReceiveMain));
			ReceivingThread.Start();
		}
		public override string ToString() {
			return "es";
		}
		public override int GetHashCode() {
			return this.LocalEndPoint.GetHashCode()+this.RemoteEndPoint.GetHashCode();
		}
		protected override void Disposing() {
			if(Thread.CurrentThread!=ReceivingThread) {
				ReceivingThread.Interrupt();
				ReceivingThread.Abort();
			}
			base.Disposing();
		}
		void ReceiveMain() {
			Thread.CurrentThread.Name = "ExternalSocket.ReceiveMain";
			Thread.CurrentThread.IsBackground = true;
			byte[] buf = new byte[1024];
			try {
				while(realSocket.Connected) {
					int iosize = realSocket.Receive(buf);
					rsize += iosize;
					if(iosize==0) {
						// Graceful reading shutdown
						this.symmetricSocket.NotifyShutdown();
						break;
					} else {
						//Console.WriteLine("ext.socket received {0} bytes",iosize);
						this.symmetricSocket.Write(buf,0,iosize);
					}
				}
			} catch(Exception ex) {
				Console.WriteLine(ex);
			} finally {
				this.symmetricSocket.NotifyClose();
			}
		}
		public override bool Connected {
			get {
				return true;
			}
		}
		public override IDrawingEntity Owner {
			get {
				return this.Host;
			}
		}
		public override void Close() {
			realSocket.Close();
			base.Close();
		}
		public override Socket RequestConnect(Socket socket) {
			throw new NotImplementedException();
		}
		public override void NotifyShutdown() {
			realSocket.Shutdown(SocketShutdown.Send);
		}
		public override void NotifyClose() {
			realSocket.Close();
		}
		int wsize = 0;
		int rsize = 0;
		public override int Write(byte[] buf, int offset, int size) {
			try {
				int iosize = realSocket.Send(buf,offset,size,SocketFlags.None);
				lock(this.History) this.History.Write(buf,offset,iosize);
				wsize += iosize;
				return iosize;
			} catch(Exception ex) {
				Console.WriteLine(ex);
				return -1;
			}
		}
		public override void MakeConnectionDown() {
			this.Close();
		}

		public override int RecvSize {
			get {
				return rsize;
			}
		}

		public override int SendSize {
			get {
				return wsize;
			}
		}

	}
}
