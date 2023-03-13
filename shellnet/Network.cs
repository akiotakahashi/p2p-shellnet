using System;
using System.Net;

namespace Shellnet {
	class Segment {
		public readonly Shellnet Shellnet;
		public readonly Segment Parent;
		public readonly DeviceList Devices = new DeviceList();
		/*
		public readonly HostAddressTable AddressMap = new HostAddressTable();
		public readonly HostNameTable HostNameMap = new HostNameTable();
		*/

		public Segment(Shellnet shellnet) {
			Shellnet = shellnet;
			Parent = null;
		}

		#region	DNS services

		public Device QueryDevice(IPAddress address) {
			Device device = this.GetDeviceByAddress(address);
			if(device!=null) return device;
			return Shellnet.QueryDevice(address);
		}

		public Device GetDeviceByName(string hostname) {
			if(String.Compare(hostname,"localhost",true)==0) throw new InvalidProgramException();
			lock(this) {
				foreach(Device device in Devices) {
					if(device.Host.HostName==hostname) {
						return device;
					}
				}
			}
			return Parent.GetDeviceByName(hostname);
		}

		public Device GetDeviceByAddress(IPAddress address) {
			if(address.Equals(IPAddress.Loopback)) throw new InvalidProgramException();
			if(address.Equals(IPAddress.IPv6Loopback)) throw new InvalidProgramException();
			lock(Devices.SyncRoot) {
				foreach(Device device in Devices) {
					int i = device.Addresses.IndexOf(address);
					if(i>=0) return device;
				}
			}
			return null;
		}
		
		#endregion

		internal virtual void Register() {
			Shellnet.Register(this);
		}

		internal void Register(Device device) {
			lock(Devices.SyncRoot) {
				Devices.Add(device);
				/*
				foreach(IPAddress address in device.Addresses) {
					AddressMap[address] = host;
				}
				HostNameMap[host.HostEntry.HostName] = device;
				if(host.HostEntry.Aliases!=null) {
					foreach(string hostname in host.HostEntry.Aliases) {
						HostNameMap[hostname] = host;
					}
				}
				*/
			}
		}

		public SocketBase TranslateSocket(SocketBase socket, IPEndPoint rep) {
			return socket;
		}

		public SocketBase AcceptSocket(SocketBase socket, IPEndPoint rep) {
			return socket;
		}

	}
}
