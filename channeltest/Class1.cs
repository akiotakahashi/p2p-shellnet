using System;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels.Pipe;

using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Soap;

using System.Threading;
using System.Collections.Specialized;

namespace channeltest {
	class Class1 : MarshalByRefObject {
		public override object InitializeLifetimeService() {
			return null;
		}
		public void Print(string msg) {
			throw new Exception();
			Console.WriteLine(msg);
		}
		public void CallMe(EventHandler handler) {
			handler(this,null);
		}

		public static ManualResetEvent evt = new ManualResetEvent(false);
		public static string msg;
		public static void Callback(object sender, EventArgs e) {
			Console.WriteLine("callbacked {0}", msg);
			evt.Set();
			while(true) {
				System.Threading.Thread.Sleep(10000);
			}
		}

		static Class1 cls;

		static void Main(string[] args) {
			RemotingConfiguration.Configure(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
			if(RemotingConfiguration.CustomErrorsEnabled(true)) throw new SystemException();

			SoapServerFormatterSinkProvider serverSinkProvider = new SoapServerFormatterSinkProvider();
			SoapClientFormatterSinkProvider clientSinkProvider = new SoapClientFormatterSinkProvider();
			serverSinkProvider.TypeFilterLevel = TypeFilterLevel.Full;
			
			ListDictionary channelProperties = new ListDictionary();
			if(args.Length==0) channelProperties.Add("port", 9000);
			HttpChannel channel = new HttpChannel(channelProperties,clientSinkProvider,serverSinkProvider);
			/*///
			PipeChannel channel = new PipeChannel(args.Length==0 ? "localhost" : "Auto",serverSinkProvider);
			///*/
			///
			ChannelServices.RegisterChannel(channel);

			if(args.Length==0) {
				cls = new Class1();
				ObjRef objref = RemotingServices.Marshal(cls,"Class1");
				Console.ReadLine();
			} else {
				cls = (Class1)Activator.GetObject(typeof(Class1),"http://localhost:9000/Class1");
				cls.Print("hello");
				msg = "ok!";
				(new Thread(new ThreadStart(engage))).Start();
				evt.WaitOne();
				Console.WriteLine("call 2nd method");
				cls.Print("2nd");
			}

		}

		static void engage() {
			cls.CallMe(new EventHandler(Callback));
		}

	}
}
