using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;

using System.Threading;

namespace Shellnet {
	abstract class Host : MarshalByRefObject, IHost, IDisposable, IDrawingEntity {

		public readonly Shellnet Shellnet;
		public string hostName;
		public readonly PeerList Peers = new PeerList();
		public readonly DeviceList Devices = new DeviceList();

		public override object InitializeLifetimeService() {
			return null;
		}

		public Host(Shellnet shellnet) {
			Shellnet = shellnet;
		}

		public IPHostEntry HostEntry {
			get {
				ArrayList addresses = new ArrayList();
				lock(Devices.SyncRoot) {
					foreach(Device device in Devices) {
						addresses.AddRange(device.Addresses);
					}
				}
				IPHostEntry HostEntry = new IPHostEntry();
				HostEntry.AddressList = (IPAddress[])addresses.ToArray(typeof(IPAddress));
				HostEntry.Aliases = new string[0];
				HostEntry.HostName = HostName;
				return HostEntry;
			}
		}

		internal virtual void Register() {
			Shellnet.Register(this);
		}

		internal virtual void Register(Device device) {
			Devices.Add(device);
		}

		internal virtual void Register(Peer peer) {
			Peers.Add(peer);
		}

		internal virtual void Unregister(Peer peer) {
			Peers.Remove(peer);
		}
		
		#region IDisposable メンバ

		protected bool Disposed = false;
		public virtual void Dispose() {
			if(Disposed) return;
			Disposed = true;
			OnDispose(this, new EventArgs());
		}

		#endregion

		#region IHost メンバ

		public short hton(short hostshort) {
			return IPAddress.HostToNetworkOrder(hostshort);
		}

		public short ntoh(short netshort) {
			return IPAddress.NetworkToHostOrder(netshort);
		}

		public ushort hton(ushort hostshort) {
			return (ushort)IPAddress.HostToNetworkOrder((short)hostshort);
		}

		public ushort ntoh(ushort netshort) {
			return (ushort)IPAddress.NetworkToHostOrder((short)netshort);
		}

		public int hton(int hostint) {
			return IPAddress.HostToNetworkOrder(hostint);
		}

		public int ntoh(int netint) {
			return IPAddress.NetworkToHostOrder(netint);
		}

		public uint hton(uint hostint) {
			return (uint)IPAddress.HostToNetworkOrder((int)hostint);
		}

		public uint ntoh(uint netint) {
			return (uint)IPAddress.NetworkToHostOrder((int)netint);
		}

		public long hton(long hostlong) {
			return IPAddress.HostToNetworkOrder(hostlong);
		}

		public long ntoh(long netlong) {
			return IPAddress.NetworkToHostOrder(netlong);
		}

		public ulong hton(ulong hostlong) {
			return (ulong)IPAddress.HostToNetworkOrder((long)hostlong);
		}

		public ulong ntoh(ulong netlong) {
			return (ulong)IPAddress.NetworkToHostOrder((long)netlong);
		}

		public string HostName {
			get {
				return hostName;
			}
			set {
				hostName = value;
			}
		}

		public IPAddress GetIPAddress(long address) {
			try {
				return new IPAddress(address);
			} catch(Exception ex) {
				Console.WriteLine(ex);
				return null;
			}
		}

		public IPAddress GetIPAddress(byte[] address, long scopeid) {
			try {
				return new IPAddress(address,scopeid);
			} catch(Exception ex) {
				Console.WriteLine(ex);
				return null;
			}
		}

		public IPAddress StringToAddress(string str, AddressFamily af) {
			try {
				return IPAddress.Parse(str);
			} catch {
				return QueryAddressByName(str,af);
			}
		}

		public string AddressToString(IPAddress addr) {
			return addr.ToString();
		}

		IPHostEntry IHost.GetHostByName(string hostName) {
			return Shellnet.GetHostByName(hostName).HostEntry;
		}

		IPHostEntry IHost.GetHostByAddress(IPAddress address) {
			return this.QueryDevice(address).Host.HostEntry;
		}

		#endregion

		#region IDrawingEntity メンバ

		public event System.EventHandler OnDispose;

		#endregion

		public IPAddress QueryAddressByName(string hostname, AddressFamily af) {
			if(0==string.Compare(hostname,"localhost",true)) {
				switch(af) {
				case AddressFamily.InterNetwork:
					return IPAddress.Loopback;
				case AddressFamily.InterNetworkV6:
					return IPAddress.IPv6Loopback;
				default:
					throw new SocketException(WSAE.AFNOSUPPORT);
				}
			}
			return Shellnet.QueryAddressByName(hostname,af);
		}

		/// <summary>
		/// 指定されたIPに接続可能なホストのIPを返します。
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public IPAddress GetAccessibleIPAddress(IPAddress address) {
			return HostEntry.AddressList[0];
		}

		public Device GetDeviceByAddress(IPAddress address) {
			foreach(Device device in Devices) {
				if(device.Addresses.Contains(address)) {
					return device;
				}
			}
			return null;
		}

		public Device QueryDevice(IPAddress address) {
			Device device = GetDeviceByAddress(address);
			if(device!=null) return device;
			return Shellnet.QueryDevice(address);
		}

		public Device DetermineDevice(IPAddress address) {
			Device gateway = null;
			foreach(Device device in Devices) {
				if(device.Addresses.Contains(address)) {
					return device;
				} else if(device.External) {
					Device remoteDevice = device.Segment.GetDeviceByAddress(address);
					if(remoteDevice!=null) {
						return remoteDevice;
					}
					gateway = device;
				}
			}
			return gateway;
		}

		public abstract Socket RequestConnect(IPEndPoint ep, Socket socket);

	}
}
