using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using System.Net;

namespace Shellnet {
	/// <summary>
	/// frmStatPeer の概要の説明です。
	/// </summary>
	class frmStatPeer : System.Windows.Forms.Form {
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Label hostname;
		private System.Windows.Forms.Label socketnum;
		private System.Windows.Forms.Label listennum;
		private System.Windows.Forms.Label connectednum;
		private System.Windows.Forms.ListBox addresses;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label pid;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label ungraceful;
		private System.ComponentModel.IContainer components;

		public frmStatPeer(Peer peer) {
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
			//
			this.peer = peer;
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows フォーム デザイナで生成されたコード 
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.hostname = new System.Windows.Forms.Label();
			this.socketnum = new System.Windows.Forms.Label();
			this.listennum = new System.Windows.Forms.Label();
			this.connectednum = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.addresses = new System.Windows.Forms.ListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.pid = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.ungraceful = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// hostname
			// 
			this.hostname.Location = new System.Drawing.Point(168, 16);
			this.hostname.Name = "hostname";
			this.hostname.Size = new System.Drawing.Size(120, 23);
			this.hostname.TabIndex = 0;
			this.hostname.Text = "label1";
			this.hostname.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// socketnum
			// 
			this.socketnum.Location = new System.Drawing.Point(168, 168);
			this.socketnum.Name = "socketnum";
			this.socketnum.Size = new System.Drawing.Size(120, 23);
			this.socketnum.TabIndex = 2;
			this.socketnum.Text = "label3";
			this.socketnum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listennum
			// 
			this.listennum.Location = new System.Drawing.Point(168, 208);
			this.listennum.Name = "listennum";
			this.listennum.Size = new System.Drawing.Size(120, 23);
			this.listennum.TabIndex = 3;
			this.listennum.Text = "label4";
			this.listennum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// connectednum
			// 
			this.connectednum.Location = new System.Drawing.Point(168, 248);
			this.connectednum.Name = "connectednum";
			this.connectednum.Size = new System.Drawing.Size(120, 23);
			this.connectednum.TabIndex = 4;
			this.connectednum.Text = "label5";
			this.connectednum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(16, 16);
			this.label6.Name = "label6";
			this.label6.TabIndex = 5;
			this.label6.Text = "ホスト名";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(16, 96);
			this.label7.Name = "label7";
			this.label7.TabIndex = 6;
			this.label7.Text = "IPアドレス";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// addresses
			// 
			this.addresses.ItemHeight = 12;
			this.addresses.Location = new System.Drawing.Point(168, 96);
			this.addresses.Name = "addresses";
			this.addresses.Size = new System.Drawing.Size(120, 52);
			this.addresses.TabIndex = 7;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 168);
			this.label2.Name = "label2";
			this.label2.TabIndex = 8;
			this.label2.Text = "有効なソケット数";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(16, 208);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(112, 23);
			this.label8.TabIndex = 9;
			this.label8.Text = "Listen中のソケット数";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(16, 248);
			this.label9.Name = "label9";
			this.label9.TabIndex = 10;
			this.label9.Text = "接続中のソケット数";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Interval = 1000;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 56);
			this.label1.Name = "label1";
			this.label1.TabIndex = 11;
			this.label1.Text = "プロセスID";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pid
			// 
			this.pid.Location = new System.Drawing.Point(168, 56);
			this.pid.Name = "pid";
			this.pid.TabIndex = 12;
			this.pid.Text = "label3";
			this.pid.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 288);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(136, 23);
			this.label3.TabIndex = 13;
			this.label3.Text = "シャットダウンのないクローズ";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// ungraceful
			// 
			this.ungraceful.Location = new System.Drawing.Point(168, 288);
			this.ungraceful.Name = "ungraceful";
			this.ungraceful.Size = new System.Drawing.Size(120, 23);
			this.ungraceful.TabIndex = 0;
			this.ungraceful.Text = "654654";
			this.ungraceful.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// frmStatPeer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(306, 338);
			this.Controls.Add(this.ungraceful);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.pid);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.addresses);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.connectednum);
			this.Controls.Add(this.listennum);
			this.Controls.Add(this.socketnum);
			this.Controls.Add(this.hostname);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmStatPeer";
			this.Text = "プロセス情報";
			this.Load += new System.EventHandler(this.frmStatPeer_Load);
			this.ResumeLayout(false);

		}
		#endregion

		Peer peer;

		private void frmStatPeer_Load(object sender, System.EventArgs e) {
			timer1_Tick(null,null);
		}

		private void timer1_Tick(object sender, System.EventArgs e) {
			this.hostname.Text = peer.Host.HostName;
			this.pid.Text = peer.ProcessId.ToString();
			this.addresses.Items.Clear();
			foreach(Device device in peer.Host.Devices) {
				foreach(IPAddress addr in device.Addresses) {
					this.addresses.Items.Add(addr);
				}
			}
			this.socketnum.Text = peer.Sockets.Count.ToString();
			int lnum=0;
			int cnum=0;
			lock(peer.Sockets.SyncRoot) {
				foreach(Socket socket in peer.Sockets) {
					switch(socket.State) {
					case SocketState.LISTEN:
						++lnum;
						break;
					case SocketState.CONNECT:
						++cnum;
						break;
					}
				}
			}
			this.listennum.Text = lnum.ToString()+" 個";
			this.connectednum.Text = cnum.ToString()+" 個";
			this.ungraceful.Text = peer.NumberOfNongraceful.ToString()+" 回";
		}
	}
}
