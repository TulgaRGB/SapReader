﻿using System;
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
    public partial class Login : Form
    {
       public bool cancel = true;
        public Login(string login, string pass, string text)
        {
            InitializeComponent();
            if(text != "")
            label2.Text = text;
            new LSFB(this,1,0,0,false);
            this.textBox1.Text = login;
            this.pass.Text = pass;
            button1.Click += (object sender, EventArgs e) =>
            {
                cancel = false;
                Close();
            };
            ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Main.fc.ClientSend("<REQUEST type='demo'/>");
            Close();
        }
    }
}
