namespace SapReader
{
    partial class About
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.p = new System.Windows.Forms.Label();
            this.h1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // p
            // 
            this.p.AutoSize = true;
            this.p.Location = new System.Drawing.Point(12, 167);
            this.p.MaximumSize = new System.Drawing.Size(257, 0);
            this.p.Name = "p";
            this.p.Size = new System.Drawing.Size(84, 26);
            this.p.TabIndex = 8;
            this.p.Text = "Текст 1 строка\r\nТекст 2 строка";
            // 
            // h1
            // 
            this.h1.AutoSize = true;
            this.h1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.h1.Location = new System.Drawing.Point(12, 119);
            this.h1.MaximumSize = new System.Drawing.Size(257, 0);
            this.h1.Name = "h1";
            this.h1.Size = new System.Drawing.Size(107, 48);
            this.h1.TabIndex = 7;
            this.h1.Text = "Заголовок\r\nПодстрока";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SapReader.Properties.Resources.ls1;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(257, 104);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 466);
            this.Controls.Add(this.p);
            this.Controls.Add(this.h1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "About";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "О программе";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label p;
        private System.Windows.Forms.Label h1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}