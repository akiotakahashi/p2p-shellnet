/*=====================================================================

  File:        PipeChannel.cs

=====================================================================*/

using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Channels;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections;
using System.Threading;

namespace System.Runtime.Remoting.Channels.Pipe {
	public class PipeChannel : BaseChannelWithProperties, IChannel, IChannelReceiver, IChannelSender {
		private PipeClientChannel _clientChannel = null;
		private PipeServerChannel _serverChannel = null;

		public PipeChannel() {
			_clientChannel = new PipeClientChannel();
		}

		public PipeChannel(string name, IServerChannelSinkProvider serverSinkProvider) : this() {
			_serverChannel = new PipeServerChannel(name,serverSinkProvider);
		}

		public PipeChannel(string name) : this(name,null) {
		}

		public PipeChannel(
			IDictionary properties, 
			IClientChannelSinkProvider clientProviderChain,
			IServerChannelSinkProvider serverProviderChain
			) {
			_clientChannel = new PipeClientChannel(properties, clientProviderChain);
			_serverChannel = new PipeServerChannel(properties, serverProviderChain);
		}

		// IChannel
		public String ChannelName     { get { return(_clientChannel.ChannelName); } }
		public int    ChannelPriority { get { return(_clientChannel.ChannelPriority); } }
        
		public String Parse(String url, out string uri) {
			return(PipeConnection.Parse(url, out uri));
		}
        
		// IChannelSender
		public IMessageSink CreateMessageSink(String url, Object data, out String uri) {
			return _clientChannel.CreateMessageSink(url, data, out uri);
		}

		// IChannelReciever
		public Object ChannelData {
			get { return (_serverChannel == null) ? null : _serverChannel.ChannelData; }
		}

		public String[] GetUrlsForUri(String objectURI) {
			if (_serverChannel != null) {
				return _serverChannel.GetUrlsForUri(objectURI);            
			}
			else {
				return null;
			}
		}
       
		public void StartListening(Object data) {
			if (_serverChannel != null)
				_serverChannel.StartListening(data);
		}

		public void StopListening(Object data) {
			if (_serverChannel != null)
				_serverChannel.StopListening(data);
		}

		public void Dispose() {
			if (_serverChannel != null)
				_serverChannel.Dispose();

			if (_clientChannel != null)
				_clientChannel.Dispose();
		}

	}
}
