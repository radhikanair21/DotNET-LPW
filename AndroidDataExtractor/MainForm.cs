// MainForm.cs
using System;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace AndroidDataExtractor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string result = RunAdbCommand("devices");
            if (result.Contains("device\n"))
            {
                label1.Text = "Connected";
                label1.ForeColor = System.Drawing.Color.Green;
                textBox1.Text = result;
            }
            else
            {
                label1.Text = "Not Connected";
                label1.ForeColor = System.Drawing.Color.Red;
                textBox1.Text = result;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.PerformClick();
        }

        private string RunAdbCommand(string arguments)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "adb",
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi))
            {
                return process.StandardOutput.ReadToEnd();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string contacts = RunAdbCommand("shell content query --uri content://contacts/phones");
            textBox2.Text = "Contacts Loaded";
            LoadIntoGrid(dataGridView1, contacts);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveToDatabase(dataGridView1, "ContactsTable", new[] { "Name", "PhoneNumber" });
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string sms = RunAdbCommand("shell content query --uri content://sms");
            textBox3.Text = "Messages Loaded";
            LoadIntoGrid(dataGridView2, sms);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SaveToDatabase(dataGridView2, "MessagesTable", new[] { "Sender", "MessageContent", "Timestamp" });
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string calls = RunAdbCommand("shell content query --uri content://call_log/calls");
            textBox4.Text = "Call Logs Loaded";
            LoadIntoGrid(dataGridView3, calls);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SaveToDatabase(dataGridView3, "CallLogsTable", new[] { "PhoneNumber", "CallType", "Duration" });
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string cpu = RunAdbCommand("shell cat /proc/cpuinfo");
            string mem = RunAdbCommand("shell cat /proc/meminfo");
            textBox5.Text = cpu;
            textBox6.Text = mem;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection("YourConnectionString"))
            {
                con.Open();
                string query = "INSERT INTO DeviceInfoTable (CPUInfo, MemoryInfo) VALUES (@cpu, @mem)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@cpu", textBox5.Text);
                    cmd.Parameters.AddWithValue("@mem", textBox6.Text);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void LoadIntoGrid(DataGridView dgv, string adbOutput)
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();

            var lines = adbOutput.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var cells = line.Split(' ');
                if (dgv.Columns.Count < cells.Length)
                {
                    dgv.Columns.Clear();
                    for (int i = 0; i < cells.Length; i++)
                        dgv.Columns.Add("col" + i, "Field " + i);
                }
                dgv.Rows.Add(cells);
            }
        }

        private void SaveToDatabase(DataGridView dgv, string tableName, string[] columnNames)
        {
            using (SqlConnection con = new SqlConnection("YourConnectionString"))
            {
                con.Open();
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue;

                    string columns = string.Join(", ", columnNames);
                    string values = string.Join(", ", columnNames.Select(c => "@" + c));

                    string query = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        for (int i = 0; i < columnNames.Length; i++)
                        {
                            cmd.Parameters.AddWithValue("@" + columnNames[i], row.Cells[i].Value?.ToString() ?? "");
                        }
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
