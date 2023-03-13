using System;
using System.Collections;

namespace Shellnet {
	class DeviceList : CollectionBase {
		public object SyncRoot {
			get {
				return this;
			}
		}
		protected override void OnValidate(object value) {
			if(!(value is Device)) throw new ArgumentException();
			base.OnValidate (value);
		}
		public Device this[int index] {
			get {
				return (Device)base.List[index];
			}
			set {
				base.InnerList[index] = value;
			}
		}
		public void Add(Device item) {
			base.InnerList.Add(item);
		}
	}
}
