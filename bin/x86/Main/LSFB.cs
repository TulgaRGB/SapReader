using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using System.IO;
using Microsoft.Win32;

namespace LS
{
    public class LSFB
    {
        public static List<Form> AllForms = new List<Form>();
        public static Form MainForm = null;
        
        public int startoffeild = 35;
        Form f;
        public string[] customtext = new string[2];
        string[] title = { "r", "1", "0", "s" };
        public EventHandler[] acts;
        public Panel work;
        private Label resize;
        public LSFB(Form f, int buttonCount = 3, int sfeild = 0, int sfeildH=0, bool sizeble = true, bool autoscroll = false,  EventHandler help = null)
        {
            this.f = f;
            if (MainForm == null)
                MainForm = f;
            if (f != MainForm)
            {
                AllForms.Add(f);
                f.FormClosed += (object sender, FormClosedEventArgs e) => { AllForms.Remove(f); };
            }
            MainForm.BackColorChanged += (object sender, EventArgs e) => { foreach (Form fo in AllForms) fo.BackColor = MainForm.BackColor; };
            MainForm.ForeColorChanged += (object sender, EventArgs e) => { foreach (Form fo in AllForms) fo.ForeColor = MainForm.ForeColor; };
            List<Control> temp = new List<Control>();
            foreach (Control c in f.Controls)
                if (c.Tag + "" != "OnBorder")
                    temp.Add(c);
            foreach (Control c in temp)
            {
                f.Controls.Remove(c);
            }
            f.Width += 24;
            f.Height += startoffeild + 12;
            startoffeild += sfeild;
            f.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
            foreach (Control c in f.Controls)
                if (c.Tag + "" == "OnBorder")
                    c.Top += 35;
            f.FormBorderStyle = FormBorderStyle.None;
            f.ControlBox = false;
            f.ForeColor = MainForm.ForeColor;
            f.BackColor = MainForm.BackColor;
            acts = new EventHandler[]
            {
                new EventHandler((object sender, EventArgs e) => { f.Close(); }),
                new EventHandler((object sender, EventArgs e) => { if(f.WindowState == FormWindowState.Normal) { f.WindowState = FormWindowState.Maximized; ((Button)sender).Text = "2"; resize.Visible = false; }  else { f.WindowState = FormWindowState.Normal; ((Button)sender).Text = "1"; resize.Visible = true; } }),
                new EventHandler((object sender, EventArgs e) => { f.WindowState = FormWindowState.Minimized; }),
                help
            };
            for (int i = 0; i < buttonCount; i++)
            {
                if (buttonCount == 2 && i == 1)
                    i++;
                Button b = new Button
                {
                    FlatStyle = FlatStyle.Flat,
                    Text = title[i],
                    Size = new Size(34, 26),
                    Location = new Point(f.Width - 34 * (i + ((buttonCount == 2 && i == 2) ? 0 : 1)), 0),
                    Font = new Font("Marlett", 8.5f),
                    Tag = "OnBorder"
                };
                if (i == 1)
                    f.MouseDoubleClick += (object sender, MouseEventArgs e) =>
                    {
                        if (e.Button == MouseButtons.Left)
                            if (f.WindowState == FormWindowState.Normal) { f.WindowState = FormWindowState.Maximized; b.Text = "2"; }
                            else
                            {
                                f.WindowState = FormWindowState.Normal; b.Text = "1";
                            }
                    };
                b.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                b.Click += acts[i];
                b.FlatAppearance.BorderSize = 0;
                f.Controls.Add(b);
            }
            Label main = new Label
            {
                Text = customtext[0] + f.Text + customtext[1],
                Location = new Point(12, 9),
                AutoSize = true,
                Font = sizer(11),
                Tag = "OnBorder"
            };
            f.Controls.Add(main);
            if (sizeble)
            {
                resize = new Label
                {
                    Text = "o",
                    Location = new Point(f.Width - 15, f.Height - 13),
                    AutoSize = true,
                    Font = new Font("Marlett", 8.5f),
                    Anchor = (AnchorStyles.Bottom | AnchorStyles.Right),
                    Tag = "OnBorder"
                };
                resize.Cursor = Cursors.SizeNWSE;
                resize.MouseMove += (object sender, MouseEventArgs e) =>
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        if (Cursor.Position.X - f.Left > main.Left + main.Width + 34 * buttonCount)
                            f.Width = Cursor.Position.X - f.Left;
                        if (Cursor.Position.Y - f.Top > startoffeild)
                            f.Height = Cursor.Position.Y - f.Top;
                        f.Invalidate();
                    }
                };
                f.Controls.Add(resize);
            }
            work = new Panel
            {
                BackColor = Color.FromArgb(255, f.BackColor.R - 17 > 0 ? f.BackColor.R - 17 : f.BackColor.R + 17, f.BackColor.G - 17 > 0 ? f.BackColor.G - 17 : f.BackColor.G + 17, f.BackColor.B - 20 > 0 ? f.BackColor.B - 20 : f.BackColor.B + 20),
                Location = new Point(12 + sfeildH, startoffeild),
                Width = f.Width - 24 - sfeildH,
                Height = f.Height - 12 - startoffeild,
                Dock = DockStyle.Fill,
                Anchor = (AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom),
                Tag = "OnBorder",
                AutoScroll = autoscroll
            };
            f.BackColorChanged += (object sender, EventArgs e) =>
            {
                work.BackColor = Color.FromArgb(255, f.BackColor.R - 17 > 0 ? f.BackColor.R - 17 : f.BackColor.R + 17, f.BackColor.G - 17 > 0 ? f.BackColor.G - 17 : f.BackColor.G + 17, f.BackColor.B - 20 > 0 ? f.BackColor.B - 20 : f.BackColor.B + 20);
                foreach (Control c in ControlsLikeWork)
                    c.BackColor = work.BackColor;
                f.Invalidate();
            };
            f.Controls.Add(work);
            foreach (Control c in temp)
            {
                work.Controls.Add(c);
                c.Top -= sfeild;
                c.Left -= sfeildH;
            }            
            main.MouseDown += Form1_MouseDown;
            main.MouseMove += Form1_MouseMove;
            f.MouseDown += Form1_MouseDown;
            f.MouseMove += Form1_MouseMove;
            f.TextChanged += (object sender, EventArgs e) => { main.Text = customtext[0] + f.Text + customtext[1]; };
        }
        List<Control> ControlsLikeWork = new List<Control>();
        public void MakeControlLikeWork(Control control, bool Color = true, bool Location = true, bool Size = true)
        {
            if (Location)
                control.Location = work.Location;
            if (Size)
                control.Size = work.Size;
            if (Color)
                control.BackColor = work.BackColor;
            ControlsLikeWork.Add(control);
        }
        private Point MouseDownLocation;
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                MouseDownLocation = e.Location;

            }
        }
        public Control Find(Control.ControlCollection cc, string name)
        {
            return cc.Find(name, false).Last();
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                if (MouseDownLocation.Y < 35)
                {
                    if ((e.X - MouseDownLocation.X < 0 && f.Left > 0) || (e.X - MouseDownLocation.X > 0 && f.Left + f.Width < Screen.PrimaryScreen.WorkingArea.Width))
                        f.Left = e.X + f.Left - MouseDownLocation.X;
                    if ((e.Y - MouseDownLocation.Y < 0 && f.Top > 0) || (e.Y - MouseDownLocation.Y > 0 && f.Top + f.Height < Screen.PrimaryScreen.WorkingArea.Height))
                        f.Top = e.Y + f.Top - MouseDownLocation.Y;
                }
        }
        #region statfields
        public static void AddCms(RichTextBox th)
        {
            ContextMenuStrip cms = new ContextMenuStrip();
            cms.Renderer = new MyRenderer();
            cms.Items.Add("Отменить");
            cms.Items[cms.Items.Count - 1].Click += (object sender, EventArgs e) => { SendKeys.Send("^{z}"); };
            cms.Items.Add("Вернуть");
            cms.Items[cms.Items.Count - 1].Click += (object sender, EventArgs e) => { SendKeys.Send("^{y}"); };
            cms.Items.Add(new ToolStripSeparator());
            cms.Items.Add("Вырезать");
            cms.Items[cms.Items.Count - 1].Click += (object sender, EventArgs e) => { SendKeys.Send("^{x}"); };
            cms.Items.Add("Копировать");
            cms.Items[cms.Items.Count - 1].Click += (object sender, EventArgs e) => { SendKeys.Send("^{c}"); };
            cms.Items.Add("Вставить");
            cms.Items[cms.Items.Count - 1].Click += (object sender, EventArgs e) => { SendKeys.Send("^{v}"); };
            cms.Items.Add(new ToolStripSeparator());
            cms.Items.Add("Выделить всё");
            cms.Items[cms.Items.Count - 1].Click += (object sender, EventArgs e) => { SendKeys.Send("^{a}"); };
            th.ContextMenuStrip = cms;
            th.TextChanged += (object sender, EventArgs e) => { int i = th.SelectionStart; th.Text = th.Text.ToString(); th.SelectionStart = i; };
        }
        public static string ToHex(Color c)
        {
            return (c.R < 16 ? "0" : "") + Convert.ToString(c.R, 16) + (c.G < 16 ? "0" : "") + Convert.ToString(c.G, 16) + (c.B < 16 ? "0" : "") + Convert.ToString(c.B, 16);
        }
        public static Color FromHex(string hex)
        {
            return Color.FromArgb(255, Convert.ToByte(hex.Substring(0,2),16), Convert.ToByte(hex.Substring(2, 2), 16), Convert.ToByte(hex.Substring(4, 2), 16));
        }
        public static bool ResetParams(string programName)
        {
            try
            {
                RegistryKey currentUserKey = Registry.CurrentUser;
                RegistryKey sft = currentUserKey.OpenSubKey("Software", true);
                RegistryKey ls = sft.OpenSubKey("LS",true);
                RegistryKey program = ls.OpenSubKey(programName,true);
                foreach (string paramName in program.GetValueNames())
                {
                    program.DeleteValue(paramName);
                }
                program.Close();
                ls.Close();
                return true;
            }
            catch { return false; }
        }
        public static bool SaveParams(string programName, Dictionary<string,string> parames)
        {
            try
            {
                RegistryKey currentUserKey = Registry.CurrentUser;
                RegistryKey sft = currentUserKey.OpenSubKey("Software", true);
                RegistryKey ls = sft.CreateSubKey("LS");
                RegistryKey program = ls.CreateSubKey(programName);
                foreach (string k in parames.Keys)
                    if(k != "")
                    program.SetValue(k,parames[k]);
                program.Close();
                ls.Close();
                return true;
            }
            catch { return false; }
        }
        public static Dictionary<string,string> LoadParams(string programName, string[] what = null)
        {
            Dictionary<string, string> outp = new Dictionary<string, string>();
            try
            {
                RegistryKey currentUserKey = Registry.CurrentUser;
                RegistryKey sft = currentUserKey.OpenSubKey("Software", false);
                RegistryKey ls = sft.OpenSubKey("LS", false);
                RegistryKey program = ls.OpenSubKey(programName, false);
                foreach (string paramName in program.GetValueNames())
                {
                    if (what != null ? what.ToList().IndexOf(paramName) != -1 : true)
                        outp[paramName] = program.GetValue(paramName) + "";
                }
                program.Close();
                ls.Close();
            }
            catch (Exception e) { outp[""] = e.Message + ""; }
            return outp;
        }
        public static Font sizer(int size)
        {
            return new Font(FontFamily.GenericSansSerif, size, FontStyle.Regular);
        }
        public static string[] SplitByFirst(string var)
        {
            string[] tmp = var.Split('.');
            return new string[] { tmp[0], var.Replace(tmp[0] + ".", "") };
        }
        public static string XmlTostring(XmlDocument document)
        {
            String Result = "";

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);

            try
            {
                writer.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                String FormattedXML = sReader.ReadToEnd();

                Result = FormattedXML;
            }
            catch (XmlException)
            {
            }

            mStream.Close();
            writer.Close();

            return Result;
        }
        public static Dictionary<string, Control> DrawForm(Control form, string xml)
        {            
                XmlDocument doc = new XmlDocument();
                Dictionary<string, Control> dick = new Dictionary<string, Control>();
                doc.LoadXml(xml);
                form.Controls.Clear();
                foreach (XmlElement x in doc.FirstChild.ChildNodes)
                {
                    Control c = null; string text = x.InnerText; bool readOnly = x.Attributes.GetNamedItem("readonly") != null;
                    bool secured = x.Attributes.GetNamedItem("secured") != null; bool autosize = x.Attributes.GetNamedItem("size") == null;                    
                    #region switch (x.Name)
                    switch (x.Name)
                    {
                        case "panel":
                            c = new Panel { BorderStyle = BorderStyle.FixedSingle,AutoSize = autosize };
                            break;
                        case "label":
                            c = new Label { AutoSize = autosize };
                            break;
                        case "button":
                            c = new Button {FlatStyle = FlatStyle.Flat, AutoSize = autosize };
                            break;
                        case "textbox":
                            c = new TextBox { BorderStyle = BorderStyle.FixedSingle, AutoSize = autosize, ReadOnly = readOnly };
                            if (secured)
                                ((TextBox)c).PasswordChar = '•';
                            break;
                        case "richtextbox":
                            c = new RichTextBox { BorderStyle = BorderStyle.FixedSingle, AutoSize = autosize, ReadOnly = readOnly };
                            AddCms((RichTextBox)c);
                            break;
                        case "radiobutton":
                            c = new RadioButton { FlatStyle = FlatStyle.Flat, AutoSize = autosize };
                            break;
                        case "checkbox":
                            c = new ReadOnlyCheckBox { FlatStyle = FlatStyle.Flat, AutoSize = autosize, ReadOnly = readOnly };
                            break;
                        case "combobox":
                            c = new ComboBox { FlatStyle = FlatStyle.Flat, AutoSize = autosize };
                            break;
                    }
                    #endregion
                    if (c != null)
                    {
                        form.Controls.Add(c);
                        c.Text = text;
                        try
                        {
                            c.Location = new Point(
                              Convert.ToInt32(x.Attributes.GetNamedItem("location").InnerText.Split(',').First()),
                              Convert.ToInt32(x.Attributes.GetNamedItem("location").InnerText.Split(',').Last()));
                        }
                        catch { }
                        try
                        {
                            c.Font = sizer(Convert.ToInt32(x.Attributes.GetNamedItem("font").InnerText));                        
                        }
                        catch { c.Font = sizer(8); }
                        try
                        {
                            c.Size = new Size(
                                    Convert.ToInt32(x.Attributes.GetNamedItem("size").InnerText.Split(',').First()),
                                    Convert.ToInt32(x.Attributes.GetNamedItem("size").InnerText.Split(',').Last()));
                        }
                        catch { }
                        c.Enabled = x.Attributes.GetNamedItem("disabled") == null;
                        c.Visible = x.Attributes.GetNamedItem("hide") == null;
                        try
                        {
                        c.Name = x.Attributes.GetNamedItem("name").InnerText;
                        dick[x.Attributes.GetNamedItem("name").InnerText] = c;
                        }
                        catch { }
                }
                }
            return dick;   
        }
        public class MyRenderer : System.Windows.Forms.ToolStripRenderer
        {
            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                e.Graphics.DrawLine(new Pen(new SolidBrush(LSFB.MainForm.ForeColor)), 32, e.Item.Height / 2, e.Item.Width, e.Item.Height / 2);
                base.OnRenderSeparator(e);
            }
            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                e.ArrowColor = LSFB.MainForm.ForeColor;
                base.OnRenderArrow(e);
            }
            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                e.Item.ForeColor = LSFB.MainForm.ForeColor;
                e.ToolStrip.BackColor = LSFB.MainForm.BackColor;
                if (e.Item.Selected)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, LSFB.MainForm.BackColor.R + 9 < 255 ? LSFB.MainForm.BackColor.R + 9 : LSFB.MainForm.BackColor.R - 9, LSFB.MainForm.BackColor.G + 9 < 255 ? LSFB.MainForm.BackColor.G + 9 : LSFB.MainForm.BackColor.G - 9, LSFB.MainForm.BackColor.B + 9 < 255 ? LSFB.MainForm.BackColor.B + 9 : LSFB.MainForm.BackColor.B - 9)), e.Item.ContentRectangle);
                }
            }
        }
        public class ReadOnlyCheckBox : System.Windows.Forms.CheckBox
        {
            private bool readOnly;

            protected override void OnClick(EventArgs e)
            {
                // pass the event up only if its not readlonly
                if (!ReadOnly) base.OnClick(e);
            }

            public bool ReadOnly
            {
                get { return readOnly; }
                set { readOnly = value; }
            }
        }
        #endregion
    }
}
