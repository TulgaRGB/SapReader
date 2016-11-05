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
using System.Net;
using System.Numerics;

namespace SapReader
{
    public partial class Form1 : Form
    {
        public static Random RNG = new Random();
        static public BigInteger exp;
        static public BigInteger osn = Convert.ToInt32("3");
        static public BigInteger mosn = Convert.ToInt32("93563");
        static public BigInteger mosn2 = Convert.ToInt32("2147483647");
        public int mysec = RNG.Next(10000, 100000);
        string key = null;
        public static string owner = Environment.UserName;
        public static Dictionary<string, string> parames;
        public readonly static string nazvanie = "SapReader бета" + FirstTwo(Application.ProductVersion);
        LSFB lsfb;
        public static NetConnection client = new NetConnection { BufferSize = 8192 };
        public Form1()
        {
            InitializeComponent(); exp = Diff(osn, mysec, false);
            ReloadAllParams();
            browser.Dock = DockStyle.Fill;
            browser.Anchor = (AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom);
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
            conLabel.SendToBack();
            LSFB.drawForm((Control)lsfb.work, Properties.Resources.HOME);
            if (parames.ContainsKey("Bool.MaximizeOnStart") ? Convert.ToBoolean(parames["Bool.MaximizeOnStart"]) == true : false)
                WindowState = FormWindowState.Maximized;
        }
        public static BigInteger Diff(BigInteger inp, int step, bool second)
        {
            return !second ? BigInteger.Pow(inp, step) % mosn : BigInteger.Pow((BigInteger.Pow(inp, step) % mosn), (int)osn) % mosn2;
        }
        public void ParseResponse(XmlDocument _response)
        {
                XmlNode response = _response.SelectSingleNode("/RESPONSE");
                if(response.Attributes.GetNamedItem("type") != null)
                switch (response.Attributes.GetNamedItem("type").InnerText)
                {
                    case "error":
                        DebugMessage(response.InnerText);
                        break;
                    case "message":
                        DebugMessage(response.InnerText,"Pro");
                        break;
                        case "exit":
                            if (response.Attributes.GetNamedItem("result").InnerText == "succ")
                            {
                                conLabel.Text = "Вход не выполнен";
                                lsfb.work.Controls.Clear();
                                Dictionary<string,string> tmp = LSFB.drawForm((Control)lsfb.work, Properties.Resources.HOME);
                            Text = tmp["name"];
                            }
                                break;
                    case "form":
                        if(response.Attributes.GetNamedItem("pro")!=null)
                        {
                            browser.Hide();
                            lsfb.work.Controls.Clear();
                            LSFB.drawForm(lsfb.work, response.InnerText.Replace("lt;", "<").Replace("gt;", ">"));
                        }
                        else
                        new Plugy(response.InnerText.Replace("lt;","<").Replace("gt;",">"));
                        break;
                    case "forms":
                        ListView temp = new ListView();
                        temp.Resize += (object sender, EventArgs e) =>
                        {
                            foreach(ColumnHeader c in temp.Columns)
                            {
                                c.Width = temp.Width / 4;
                            }
                        };
                        temp.MouseDoubleClick += (object sender, MouseEventArgs e) =>
                        {
                            if(e.Button == MouseButtons.Left)
                            {
                                ClientSend("<QUERY type=\"form\" id=\"" + temp.SelectedItems[0].Text + "\"/>");
                            }
                        };
                        temp.View = View.Details;
                        temp.Anchor = browser.Anchor;
                        temp.Size = lsfb.work.Size;
                        lsfb.work.Controls.Add(temp);
                        temp.Columns.Add("ID", temp.Width/4);
                        temp.Columns.Add("Название", temp.Width / 4);
                        temp.Columns.Add("Автор", temp.Width / 4);
                        temp.Columns.Add("Проверенно", temp.Width / 4);
                        foreach (XmlNode f in response.ChildNodes)
                        {
                            ListViewItem item = new ListViewItem(new[] 
                            {
                                f.Attributes.GetNamedItem("id").InnerText,
                                f.Attributes.GetNamedItem("name").InnerText,
                                f.Attributes.GetNamedItem("author").InnerText,
                                f.Attributes.GetNamedItem("validated").InnerText == "True"? "ДА" : "НЕТ"
                            });
                            temp.Items.Add(item);
                            
                        }
                        break;
                    case "login":
                        if (response.Attributes.GetNamedItem("result").InnerText == "succ")
                        {
                            Text = "Pro";
                            conLabel.Text = response.Attributes.GetNamedItem("user").InnerText;
                            browser.Hide();
                            LSFB.drawForm(lsfb.work, Properties.Resources.PRO, true);
                            LSFB.CbyName(lsfb.work.Controls, "hi").Text+=", " + conLabel.Text + "!";
                            LSFB.CbyName(lsfb.work.Controls, "libPlugin").Click += (object sender, EventArgs e) =>
                            {
                                lsfb.work.Controls.Clear();
                                ClientSend("<QUERY type=\"forms\"/>");
                            };
                            LSFB.CbyName(lsfb.work.Controls, "checkApp").Click += (object sender, EventArgs e) =>
                            {
                                ClientSend("<QUERY type=\"sum\" hash=\""+ Sapphire.GetMd5HashBytes(File.ReadAllBytes(System.Reflection.Assembly.GetEntryAssembly().Location)) +"\"/>");
                            };
                            LSFB.CbyName(lsfb.work.Controls, "exitButton").Click += (object sender, EventArgs e) =>
                            {
                                ClientSend("<QUERY type=\"exit\"/>");
                            };
                        }
                        else
                            if (response.Attributes.GetNamedItem("result").InnerText == "query")
                        {
                            ClientSend("<QUERY type=\"login\" login=\"" + parames["Pro.Login"] + "\" pass=\"" + Sapphire.GetMd5Hash(parames["Pro.Pass"]) + "\" />");
                        }
                        else
                        {
                            Login tmp = new Login(parames["Pro.Login"],parames["Pro.Pass"]);
                            if(!tmp.cancel)
                            ClientSend("<QUERY type=\"login\" login=\"" + tmp.textBox1.Text + "\" pass=\"" + Sapphire.GetMd5Hash(tmp.pass.Text) + "\" />");
                        }
                        break;
                }
                
        }
        public void ClientSend(string query)
        {
            if(key != null)
            try
            {
                client.Send(Sapphire.GetCodeBytes(UnicodeEncoding.Unicode.GetBytes(query), key));
            }
            catch(Exception ex) { DebugMessage("Запрос не был отправлен:\n"+ex.Message);
            }
        }
        #region connection
        void Con()
        {
                try
                {
                    try
                    {
                        client.Disconnect();
                    }
                    catch { }
                IPAddress ip;
                    bool _ip =  IPAddress.TryParse(parames["Pro.Ip"], out ip);
                if (_ip)
                    client.Connect(ip, 228);
                else
                    client.Connect(parames["Pro.Ip"], 228);
                    client.OnDisconnect += (object sender1, NetConnection c) =>
                        {
                            key = null;
                            conLabel.Text = "";
                            DebugMessage("Потеряно соединение с сервером!");                           
                        };
                    client.OnDataReceived += (object sender1, NetConnection c, byte[] b) =>
                        {
                            if (key != null)
                            {
                                XmlDocument _response = new XmlDocument();
                            _response.LoadXml(UnicodeEncoding.Unicode.GetString(Sapphire.GetTextBytes(b, key)));
                                ParseResponse(_response);
                            }
                            else
                            {
                                    client.Send(UnicodeEncoding.Unicode.GetBytes(exp + ""));
                                    key = Sapphire.GetMd5Hash(Diff(Convert.ToInt32(UnicodeEncoding.Unicode.GetString(b)), mysec, true) + "");                          
                            }
                        };
                }
                //try{ }
                catch (Exception ex)
                {
                    DebugMessage(ex.Message + "");
                }
        }
        #endregion
        public void ReloadAllParams()
        {
            parames = LSFB.LoadParams("SapReader");
            if (parames.ContainsKey("Color.BackColor"))
                BackColor = LSFB.FromHex(parames["Color.BackColor"]);            
            if (parames.ContainsKey("Color.ForeColor"))
                ForeColor = LSFB.FromHex(parames["Color.ForeColor"]);
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
                    switch (s != null ? s.Tag + "" : s2.Tag + "")
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
                            if (nowDir + "" == nowDir.Root + "")
                                MainScr();
                            else
                                Go(nowDir.Parent.FullName + (nowDir.Parent.FullName.Last() != '\\' ? @"\" : ""));
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
                            Dictionary<string, string> tmp = LSFB.drawForm(lsfb.work, Properties.Resources.LSSL);
                            Text = tmp["name"];
                            break;
                        case "Add":
                            OpenFileDialog od = new OpenFileDialog { Filter = "*.dll|*.dll" };
                            if (od.ShowDialog() == DialogResult.OK)
                            {
                                string xml = UnicodeEncoding.UTF8.GetString(File.ReadAllBytes(od.FileName));
                                new Plugy(xml.Substring(1));
                            }
                            break;
                        #endregion
                        case "Props":
                            new Props();
                            break;
                        #region Pro
                    #endregion
                    case "InfoSum":
                        Clipboard.SetText(Sapphire.GetMd5HashBytes(File.ReadAllBytes(System.Reflection.Assembly.GetEntryAssembly().Location)));
                        DebugMessage("Хэш Сумма " + Clipboard.GetText() + " в буфере обмена","Успешно!");
                        break;
                    }
            }
            // try { }
            catch (Exception ex) { DebugMessage(ex.Message + "");}
        }
        public void DebugMessage(string text,string header = "Ошибка!")
        {
          tray.ShowBalloonTip(1000, header, text + "", ToolTipIcon.None);
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

        private void proToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (key == null)
            {
                    client = new NetConnection { BufferSize = 8192 };
                    Con();
                    conLabel.Text = "Вход не выполнен";
            }
            else
                ClientSend("<QUERY type=\"login\" login=\"" + parames["Pro.Login"] + "\" pass=\"" + Sapphire.GetMd5Hash(parames["Pro.Pass"]) + "\" />");
        }
        
    }    
}
