using System;
using System.Collections;

namespace Shellnet {
	class NetworkList : CollectionBase {
		public NetworkList() {
		}
		public object SyncRoot {
			get {
				return this;
			}
		}
		protected override void OnValidate(object value) {
			if(!(value is Segment)) throw new ArgumentException();
			base.OnValidate (value);
		}
		public Segment this[int index] {
			get {
				return (Segment)base.List[index];
			}
			set {
				base.List[index] = value;
			}
		}
		public void Add(Segment network) {
			base.InnerList.Add(network);
		}
	}
}
