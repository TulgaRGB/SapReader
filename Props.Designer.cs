﻿namespace SapReader
{
    partial class Props
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
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.rb = new System.Windows.Forms.PictureBox();
            this.gb = new System.Windows.Forms.PictureBox();
            this.bb = new System.Windows.Forms.PictureBox();
            this.mm = new System.Windows.Forms.MenuStrip();
            this.цветаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.темыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.проводникToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.proToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bb)).BeginInit();
            this.mm.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(92, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 39);
            this.label1.TabIndex = 0;
            this.label1.Text = "Выьрать цвета:\r\nЛКМ - Фон\r\nПКМ - Текст";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(95, 56);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(201, 201);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // rb
            // 
            this.rb.Location = new System.Drawing.Point(95, 263);
            this.rb.Name = "rb";
            this.rb.Size = new System.Drawing.Size(201, 21);
            this.rb.TabIndex = 2;
            this.rb.TabStop = false;
            // 
            // gb
            // 
            this.gb.Location = new System.Drawing.Point(95, 290);
            this.gb.Name = "gb";
            this.gb.Size = new System.Drawing.Size(201, 21);
            this.gb.TabIndex = 2;
            this.gb.TabStop = false;
            // 
            // bb
            // 
            this.bb.Location = new System.Drawing.Point(95, 317);
            this.bb.Name = "bb";
            this.bb.Size = new System.Drawing.Size(201, 21);
            this.bb.TabIndex = 2;
            this.bb.TabStop = false;
            // 
            // mm
            // 
            this.mm.Dock = System.Windows.Forms.DockStyle.None;
            this.mm.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.цветаToolStripMenuItem,
            this.темыToolStripMenuItem,
            this.проводникToolStripMenuItem,
            this.proToolStripMenuItem});
            this.mm.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.mm.Location = new System.Drawing.Point(9, 9);
            this.mm.Name = "mm";
            this.mm.Size = new System.Drawing.Size(87, 82);
            this.mm.TabIndex = 3;
            this.mm.Tag = "OnBorder";
            this.mm.Text = "menuStrip1";
            // 
            // цветаToolStripMenuItem
            // 
            this.цветаToolStripMenuItem.Name = "цветаToolStripMenuItem";
            this.цветаToolStripMenuItem.Size = new System.Drawing.Size(66, 19);
            this.цветаToolStripMenuItem.Text = "Цвета";
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(222, 344);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(74, 39);
            this.button1.TabIndex = 4;
            this.button1.Text = "Сохранить\r\nтему";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(95, 354);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(122, 20);
            this.textBox1.TabIndex = 5;
            // 
            // темыToolStripMenuItem
            // 
            this.темыToolStripMenuItem.Name = "темыToolStripMenuItem";
            this.темыToolStripMenuItem.Size = new System.Drawing.Size(50, 19);
            this.темыToolStripMenuItem.Text = "Темы";
            // 
            // проводникToolStripMenuItem
            // 
            this.проводникToolStripMenuItem.Name = "проводникToolStripMenuItem";
            this.проводникToolStripMenuItem.Size = new System.Drawing.Size(80, 19);
            this.проводникToolStripMenuItem.Text = "Проводник";
            // 
            // proToolStripMenuItem
            // 
            this.proToolStripMenuItem.Name = "proToolStripMenuItem";
            this.proToolStripMenuItem.Size = new System.Drawing.Size(80, 19);
            this.proToolStripMenuItem.Text = "Pro";
            // 
            // Props
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 441);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.bb);
            this.Controls.Add(this.gb);
            this.Controls.Add(this.rb);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mm);
            this.MainMenuStrip = this.mm;
            this.Name = "Props";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройки";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bb)).EndInit();
            this.mm.ResumeLayout(false);
            this.mm.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox rb;
        private System.Windows.Forms.PictureBox gb;
        private System.Windows.Forms.PictureBox bb;
        private System.Windows.Forms.MenuStrip mm;
        private System.Windows.Forms.ToolStripMenuItem цветаToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolStripMenuItem темыToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem проводникToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem proToolStripMenuItem;
    }
}