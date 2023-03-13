using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using System.Net;
using System.Collections.Specialized;

namespace Shellnet {
	/// <summary>
	/// frmMonitor の概要の説明です。
	/// </summary>
	class frmMonitor : System.Windows.Forms.Form {
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Timer tmRefresh;
		private System.Windows.Forms.MenuItem menuFile;
		private System.Windows.Forms.MenuItem menuFileExitApplication;
		private System.Windows.Forms.MenuItem menuHost;
		private System.Windows.Forms.MenuItem menuHostCreateExternalHost;
		private System.Windows.Forms.MenuItem menuSelect;
		private System.Windows.Forms.MenuItem menuSelectAllHosts;
		private System.Windows.Forms.MenuItem menuSelectNoItems;
		private System.Windows.Forms.MenuItem menuSelectAllPeers;
		private System.Windows.Forms.MenuItem menuSelectAllSessions;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.PictureBox figure;
		private System.Windows.Forms.ToolBarButton tbbAutoUpdate;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton tbbMove;
		private System.Windows.Forms.ToolBarButton tbbSelect;
		private System.Windows.Forms.ToolBar tbMain;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.ToolBarButton tbbScaleWhole;
		private System.Windows.Forms.ToolBarButton tbbScaleWide;
		private System.Windows.Forms.ToolBarButton tbbScaleNarrow;
		private System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.ContextMenu menuPeer;
		private System.Windows.Forms.ContextMenu menuConnection;
		private System.Windows.Forms.MenuItem menuConnectionDisconnect;
		private System.Windows.Forms.MenuItem menuPeerNetsystemDown;
		public Shellnet shellnet;

