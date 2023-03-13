using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;

namespace Shellnet {
	/// <summary>
	/// frmStatSocket の概要の説明です。
	/// </summary>
	class frmStatSocket : System.Windows.Forms.Form {
		private System.Windows.Forms.Label status;
		private System.Windows.Forms.Label localaddress;
		private System.Windows.Forms.Label remoteaddress;
		private System.Windows.Forms.Label recvsize;
		private System.Windows.Forms.Label sendsize;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Button button1;
		private System.ComponentModel.IContainer components;

		public frmStatSocket(Socket socket) {
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
			//
			this.socket = socket;
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
			this.status = new System.Windows.Forms.Label();
			this.localaddress = new System.Windows.Forms.Label();
			this.remoteaddress = new System.Windows.Forms.Label();
			this.recvsize = new System.Windows.Forms.Label();
			this.sendsize = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// status
			// 
			this.status.Location = new System.Drawing.Point(112, 8);
			this.status.Name = "status";
			this.status.Size = new System.Drawing.Size(120, 24);
			this.status.TabIndex = 0;
			this.status.Text = "label1";
			this.status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// localaddress
			// 
			this.localaddress.Location = new System.Drawing.Point(112, 48);
			this.localaddress.Name = "localaddress";
			this.localaddress.Size = new System.Drawing.Size(120, 23);
			this.localaddress.TabIndex = 1;
			this.localaddress.Text = "label2";
			this.localaddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// remoteaddress
			// 
			this.remoteaddress.Location = new System.Drawing.Point(112, 88);
			this.remoteaddress.Name = "remoteaddress";
			this.remoteaddress.Size = new System.Drawing.Size(120, 23);
			this.remoteaddress.TabIndex = 2;
			this.remoteaddress.Text = "label3";
			this.remoteaddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// recvsize
			// 
			this.recvsize.Location = new System.Drawing.Point(112, 128);
			this.recvsize.Name = "recvsize";
			this.recvsize.Size = new System.Drawing.Size(120, 23);
			this.recvsize.TabIndex = 3;
			this.recvsize.Text = "label4";
			this.recvsize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// sendsize
			// 
			this.sendsize.Location = new System.Drawing.Point(112, 168);
			this.sendsize.Name = "sendsize";
			this.sendsize.Size = new System.Drawing.Size(120, 23);
			this.sendsize.TabIndex = 4;
			this.sendsize.Text = "label5";
			this.sendsize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(8, 8);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(96, 24);
			this.label6.TabIndex = 5;
			this.label6.Text = "状態";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(8, 48);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(96, 23);
			this.label7.TabIndex = 6;
			this.label7.Text = "ローカルアドレス";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(8, 88);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(96, 23);
			this.label8.TabIndex = 7;
			this.label8.Text = "リモートアドレス";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(8, 128);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(96, 23);
			this.label9.TabIndex = 8;
			this.label9.Text = "受信量";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(8, 168);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(96, 23);
			this.label10.TabIndex = 9;
			this.label10.Text = "送信量";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Interval = 1000;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(256, 32);
			this.textBox1.MaxLength = 102400;
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox1.Size = new System.Drawing.Size(416, 128);
			this.textBox1.TabIndex = 10;
			this.textBox1.Text = "";
			// 
			// comboBox1
			// 
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.Items.AddRange(new object[] {
														   "ASCII",
														   "UTF-8",
														   "UTF-7",
														   "Unicode",
														   "Shift_JIS",
														   "EUC-JP"});
			this.comboBox1.Location = new System.Drawing.Point(256, 8);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(121, 20);
			this.comboBox1.TabIndex = 11;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(600, 168);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 24);
			this.button1.TabIndex = 12;
			this.button1.Text = "クリア";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// frmStatSocket
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(682, 202);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.sendsize);
			this.Controls.Add(this.recvsize);
			this.Controls.Add(this.remoteaddress);
			this.Controls.Add(this.localaddress);
			this.Controls.Add(this.status);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmStatSocket";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ソケット情報";
			this.Load += new System.EventHandler(this.frmStatSocket_Load);
			this.ResumeLayout(false);

		}
		#endregion

		Socket socket;
		private void frmStatSocket_Load(object sender, System.EventArgs e) {
			timer1_Tick(null,null);
		}

		private void timer1_Tick(object sender, System.EventArgs e) {
			this.status.Text = socket.State.ToString();
			if(socket.LocalEndPoint==null) {
				this.localaddress.Text = "(not available)";
			} else {
				this.localaddress.Text = socket.LocalEndPoint.ToString();
			}
			if(socket.RemoteEndPoint==null) {
				this.remoteaddress.Text = "(not available)";
			} else {
				this.remoteaddress.Text = socket.RemoteEndPoint.ToString();
			}
			this.recvsize.Text = socket.RecvSize.ToString()+" [byte]";
			this.sendsize.Text = socket.SendSize.ToString()+" [byte]";
		}

		private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e) {
			try {
				Encoding enc = Encoding.GetEncoding(comboBox1.Text);
				lock(socket.History) {
					textBox1.Text = enc.GetString(socket.History.ToArray());
				}
			} catch {
			}
		}

		private void button1_Click(object sender, System.EventArgs e) {
			lock(socket.History) socket.History.SetLength(0);
		}
	}
}
