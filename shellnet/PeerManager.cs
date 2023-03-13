using System;
using System.Threading;
using System.Collections;

namespace Shellnet {
	internal class PeerManager {
		readonly ArrayList mutexs = new ArrayList();
		readonly ArrayList peers = new ArrayList();
		readonly Queue queue = new Queue();
		readonly AutoResetEvent signalAdd = new AutoResetEvent(false);
		readonly Thread thdEngage;
		public PeerManager() {
			thdEngage = new Thread(new ThreadStart(Engage));
			thdEngage.Start();
		}
		public void Register(Peer peer, string mutexName) {
			if(peer==null) throw new ArgumentNullException("peer‚ªnull‚Å‚·B");
			Mutex m = new Mutex(false,mutexName);
			lock(queue.SyncRoot) {
				queue.Enqueue(m);
				queue.Enqueue(peer);
			}
			signalAdd.Set();
		}
		void Engage() {
			Thread.CurrentThread.Name = "PeerManager.Engage";
			Thread.CurrentThread.IsBackground = true;
			while(true) {
				WaitHandle[] signals = new WaitHandle[1+mutexs.Count];
				signals[0] = signalAdd;
				mutexs.CopyTo(signals,1);
				int index = System.Threading.WaitHandle.WaitAny(signals);
				index %= 128;
				if(index==0) {
					Console.WriteLine("detect modifying signal");
				} else {
					--index;
					Console.WriteLine("detect #{0} keepalive disconnecting",index);
					Peer peer = (Peer)peers[index];
					mutexs.RemoveAt(index);
					peers.RemoveAt(index);
					peer.Dispose();
				}
				lock(queue.SyncRoot) {
					while(queue.Count>0) {
						mutexs.Add(queue.Dequeue());
						peers.Add(queue.Dequeue());
					}
				}
			}
		}
	}
}
