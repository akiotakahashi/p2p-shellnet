using System;
using System.IO;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;

namespace Shellnet {

	public class MyClientChannelSink : BaseChannelSinkWithProperties, IClientChannelSink {

		private IClientChannelSink nextSink;
		
		public MyClientChannelSink(IClientChannelSink sink) {
			this.nextSink = sink;
		}
	
		#region IClientChannelSink ÉÅÉìÉo

		public void AsyncProcessRequest(IClientChannelSinkStack sinkStack, System.Runtime.Remoting.Messaging.IMessage msg, ITransportHeaders headers, Stream stream) {
			nextSink.AsyncProcessRequest(sinkStack,msg,headers,stream);
		}

		public void ProcessMessage(System.Runtime.Remoting.Messaging.IMessage msg, ITransportHeaders requestHeaders, Stream requestStream, out ITransportHeaders responseHeaders, out Stream responseStream) {
			nextSink.ProcessMessage(msg,requestHeaders,requestStream, out responseHeaders, out responseStream);
		}

		public void AsyncProcessResponse(IClientResponseChannelSinkStack sinkStack, object state, ITransportHeaders headers, Stream stream) {
			nextSink.AsyncProcessResponse(sinkStack,state,headers,stream);
		}

		public Stream GetRequestStream(System.Runtime.Remoting.Messaging.IMessage msg, ITransportHeaders headers) {
			return nextSink.GetRequestStream(msg,headers);
		}

		public IClientChannelSink NextChannelSink {
			get {
				return nextSink;
			}
		}

		#endregion

	}
}
