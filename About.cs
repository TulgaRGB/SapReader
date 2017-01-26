using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SapReader
{
    public partial class About : Form
    {
        public About(string h)
        {
            InitializeComponent();
            LS.LSFB t = new LS.LSFB(this, 1, 0, 0, false);
            h1.BackColor = t.work.BackColor;
            p.BackColor = t.work.BackColor;
            h1.Text = h;
            switch (h)
            {
                case "SapphireReader":
                    p.Text = "SapphireReader powered by Sapphire™\nSapphireReader Version: " + ProductVersion + "\nSapphire™ Version: " + LS.Sapphire.ver + "\nЛицензия для " + Main.owner + "\n© " +DateTime.Now.Year + " LISENKO SOFT\n\nПрограмма предназначена для работы в двух режимах: проводник с возможностью шифровать файлы и архиватор зашифрованных архивов.\n\n" + LS.Sapphire.About;
                    break;
                case "LSsec lite":
                    p.Text = "sosi";
                    break;
                case "Тесты Sapphire":
                    p.Text = "Немного цифр:\nВарианты ключей для Sapphire: 2^32 = " + Math.Pow(2, 32) + "\nДля Sapphire 2: (2^32)^4 = 16^32 = " + Math.Pow(16,32) + "\nДля Sapphire-16: 16^128 = " + Math.Pow(16,128) + "\n!!!ИЛИ!!!\n1" + Zero(154) ; 
                    break;
            }
            ShowDialog();
        }
        public string Zero(int count)
        {
            string lel = "";
            for (int i = 0; i < count; i++)
                lel += "0";
            return lel;
        }
    }
}
