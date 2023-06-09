/*=====================================================================

  File:        PipeServerChannel.cs

=====================================================================*/

using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Channels;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Runtime.Remoting.Channels.Pipe {
	public class PipeServerChannel : BaseChannelWithProperties, IChannel, IChannelReceiver {
		private const String  ChannelScheme = "pipe";

		private const int     DefaultChannelPriority=1;
		private const String  DefaultChannelName = "Pipe";    

		private int m_ChannelPriority;
		private String m_ChannelName;

		private String m_pipeName = null;
		private IntPtr m_pipeSecurityDescriptor = IntPtr.Zero;
		private Thread                     _listener;
		private AutoResetEvent             _event;

		private IServerChannelSinkProvider _serverSinkProvider;
		private PipeServerTransportSink    _transportSink;

		private PipeConnection             _pipe;
		private ChannelDataStore           _data;

		// Internal accessors:
		internal String PipeName { get { return(m_pipeName); } }

		public PipeServerChannel(string name, IServerChannelSinkProvider serverSinkProvider) {
			m_pipeName = name;
			InitDefaults();
			InitProperties(null);
			InitProviders(serverSinkProvider);
		}

		public PipeServerChannel(string name) : this(name, new SoapServerFormatterSinkProvider()) {
		}

		// CTOR used via the configuration file
		public PipeServerChannel(IDictionary properties, IServerChannelSinkProvider serverProviderChain) {
			InitDefaults();
			InitProperties(properties);
			InitProviders(serverProviderChain);
		}

		internal void InitDefaults() {
			m_ChannelPriority = DefaultChannelPriority;
			m_ChannelName = DefaultChannelName;
		}

		internal void InitProperties(IDictionary properties) {
            
			if(properties != null) {
				foreach (DictionaryEntry entry in properties) {
					switch ((String) entry.Key) {
					case "name": m_ChannelName = (String) entry.Value;
						break;

					case "priority": m_ChannelPriority = Convert.ToInt32(entry.Value);
						break;

					case "pipe": m_pipeName = (String) entry.Value;
                        
						if (String.Compare(m_pipeName, "Auto", true) == 0) {
							m_pipeName = Guid.NewGuid().ToString();
						}
						break;

					case "securityDescriptor": m_pipeSecurityDescriptor = (IntPtr) entry.Value;
						break;
					}
				}
			}
		}

		internal void InitProviders(IServerChannelSinkProvider serverProviderChain) {
			_listener = null;
			_event = new AutoResetEvent(false);

			_data = new ChannelDataStore(null);
			_data.ChannelUris = new String[1];
			_data.ChannelUris[0] = ChannelScheme + "://" + m_pipeName;

			_serverSinkProvider = serverProviderChain;

			// Create the default sink chain if one was not passed in
			if(_serverSinkProvider == null) {
				_serverSinkProvider = CreateDefaultServerProviderChain();
			}

			// Collect the rest of the channel data:
			IServerChannelSinkProvider provider = _serverSinkProvider;
			while(provider != null) {
				provider.GetChannelData(_data);
				provider = provider.Next;
			}

			IServerChannelSink next = ChannelServices.CreateServerChannelSinkChain(_serverSinkProvider, this);
			_transportSink = new PipeServerTransportSink(next);

			StartListening(null);
		}

		private IServerChannelSinkProvider CreateDefaultServerProviderChain() {
			SoapServerFormatterSinkProvider serverSinkProvider = new SoapServerFormatterSinkProvider();
			serverSinkProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
			return serverSinkProvider;
		} // CreateDefaultServerProviderChain

		// IChannel
		public String ChannelName { get { return(m_ChannelName); } }
		public int    ChannelPriority { get { return(1); } }

		public String Parse(String url, out string uri) {
			return(PipeConnection.Parse(url, out uri));
		}

		// IChannelReceiver
		public String[] GetUrlsForUri(String objuri) {
			DBG.Info(null, "GetUrlsForUri: Looking up URL for uri = " + objuri);
			String[] arr = new String[1];
            
			if(!objuri.StartsWith("/"))
				objuri = "/" + objuri;

			arr[0] = ChannelScheme + "://" + m_pipeName + objuri;

			return(arr);
		}

		public void StartListening(Object data) {
			DBG.Info(null, "Starting to listen...");

			_listener = new Thread(new ThreadStart(this.ListenerMain));
			_listener.IsBackground = true;
			_listener.Start();
		}

		public void StopListening(Object data) {
			if (_listener != null) {
				DBG.Info(null, "Stop the listening thread...");
				_listener.Abort();
				_listener = null;
			}
		}

		public Object ChannelData {
			get { 
				DBG.Info(null, "ChannelData");
				// Return a blob that can be use to reconnect:
				return(_data);
			}
		}

		private void ListenerMain() {
			// Common ThreadStart delegate
			ThreadStart ts = new ThreadStart(this.ServerMain);

			Thread.CurrentThread.IsBackground = true;

			while(true) {
				try {
					_pipe  = new PipeConnection(m_pipeName, true, m_pipeSecurityDescriptor);

					//TODO switch to use completion ports
					// Wait for a client to connect
					bool connected = _pipe.WaitForConnect();

					if (!connected) {
						throw new PipeIOException("Could not connect to the pipe - os returned " + Marshal.GetLastWin32Error());
					}
					else {
						//TODO Consider using ThreadPool.QueueUserWorkItem

						Thread server = new Thread(ts);
						server.IsBackground = true;
						server.Start(); 
					}

					// Wait for the handler to spin up:
					_event.WaitOne();
				}
				catch(Exception) {
				}

			}
		}

		//public EventHandler OnTerminate;

		private void ServerMain() {
			Thread.CurrentThread.Name = "PipeServerChannel.ServerMain";
			PipeConnection pipe = _pipe;
			_pipe = null;

			// Signal the guy to start waiting again... (w/ new event and endpoint)
			_event.Set();
            
			try {
				//TODO close the connection on a timeout 
				//TODO if no activity for Nnnn milliseconds
				while(true) {
					ITransportHeaders headers;
					Stream request;
					try {
						pipe.BeginReadMessage();
					} catch {
						//if(OnTerminate!=null) OnTerminate(null,null);
						Console.WriteLine("Pipe was disconnected from client.");
						throw;
					}
					headers = pipe.ReadHeaders();
					request = pipe.ReadStream();
					pipe.EndReadMessage();

					ServerChannelSinkStack stack = new ServerChannelSinkStack();
					stack.Push(_transportSink, null);
                    
					IMessage responseMsg;
					ITransportHeaders responseHeaders;
					Stream responseStream;
                    
					ServerProcessing processing = _transportSink.NextChannelSink.ProcessMessage(stack, 
						null,
						headers,
						request,
						out responseMsg,
						out responseHeaders,
						out responseStream);
                    
					// handle response
					switch (processing) {
					case ServerProcessing.Complete:
						// Send the response. Call completed synchronously.
						stack.Pop(_transportSink);
						WriteClientResponse(pipe, responseHeaders, responseStream);
						break;
					case ServerProcessing.OneWay:
						WriteClientResponse(pipe, responseHeaders, null);
						break;
					case ServerProcessing.Async:
						stack.StoreAndDispatch(_transportSink, null);
						break;
					}
				}
			} catch(Exception e) {
				DBG.Info(null, "Terminating client connection: " + e);
			} finally {
				pipe.Dispose();
			}
		}

		private void WriteClientResponse(PipeConnection pipe,
			ITransportHeaders headers,
			Stream responseStream) {
			String uri;
			Object oUri = headers[CommonTransportKeys.RequestUri];
			if (oUri != null) {
				uri = oUri.ToString();
			}
			else {
				uri = "";
			}

			pipe.BeginWriteMessage();
			pipe.WriteHeaders(
				uri,
				headers);

			pipe.Write(responseStream);
			pipe.EndWriteMessage();
		}

		public void Dispose() {
			StopListening(null);

			if (_pipe != null) {
				_pipe.Dispose();
				_pipe = null;
			}

			//TODO: Cancel listeners.
		}
	}

	internal class PipeServerTransportSink : IServerChannelSink {
		private IServerChannelSink _next;
        
		public PipeServerTransportSink(IServerChannelSink next) {
			_next = next;
		}

		public ServerProcessing ProcessMessage(IServerChannelSinkStack sinkStack,
			IMessage requestMsg,
			ITransportHeaders requestHeaders, 
			Stream requestStream,
			out IMessage msg, 
			out ITransportHeaders responseHeaders,
			out Stream responseStream)
		{
			// NOTE: This doesn't have to be implemented because the server transport
			//   sink is always first.
			throw new NotSupportedException();
		}
           

		public void AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack, Object state,
			IMessage msg, ITransportHeaders headers, Stream stream)
		{
			throw new NotSupportedException();
		} // AsyncProcessResponse


		public Stream GetResponseStream(IServerResponseChannelSinkStack sinkStack, Object state,
			IMessage msg, ITransportHeaders headers)
		{            
			// We always want a stream to read from.
			return null;
		} // GetResponseStream


		public IServerChannelSink NextChannelSink {
			get { return _next; }
		}


		public IDictionary Properties {
			get { return null; }
		} // Properties
        
		//
		// end of IServerChannelSink implementation
		//        
	}


	[Serializable]
	internal class PipeChannelData {
		private String m_pipeName;
        
		internal PipeChannelData(PipeServerChannel chan) {
			m_pipeName = chan.PipeName;
			//TODO:  machine-name, etc.
		}

		internal String PipeName { get { return(m_pipeName); } }
	}
}
