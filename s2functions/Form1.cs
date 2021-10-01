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
    public partial class Form1 : Form
    {
        
        
        public static DataTable GetDataTabletFromCSVFile(string filePath, bool isHeadings)
        {
            DataTable MethodResult = null;
            try
            {
                using (TextFieldParser TextFieldParser = new TextFieldParser(filePath))
                {
                    if (isHeadings)
                    {
                        MethodResult = GetDataTableFromTextFieldParser(TextFieldParser);

                    }
                    else
                    {
                        MethodResult = GetDataTableFromTextFieldParserNoHeadings(TextFieldParser);

                    }

                }

            }
            catch (Exception ex)
            {
                //ex.HandleException();
            }
            return MethodResult;
        }

        public static DataTable GetDataTableFromCsvString(string csvBody, bool isHeadings)
        {
            DataTable MethodResult = null;
            try
            {
                MemoryStream MemoryStream = new MemoryStream();


                StreamWriter StreamWriter = new StreamWriter(MemoryStream);

                StreamWriter.Write(csvBody);

                StreamWriter.Flush();


                MemoryStream.Position = 0;


                using (TextFieldParser TextFieldParser = new TextFieldParser(MemoryStream))
                {
                    if (isHeadings)
                    {
                        MethodResult = GetDataTableFromTextFieldParser(TextFieldParser);

                    }
                    else
                    {
                        MethodResult = GetDataTableFromTextFieldParserNoHeadings(TextFieldParser);

                    }

                }

            }
            catch (Exception ex)
            {
                //ex.HandleException();
            }
            return MethodResult;
        }

        public static DataTable GetDataTableFromRemoteCsv(string url, bool isHeadings)
        {
            DataTable MethodResult = null;
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                StreamReader StreamReader = new StreamReader(httpWebResponse.GetResponseStream());

                using (TextFieldParser TextFieldParser = new TextFieldParser(StreamReader))
                {
                    if (isHeadings)
                    {
                        MethodResult = GetDataTableFromTextFieldParser(TextFieldParser);

                    }
                    else
                    {
                        MethodResult = GetDataTableFromTextFieldParserNoHeadings(TextFieldParser);

                    }

                }

            }
            catch (Exception ex)
            {
                //ex.HandleException();
            }
            return MethodResult;
        }


        private static DataTable GetDataTableFromTextFieldParser(TextFieldParser textFieldParser)
        {
            DataTable MethodResult = null;
            try
            {
                textFieldParser.SetDelimiters(new string[] { "," });

                textFieldParser.HasFieldsEnclosedInQuotes = true;


                string[] ColumnFields = textFieldParser.ReadFields();

                DataTable dt = new DataTable();

                foreach (string ColumnField in ColumnFields)
                {
                    DataColumn DataColumn = new DataColumn(ColumnField);

                    DataColumn.AllowDBNull = true;

                    dt.Columns.Add(DataColumn);

                }


                while (!textFieldParser.EndOfData)
                {
                    string[] Fields = textFieldParser.ReadFields();


                    for (int i = 0; i < Fields.Length; i++)
                    {
                        if (Fields[i] == "")
                        {
                            Fields[i] = null;

                        }

                    }

                    dt.Rows.Add(Fields);

                }

                MethodResult = dt;

            }
            catch (Exception ex)
            {
                //ex.HandleException();
            }
            return MethodResult;
        }

        private static DataTable GetDataTableFromTextFieldParserNoHeadings(TextFieldParser textFieldParser)
        {
            DataTable MethodResult = null;
            try
            {
                textFieldParser.SetDelimiters(new string[] { "," });

                textFieldParser.HasFieldsEnclosedInQuotes = true;

                bool FirstPass = true;

                DataTable dt = new DataTable();

                while (!textFieldParser.EndOfData)
                {
                    string[] Fields = textFieldParser.ReadFields();

                    if (FirstPass)
                    {
                        for (int i = 0; i < Fields.Length; i++)
                        {
                            DataColumn DataColumn = new DataColumn("Column " + i);

                            DataColumn.AllowDBNull = true;

                            dt.Columns.Add(DataColumn);

                        }

                        FirstPass = false;

                    }

                    for (int i = 0; i < Fields.Length; i++)
                    {
                        if (Fields[i] == "")
                        {
                            Fields[i] = null;

                        }

                    }

                    dt.Rows.Add(Fields);

                }

                MethodResult = dt;

            }
            catch (Exception ex)
            {
                
            }
            return MethodResult;
        }
        public Form1()
        {
            InitializeComponent();

            advancedDataGridView1.DataSource = GetDataTableFromRemoteCsv("https://raw.githubusercontent.com/leafized/s2functions/main/variables/levelvariables.csv", true);
            //textBox2.Text = reply;
        }
        List<string> items = new List<string>();
        List<string> vars = new List<string>();

        public static async Task PastebinExample()
        {
            //before using any class in the api you must enter your api dev key
            Pastebin.DevKey = "ca6a2bb8c3a9b5de29e4e881d92e65a7";
            //you can see yours here: https://pastebin.com/api#1
            try
            {
                // login and get user object
                User me = await Pastebin.LoginAsync("Leafized", "Max0snha");
                // user contains information like e-mail, location etc...
                //("{0}({1}) lives in {2}", me, me.Email, me.Location);
                // lists all pastes for this user
                foreach (Paste paste in await me.ListPastesAsync(3)) // we limmit the results to 3
                {
                    Console.WriteLine(paste.Title);
                }

                string code = "<your fancy &code#() goes here>";
                //creates a new paste and get paste object
                Paste newPaste = await me.CreatePasteAsync(code, "MyPasteTitle", Language.HTML5, Visibility.Public, Expiration.TenMinutes);
                //newPaste now contains the link returned from the server
                Console.WriteLine("URL: {0}", newPaste.Url);
                Console.WriteLine("Paste key: {0}", newPaste.Key);
                Console.WriteLine("Content: {0}", newPaste.Text);
                //deletes the paste we just created
                await me.DeletePasteAsync(newPaste);

                //lists all currently trending pastes(similar to me.ListPastes())
                foreach (Paste paste in await Pastebin.ListTrendingPastesAsync())
                {
                    Console.WriteLine("{0} - {1}", paste.Title, paste.Url);
                }
                //you can create pastes directly from Pastebin static class but they are created as guests and you have a limited number of guest uploads
                Paste anotherPaste = await Paste.CreateAsync("another paste", "MyPasteTitle2", Language.CSharp, Visibility.Unlisted, Expiration.OneHour);
                Console.WriteLine(anotherPaste.Title);
            }
            catch (PastebinException ex) //api throws PastebinException
            {
                //in the Parameter property you can see what invalid parameter was sent
                //here we check if the exeption is thrown because of invalid login details
                if (ex.Parameter == PastebinException.ParameterType.Login)
                {
                    //
                }
                else
                {
                    throw; //all other types are rethrown and not swalowed!
                }
            }
            Console.ReadKey();
        }
            private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            foreach (string str in items)
            {
                if (str.StartsWith(textBox1.Text, StringComparison.CurrentCultureIgnoreCase))
                {
                    listBox1.Items.Add(str);
                }
            }
        }


        public DataTable readCsv(string fileName)
        {
            DataTable dt = new DataTable("Data");
            WebClient client = new WebClient();
            string reply = client.DownloadString("https://raw.githubusercontent.com/leafized/s2functions/main/variables/levelvariables.csv");
            using (OleDbConnection cn = new OleDbConnection( reply ) ) ;


            return dt;
        }
        private void Form1_Load_1(object sender, EventArgs e)
        {
            items.AddRange(new string[] { 
                "giveLoadout", 
                "giveKillstreak", 
                "SetWeaponAmmoClip",
                "SetWeaponAmmoStock",
                "GiveStartAmmo",
                "GiveMaxAmmo",
                "Place Holder",
                "Place Holder"

            });

            vars.AddRange(new string[] {
                "level.players",
                "level.gametypes",
                "Place Holder",
                "Place Holder",
                "Place Holder",
                "Place Holder",
                "Place Holder",
                "Place Holder",
                "Place Holder"

            });
            foreach (string str in items)
            {
                listBox1.Items.Add(str);
            }
            foreach (string str in vars)
            {
                listBox2.Items.Add(str);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem.ToString() == "giveLoadout") textBox2.Text = ("self maps\\mp\\gametypes\\_class::func_0F35( team , class )");
            if (listBox1.SelectedItem.ToString() == "giveKillstreak") textBox2.Text = (@"self maps\mp\killstreaks\_killstreaks::func_478D( streakName,isEarned,awardXp,owner,slotNumber,streakID )");
            if (listBox1.SelectedItem.ToString() == "SetWeaponAmmoClip") textBox2.Text = (@"self method_82FA( weapon  , amount )");
            if (listBox1.SelectedItem.ToString() == "SetWeaponAmmoStock") textBox2.Text = (@"SetWeaponAmmoStock(weapon , amount)");
            if (listBox1.SelectedItem.ToString() == "Place Holder") textBox2.Text = (@"This is a placeholder string");

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            listBox2.Items.Clear();

            foreach (string str in vars)
            {
                if (str.StartsWith(textBox3.Text, StringComparison.CurrentCultureIgnoreCase))
                {
                    listBox2.Items.Add(str);
                }
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem.ToString() == "level.players") textBox2.Text = ("level.players");
            if (listBox2.SelectedItem.ToString() == "level.gametypes") textBox2.Text = (@"level.var_3FDC");
            if (listBox2.SelectedItem.ToString() == "Place Holder") textBox2.Text = (@"This is a placeholder string");
        }

        private void advancedDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void hudmessagecsvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            advancedDataGridView1.DataSource = GetDataTableFromRemoteCsv("https://raw.githubusercontent.com/leafized/s2functions/main/functions/maps/mp/gametypes/_hud_message.csv", true);
        }

        private void playerBuiltinsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            advancedDataGridView1.DataSource = GetDataTableFromRemoteCsv("https://raw.githubusercontent.com/leafized/s2functions/main/functions/builtins.csv", true);
        }

        private void dvarsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            advancedDataGridView1.DataSource = GetDataTableFromRemoteCsv("https://raw.githubusercontent.com/leafized/s2functions/main/dvars.csv", true);
        }
    }
}
