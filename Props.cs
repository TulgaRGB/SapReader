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

namespace SapReader
{
    public partial class Props : Form
    {
        LSFB main;
        public Props()
        {
            InitializeComponent();
            main = new LS.LSFB(this, 1, 0, false );
            Image image = new Bitmap(201,201);
            using (Graphics g = Graphics.FromImage(image))
            {
                g.FillEllipse(Brushes.White, 0, 0, 201, 201);
                for (int y = -100; y < 101; y++)
                {
                    for (int x = -100; x < 101; x++)
                    {
                        Color c = ((Bitmap)image).GetPixel(x + 100, y + 100);
                        if (c.A > 0)
                        {
                            Point pol = DecToPol(x, y);
                            Color rgb = HSV(pol.Y, pol.X);
                            g.FillRectangle(new SolidBrush(rgb), x + 100, y + 100, 1, 1);
                        }
                    }
                }
            }
            pictureBox1.Image = image;
            pictureBox1.MouseMove += pick;
            pictureBox1.MouseClick += pick;
            pictureBox1.Paint += (object sender, PaintEventArgs e) =>
            {
               
            };
            ShowDialog();
        }        
        void pick(object sender, MouseEventArgs e)
        {
                Color pxl = ((Bitmap)(pictureBox1.Image)).GetPixel(e.X > 200? 200: e.X < 0? 0 : e.X, e.Y > 200? 200: e.Y < 0? 0 : e.Y);
                if (pxl.A == 255)
                    if (e.Button == MouseButtons.Left)
                    {
                        LSFB.MainForm.BackColor = Color.FromArgb(255, pxl.R, pxl.G, pxl.B);
                        Color tmp = LSFB.MainForm.ForeColor;
                        LSFB.MainForm.ForeColor = LSFB.MainForm.BackColor;
                        LSFB.MainForm.ForeColor = tmp;
                    }
                    else
                        if (e.Button == MouseButtons.Right)
                        LSFB.MainForm.ForeColor = Color.FromArgb(255, pxl.R, pxl.G, pxl.B);
        }
        static Color HSV(int deg, int rad)
        {
            if (rad == 101)
                rad = 100;
            int r = 0;
            int g = 0;
            int b = 0;
            if(deg < -59 && deg > -121)
            {
                r =  (int)((1 + ((60 + deg)/60D))*255D);
            }
            if (deg < -119)
                g = (int)((1 - ((180D + deg) / 60)) * 255D);
            if(deg > 119)
                b = (int)((1 -((180D - deg) / 60)) * 255D);
            if(deg> 60 && deg <  121)
                r = (int)(((120D - deg) / 60) * 255D);
            if (deg < 61 && deg > 0)
                g = (int)(deg / 60D * 255D);
            if (deg < 0 && deg > -60)
                b = - (int)(deg/60D * 255D);
            if (deg < 61 && deg > -61)
                r = 255;
            if (deg < -59)
                b = 255;
            if (deg > 59)
                g = 255;
            return Color.FromArgb((int)(rad/100D*255),r,g,b);
        }
        static Point DecToPol(int x, int y)
        {
            Point point = new Point();
            point.X = (int)Math.Sqrt(x * x + y * y);
            point.Y =(int)(Math.Atan2(y, x) * 180 / Math.PI);
            return point;
        }
    }
}
