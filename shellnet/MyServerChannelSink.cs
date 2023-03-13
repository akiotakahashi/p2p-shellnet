using System;
using System.IO;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;

namespace Shellnet {

	class MyServerChannelSink : BaseChannelSinkWithProperties, IServerChannelSink {

		private IServerChannelSink nextSink;

		public MyServerChannelSink(IServerChannelSink sink) {
			this.nextSink = sink;
		}

		#region IServerChannelSink ÉÅÉìÉo

		Stream Dump(ITransportHeaders headers, Stream stream) {
			lock(typeof(MyServerChannelSink)) {
				/*
				foreach(DictionaryEntry entry in headers) {
					Console.WriteLine("{0}: {1}",entry.Key,entry.Value);
				}
				Console.WriteLine();
				*/
				MemoryStream mem = new MemoryStream();
				byte[] buf = new byte[1024];
				int iosize;
				while((iosize=stream.Read(buf,0,buf.Length))>0) {
					mem.Write(buf,0,iosize);
				}
				try {
					if(!Directory.Exists("soapmsg")) Directory.CreateDirectory("soapmsg");
					using(FileStream writer = new FileStream(@"soapmsg\"+mem.Length.ToString().PadLeft(7,'0')+".xml",FileMode.Create)) {
						byte[] contents = mem.ToArray();
						writer.Write(contents,0,contents.Length);
					}
				} catch {
				}
				mem.Seek(0,SeekOrigin.Begin);
				return mem;
			}
		}

		public System.IO.Stream GetResponseStream(IServerResponseChannelSinkStack sinkStack, object state, System.Runtime.Remoting.Messaging.IMessage msg, ITransportHeaders headers) {
			return nextSink.GetResponseStream(sinkStack,state,msg,headers);
		}

		public System.Runtime.Remoting.Channels.ServerProcessing ProcessMessage(IServerChannelSinkStack sinkStack, System.Runtime.Remoting.Messaging.IMessage requestMsg, ITransportHeaders requestHeaders, System.IO.Stream requestStream, out System.Runtime.Remoting.Messaging.IMessage responseMsg, out ITransportHeaders responseHeaders, out System.IO.Stream responseStream) {
			lock(typeof(MyServerChannelSink)) {
				requestStream = this.Dump(requestHeaders,requestStream);
			}
			ServerProcessing sp = nextSink.ProcessMessage(sinkStack,requestMsg,requestHeaders,requestStream
				, out responseMsg, out responseHeaders, out responseStream);
			lock(typeof(MyServerChannelSink)) {
				responseStream = this.Dump(responseHeaders,responseStream);
			}
			return sp;
		}

		public void AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack, object state, System.Runtime.Remoting.Messaging.IMessage msg, ITransportHeaders headers, System.IO.Stream stream) {
			nextSink.AsyncProcessResponse(sinkStack,state,msg,headers,stream);
		}

		public IServerChannelSink NextChannelSink {
			get {
				// TODO:  MyServerChannelSink.NextChannelSink getter é¿ëïÇí«â¡ÇµÇ‹Ç∑ÅB
				return nextSink;
			}
		}

		#endregion

		public override ICollection Keys {
			get {
				return new ArrayList();
			}
		}

	}

}