		public frmMonitor(Shellnet shellnet) {
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
			//
			this.shellnet = shellnet;
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
			this.components = new System.ComponentModel.Container();
			this.tmRefresh = new System.Windows.Forms.Timer(this.components);
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.menuFile = new System.Windows.Forms.MenuItem();
			this.menuFileExitApplication = new System.Windows.Forms.MenuItem();
			this.menuHost = new System.Windows.Forms.MenuItem();
			this.menuHostCreateExternalHost = new System.Windows.Forms.MenuItem();
			this.menuSelect = new System.Windows.Forms.MenuItem();
			this.menuSelectNoItems = new System.Windows.Forms.MenuItem();
			this.menuSelectAllHosts = new System.Windows.Forms.MenuItem();
			this.menuSelectAllPeers = new System.Windows.Forms.MenuItem();
			this.menuSelectAllSessions = new System.Windows.Forms.MenuItem();
			this.statusBar = new System.Windows.Forms.StatusBar();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.figure = new System.Windows.Forms.PictureBox();
			this.tbbAutoUpdate = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
			this.tbbMove = new System.Windows.Forms.ToolBarButton();
			this.tbbSelect = new System.Windows.Forms.ToolBarButton();
			this.tbMain = new System.Windows.Forms.ToolBar();
			this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
			this.tbbScaleWide = new System.Windows.Forms.ToolBarButton();
			this.tbbScaleNarrow = new System.Windows.Forms.ToolBarButton();
			this.tbbScaleWhole = new System.Windows.Forms.ToolBarButton();
			this.menuPeer = new System.Windows.Forms.ContextMenu();
			this.menuPeerNetsystemDown = new System.Windows.Forms.MenuItem();
			this.menuConnection = new System.Windows.Forms.ContextMenu();
			this.menuConnectionDisconnect = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// tmRefresh
			// 
			this.tmRefresh.Enabled = true;
			this.tmRefresh.Tick += new System.EventHandler(this.tmRefresh_Tick);
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuFile,
																					 this.menuHost,
																					 this.menuSelect});
			// 
			// menuFile
			// 
			this.menuFile.Index = 0;
			this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuFileExitApplication});
			this.menuFile.Text = "ファイル";
			// 
			// menuFileExitApplication
			// 
			this.menuFileExitApplication.Index = 0;
			this.menuFileExitApplication.Text = "終了";
			// 
			// menuHost
			// 
			this.menuHost.Index = 1;
			this.menuHost.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuHostCreateExternalHost});
			this.menuHost.Text = "ホスト";
			// 
			// menuHostCreateExternalHost
			// 
			this.menuHostCreateExternalHost.Index = 0;
			this.menuHostCreateExternalHost.Text = "外部ホストを作成";
			this.menuHostCreateExternalHost.Click += new System.EventHandler(this.menuHostCreateExternalHost_Click);
			// 
			// menuSelect
			// 
			this.menuSelect.Index = 2;
			this.menuSelect.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.menuSelectNoItems,
																					   this.menuSelectAllHosts,
																					   this.menuSelectAllPeers,
																					   this.menuSelectAllSessions});
			this.menuSelect.Text = "選択";
			// 
			// menuSelectNoItems
			// 
			this.menuSelectNoItems.Index = 0;
			this.menuSelectNoItems.Text = "選択解除";
			// 
			// menuSelectAllHosts
			// 
			this.menuSelectAllHosts.Index = 1;
			this.menuSelectAllHosts.Text = "全てのホストを選択";
			// 
			// menuSelectAllPeers
			// 
			this.menuSelectAllPeers.Index = 2;
			this.menuSelectAllPeers.Text = "全てのピアを選択";
			// 
			// menuSelectAllSessions
			// 
			this.menuSelectAllSessions.Index = 3;
			this.menuSelectAllSessions.Text = "全てのセッションを選択";
			// 
			// statusBar
			// 
			this.statusBar.Location = new System.Drawing.Point(0, 427);
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size(632, 22);
			this.statusBar.TabIndex = 9;
			this.statusBar.Text = "――起動しています";
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter1.Location = new System.Drawing.Point(0, 41);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(632, 3);
			this.splitter1.TabIndex = 12;
			this.splitter1.TabStop = false;
			// 
			// figure
			// 
			this.figure.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.figure.Dock = System.Windows.Forms.DockStyle.Fill;
			this.figure.Location = new System.Drawing.Point(0, 44);
			this.figure.Name = "figure";
			this.figure.Size = new System.Drawing.Size(632, 383);
			this.figure.TabIndex = 13;
			this.figure.TabStop = false;
			this.figure.Click += new System.EventHandler(this.figure_Click);
			this.figure.Paint += new System.Windows.Forms.PaintEventHandler(this.figure_Paint);
			this.figure.MouseUp += new System.Windows.Forms.MouseEventHandler(this.figure_MouseUp);
			this.figure.DoubleClick += new System.EventHandler(this.figure_DoubleClick);
			this.figure.MouseMove += new System.Windows.Forms.MouseEventHandler(this.figure_MouseMove);
			this.figure.MouseDown += new System.Windows.Forms.MouseEventHandler(this.figure_MouseDown);
			// 
			// tbbAutoUpdate
			// 
			this.tbbAutoUpdate.Pushed = true;
			this.tbbAutoUpdate.Text = "更新";
			// 
			// toolBarButton1
			// 
			this.toolBarButton1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbbMove
			// 
			this.tbbMove.Pushed = true;
			this.tbbMove.Text = "移動";
			// 
			// tbbSelect
			// 
			this.tbbSelect.Text = "選択";
			// 
			// tbMain
			// 
			this.tbMain.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																					  this.tbbAutoUpdate,
																					  this.toolBarButton1,
																					  this.tbbMove,
																					  this.tbbSelect,
																					  this.toolBarButton2,
																					  this.tbbScaleWide,
																					  this.tbbScaleNarrow,
																					  this.tbbScaleWhole});
			this.tbMain.ButtonSize = new System.Drawing.Size(31, 35);
			this.tbMain.DropDownArrows = true;
			this.tbMain.Location = new System.Drawing.Point(0, 0);
			this.tbMain.Name = "tbMain";
			this.tbMain.ShowToolTips = true;
			this.tbMain.Size = new System.Drawing.Size(632, 41);
			this.tbMain.TabIndex = 11;
			this.tbMain.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.tbMain_ButtonClick);
			// 
			// toolBarButton2
			// 
			this.toolBarButton2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbbScaleWide
			// 
			this.tbbScaleWide.Text = "－";
			this.tbbScaleWide.ToolTipText = "視野を広くして、縮小します。";
			// 
			// tbbScaleNarrow
			// 
			this.tbbScaleNarrow.Text = "＋";
			this.tbbScaleNarrow.ToolTipText = "視野を狭くして、拡大します。";
			// 
			// tbbScaleWhole
			// 
			this.tbbScaleWhole.Text = "全体";
			this.tbbScaleWhole.ToolTipText = "視点とスケールを変更して、すべてのプロセスを表示します。";
			// 
			// menuPeer
			// 
			this.menuPeer.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuPeerNetsystemDown});
			// 
			// menuPeerNetsystemDown
			// 
			this.menuPeerNetsystemDown.Index = 0;
			this.menuPeerNetsystemDown.Text = "ネットサブシステムの停止";
			this.menuPeerNetsystemDown.Click += new System.EventHandler(this.menuPeerNetsystemDown_Click);
			// 
			// menuConnection
			// 
			this.menuConnection.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						   this.menuConnectionDisconnect});
			// 
			// menuConnectionDisconnect
			// 
			this.menuConnectionDisconnect.Index = 0;
			this.menuConnectionDisconnect.Text = "この接続を切断";
			this.menuConnectionDisconnect.Click += new System.EventHandler(this.menuConnectionDisconnect_Click);
			// 
			// frmMonitor
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(632, 449);
			this.Controls.Add(this.figure);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.tbMain);
			this.Controls.Add(this.statusBar);
			this.Menu = this.mainMenu;
			this.Name = "frmMonitor";
			this.Text = "Shellnet モニタ";
			this.Load += new System.EventHandler(this.frmMonitor_Load);
			this.Closed += new System.EventHandler(this.frmMonitor_Closed);
			this.ResumeLayout(false);

		}
		#endregion

		Random rnd = new Random();
		ViewWorld world = new ViewWorld();
		Snapshot snapshot = null;

		private void SetupObjects() {

			lock(shellnet.Hosts.SyncRoot) {
				foreach(Host host in shellnet.Hosts) {
					if(!world.ContainsEntity(host)) {
						CircleItem item = new CircleItem(host);
						world.AddItem(item);
					}
					DrawingItem di = world.GetItemFromEntity(host);
					lock(host.Devices.SyncRoot) {
						foreach(Device device in host.Devices) {
							if(!di.ContainsEntity(device)) {
								CircleItem item = new CircleItem(device);
								item.OnBorders = true;
								item.Radius *= 0.4;
								di.AddItem(item);
							}
						}
					}
					lock(host.Peers.SyncRoot) {
						foreach(Peer peer in host.Peers) {
							if(!di.ContainsEntity(peer)) {
								CircleItem item = new CircleItem(peer);
								item.Foreground = Brushes.LightGreen;
								item.Location = Vector.Random();
								item.Labels.Add(peer.PeerId.ToString());
								di.AddItem(item);
							}
						}
					}
				}
			}

			lock(shellnet.Hosts.SyncRoot) {
				foreach(Host host in shellnet.Hosts) {
					lock(host.Peers.SyncRoot) {
						foreach(Peer peer in host.Peers) {
							lock(peer.Sockets.SyncRoot) {
								foreach(Socket socket in peer.Sockets) {
									if(socket.Connected && !world.ContainsEntity(socket)) {
										LineItem item = new LineItem(socket);
										item.Item1 = world.GetItemFromEntity(peer);
										item.Item2 = world.GetItemFromEntity(socket.RemoteDevice);
										if(item.Available) world.AddItem(item);
									}
									if(socket.Connected && !world.ContainsEntity(socket.Session)) {
										LineItem item = new LineItem(socket.Session);
										item.ForegroundPen = Pens.Black;
										item.ForegroundBrush = Brushes.Black;
										item.Item1 = world.GetItemFromEntity(socket.Owner);
										item.Item2 = world.GetItemFromEntity(socket.PairSocket.Owner);
										if(item.Available) world.AddItem(item);
									}
								}
							}
						}
					}
				}
			}

			snapshot = world.GetSnapshot();

		}

		// 描画ステータス
		Image img = null;
		Vector origin = Vector.Zero;
		double scale = 10;

		bool imageinvalidated = true;

		private void Draw() {
			imageinvalidated = true;
			figure.Invalidate();
			if(this.SelectedItem!=null && this.SelectedItem.Entity is Socket) {
				Socket socket = (Socket)this.SelectedItem.Entity;
				this.statusBar.Text = string.Format("受信{0}bytes / 送信{1}bytes"
					,socket.RecvSize,socket.SendSize);
			}
		}

		class WindowCoordinateConverter : CoordinateConverter {
			frmMonitor form;
			public WindowCoordinateConverter(frmMonitor form) {
				this.form = form;
			}
			public override double ConvertDistance(double l) {
				return l*form.scale;
			}
			public override Vector ConvertPosition(Vector p) {
				return (p-form.origin)*form.scale+(new Vector(form.img.Width/2.0,form.img.Height/2.0));
			}
		}

		private void tmRefresh_Tick(object sender, System.EventArgs e) {
			try {
				SetupObjects();
				world.BeginLayout();
				world.Perform();
				world.ForceFeedback();
				world.Layout();
				world.EndLayout();
				Draw();
			} catch {
			}
		}

		private void frmMonitor_Closed(object sender, System.EventArgs e) {
		}

		private void menuHostCreateExternalHost_Click(object sender, System.EventArgs e) {
			frmCreateExternalHost dlg = new frmCreateExternalHost(this);
			if(DialogResult.OK==dlg.ShowDialog(this)) {
				//Host host = new ExternalHost();
			}
		}

		private void tbMain_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e) {
			if(e.Button==this.tbbAutoUpdate) {
				e.Button.Pushed = !e.Button.Pushed;
				this.tmRefresh.Enabled = e.Button.Pushed;
			} else if(e.Button==this.tbbScaleWide) {
				scale /= 1.5;
				Draw();
				this.statusBar.Text = string.Format("スケール = {0:f05}",scale);
			} else if(e.Button==this.tbbScaleNarrow) {
				scale *= 1.5;
				Draw();
				this.statusBar.Text = string.Format("スケール = {0:f05}",scale);
			} else if(e.Button==this.tbbScaleWhole) {
				if(world.Items.Count==0) {
					// NOP
				} else if(world.Items.Count==1) {
					DrawingItem pi = (DrawingItem)world.Items[0];
					this.origin = pi.GlobalCenter;
				} else {
					Vector p0 = new Vector(double.MaxValue,double.MaxValue);
					Vector p1 = new Vector(double.MinValue,double.MinValue);
					foreach(DrawingItem pi in world.Items) {
						Vector p = pi.GlobalCenter;
						p0.x = Math.Min(p0.x,p.x);
						p0.y = Math.Min(p0.y,p.y);
						p1.x = Math.Max(p1.x,p.x);
						p1.y = Math.Max(p1.y,p.y);
					}
					this.origin = (p1+p0)/2;
					this.scale = Math.Min(
						(img.Width/2*0.7)/(p1.x-origin.x),
						(img.Height/2*0.7)/(p1.y-origin.y));
				}
				Draw();
			}
		}

		private Vector WindowToView(Vector p) {
			return (p-((new Vector(figure.Width,figure.Height))/2))/scale+origin;
		}

		private Vector ViewToWindow(Vector p) {
			return (p-origin)*scale + ((new Vector(figure.Width,figure.Height))/2);
		}

		private enum MouseOperationState {
			Initial,
			Abort,
			Move,
			Focus,
			Select,
		}

		private MouseOperationState mouseOpState = MouseOperationState.Initial;
		private MouseButtons initialButtons = MouseButtons.None;
		private Vector pointedPosition = Vector.Zero;
		private Vector currentPosition = Vector.Zero;
		private Vector offsetFromMouse = Vector.Zero;

		private DrawingItem SelectedItem;
		private DrawingItem FocusedItem;
		private void SetSelectedItem(DrawingItem item) {
			if(SelectedItem!=null) SelectedItem.Selected = false;
			if(SelectedItem!=item) Draw();
			SelectedItem = item;
			if(SelectedItem!=null) {
				SelectedItem.Selected = true;
			}
		}
		private void SetFocusedItem(DrawingItem item) {
			if(FocusedItem!=null) FocusedItem.Focused = false;
			if(FocusedItem!=item) Draw();
			FocusedItem = item;
			if(FocusedItem!=null) {
				FocusedItem.Focused = true;
				if(FocusedItem.Entity is Peer) {
					figure.ContextMenu = this.menuPeer;
				} else if(FocusedItem.Entity is Socket) {
					figure.ContextMenu = this.menuConnection;
				}
			} else {
				figure.ContextMenu = null;
			}
		}

		private void figure_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			try {
				switch(mouseOpState) {
				default:
					return;
				case MouseOperationState.Initial:
					initialButtons = e.Button;
					pointedPosition = WindowToView(new Vector(e.X,e.Y));
					switch(e.Button) {
					default:
						return;
					case MouseButtons.Left:
						DrawingItem hititem = null;
						if(snapshot!=null) {
							hititem = snapshot.GetItemAt(pointedPosition);
						}
						SetSelectedItem(hititem);
						if(hititem!=null) {
							mouseOpState = MouseOperationState.Focus;
							offsetFromMouse = hititem.GlobalCenter-pointedPosition;
							this.tmRefresh.Stop();
						} else {
							mouseOpState = MouseOperationState.Select;
							this.tmRefresh.Stop();
						}
						return;
					case MouseButtons.Right:
						mouseOpState = MouseOperationState.Move;
						return;
					}
				case MouseOperationState.Abort:
					return;
				case MouseOperationState.Move:
					return;
				case MouseOperationState.Select:
					if(e.Button==MouseButtons.Right) {
						this.tmRefresh.Enabled = this.tbbAutoUpdate.Pushed;
						mouseOpState = MouseOperationState.Abort;
					}
					return;
				}
			} finally {
				figure.Update();
			}
		}

		private void figure_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			switch(mouseOpState) {
			case MouseOperationState.Focus:
			case MouseOperationState.Select:
				this.tmRefresh.Enabled = this.tbbAutoUpdate.Pushed;
				break;
			}
			if(Control.MouseButtons==MouseButtons.None) {
				mouseOpState = MouseOperationState.Initial;
			}
			Draw();
		}

		private void figure_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			try {
				currentPosition = WindowToView(new Vector(e.X,e.Y));
				switch(mouseOpState) {
				default:
					this.statusBar.Text = string.Format("マウスの座標 (x, y) = ({0:f03}, {1:f03})"
						,currentPosition.x,currentPosition.y);
					DrawingItem hititem = null;
					if(snapshot!=null) hititem = snapshot.GetItemAt(currentPosition);
					this.SetFocusedItem(hititem);
					break;
				case MouseOperationState.Move:
					origin += pointedPosition-currentPosition;
					Draw();
					break;
				case MouseOperationState.Focus:
					SelectedItem.Move(-(SelectedItem.GlobalCenter-currentPosition)+SelectedItem.LocalCenter+offsetFromMouse);
					Draw();
					break;
				case MouseOperationState.Select:
					this.statusBar.Text = string.Format("範囲 (x0, y0)-(x1,y1) = ({0:f03}, {1:f03})-({2:f03}, {3:f03})"
						,pointedPosition.x,pointedPosition.y
						,currentPosition.x,currentPosition.y);
					Draw();
					break;
				}
			} catch(Exception ex) {
				this.statusBar.Text = ex.Message;
			}
		}

		private void figure_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			if(img==null || figure.Size!=img.Size) {
				img = new Bitmap(figure.Size.Width,figure.Size.Height);
				//figure.Image = img;
			}
			if(imageinvalidated) {
				if(snapshot==null || snapshot.Count==0) {
					using(Graphics g = Graphics.FromImage(img)) {
						g.Clear(Color.LightGray);
						using(Font font = new Font(FontFamily.GenericSerif,9)) {
							string text = "エンティティが存在しません";
							SizeF sz = g.MeasureString(text,font);
							g.DrawString(text,font,Brushes.Black
								,img.Width/2-sz.Width/2
								,img.Height/2-sz.Height/2
								);
						}
					}
				} else {
					using(Graphics g = Graphics.FromImage(img)) {
						CoordinateConverter conv = new WindowCoordinateConverter(this);
						g.Clear(Color.White);
						foreach(DrawingItem item in snapshot) {
							item.Draw(g,conv);
						}
						foreach(DrawingItem item in snapshot) {
							item.DrawText(g,conv);
						}
					}
				}
				imageinvalidated = false;
			}
			e.Graphics.DrawImage(img,0,0);
			if(this.mouseOpState==MouseOperationState.Select) {
				Point sp = ViewToWindow(pointedPosition);
				Point cp = ViewToWindow(currentPosition);
				Rectangle rc = Rectangle.FromLTRB(
					Math.Min(sp.X,cp.X),Math.Min(sp.Y,cp.Y),
					Math.Max(sp.X,cp.X),Math.Max(sp.Y,cp.Y)
					);
				using(Brush brush = new SolidBrush(Color.FromArgb(64,Color.Blue))) {
					e.Graphics.FillRectangle(brush,rc);
				}
				e.Graphics.DrawRectangle(Pens.Blue,rc);
			}
		}

		private void figure_Click(object sender, System.EventArgs e) {
		}

		private void menuConnectionDisconnect_Click(object sender, System.EventArgs e) {
			if(this.FocusedItem==null) return;
			if(!(this.FocusedItem.Entity is Socket)) return;
			Socket socket = (Socket)this.FocusedItem.Entity;
			socket.MakeConnectionDown();
		}

		private void menuPeerNetsystemDown_Click(object sender, System.EventArgs e) {
			if(this.FocusedItem==null) return;
			if(!(this.FocusedItem.Entity is Peer)) return;
			Peer peer = (Peer)this.FocusedItem.Entity;
			peer.MakeNetSubsystemDown();
		}

		private void figure_DoubleClick(object sender, System.EventArgs e) {
			if(this.SelectedItem==null || this.SelectedItem.Entity==null) return;
			object entity = this.SelectedItem.Entity;
			Form frm = null;
			if(entity is Peer) {
				frm = new frmStatPeer((Peer)entity);
			} else if(entity is Socket) {
				frm = new frmStatSocket((Socket)entity);
			} else return;
			frm.Show();
		}

		private void frmMonitor_Load(object sender, System.EventArgs e) {
		
		}

		public Segment SelectedNetwork {
			get {
				return shellnet.EnclosingNetwork;
			}
		}

	}

}
