using System;
using System.Collections;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Lifetime;


namespace Shellnet {
	class Shellnet : MarshalByRefObject, IShellnet {

		public override object InitializeLifetimeService() {
			return null;
		}
		
		public readonly NetworkList Segments = new NetworkList();
		public readonly HostList Hosts = new HostList();
		public Segment EnclosingNetwork;
		public InternalHost EnclosingHost;

		public bool AutoGenerateExternalHost = true;

		public Shellnet() {
			Console.WriteLine("constructing shellnet");

			EnclosingNetwork = new Segment(this);
			EnclosingNetwork.Register();
			
			IPHostEntry he = Dns.GetHostByName(Dns.GetHostName());
			EnclosingHost = new InternalHost(this);
			EnclosingHost.CreateDevice(EnclosingNetwork,he.AddressList);
			EnclosingHost.HostName = he.HostName;
			EnclosingHost.Register();
			
			Console.WriteLine("enclosing host is {0} ({1})", EnclosingHost.HostEntry.HostName, EnclosingHost.HostEntry.AddressList[0]);
		}

		public void Register(Segment network) {
			Segments.Add(network);
		}

		public void Register(Host host) {
			Hosts.Add(host);
		}

		public void CreateNewHost(IPAddress address, string hostname) {
			EnclosingHost = new InternalHost(this);
			EnclosingHost.CreateDevice(EnclosingNetwork, new IPAddress[]{address});
			EnclosingHost.HostName = hostname;
			EnclosingHost.Register();
		}

		#region IShellnet ÉÅÉìÉo

		public IPeer Startup(int pid, string keepaliveMutexName) {
			//throw new Exception();
			Console.WriteLine("startup");
			try {
				return new Peer(EnclosingHost,pid,keepaliveMutexName);
			} catch(Exception ex) {
				Console.WriteLine("failed to startup new peer.");
				Console.WriteLine(ex);
				return null;
			}
		}
		
		public void Keepalive(EventHandler callback) {
			callback(this,null);
		}

		public long inet_addr(string address) {
			return IPAddress.Parse(address).Address;
		}

		[DllImport("ws2_32")] extern unsafe static int WSAEnumProtocolsA(int[] lpiProtocols, byte* buffer, uint* length);
		unsafe public int _EnumProtocolsA(int[] iProtocols, byte[] protocolBuffer, ref uint dwBufferLength) {
			fixed(byte* p = &protocolBuffer[0]) {
				fixed(uint* q = &dwBufferLength) {
					return WSAEnumProtocolsA(iProtocols,p,q);
				}
			}
		}

		[DllImport("ws2_32")] extern unsafe static int WSAEnumProtocolsW(int[] lpiProtocols, byte* buffer, uint* length);
		unsafe public int _EnumProtocolsW(int[] iProtocols, byte[] protocolBuffer, ref uint dwBufferLength) {
			fixed(byte* p = &protocolBuffer[0]) {
				fixed(uint* q = &dwBufferLength) {
					return WSAEnumProtocolsW(iProtocols,p,q);
				}
			}
		}

		#endregion

		#region DNS

		public IPAddress QueryAddressByName(string hostname, AddressFamily af) {
			lock(Hosts.SyncRoot) {
                Host host = this.QueryHostByName(hostname);
				if(host==null) return null;
				foreach(Device device in host.Devices) {
					if(!device.External) continue;
					foreach(IPAddress address in device.Addresses) {
						if(address.AddressFamily==af) return address;
					}
				}
			}
			return null;
		}

		private Hashtable queryingHosts = new Hashtable();

		public Host GetHostByName(string hostName) {
			lock(Hosts.SyncRoot) {
				foreach(Host host in Hosts) {
					if(string.Compare(host.HostName,hostName,true)==0) {
						return host;
					}
				}
			}
			return null;
		}

		public Host QueryHostByName(string hostName) {
			Host host = this.GetHostByName(hostName);
			if(host!=null || !this.AutoGenerateExternalHost) {
				return host;
			}
			bool wait;
			ManualResetEvent evt;
			lock(queryingHosts.SyncRoot) {
				if(queryingHosts.ContainsKey(hostName.ToLower())) {
					wait = true;
					evt = (ManualResetEvent)queryingHosts[hostName.ToLower()];
				} else {
					wait = false;
					evt = new ManualResetEvent(false);
					queryingHosts.Add(hostName.ToLower(),evt);
				}
			}
			if(wait) {
				evt.WaitOne();
				return this.GetHostByName(hostName);
			} else {
				try {
					IPHostEntry entry = Dns.GetHostByName(hostName);
					ExternalHost exthost = new ExternalHost(this,entry.AddressList[0]);
					exthost.HostName = entry.HostName;
					exthost.CreateDevice(this.EnclosingNetwork,entry.AddressList);
					exthost.Register();
					return exthost;
				} catch(Exception ex) {
					Console.WriteLine(ex);
					return null;
				} finally {
					evt.Set();
				}
			}
		}

		public Device GetDeviceByAddress(IPAddress address) {
			lock(Segments.SyncRoot) {
				foreach(Segment segment in Segments) {
					Device device = segment.GetDeviceByAddress(address);
					if(device!=null) return device;
				}
			}
			return null;
		}

		public Device QueryDevice(IPAddress address) {
			Device device = GetDeviceByAddress(address);
			if(device!=null) return device;
			bool wait;
			ManualResetEvent evt;
			lock(queryingHosts.SyncRoot) {
				if(queryingHosts.ContainsKey(address.ToString())) {
					wait = true;
					evt = (ManualResetEvent)queryingHosts[address.ToString()];
				} else {
					wait = false;
					evt = new ManualResetEvent(false);
					queryingHosts.Add(address.ToString(),evt);
				}
			}
			if(wait) {
				evt.WaitOne();
				return this.GetDeviceByAddress(address);
			} else {
				try {
					IPHostEntry entry = Dns.GetHostByAddress(address);
					ExternalHost exthost = new ExternalHost(this,entry.AddressList[0]);
					exthost.HostName = entry.HostName;
					device = exthost.CreateDevice(this.EnclosingNetwork,entry.AddressList);
					exthost.Register();
					return device;
				} catch(Exception ex) {
					Console.WriteLine(ex);
					return null;
				} finally {
					evt.Set();
				}
			}
		}

		#endregion

	}
}
