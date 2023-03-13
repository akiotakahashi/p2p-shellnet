using System;
using System.Collections;

namespace Shellnet {
	class HostList : CollectionBase {
		public object SyncRoot {
			get {
				return this;
			}
		}
		protected override void OnValidate(object value) {
			if(!(value is Host)) throw new ArgumentException();
			base.OnValidate (value);
		}
		public Host this[int index] {
			get {
				return (Host)base.List[index];
			}
			set {
				base.InnerList[index] = value;
			}
		}
		public void Add(Host host) {
			base.InnerList.Add(host);
		}
	}
}
