using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Data.OleDb;
using System.IO;
using System.Globalization;
using Microsoft.VisualBasic.FileIO;
using PastebinAPI;
using Topshelf.Runtime.Windows;

namespace s2functions
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            WebClient client = new WebClient();
            string reply = client.DownloadString("https://pastebin.com/raw/ibKPM26U");
            textBox1.Text = reply;
        }
    }
}
