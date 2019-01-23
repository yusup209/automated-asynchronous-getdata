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
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace asynchronous_getdata
{
    public partial class Form1 : Form
    {
        WebClient wb;
        static string api_url = "https://jsonplaceholder.typicode.com/posts";
        string data_count = "";
        int test_count = 1;

        public Form1()
        {
            InitializeComponent();
        }

        //get data
        private void get_data()
        {
            wb = new WebClient();
            dataGridView1.Invoke(new Action(() => dataGridView1.Rows.Clear()));
            string str_json = wb.DownloadString(api_url);
            //PostsClass posts = JsonConvert.DeserializeObject<PostsClass>(str_json);
            JArray obj = JArray.Parse(str_json);
            
            foreach (JObject o in obj.Children<JObject>())
            {
                foreach (JProperty p in o.Properties())
                {
                    dataGridView1.Invoke(new Action(() => dataGridView1.Rows.Add(o["userId"].ToString(), o["id"].ToString(), o["title"].ToString(), o["body"].ToString())));
                }
            }
            data_count = obj.Count().ToString();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            button1.Invoke(new Action(() => button1.Text = "STOP"));
            button1.Invoke(new Action(() => button1.BackColor = Color.Crimson));
            get_data();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripStatusLabel1.Text = "Completed!";
            button1.Text = "START";
            button1.BackColor = SystemColors.MenuHighlight;
            //backgroundWorker1.Dispose();
            test_count++;
            toolStripStatusLabel2.Text = Convert.ToString(test_count) + "x Times";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "START")
            {
                timer1.Stop();
                timer1.Enabled = false;
                button1.Text = "STOP";
                button1.BackColor = Color.Crimson;
                backgroundWorker1.RunWorkerAsync();
                timer1.Enabled = true;
                timer1.Start();
            } else if (button1.Text == "STOP")
            {
                button1.Text = "START";
                button1.BackColor = SystemColors.MenuHighlight;

                if (backgroundWorker1.CancellationPending)
                {
                    backgroundWorker1.CancelAsync();
                }

                timer1.Stop();
                timer1.Enabled = false;
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Working...";
            backgroundWorker1.RunWorkerAsync();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel3.Text = "Mem. Usage : " + Convert.ToString((Process.GetCurrentProcess().PrivateMemorySize64 - (10*1024*1024)) / (1024 * 1024)) + " MegaBytes";
        }
    }
}
