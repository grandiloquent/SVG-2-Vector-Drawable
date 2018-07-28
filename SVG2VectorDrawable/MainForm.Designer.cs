/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2018/7/29
 * Time: 6:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace SVG2VectorDrawable
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Button ok;
		
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
			this.ok = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// ok
			// 
			this.ok.Location = new System.Drawing.Point(1, 12);
			this.ok.Name = "ok";
			this.ok.Size = new System.Drawing.Size(75, 23);
			this.ok.TabIndex = 0;
			this.ok.Text = "Convert";
			this.ok.UseVisualStyleBackColor = true;
			this.ok.Click += new System.EventHandler(this.OkClick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(116, 49);
			this.Controls.Add(this.ok);
			this.Name = "MainForm";
			this.Text = "SVG2VectorDrawable";
			this.ResumeLayout(false);

		}
	}
}
