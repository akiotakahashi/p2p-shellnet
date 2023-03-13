using System;
using System.Net;
using System.Collections;

namespace Shellnet {
	class HostNameTable : DictionaryBase {
		protected override void OnValidate(object key, object value) {
			if(!(key is string)) throw new ArgumentException();
			if(!(value is Device)) throw new ArgumentException();
			base.OnValidate(key, value);
		}
		public Device this[string hostname] {
			get {
				return (Device)base.InnerHashtable[hostname];
			}
			set {
				base.InnerHashtable[hostname] = value;
			}
		}
	}
	class HostAddressTable : DictionaryBase {
		protected override void OnValidate(object key, object value) {
			if(!(key is IPAddress)) throw new ArgumentException();
			if(!(value is Device)) throw new ArgumentException();
			base.OnValidate(key, value);
		}
		public Device this[IPAddress address] {
			get {
				return (Device)base.InnerHashtable[address];
			}
			set {
				base.InnerHashtable[address] = value;
			}
		}
	}
}
