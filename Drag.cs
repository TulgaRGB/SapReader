using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using LS;

namespace SapReader
{
    public partial class Drag : Form
    {
        Form parent;
        public enum state
        {
            none, encrypt, decrypt, open
        }
        List<string> headers = new List<string>() { "Шифровать", "Расшифровать cюда", "Открыть"};
        public Drag(Form parent)
        {
            InitializeComponent();
            this.parent = parent;
            BackColor = Color.Red;
            ForeColor = parent.ForeColor;
            Top = parent.Top + parent.Height / 2 - Height / 2;
            Left = parent.Left + parent.Width / 2 - Width / 2;
            foreach(string s in headers)
            {
                Label b = new Label {Size = new Size(90,90), FlatStyle = FlatStyle.Flat, BackColor  = parent.BackColor, Text = s, Font = LSFB.sizer(7), AutoSize = false, TextAlign = ContentAlignment.MiddleCenter };
                flowLayoutPanel1.Controls.Add(b);
            }
        }

        public enum GWL
        {
            ExStyle = -20
        }

        public enum WS_EX
        {
            Transparent = 0x20,
            Layered = 0x80000
        }

        public enum LWA
        {
            ColorKey = 0x1,
            Alpha = 0x2
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLong(IntPtr hWnd, GWL nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong(IntPtr hWnd, GWL nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, int crKey, byte alpha, LWA dwFlags);

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            int wl = GetWindowLong(this.Handle, GWL.ExStyle);
            wl = wl | 0x80000 | 0x20;
            SetWindowLong(this.Handle, GWL.ExStyle, wl);
        }
        public state State = state.none;
        public void Main_DragOver(object sender, DragEventArgs e)
        {
            foreach (Label b in flowLayoutPanel1.Controls)
            {
                b.BackColor = parent.BackColor;
            }
            int left = e.X - Left;
            int top = e.Y - Top;
            if (left < Width && left > 0 && top > 0 && top < Height)
            {
                foreach(Label b in flowLayoutPanel1.Controls)
                {
                    if (b.Bounds.Contains(left,top))
                    {
                        State = (state)(headers.IndexOf(b.Text)+1);
                        b.BackColor = LSFB.Colorize(parent.BackColor);
                    }
                }
            }
            else
            {
                State = state.none;
            }
        }

        private void Drag_Activated(object sender, EventArgs e)
        {
            ForeColor = parent.ForeColor;
            Top = parent.Top + parent.Height / 2 - Height / 2;
            Left = parent.Left + parent.Width / 2 - Width / 2;
        }
    }
}
