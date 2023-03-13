using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using System.Net;
using System.Net.Sockets;
using System.Threading;

using System.Runtime.InteropServices;

namespace ws2simpleapp {
	/// <summary>
	/// Form1 の概要の説明です。
	/// </summary>
	public class Form1 : System.Windows.Forms.Form {
		private System.Windows.Forms.Label status;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.CheckedListBox peers;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox listenPort;
		private System.Windows.Forms.Button ctrlAbortListening;
		private System.Windows.Forms.Button ctrlBeginListening;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox connectPort;
		private System.Windows.Forms.ComboBox connectAddress;
		private System.Windows.Forms.Button ctrlBeginConnecting;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.Button ctrlSendMsg;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button ctrlDisconnect;
		private System.Windows.Forms.ListBox connections;
		private System.Windows.Forms.MenuItem menuPeer;
		private System.Windows.Forms.MenuItem menuFile;
		private System.Windows.Forms.MenuItem menuPeerCreate;
		private System.Windows.Forms.MenuItem menuFileExit;
		private System.Windows.Forms.ToolBar tbMain;
		private System.Windows.Forms.ToolBarButton tbbCreatePeer;
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1() {
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
			//
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
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
			this.status = new System.Windows.Forms.Label();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuFile = new System.Windows.Forms.MenuItem();
			this.menuFileExit = new System.Windows.Forms.MenuItem();
			this.menuPeer = new System.Windows.Forms.MenuItem();
			this.menuPeerCreate = new System.Windows.Forms.MenuItem();
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.tbMain = new System.Windows.Forms.ToolBar();
			this.tbbCreatePeer = new System.Windows.Forms.ToolBarButton();
			this.peers = new System.Windows.Forms.CheckedListBox();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.label4 = new System.Windows.Forms.Label();
			this.listenPort = new System.Windows.Forms.ComboBox();
			this.ctrlAbortListening = new System.Windows.Forms.Button();
			this.ctrlBeginListening = new System.Windows.Forms.Button();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.connectPort = new System.Windows.Forms.ComboBox();
			this.connectAddress = new System.Windows.Forms.ComboBox();
			this.ctrlBeginConnecting = new System.Windows.Forms.Button();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.ctrlSendMsg = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.ctrlDisconnect = new System.Windows.Forms.Button();
			this.connections = new System.Windows.Forms.ListBox();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.SuspendLayout();
			// 
			// status
			// 
			this.status.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.status.Location = new System.Drawing.Point(8, 8);
			this.status.Name = "status";
			this.status.Size = new System.Drawing.Size(568, 16);
			this.status.TabIndex = 1;
			this.status.Text = "label1";
			this.status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuFile,
																					  this.menuPeer});
			// 
			// menuFile
			// 
			this.menuFile.Index = 0;
			this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuFileExit});
			this.menuFile.Text = "ファイル";
			// 
			// menuFileExit
			// 
			this.menuFileExit.Index = 0;
			this.menuFileExit.Text = "終了";
			this.menuFileExit.Click += new System.EventHandler(this.menuFileExit_Click);
			// 
			// menuPeer
			// 
			this.menuPeer.Index = 1;
			this.menuPeer.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuPeerCreate});
			this.menuPeer.Text = "ピア";
			// 
			// menuPeerCreate
			// 
			this.menuPeerCreate.Index = 0;
			this.menuPeerCreate.Text = "新しいピアを起動";
			this.menuPeerCreate.Click += new System.EventHandler(this.menuPeerCreate_Click);
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point(0, 395);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Size = new System.Drawing.Size(584, 22);
			this.statusBar1.TabIndex = 6;
			this.statusBar1.Text = "statusBar1";
			// 
			// tbMain
			// 
			this.tbMain.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																					  this.tbbCreatePeer});
			this.tbMain.DropDownArrows = true;
			this.tbMain.Location = new System.Drawing.Point(0, 0);
			this.tbMain.Name = "tbMain";
			this.tbMain.ShowToolTips = true;
			this.tbMain.Size = new System.Drawing.Size(584, 41);
			this.tbMain.TabIndex = 0;
			this.tbMain.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.tbMain_ButtonClick);
			// 
			// tbbCreatePeer
			// 
			this.tbbCreatePeer.Text = "ピア作成";
			// 
			// peers
			// 
			this.peers.Dock = System.Windows.Forms.DockStyle.Left;
			this.peers.Location = new System.Drawing.Point(0, 41);
			this.peers.Name = "peers";
			this.peers.Size = new System.Drawing.Size(120, 354);
			this.peers.TabIndex = 2;
			this.peers.SelectedIndexChanged += new System.EventHandler(this.peers_SelectedIndexChanged);
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(120, 41);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 354);
			this.splitter1.TabIndex = 3;
			this.splitter1.TabStop = false;
			// 
			// textBox2
			// 
			this.textBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.textBox2.Location = new System.Drawing.Point(123, 307);
			this.textBox2.Multiline = true;
			this.textBox2.Name = "textBox2";
			this.textBox2.ReadOnly = true;
			this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox2.Size = new System.Drawing.Size(461, 88);
			this.textBox2.TabIndex = 7;
			this.textBox2.Text = "";
			// 
			// splitter2
			// 
			this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter2.Location = new System.Drawing.Point(123, 304);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(461, 3);
			this.splitter2.TabIndex = 8;
			this.splitter2.TabStop = false;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(123, 41);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(461, 263);
			this.tabControl1.TabIndex = 9;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.label4);
			this.tabPage1.Controls.Add(this.listenPort);
			this.tabPage1.Controls.Add(this.ctrlAbortListening);
			this.tabPage1.Controls.Add(this.ctrlBeginListening);
			this.tabPage1.Location = new System.Drawing.Point(4, 21);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(453, 238);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "接続待機";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(16, 24);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 16);
			this.label4.TabIndex = 0;
			this.label4.Text = "ポート";
			// 
			// listenPort
			// 
			this.listenPort.Items.AddRange(new object[] {
															"5000",
															"5001",
															"5002",
															"5003",
															"5004",
															"5005",
															"5006",
															"5007",
															"5008",
															"5009"});
			this.listenPort.Location = new System.Drawing.Point(16, 40);
			this.listenPort.Name = "listenPort";
			this.listenPort.Size = new System.Drawing.Size(121, 20);
			this.listenPort.TabIndex = 1;
			this.listenPort.Text = "80";
			// 
			// ctrlAbortListening
			// 
			this.ctrlAbortListening.Enabled = false;
			this.ctrlAbortListening.Location = new System.Drawing.Point(144, 80);
			this.ctrlAbortListening.Name = "ctrlAbortListening";
			this.ctrlAbortListening.Size = new System.Drawing.Size(80, 40);
			this.ctrlAbortListening.TabIndex = 3;
			this.ctrlAbortListening.Text = "待機中止";
			this.ctrlAbortListening.Click += new System.EventHandler(this.ctrlAbortListening_Click);
			// 
			// ctrlBeginListening
			// 
			this.ctrlBeginListening.Location = new System.Drawing.Point(144, 32);
			this.ctrlBeginListening.Name = "ctrlBeginListening";
			this.ctrlBeginListening.Size = new System.Drawing.Size(80, 40);
			this.ctrlBeginListening.TabIndex = 2;
			this.ctrlBeginListening.Text = "待機開始";
			this.ctrlBeginListening.Click += new System.EventHandler(this.ctrlBeginListening_Click);
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.label3);
			this.tabPage2.Controls.Add(this.label2);
			this.tabPage2.Controls.Add(this.connectPort);
			this.tabPage2.Controls.Add(this.connectAddress);
			this.tabPage2.Controls.Add(this.ctrlBeginConnecting);
			this.tabPage2.Location = new System.Drawing.Point(4, 21);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(453, 238);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "接続開始";
			this.tabPage2.Visible = false;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(168, 32);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 16);
			this.label3.TabIndex = 2;
			this.label3.Text = "ポート";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(96, 16);
			this.label2.TabIndex = 0;
			this.label2.Text = "IPアドレス";
			// 
			// connectPort
			// 
			this.connectPort.Items.AddRange(new object[] {
															 "5000",
															 "5001",
															 "5002",
															 "5003",
															 "5004",
															 "5005",
															 "5006",
															 "5007",
															 "5008",
															 "5009"});
			this.connectPort.Location = new System.Drawing.Point(168, 48);
			this.connectPort.Name = "connectPort";
			this.connectPort.Size = new System.Drawing.Size(121, 20);
			this.connectPort.TabIndex = 3;
			this.connectPort.Text = "80";
			// 
			// connectAddress
			// 
			this.connectAddress.Items.AddRange(new object[] {
																"127.0.0.1",
																"192.138.1.1",
																"192.168.11.38",
																"133.78.201.69"});
			this.connectAddress.Location = new System.Drawing.Point(24, 48);
			this.connectAddress.Name = "connectAddress";
			this.connectAddress.Size = new System.Drawing.Size(136, 20);
			this.connectAddress.TabIndex = 1;
			this.connectAddress.Text = "127.0.0.1";
			// 
			// ctrlBeginConnecting
			// 
			this.ctrlBeginConnecting.Location = new System.Drawing.Point(304, 40);
			this.ctrlBeginConnecting.Name = "ctrlBeginConnecting";
			this.ctrlBeginConnecting.Size = new System.Drawing.Size(75, 32);
			this.ctrlBeginConnecting.TabIndex = 4;
			this.ctrlBeginConnecting.Text = "に接続開始";
			this.ctrlBeginConnecting.Click += new System.EventHandler(this.ctrlBeginConnecting_Click);
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.ctrlSendMsg);
			this.tabPage3.Controls.Add(this.textBox1);
			this.tabPage3.Controls.Add(this.ctrlDisconnect);
			this.tabPage3.Controls.Add(this.connections);
			this.tabPage3.Location = new System.Drawing.Point(4, 21);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(453, 238);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "確立済み接続";
			this.tabPage3.Visible = false;
			// 
			// ctrlSendMsg
			// 
			this.ctrlSendMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ctrlSendMsg.Location = new System.Drawing.Point(381, 199);
			this.ctrlSendMsg.Name = "ctrlSendMsg";
			this.ctrlSendMsg.Size = new System.Drawing.Size(64, 23);
			this.ctrlSendMsg.TabIndex = 3;
			this.ctrlSendMsg.Text = "送信";
			this.ctrlSendMsg.Click += new System.EventHandler(this.ctrlSendMsg_Click);
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point(16, 151);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(357, 72);
			this.textBox1.TabIndex = 2;
			this.textBox1.Text = "GET / HTTP/1.0";
			// 
			// ctrlDisconnect
			// 
			this.ctrlDisconnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ctrlDisconnect.Location = new System.Drawing.Point(381, 16);
			this.ctrlDisconnect.Name = "ctrlDisconnect";
			this.ctrlDisconnect.Size = new System.Drawing.Size(64, 23);
			this.ctrlDisconnect.TabIndex = 1;
			this.ctrlDisconnect.Text = "切断";
			this.ctrlDisconnect.Click += new System.EventHandler(this.ctrlDisconnect_Click);
			// 
			// connections
			// 
			this.connections.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.connections.ItemHeight = 12;
			this.connections.Location = new System.Drawing.Point(16, 16);
			this.connections.Name = "connections";
			this.connections.Size = new System.Drawing.Size(357, 112);
			this.connections.TabIndex = 0;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(584, 417);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.splitter2);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.peers);
			this.Controls.Add(this.tbMain);
			this.Controls.Add(this.statusBar1);
			this.Controls.Add(this.status);
			this.Menu = this.mainMenu1;
			this.Name = "Form1";
			this.Text = "コンソール";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.Closed += new System.EventHandler(this.Form1_Closed);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main() {
			Application.Run(new Form1());
		}

		[DllImport("ssp")]
		extern static int SNGetCurrentPeerId();
		[DllImport("ssp")]
		extern static int SNCreatePeer();
		[DllImport("ssp")]
		extern static int SNActivatePeer(int peerid);
		[DllImport("ssp")]
		extern static void SNShutdownPeer(int peerid);

		private void Form1_Load(object sender, System.EventArgs e) {
			if(System.Net.Sockets.Socket.SupportsIPv6) {
				status.Text = "システムはIPv6をサポートしています";
			} else {
				status.Text = "システムはIPv6をサポートしていません";
			}
			peers.Items.Add(SNGetCurrentPeerId(),CheckState.Unchecked);
		}

		class SocketItem {
			public Socket socket;
			public SocketItem(Socket socket) {
				this.socket = socket;
			}
			public override string ToString() {
				return socket.LocalEndPoint+" - "+socket.RemoteEndPoint;
			}
		}

		private void ctrlBeginListening_Click(object sender, System.EventArgs e) {
			ListenMain lm = new ListenMain();
			lm.form = this;
			lm.listen = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Unspecified);
			lm.listen.Bind(new IPEndPoint(IPAddress.Any,int.Parse(listenPort.Text)));
			listenPort.Text = (int.Parse(listenPort.Text)+1).ToString();
			lm.listen.Listen(-1);
			this.Text = lm.listen.LocalEndPoint.ToString();
			(new Thread(new ThreadStart(lm.Main))).Start();
			/*
			ctrlBeginListening.Enabled = false;
			ctrlAbortListening.Enabled = true;
			*/
		}

		class ListenMain {
			public Socket listen;
			public Form1 form;
			public void Main() {
				Console.Error.WriteLine("ListenMain {0}",listen.LocalEndPoint);
				Thread.CurrentThread.IsBackground = true;
				while(true) {
					Socket socket = listen.Accept();
					ServerMain sm = new ServerMain();
					sm.form = form;
					sm.socket = socket;
					sm.Run();
					//connections.Items.Add(new SocketItem(socket));
				}
			}
		}

		class ServerMain {
			delegate void AppendTextDelegate(string text);
			public Socket socket;
			public Form1 form;
			public void Run() {
				(new Thread(new ThreadStart(Main))).Start();
			}
			private void Main() {
				Console.Error.WriteLine("ServerMain {0}",socket.LocalEndPoint);
				Thread.CurrentThread.IsBackground = true;
				try {
					using(StreamReader reader = new StreamReader(new NetworkStream(socket))) {
						while(socket.Connected) {
							string text = socket.RemoteEndPoint+">"+reader.ReadLine()+"\r\n";
							if(form.textBox2.TextLength+text.Length>=form.textBox2.MaxLength) {
								form.textBox2.SelectionStart = 0;
								form.textBox2.SelectionLength = form.textBox2.MaxLength/2;
								form.textBox2.SelectedText = "";
							}
							form.textBox2.Invoke(new AppendTextDelegate(form.textBox2.AppendText), new object[]{text});
							form.textBox2.SelectionStart = form.textBox2.TextLength;
						}
					}
				} catch(Exception ex) {
					Console.Error.WriteLine(ex);
				} finally {
					Console.Error.WriteLine("exit ServerMain {0}",socket.LocalEndPoint);
				}
			}
		}

		private void ctrlBeginConnecting_Click(object sender, System.EventArgs e) {
			Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Unspecified);
			socket.Connect(new IPEndPoint(IPAddress.Parse(connectAddress.Text),int.Parse(connectPort.Text)));
			connections.Items.Add(new SocketItem(socket));
			ServerMain sm = new ServerMain();
			sm.form = this;
			sm.socket = socket;
			sm.Run();
		}

		private void ctrlAbortListening_Click(object sender, System.EventArgs e) {
			ctrlBeginListening.Enabled = true;
			ctrlAbortListening.Enabled = false;
		}

		private void ctrlDisconnect_Click(object sender, System.EventArgs e) {
			SocketItem item = (SocketItem)connections.SelectedItem;
			if(item!=null) {
				item.socket.Close();
				if(connections.Items.Count>1) {
					connections.Items.Remove(item);
					connections.SelectedIndex++;
				} else {
					connections.Items.Clear();
				}
			}
		}

		private void peers_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(peers.SelectedIndex>=0) {
				SNActivatePeer((int)peers.SelectedItem);
			}
		}

		private void ctrlDeletePeer_Click(object sender, System.EventArgs e) {
			foreach(int peerid in new ArrayList(peers.CheckedItems)) {
				if(peerid!=SNGetCurrentPeerId()) {
					SNShutdownPeer(peerid);
					peers.Items.Remove(peerid);
				}
			}
		}

		private void Form1_Closed(object sender, System.EventArgs e) {
			Application.Exit();
		}

		private void ctrlSendMsg_Click(object sender, System.EventArgs e) {
			try {
				SocketItem item = (SocketItem)connections.SelectedItem;
				if(item==null) return;
				textBox2.AppendText("送信:"+textBox1.Text+" to "+item.socket.RemoteEndPoint+" from "+item.socket.LocalEndPoint+"\r\n");
				textBox2.SelectionStart = textBox2.TextLength;
				item.socket.Send(System.Text.Encoding.UTF8.GetBytes(this.textBox1.Text+"\r\n"));
			} catch(Exception ex) {
				Console.Error.WriteLine(ex);
			}
		}

		private void menuPeerCreate_Click(object sender, System.EventArgs e) {
			peers.SelectedIndex = peers.Items.Add(SNCreatePeer(),CheckState.Unchecked);
		}

		private void menuFileExit_Click(object sender, System.EventArgs e) {
			Application.Exit();
		}

		private void tbMain_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e) {
			if(e.Button==this.tbbCreatePeer) {
				menuPeerCreate_Click(sender,e);
			}
		}

	}
}
