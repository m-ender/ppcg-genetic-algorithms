namespace ppcggacscontroller
{
	partial class Display
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.viewF = new System.Windows.Forms.PictureBox();
			this.tpfF = new System.Windows.Forms.TrackBar();
			this.stF = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.viewF)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tpfF)).BeginInit();
			this.SuspendLayout();
			// 
			// viewF
			// 
			this.viewF.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.viewF.Location = new System.Drawing.Point(0, 82);
			this.viewF.Name = "viewF";
			this.viewF.Size = new System.Drawing.Size(676, 222);
			this.viewF.TabIndex = 0;
			this.viewF.TabStop = false;
			this.viewF.Paint += new System.Windows.Forms.PaintEventHandler(this.ViewFPaint);
			// 
			// tpfF
			// 
			this.tpfF.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.tpfF.LargeChange = 50;
			this.tpfF.Location = new System.Drawing.Point(12, 12);
			this.tpfF.Maximum = 200;
			this.tpfF.Minimum = 1;
			this.tpfF.Name = "tpfF";
			this.tpfF.Size = new System.Drawing.Size(651, 42);
			this.tpfF.SmallChange = 5;
			this.tpfF.TabIndex = 1;
			this.tpfF.TickFrequency = 5;
			this.tpfF.Value = 1;
			this.tpfF.Scroll += new System.EventHandler(this.TpfFScroll);
			// 
			// stF
			// 
			this.stF.Location = new System.Drawing.Point(12, 54);
			this.stF.Name = "stF";
			this.stF.Size = new System.Drawing.Size(135, 24);
			this.stF.TabIndex = 2;
			this.stF.Text = "Show Teleport Arrows";
			this.stF.UseVisualStyleBackColor = true;
			this.stF.CheckedChanged += new System.EventHandler(this.StFCheckedChanged);
			// 
			// Display
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(675, 304);
			this.Controls.Add(this.stF);
			this.Controls.Add(this.viewF);
			this.Controls.Add(this.tpfF);
			this.Name = "Display";
			this.Text = "Display";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DisplayFormClosing);
			((System.ComponentModel.ISupportInitialize)(this.viewF)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tpfF)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.CheckBox stF;
		private System.Windows.Forms.TrackBar tpfF;
		private System.Windows.Forms.PictureBox viewF;
	}
}
