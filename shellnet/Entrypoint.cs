using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Pipe;

using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Shellnet {
	class Entrypoint {
		[System.Runtime.InteropServices.DllImport("kernel32.dll")]
		extern static short QueryPerformanceCounter(out long x);
		[System.Runtime.InteropServices.DllImport("kernel32.dll")]
		extern static short QueryPerformanceFrequency(out long x);
		static Shellnet shellnet;
		static System.LocalDataStoreSlot slotPeer;
		static Entrypoint() {
			slotPeer = Thread.AllocateDataSlot();
		}
		static void Main() {

			Thread.CurrentThread.Name = "Shellnet.Main";

			long freq;
			QueryPerformanceFrequency(out freq);

			/*
			if(Console.Out==Console.Error) {
				Console.SetOut(TextWriter.Synchronized(Console.Out));
			} else {
				Console.SetOut(TextWriter.Synchronized(Console.Out));
				Console.SetError(TextWriter.Synchronized(Console.Error));
			}
			*/

			RemotingConfiguration.Configure(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
			if(RemotingConfiguration.CustomErrorsEnabled(false)==true) throw new ApplicationException();

			/*
			//RemotingConfiguration.Configure("shellnet.exe.config");
			IChannel chnl = new PipeChannel("shellnet");
			ChannelServices.RegisterChannel(chnl);
			//((PipeChannel)ChannelServices.GetChannel("Pipe")).OnTerminate += new EventHandler(Entrypoint_OnTerminate);
			*/

			shellnet = new Shellnet();
			ObjRef objRef = RemotingServices.Marshal(shellnet,"shellnet");
			Console.WriteLine(objRef.URI);

			(new Thread(new ThreadStart(WinMain))).Start();

			while(true) {
				Console.ReadLine();
				TextWriter stdout = Console.Out;
				MemoryStream stream = new MemoryStream();
				StreamWriter buf = new StreamWriter(stream);
				Console.SetOut(TextWriter.Synchronized(buf));
				Console.Error.Write(">");
				string line;
				while((line=Console.ReadLine().Trim()).Length>0) {
					Regex re = new Regex(@"^(?:(?<item>(?:'(?:''|[^'])*'|[^\s']+))(?:\s+|$))+".Replace('\'','\"'));
					if(!re.IsMatch(line)) {
						Console.Error.WriteLine("Invalid format");
					} else {
						CaptureCollection items = re.Match(line).Groups["item"].Captures;
						try {
							switch(items[0].Value.ToLower()) {
							case "exit":
								goto exit;
							case "gc":
								Console.Error.Write("garbage collecting...");
								System.GC.Collect();
								Console.Error.WriteLine("completed");
								break;
							case "newhost":
								IPAddress address;
								string hostname;
								address = IPAddress.Parse(items[1].Value);
								hostname = address.ToString();
								shellnet.CreateNewHost(address,hostname);
								break;
							case "monitor":
								(new Thread(new ThreadStart(WinMain))).Start();
								break;
							case "alloc":
								Console.Error.WriteLine("{0} bytes are allocated", (new byte[int.Parse(items[1].Value)]).Length);
								break;
							case "stat":
							case "stress":
								Process process;
								if(items.Count==1) {
									process = Process.GetCurrentProcess();
								} else {
									try {
										process = Process.GetProcessById(int.Parse(items[1].Value));
									} catch(System.FormatException) {
										process = Process.GetProcessesByName(items[1].Value)[0];
									}
								}
								process.Refresh();
								switch(items[0].Value.ToLower()) {
								case "stat":
									Console.Error.WriteLine("WorkingSet              : {0,11:f02} KB",process.WorkingSet/1024.0);
									Console.Error.WriteLine("PrivateMemorySize       : {0,11:f02} KB",process.PrivateMemorySize/1024.0);
									Console.Error.WriteLine("VirtualMemorySize       : {0,11:f02} KB",process.VirtualMemorySize/1024.0);
									Console.Error.WriteLine("PagedMemorySize         : {0,11:f02} KB",process.PagedMemorySize/1024.0);
									Console.Error.WriteLine("PagedSystemMemorySize   : {0,11:f02} KB",process.PagedSystemMemorySize/1024.0);
									Console.Error.WriteLine("NonpagedSystemMemorySize: {0,11:f02} KB",process.NonpagedSystemMemorySize/1024.0);
									Console.Error.WriteLine("TotalProcessorTime      : {0}",process.TotalProcessorTime);
									break;
								case "stress":
									TimeSpan t0 = process.TotalProcessorTime;
									for(int i=0; i<5; ++i) {
										Thread.Sleep(TimeSpan.FromSeconds(1));
										process.Refresh();
										TimeSpan t1 = process.TotalProcessorTime;
										TimeSpan dt = t1-t0;
										Console.Error.WriteLine("consumed time: {0,7:f02}ms ({1:f02}%)"
											,dt.Ticks*100.0/1000/1000,dt.TotalMilliseconds/1000.0*100);
										t0 = t1;
									}
									break;
								}
								break;
							default:
								Console.Error.WriteLine("Invalid command, {0}", items[0].Value);
								break;
							}
						} catch {
							Console.Error.WriteLine("an exception occurs");
						}
					}
					Console.Error.Write(">");
				}
				buf.Flush();
				stream.Seek(0,SeekOrigin.Begin);
				stdout.Write(new StreamReader(stream).ReadToEnd());
				Console.SetOut(stdout);
			}
			
		exit:
			Application.Exit();
			RemotingServices.Unmarshal(objRef);
			RemotingServices.Disconnect(shellnet);
			//ChannelServices.UnregisterChannel(chnl);
			shellnet = null;
		}

		[STAThread]
		static void WinMain() {
			Thread.CurrentThread.Name = "Shellnet.WinMain";
			Application.Run(new frmMonitor(shellnet));
		}

		public static void AssociatePeerWithThread(Peer peer) {
			Thread.SetData(slotPeer, peer);
		}

		private static void Entrypoint_OnTerminate(object sender, EventArgs e) {
			Peer peer = (Peer)Thread.GetData(slotPeer);
			if(peer!=null) {
				peer.Dispose();
			}
		}
	}
}
