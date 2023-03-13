using System;
using System.Collections;

namespace Shellnet {
	class PeerList : CollectionBase {
		public object SyncRoot {
			get {
				return this;
			}
		}
		protected override void OnValidate(object value) {
			if(!(value is Peer)) throw new ArgumentException();
			base.OnValidate (value);
		}
		public Peer this[int index] {
			get {
				return (Peer)base.List[index];
			}
			set {
				base.InnerList[index] = index;
			}
		}
		public void Add(Peer peer) {
			lock(this.SyncRoot) {
				base.InnerList.Add(peer);
			}
		}
		public void Remove(Peer peer) {
			lock(this.SyncRoot) {
				base.InnerList.Remove(peer);
			}
		}
	}
}
