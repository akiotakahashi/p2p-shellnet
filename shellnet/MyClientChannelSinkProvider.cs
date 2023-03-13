using System;
using System.IO;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;

namespace Shellnet {
	/// <summary>
	/// MyClientChannelSinkProvider ÇÃäTóvÇÃê‡ñæÇ≈Ç∑ÅB
	/// </summary>
	public class MyClientChannelSinkProvider : IClientChannelSinkProvider {

		private IClientChannelSinkProvider nextProvider;
		
		public MyClientChannelSinkProvider(IDictionary properties, ICollection providerData) {
		}
		
		public MyClientChannelSinkProvider(IClientChannelSinkProvider provider) {
			this.nextProvider = provider;
		}
	
		#region IClientChannelSinkProvider ÉÅÉìÉo

		public IClientChannelSink CreateSink(IChannelSender channel, string url, object remoteChannelData) {
			return new MyClientChannelSink(nextProvider.CreateSink(channel,url,remoteChannelData));
		}

		public IClientChannelSinkProvider Next {
			get {
				return this.nextProvider;
			}
			set {
				this.nextProvider = value;
			}
		}

		#endregion
	}
}
