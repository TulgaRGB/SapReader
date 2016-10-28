namespace SapReader
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
            this.mm = new System.Windows.Forms.MenuStrip();
            this.настройкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.цветаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.темыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.проводникToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.proToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.Настройки = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.Цвета = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.bb = new System.Windows.Forms.PictureBox();
            this.gb = new System.Windows.Forms.PictureBox();
            this.rb = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Темы = new System.Windows.Forms.TabPage();
            this.Проводник = new System.Windows.Forms.TabPage();
            this.Pro = new System.Windows.Forms.TabPage();
            this.мойВыборToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Мойвыбор = new System.Windows.Forms.TabPage();
            this.mm.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.Настройки.SuspendLayout();
            this.Цвета.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // mm
            // 
            this.mm.Dock = System.Windows.Forms.DockStyle.None;
            this.mm.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.настройкиToolStripMenuItem,
            this.цветаToolStripMenuItem,
            this.темыToolStripMenuItem,
            this.проводникToolStripMenuItem,
            this.мойВыборToolStripMenuItem,
            this.proToolStripMenuItem});
            this.mm.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.mm.Location = new System.Drawing.Point(-6, 9);
            this.mm.Name = "mm";
            this.mm.Size = new System.Drawing.Size(89, 120);
            this.mm.TabIndex = 3;
            this.mm.Tag = "OnBorder";
            this.mm.Text = "menuStrip1";
            // 
            // настройкиToolStripMenuItem
            // 
            this.настройкиToolStripMenuItem.Name = "настройкиToolStripMenuItem";
            this.настройкиToolStripMenuItem.Size = new System.Drawing.Size(82, 19);
            this.настройкиToolStripMenuItem.Text = "Настройки";
            this.настройкиToolStripMenuItem.Click += new System.EventHandler(this.tabSelect);
            // 
            // цветаToolStripMenuItem
            // 
            this.цветаToolStripMenuItem.Name = "цветаToolStripMenuItem";
            this.цветаToolStripMenuItem.Size = new System.Drawing.Size(82, 19);
            this.цветаToolStripMenuItem.Text = "Цвета";
            this.цветаToolStripMenuItem.Click += new System.EventHandler(this.tabSelect);
            // 
            // темыToolStripMenuItem
            // 
            this.темыToolStripMenuItem.Name = "темыToolStripMenuItem";
            this.темыToolStripMenuItem.Size = new System.Drawing.Size(82, 19);
            this.темыToolStripMenuItem.Text = "Темы";
            this.темыToolStripMenuItem.Click += new System.EventHandler(this.tabSelect);
            // 
            // проводникToolStripMenuItem
            // 
            this.проводникToolStripMenuItem.Name = "проводникToolStripMenuItem";
            this.проводникToolStripMenuItem.Size = new System.Drawing.Size(82, 19);
            this.проводникToolStripMenuItem.Text = "Проводник";
            this.проводникToolStripMenuItem.Click += new System.EventHandler(this.tabSelect);
            // 
            // proToolStripMenuItem
            // 
            this.proToolStripMenuItem.Name = "proToolStripMenuItem";
            this.proToolStripMenuItem.Size = new System.Drawing.Size(82, 19);
            this.proToolStripMenuItem.Text = "Pro";
            this.proToolStripMenuItem.Click += new System.EventHandler(this.tabSelect);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.Настройки);
            this.tabControl.Controls.Add(this.Цвета);
            this.tabControl.Controls.Add(this.Темы);
            this.tabControl.Controls.Add(this.Проводник);
            this.tabControl.Controls.Add(this.Мойвыбор);
            this.tabControl.Controls.Add(this.Pro);
            this.tabControl.Location = new System.Drawing.Point(84, 9);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(228, 420);
            this.tabControl.TabIndex = 6;
            // 
            // Настройки
            // 
            this.Настройки.Controls.Add(this.label2);
            this.Настройки.Controls.Add(this.button6);
            this.Настройки.Controls.Add(this.button4);
            this.Настройки.Controls.Add(this.button5);
            this.Настройки.Controls.Add(this.button3);
            this.Настройки.Controls.Add(this.button2);
            this.Настройки.Location = new System.Drawing.Point(4, 22);
            this.Настройки.Name = "Настройки";
            this.Настройки.Padding = new System.Windows.Forms.Padding(3);
            this.Настройки.Size = new System.Drawing.Size(220, 394);
            this.Настройки.TabIndex = 0;
            this.Настройки.Text = "Настройки";
            this.Настройки.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(60, 370);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Подтвердите";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label2.Visible = false;
            // 
            // button6
            // 
            this.button6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button6.Location = new System.Drawing.Point(3, 174);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(211, 50);
            this.button6.TabIndex = 0;
            this.button6.Text = "Импорт";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button4.Location = new System.Drawing.Point(3, 62);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(211, 50);
            this.button4.TabIndex = 0;
            this.button4.Text = "Загрузить";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button5.Location = new System.Drawing.Point(3, 118);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(211, 50);
            this.button5.TabIndex = 0;
            this.button5.Text = "Экспорт";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button3.Location = new System.Drawing.Point(3, 6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(211, 50);
            this.button3.TabIndex = 0;
            this.button3.Text = "Сохранить";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(139, 365);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 0;
            this.button2.Text = "Сбросить";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Цвета
            // 
            this.Цвета.BackColor = System.Drawing.Color.Transparent;
            this.Цвета.Controls.Add(this.textBox1);
            this.Цвета.Controls.Add(this.button1);
            this.Цвета.Controls.Add(this.bb);
            this.Цвета.Controls.Add(this.gb);
            this.Цвета.Controls.Add(this.rb);
            this.Цвета.Controls.Add(this.pictureBox1);
            this.Цвета.Controls.Add(this.label1);
            this.Цвета.Location = new System.Drawing.Point(4, 22);
            this.Цвета.Name = "Цвета";
            this.Цвета.Padding = new System.Windows.Forms.Padding(3);
            this.Цвета.Size = new System.Drawing.Size(220, 394);
            this.Цвета.TabIndex = 1;
            this.Цвета.Text = "Цвета";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(11, 353);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(122, 20);
            this.textBox1.TabIndex = 12;
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(138, 343);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(74, 39);
            this.button1.TabIndex = 11;
            this.button1.Text = "Сохранить\r\nтему";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // bb
            // 
            this.bb.Location = new System.Drawing.Point(11, 316);
            this.bb.Name = "bb";
            this.bb.Size = new System.Drawing.Size(201, 21);
            this.bb.TabIndex = 8;
            this.bb.TabStop = false;
            // 
            // gb
            // 
            this.gb.Location = new System.Drawing.Point(11, 289);
            this.gb.Name = "gb";
            this.gb.Size = new System.Drawing.Size(201, 21);
            this.gb.TabIndex = 9;
            this.gb.TabStop = false;
            // 
            // rb
            // 
            this.rb.Location = new System.Drawing.Point(11, 262);
            this.rb.Name = "rb";
            this.rb.Size = new System.Drawing.Size(201, 21);
            this.rb.TabIndex = 10;
            this.rb.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(11, 55);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(201, 201);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 39);
            this.label1.TabIndex = 6;
            this.label1.Text = "Выьрать цвета:\r\nЛКМ - Фон\r\nПКМ - Текст";
            // 
            // Темы
            // 
            this.Темы.Location = new System.Drawing.Point(4, 22);
            this.Темы.Name = "Темы";
            this.Темы.Size = new System.Drawing.Size(220, 394);
            this.Темы.TabIndex = 2;
            this.Темы.Text = "Темы";
            this.Темы.UseVisualStyleBackColor = true;
            // 
            // Проводник
            // 
            this.Проводник.Location = new System.Drawing.Point(4, 22);
            this.Проводник.Name = "Проводник";
            this.Проводник.Size = new System.Drawing.Size(220, 394);
            this.Проводник.TabIndex = 3;
            this.Проводник.Text = "Проводник";
            this.Проводник.UseVisualStyleBackColor = true;
            // 
            // Pro
            // 
            this.Pro.Location = new System.Drawing.Point(4, 22);
            this.Pro.Name = "Pro";
            this.Pro.Size = new System.Drawing.Size(220, 394);
            this.Pro.TabIndex = 4;
            this.Pro.Text = "Pro";
            this.Pro.UseVisualStyleBackColor = true;
            // 
            // мойВыборToolStripMenuItem
            // 
            this.мойВыборToolStripMenuItem.Name = "мойВыборToolStripMenuItem";
            this.мойВыборToolStripMenuItem.Size = new System.Drawing.Size(82, 19);
            this.мойВыборToolStripMenuItem.Text = "Мой выбор";
            // 
            // Мойвыбор
            // 
            this.Мойвыбор.Location = new System.Drawing.Point(4, 22);
            this.Мойвыбор.Name = "Мойвыбор";
            this.Мойвыбор.Size = new System.Drawing.Size(220, 394);
            this.Мойвыбор.TabIndex = 5;
            this.Мойвыбор.Text = "Мой выбор";
            this.Мойвыбор.UseVisualStyleBackColor = true;
            // 
            // Props
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 441);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.mm);
            this.MainMenuStrip = this.mm;
            this.Name = "Props";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройки";
            this.mm.ResumeLayout(false);
            this.mm.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.Настройки.ResumeLayout(false);
            this.Настройки.PerformLayout();
            this.Цвета.ResumeLayout(false);
            this.Цвета.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip mm;
        private System.Windows.Forms.ToolStripMenuItem цветаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem темыToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem проводникToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem proToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem настройкиToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage Настройки;
        private System.Windows.Forms.TabPage Цвета;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox bb;
        private System.Windows.Forms.PictureBox gb;
        private System.Windows.Forms.PictureBox rb;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TabPage Темы;
        private System.Windows.Forms.TabPage Проводник;
        private System.Windows.Forms.TabPage Pro;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem мойВыборToolStripMenuItem;
        private System.Windows.Forms.TabPage Мойвыбор;
    }
}