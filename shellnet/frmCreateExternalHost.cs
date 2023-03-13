using System;
using System.Net;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Shellnet {
	/// <summary>
	/// frmCreateExternalHost の概要の説明です。
	/// </summary>
	class frmCreateExternalHost : System.Windows.Forms.Form {
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.TextBox dnsname;
		private System.Windows.Forms.TextBox intIPAddress;
		private System.Windows.Forms.TextBox extIPAddress;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button ctrlResolve;
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

		private frmMonitor monitor;

		public frmCreateExternalHost(frmMonitor monitor) {
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
			//
			this.monitor = monitor;
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
			this.intIPAddress = new System.Windows.Forms.TextBox();
			this.dnsname = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.extIPAddress = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.ctrlResolve = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// intIPAddress
			// 
			this.intIPAddress.Location = new System.Drawing.Point(136, 144);
			this.intIPAddress.Name = "intIPAddress";
			this.intIPAddress.Size = new System.Drawing.Size(216, 19);
			this.intIPAddress.TabIndex = 6;
			this.intIPAddress.Text = "";
			// 
			// dnsname
			// 
			this.dnsname.Location = new System.Drawing.Point(136, 40);
			this.dnsname.Name = "dnsname";
			this.dnsname.Size = new System.Drawing.Size(216, 19);
			this.dnsname.TabIndex = 1;
			this.dnsname.Text = "www.goo.ne.jp";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(40, 144);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 16);
			this.label1.TabIndex = 5;
			this.label1.Text = "内部IPアドレス";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(40, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 16);
			this.label2.TabIndex = 0;
			this.label2.Text = "DNS名";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(208, 200);
			this.button1.Name = "button1";
			this.button1.TabIndex = 7;
			this.button1.Text = "作成";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button2.Location = new System.Drawing.Point(296, 200);
			this.button2.Name = "button2";
			this.button2.TabIndex = 8;
			this.button2.Text = "中止";
			// 
			// extIPAddress
			// 
			this.extIPAddress.Location = new System.Drawing.Point(136, 104);
			this.extIPAddress.Name = "extIPAddress";
			this.extIPAddress.Size = new System.Drawing.Size(216, 19);
			this.extIPAddress.TabIndex = 4;
			this.extIPAddress.Text = "";
			this.extIPAddress.Leave += new System.EventHandler(this.extIPAddress_Leave);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(40, 104);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 16);
			this.label3.TabIndex = 3;
			this.label3.Text = "外部IPアドレス";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// ctrlResolve
			// 
			this.ctrlResolve.Location = new System.Drawing.Point(280, 64);
			this.ctrlResolve.Name = "ctrlResolve";
			this.ctrlResolve.TabIndex = 2;
			this.ctrlResolve.Text = "解決";
			this.ctrlResolve.Click += new System.EventHandler(this.ctrlResolve_Click);
			// 
			// frmCreateExternalHost
			// 
			this.AcceptButton = this.button1;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.button2;
			this.ClientSize = new System.Drawing.Size(394, 255);
			this.Controls.Add(this.ctrlResolve);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.extIPAddress);
			this.Controls.Add(this.dnsname);
			this.Controls.Add(this.intIPAddress);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "frmCreateExternalHost";
			this.Text = "新規外部ホスト";
			this.ResumeLayout(false);

		}
		#endregion

		private void extIPAddress_Leave(object sender, System.EventArgs e) {
			if(intIPAddress.Text.Length==0) {
				intIPAddress.Text = extIPAddress.Text;
			}
		}

		private void ctrlResolve_Click(object sender, System.EventArgs e) {
			this.Cursor = Cursors.WaitCursor;
			try {
				IPHostEntry entry = System.Net.Dns.Resolve(dnsname.Text);
				extIPAddress.Text = entry.AddressList[0].ToString();
				intIPAddress.Text = extIPAddress.Text;
			} catch(Exception ex) {
				MessageBox.Show(this,ex.Message);
			} finally {
				this.Cursor = Cursors.Default;
			}
		}

		private void button1_Click(object sender, System.EventArgs e) {
			ExternalHost extHost = new ExternalHost(monitor.shellnet, IPAddress.Parse(extIPAddress.Text));
			extHost.CreateDevice(monitor.SelectedNetwork, new IPAddress[]{IPAddress.Parse(intIPAddress.Text)});
			extHost.HostName = dnsname.Text;
			extHost.Register();
			this.Hide();
		}
	}
}
