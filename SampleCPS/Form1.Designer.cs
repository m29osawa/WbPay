namespace SampleCPS
{
	partial class Form1
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent() {
			this.buttonPay = new System.Windows.Forms.Button();
			this.buttonRefund = new System.Windows.Forms.Button();
			this.textAmount = new System.Windows.Forms.TextBox();
			this.textUserCode = new System.Windows.Forms.TextBox();
			this.textRefundAmount = new System.Windows.Forms.TextBox();
			this.listOrder = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.buttonConfirm = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonPay
			// 
			this.buttonPay.Location = new System.Drawing.Point(164, 60);
			this.buttonPay.Name = "buttonPay";
			this.buttonPay.Size = new System.Drawing.Size(87, 25);
			this.buttonPay.TabIndex = 0;
			this.buttonPay.Text = "Pay";
			this.buttonPay.UseVisualStyleBackColor = true;
			this.buttonPay.Click += new System.EventHandler(this.buttonPay_Click);
			// 
			// buttonRefund
			// 
			this.buttonRefund.Location = new System.Drawing.Point(160, 60);
			this.buttonRefund.Name = "buttonRefund";
			this.buttonRefund.Size = new System.Drawing.Size(87, 25);
			this.buttonRefund.TabIndex = 1;
			this.buttonRefund.Text = "Refund";
			this.buttonRefund.UseVisualStyleBackColor = true;
			this.buttonRefund.Click += new System.EventHandler(this.buttonRefund_Click);
			// 
			// textAmount
			// 
			this.textAmount.ImeMode = System.Windows.Forms.ImeMode.Disable;
			this.textAmount.Location = new System.Drawing.Point(8, 40);
			this.textAmount.Name = "textAmount";
			this.textAmount.Size = new System.Drawing.Size(140, 20);
			this.textAmount.TabIndex = 3;
			// 
			// textUserCode
			// 
			this.textUserCode.ImeMode = System.Windows.Forms.ImeMode.Disable;
			this.textUserCode.Location = new System.Drawing.Point(8, 84);
			this.textUserCode.Name = "textUserCode";
			this.textUserCode.Size = new System.Drawing.Size(140, 20);
			this.textUserCode.TabIndex = 4;
			// 
			// textRefundAmount
			// 
			this.textRefundAmount.ImeMode = System.Windows.Forms.ImeMode.Disable;
			this.textRefundAmount.Location = new System.Drawing.Point(4, 64);
			this.textRefundAmount.Name = "textRefundAmount";
			this.textRefundAmount.Size = new System.Drawing.Size(144, 20);
			this.textRefundAmount.TabIndex = 5;
			// 
			// listOrder
			// 
			this.listOrder.FormattingEnabled = true;
			this.listOrder.HorizontalScrollbar = true;
			this.listOrder.Location = new System.Drawing.Point(284, 52);
			this.listOrder.Name = "listOrder";
			this.listOrder.ScrollAlwaysVisible = true;
			this.listOrder.Size = new System.Drawing.Size(576, 277);
			this.listOrder.TabIndex = 6;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(51, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "Amount";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 68);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(62, 13);
			this.label2.TabIndex = 8;
			this.label2.Text = "UserCode";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textUserCode);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.textAmount);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.buttonPay);
			this.groupBox1.Location = new System.Drawing.Point(8, 16);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(268, 112);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "支払";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.buttonRefund);
			this.groupBox2.Controls.Add(this.textRefundAmount);
			this.groupBox2.Location = new System.Drawing.Point(8, 140);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(268, 92);
			this.groupBox2.TabIndex = 10;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "返金";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(4, 48);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(90, 13);
			this.label4.TabIndex = 7;
			this.label4.Text = "RefundAmount";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(4, 24);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(184, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "トランザクション記録でオーダを選ぶ";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label5);
			this.groupBox3.Controls.Add(this.buttonConfirm);
			this.groupBox3.ForeColor = System.Drawing.SystemColors.ControlText;
			this.groupBox3.Location = new System.Drawing.Point(8, 240);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(268, 88);
			this.groupBox3.TabIndex = 11;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "確認";
			this.groupBox3.Visible = false;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(4, 24);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(184, 13);
			this.label5.TabIndex = 7;
			this.label5.Text = "トランザクション記録でオーダを選ぶ";
			this.label5.Visible = false;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(468, 12);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(109, 13);
			this.label6.TabIndex = 12;
			this.label6.Text = "トランザクション記録";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(284, 36);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(48, 13);
			this.label7.TabIndex = 13;
			this.label7.Text = "オーダID";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(452, 36);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(33, 13);
			this.label8.TabIndex = 14;
			this.label8.Text = "処理";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(504, 36);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(33, 13);
			this.label9.TabIndex = 15;
			this.label9.Text = "金額";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(544, 36);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(84, 13);
			this.label10.TabIndex = 16;
			this.label10.Text = "支払チャンネル";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(628, 36);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(59, 13);
			this.label11.TabIndex = 17;
			this.label11.Text = "取引時間";
			// 
			// buttonConfirm
			// 
			this.buttonConfirm.Location = new System.Drawing.Point(160, 52);
			this.buttonConfirm.Name = "buttonConfirm";
			this.buttonConfirm.Size = new System.Drawing.Size(87, 25);
			this.buttonConfirm.TabIndex = 2;
			this.buttonConfirm.Text = "Confirm";
			this.buttonConfirm.UseVisualStyleBackColor = true;
			this.buttonConfirm.Visible = false;
			this.buttonConfirm.Click += new System.EventHandler(this.buttonConfirm_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(873, 343);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.listOrder);
			this.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Name = "Form1";
			this.Text = "WbCPSPay サンプル";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonPay;
		private System.Windows.Forms.Button buttonRefund;
		private System.Windows.Forms.TextBox textAmount;
		private System.Windows.Forms.TextBox textUserCode;
		private System.Windows.Forms.TextBox textRefundAmount;
		private System.Windows.Forms.ListBox listOrder;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Button buttonConfirm;
	}
}

