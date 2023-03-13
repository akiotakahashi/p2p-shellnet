using System;
using System.Net;
using System.Collections;

namespace Shellnet {
	abstract class Device : IDrawingEntity, IDisposable {
		public readonly Host Host = null;
		public readonly Segment Segment = null;
		public readonly AddressList Addresses = new AddressList();
		public Device(Host host, Segment segment) {
			Host = host;
			Segment = segment;
		}
		public virtual void Register() {
			Host.Register(this);
			if(Segment!=null) Segment.Register(this);
		}
		public virtual bool External {
			get {
				return true;
			}
		}
		public virtual Socket RequestConnect(IPEndPoint ep, Socket socket) {
			return Host.RequestConnect(ep,socket);
		}

		#region IDrawingEntity ƒƒ“ƒo

		public event System.EventHandler OnDispose;

		#endregion

		#region IDisposable ƒƒ“ƒo

		bool Disposed = false;
		public void Dispose() {
			if(Disposed) return;
			Disposed = true;
			OnDispose(this,null);
		}

		#endregion

	}
	class AddressList : CollectionBase {
		protected override void OnValidate(object value) {
			if(!(value is IPAddress)) throw new InvalidCastException();
		}
		public object SyncRoot {
			get {
				return this;
			}
		}
		public IPAddress this[int index] {
			get {
				return (IPAddress)base.List[index];
			}
			set {
				base.List[index] = value;
			}
		}
		public void Add(IPAddress address) {
			base.InnerList.Add(address);
		}
		public void AddRange(ICollection c) {
			base.InnerList.AddRange(c);
		}
		public bool Contains(IPAddress address) {
			return base.InnerList.Contains(address);
		}
		public int IndexOf(IPAddress address) {
			return base.InnerList.IndexOf(address);
		}
	}
}
