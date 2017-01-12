using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LS;
using System.IO;

namespace SapReader
{
    public partial class Crypy : Form
    {
        // 6 1
        LSFB main;
        int ok = 0;
        public List<LSFB.ReadOnlyCheckBox> Items = new List<LSFB.ReadOnlyCheckBox>();
        public Crypy(ListView.SelectedListViewItemCollection files, bool encrypt)
        {
            InitializeComponent();
            Text = "Шифроване файлов - готово к шифрованию";
            this.encrypt = encrypt;
            radioButton1.Checked = encrypt;
            radioButton2.Checked = !encrypt;
            foreach (ListViewItem s in files)
            {
                LSFB.ReadOnlyCheckBox t = new LSFB.ReadOnlyCheckBox
                {
                    ReadOnly = true,
                    Location = new Point(12, 6 + 23 * files.IndexOf(s)),
                    Text = s.Text,
                    AutoSize = true
                };                
                Items.Add(t);
            }
            main = new LSFB(this, 0, 68, 0, false);
            Height = Items.Last().Top < Screen.PrimaryScreen.WorkingArea.Height/2 - 130? Items.Last().Top + 133 : Screen.PrimaryScreen.WorkingArea.Height / 2;
            pb.ForeColor = BackColor;
            main.work.AutoScroll = true;
            foreach (LSFB.ReadOnlyCheckBox c in Items)
                main.work.Controls.Add(c);
            Width -= 24;
        }
        delegate void CreateErrorDel(int i, string text);
        void CreateError(int i, string text)
        {
            if (InvokeRequired)
                Invoke(new CreateErrorDel(CreateError), i, text);
            else
            {
                Items[i].Text += " - " + text;
            }
        }
        delegate void ChangeCheckDel(int i, CheckState cs);
        void ChangeCheck(int i, CheckState cs)
        {
            if (InvokeRequired)
                Invoke(new ChangeCheckDel(ChangeCheck), i, cs);
            else
            {
                Items[i].CheckState = cs;
            }
        }
        delegate void ChangeBarDel(int i, bool done);
        void ChangeBar(int i, bool done)
        {
            if (InvokeRequired)
                Invoke(new ChangeBarDel(ChangeBar), i, done);
            else
            {
                pb.Value = i;
                Text = "Шифроване файлов - процесс шифрования завершён" + (!done? " на " + i + "%" : ", " + (int)(ok * 100D / Items.Count) + "% файлов шифрованно");
                if (done)
                {
                    button2.Enabled = true;
                    button2.Text = "Готово.";
                }
            }            
        }
        bool encrypt = true;
        string key = "";
        private void Progress_Load(object sender, EventArgs e)
        {
            textBox1.Enabled = false;
            radioButton1.Enabled = false;
            radioButton2.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            key = textBox1.Text;
            textBox1.PasswordChar = '•';
            if(encrypt)
            Main.history.Add(new List<string> {key });
            new Task(() =>
            {
                ChangeBar(0, false);
                foreach (LSFB.ReadOnlyCheckBox s in Items)
                {
                    try
                    {
                        string way = null;
                        Invoke((MethodInvoker) delegate { way = Main.main.nowDir + s.Text; });
                        if (File.GetAttributes(way).HasFlag(FileAttributes.Directory))
                        {
                            ChangeCheck(Items.IndexOf(s), CheckState.Indeterminate);
                            CreateError(Items.IndexOf(s), "Не является файлом!");
                        }
                        else
                        {
                            File.WriteAllBytes(way, encrypt? Sapphire.GetCodeBytes(File.ReadAllBytes(way), key) : Sapphire.GetTextBytes(File.ReadAllBytes(way), key));
                            ChangeCheck(Items.IndexOf(s), CheckState.Checked);
                            if(encrypt)
                            Main.history.Last().Add(way);
                            CreateError(Items.IndexOf(s), "Успешно " + (encrypt? "за" : "рас") + "шифрован!" );
                            ok++;
                        }
                    }
                    catch (Exception ex)
                    {
                        ChangeCheck(Items.IndexOf(s), CheckState.Indeterminate);
                        CreateError(Items.IndexOf(s), "Ошибка: " + ex.Message + "!");
                    }
                    ChangeBar((int)(((double)Items.IndexOf(s)+1D)*100D/(double)Items.Count), false);
                }
                ChangeBar(100, true);
            }).Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            encrypt = radioButton1.Checked;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                Progress_Load(null,null);
            }
        }
    }
}
