using System;
using System.IO;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;

namespace Shellnet {

	class MyServerChannelSinkProvider : IServerChannelSinkProvider {

		private IServerChannelSinkProvider nextProvider;

		public MyServerChannelSinkProvider(IDictionary properties, ICollection providerData) {
		}

		public MyServerChannelSinkProvider(IServerChannelSinkProvider provider) {
			this.nextProvider = provider;
		}

		#region IServerChannelSinkProvider �����o

		public IServerChannelSink CreateSink(IChannelReceiver channel) {
			return new MyServerChannelSink(nextProvider.CreateSink(channel));
		}

		public IServerChannelSinkProvider Next {
			get {
				return nextProvider;
			}
			set {
				nextProvider = value;
			}
		}

		public void GetChannelData(IChannelDataStore channelData) {
		}

		#endregion

	}
}
