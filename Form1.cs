using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LS;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.IO;
using System.Media;
using System.Xml;

namespace SapReader
{
    public partial class Form1 : Form
    {
        public static string owner = Environment.UserName;
        public readonly static string nazvanie = "SapReader бета" + FirstTwo(Application.ProductVersion);
        LSFB lsfb;
        public Form1()
        {
            InitializeComponent();
            #region loadParams
            Dictionary<string, string> tmp = LSFB.LoadReg("SapReader");
            #endregion
            browser.Dock = DockStyle.Fill;
            browser.Anchor = (AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom);
            if (tmp.ContainsKey("Colors.BackColor"))
                BackColor = LSFB.FromHex(tmp["Colors.BackColor"]);
            if (tmp.ContainsKey("Colors.ForeColor"))
                ForeColor = LSFB.FromHex(tmp["Colors.ForeColor"]);
            LSFB.MainForm = this;
            Size = new Size(Screen.PrimaryScreen.WorkingArea.Width / 2, Screen.PrimaryScreen.WorkingArea.Height / 2);
            lsfb = new LS.LSFB(this,4, 24, autoscroll:true, help: (object sender, EventArgs e) => { new About("SapphireReader"); });
            lsfb.customtext[0] = nazvanie + " - ";
            Text = "Добро пожаловать!";
            mm.Renderer = new LSFB.MyRenderer();
            cms.Renderer = new LSFB.MyRenderer();
            browser.ContextMenuStrip = cms;
            browser.ForeColor = ForeColor;
            lsfb.MakeControlLikeWork(browser);
            browser.Hide();
            LSFB.drawForm((Control)lsfb.work, Properties.Resources.HOME);
        }
        #region menu options
        public void MainScr()
        {
            nowDir = null;
            Text = "Устройства и диски";
            browser.Items.Clear();
            browser.LargeImageList = new ImageList();
            browser.LargeImageList.ImageSize = new Size(32, 32);
            foreach (DriveInfo d in DriveInfo.GetDrives())
            {
                browser.LargeImageList.Images.Add(Properties.Resources.Folder);
                browser.Items.Add(d.Name.Replace(@"\",""), browser.LargeImageList.Images.Count-1); 
            }
            browser.Show();
        }
        public ToolStripMenuItem PlugMaker(string name)
        {
            if(!plugs.Keys.Contains(name))
            {
                ToolStripMenuItem temp = new ToolStripMenuItem { Text = name };
                temp.Click += (object sender, EventArgs e) => {
                    browser.Hide();
                    LSFB.drawForm(lsfb.work, plugs[name]);
                };
                return temp;
            }
            return null;
        }
        public Dictionary<string, string> plugs = new Dictionary<string, string>();
        public DirectoryInfo nowDir = null;
        public void Go(string way)
        {
            DirectoryInfo bu = nowDir;
            try
            {
                nowDir = new DirectoryInfo(way);
                Text = way;
                browser.Items.Clear();
                browser.LargeImageList = new ImageList();
                browser.LargeImageList.ImageSize = new Size(32, 32);
                foreach (string d in Directory.GetDirectories(way))
                {
                    browser.LargeImageList.Images.Add(Properties.Resources.Folder);
                    browser.Items.Add(new DirectoryInfo(d).Name, browser.LargeImageList.Images.Count - 1);
                }
                foreach (string d in Directory.GetFiles(way))
                {
                    browser.LargeImageList.Images.Add(Icon.ExtractAssociatedIcon(d));
                    browser.Items.Add(new DirectoryInfo(d).Name, browser.LargeImageList.Images.Count - 1);
                }
                browser.Show();
            } catch (Exception e)
            {
                nowDir = bu;
                browser.Items.Clear();
                browser.LargeImageList = new ImageList();
                browser.LargeImageList.Images.Add(SystemIcons.Error);
                browser.Items.Add(e.Message,0);
            }
        }
        #endregion        
        public void MenuHandler(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem s = null;
                ContextMenuStrip s2 = null;
                try
                {
                    s = (ToolStripMenuItem)sender;
                }
                catch { s2 = (ContextMenuStrip)sender; }
                switch (s != null? s.Tag + "" : s2.Tag+"")
                {
                    #region Проводник
                    case "MainScr":
                        MainScr();
                        break;
                    //separator
                    case "Root":
                        Go(nowDir.Root + "");
                        break;
                    case "Up":
                        if (nowDir+"" == nowDir.Root+"")
                            MainScr();
                        else
                            Go(nowDir.Parent.FullName + (nowDir.Parent.FullName.Last() != '\\'? @"\" : ""));
                        break;
                    case "Refr":
                        Go(nowDir.FullName);
                        break;
                    //separator
                    case "CreateFile":
                        break;
                    case "CreateFold":
                        break;
                    case "CreateArch":
                        break;
                    case "Rename":
                        break;
                    case "Delete":
                        break;
                    //separator
                    case "Encrypt":
                        Encrypt(browser.SelectedItems, true);
                        break;
                    case "Decrypt":
                        Encrypt(browser.SelectedItems, false);
                        break;
                    #endregion
                    #region Инструменты
                    case "Brow":
                        browser.Show(); lsfb.work.Controls.Clear();
                        break;
                    case "LSsl":
                        browser.Hide();
                        Dictionary<string,string> tmp = LSFB.drawForm(lsfb.work, Properties.Resources.LSSL);
                        Text = tmp["name"];
                        break;
                    case "Add":
                        OpenFileDialog od = new OpenFileDialog { Filter = "*.dll|*.dll" };
                        if(od.ShowDialog() == DialogResult.OK)
                        {
                            string xml = UnicodeEncoding.UTF8.GetString(File.ReadAllBytes(od.FileName));
                            new Plugy(xml.Substring(1));
                        }
                        break;
                    #endregion
                    case "Props":
                        new Props();
                        break;

                }
            }
            // try { }
            catch (Exception ex) { DebugMessage(ex.Message + "");}
        }
        public void DebugMessage(string text)
        {
          tray.ShowBalloonTip(1000, "Ошибка!", text + "", ToolTipIcon.None);
        }
        public void Encrypt(ListView.SelectedListViewItemCollection files, bool encrypt)
        {
            if (files.Count > 0)
            {
                Crypy prog = new Crypy(files, encrypt);
                prog.ShowDialog();
            }
        }
        public static string FirstTwo(string text)
        {
            return text.Split('.')[0] + "." + text.Split('.')[1];
        }

        private void browser_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                try
                {
                    if (File.GetAttributes(nowDir + browser.SelectedItems[0].Text).HasFlag(FileAttributes.Directory))
                        Go(nowDir + browser.SelectedItems[0].Text + @"\");
                    else
                        System.Diagnostics.Process.Start(nowDir + browser.SelectedItems[0].Text);
                }
                catch { SystemSounds.Exclamation.Play(); }
            }
        }
    }    
}
