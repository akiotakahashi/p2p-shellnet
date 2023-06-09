using System;
using System.IO;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;

namespace Shellnet {
	/// <summary>
	/// MyClientChannelSinkProvider の概要の説明です。
	/// </summary>
	public class MyClientChannelSinkProvider : IClientChannelSinkProvider {

		private IClientChannelSinkProvider nextProvider;
		
		public MyClientChannelSinkProvider(IDictionary properties, ICollection providerData) {
		}
		
		public MyClientChannelSinkProvider(IClientChannelSinkProvider provider) {
			this.nextProvider = provider;
		}
	
		#region IClientChannelSinkProvider メンバ

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
