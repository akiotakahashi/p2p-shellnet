using System;
using System.Collections;

using System.Threading;

namespace Shellnet {
	public class StreamBuffer {
		private AutoResetEvent signalAvailable = new AutoResetEvent(false);
		private Queue buffer = new Queue();
		private byte[] lastblock = null;
		const int blocksize = 10240;
		int usedblocksize = 0;
		int lastblocksize = blocksize;
		bool connectReader = true;
		bool connectWriter = true;
		int readsize = 0;
		int writesize = 0;
		ManualResetEvent signal;
		internal void SetReadSignal(ManualResetEvent signal) {
			this.signal = signal;
		}
		public bool ReaderConnected {
			get {
				return connectReader;
			}
		}
		public bool WriterConnected {
			get {
				return connectWriter;
			}
		}
		public int Available {
			get {
				if(buffer.Count==0) return 0;
				return (buffer.Count-1)*blocksize-usedblocksize+lastblocksize;
			}
		}
		public void ShutdownReading() {
			connectReader = false;
		}
		public void ShutdownWriting() {
			connectWriter = false;
		}
		public int ReadSize {
			get {
				return readsize;
			}
		}
		public int WriteSize {
			get {
				return writesize;
			}
		}
		bool aborted = false;
		public void Abort() {
			aborted = true;
			if(signal!=null) this.signal.Set();
			this.signalAvailable.Set();
		}
		public int Read(byte[] buf, int startIndex, int size, bool blocking) {
			int io = 0;
			while(!aborted && io<size) {
				int result = InternalRead(buf,startIndex+io,size-io);
				if(result>0) {
					io += result;
				} else {
					if(!connectWriter && io==0) {
						return 0;
					} else if(blocking && io==0) {
						signalAvailable.WaitOne();
					} else {
						break;
					}
				}
			}
			lock(buffer.SyncRoot) {
				if(buffer.Count>0) {
					signalAvailable.Set();
				} else {
					if(signal!=null) {
						signal.Reset();
					}
				}
			}
			readsize += io;
			return io;
		}
		private int InternalRead(byte[] buf, int startIndex, int size) {
			if(size==0) return 0;
			lock(buffer.SyncRoot) {
				if(buffer.Count==0) {
					return 0;
				} else {
					int usedsize = this.usedblocksize;
					int fillsize = -1;
					if(buffer.Count>1) {
						fillsize = blocksize;
					} else {
						fillsize = this.lastblocksize;
					}
					byte[] block;
					if(fillsize-usedsize<=size) {
						block = (byte[])buffer.Dequeue();
						this.usedblocksize = 0;
						size = fillsize-usedsize;
						if(buffer.Count==0) {
							lastblock = null;
							lastblocksize = 0;
						}
					} else {
						block = (byte[])buffer.Peek();
						this.usedblocksize += size;
					}
					Array.Copy(block,usedsize,buf,startIndex,size);
					return size;
				}
			}
		}
		public int Write(byte[] buf, int startIndex, int size) {
			if(!connectReader) throw new System.Net.Sockets.SocketException(WSAE.SHUTDOWN);
			int io = 0;
			while(io<size) {
				io += InternalWrite(buf,startIndex+io,size-io);
			}
			signalAvailable.Set();
			if(signal!=null && io>0) {
				signal.Set(); 
			}
			writesize += io;
			return io;
		}
		private int InternalWrite(byte[] buf, int startIndex, int size) {
			lock(buffer.SyncRoot) {
				if(lastblock==null || lastblocksize>=blocksize) {
					lastblock = new byte[blocksize];
					buffer.Enqueue(lastblock);
					lastblocksize = 0;
				}
				size = Math.Min(size,blocksize-lastblocksize);
				Array.Copy(buf,startIndex,lastblock,lastblocksize,size);
				lastblocksize += size;
				return size;
			}
		}
	}
}
