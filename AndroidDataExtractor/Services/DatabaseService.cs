using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace AndroidDataExtractor
{
    public partial class MainForm : Form
    {
        private SQLiteConnection dbConnection;
        private bool isConnected = false;

        public MainForm()
        {
            InitializeComponent();
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                // Create database if it doesn't exist
                if (!File.Exists("android_data.db"))
                {
                    SQLiteConnection.CreateFile("android_data.db");
                }

                // Open connection
                dbConnection = new SQLiteConnection("Data Source=android_data.db;Version=3;");
                dbConnection.Open();

                // Create tables if they don't exist
                string[] createTableCommands = new string[]
                {
                    "CREATE TABLE IF NOT EXISTS Contacts (ID INTEGER PRIMARY KEY, Name TEXT, PhoneNumber TEXT, Email TEXT)",
                    "CREATE TABLE IF NOT EXISTS SMS (ID INTEGER PRIMARY KEY, Sender TEXT, Content TEXT, Timestamp TEXT)",
                    "CREATE TABLE IF NOT EXISTS CallLogs (ID INTEGER PRIMARY KEY, Number TEXT, Type TEXT, Duration TEXT, Time TEXT)",
                    "CREATE TABLE IF NOT EXISTS DeviceInfo (ID INTEGER PRIMARY KEY, DeviceID TEXT, Model TEXT, Manufacturer TEXT, CPUInfo TEXT, MemoryInfo TEXT)"
                };

                foreach (string cmd in createTableCommands)
                {
                    using (SQLiteCommand command = new SQLiteCommand(cmd, dbConnection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database initialization error: " + ex.Message);
            }
        }

        // CONNECTION TAB
        private void button1_Click_1(object sender, EventArgs e)
        {
            // Check Connection
            isConnected = true;
            label1.Text = "Connected";
            label1.ForeColor = System.Drawing.Color.Green;
            textBox1.Text = "Device Model: Pixel 6\r\nManufacturer: Google\r\nDevice ID: ABCD1234";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Refresh Connection
            button1_Click_1(sender, e);
        }

        // CONTACTS TAB
        private void button3_Click_1(object sender, EventArgs e)
        {
            // Load Contacts
            if (!isConnected)
            {
                MessageBox.Show("Please connect to a device first.");
                return;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("PhoneNumber");
            dt.Columns.Add("Email");

            dt.Rows.Add("John Smith", "555-1234", "john@example.com");
            dt.Rows.Add("Jane Doe", "555-5678", "jane@example.com");

            dataGridView1.DataSource = dt;
            button4.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Save Contacts
            try
            {
                if (dataGridView1.DataSource == null)
                {
                    MessageBox.Show("No contacts to save.");
                    return;
                }

                DataTable dt = (DataTable)dataGridView1.DataSource;
                using (SQLiteCommand cmd = new SQLiteCommand(dbConnection))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        cmd.CommandText = "INSERT INTO Contacts (Name, PhoneNumber, Email) VALUES (@name, @phone, @email)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@name", row["Name"]);
                        cmd.Parameters.AddWithValue("@phone", row["PhoneNumber"]);
                        cmd.Parameters.AddWithValue("@email", row["Email"]);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Contacts saved to database!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving contacts: " + ex.Message);
            }
        }

        // MESSAGES TAB
        private void button5_Click_1(object sender, EventArgs e)
        {
            // Load SMS
            if (!isConnected)
            {
                MessageBox.Show("Please connect to a device first.");
                return;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("Sender");
            dt.Columns.Add("Content");
            dt.Columns.Add("Timestamp");

            dt.Rows.Add("555-1234", "Hello there", DateTime.Now.AddHours(-1));
            dt.Rows.Add("555-5678", "Meeting at 3pm", DateTime.Now.AddHours(-3));

            dataGridView2.DataSource = dt;
            button6.Enabled = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Save SMS
            try
            {
                if (dataGridView2.DataSource == null)
                {
                    MessageBox.Show("No SMS to save.");
                    return;
                }

                DataTable dt = (DataTable)dataGridView2.DataSource;
                using (SQLiteCommand cmd = new SQLiteCommand(dbConnection))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        cmd.CommandText = "INSERT INTO SMS (Sender, Content, Timestamp) VALUES (@sender, @content, @timestamp)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@sender", row["Sender"]);
                        cmd.Parameters.AddWithValue("@content", row["Content"]);
                        cmd.Parameters.AddWithValue("@timestamp", row["Timestamp"]);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("SMS saved to database!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving SMS: " + ex.Message);
            }
        }

        // CALL LOGS TAB
        private void button7_Click_1(object sender, EventArgs e)
        {
            // Load Call Logs
            if (!isConnected)
            {
                MessageBox.Show("Please connect to a device first.");
                return;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("Number");
            dt.Columns.Add("Type");
            dt.Columns.Add("Duration");
            dt.Columns.Add("Time");

            dt.Rows.Add("555-1234", "Incoming", "1m 23s", DateTime.Now.AddDays(-1));
            dt.Rows.Add("555-5678", "Outgoing", "0m 45s", DateTime.Now.AddHours(-5));

            dataGridView3.DataSource = dt;
            button8.Enabled = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // Save Call Logs
            try
            {
                if (dataGridView3.DataSource == null)
                {
                    MessageBox.Show("No call logs to save.");
                    return;
                }

                DataTable dt = (DataTable)dataGridView3.DataSource;
                using (SQLiteCommand cmd = new SQLiteCommand(dbConnection))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        cmd.CommandText = "INSERT INTO CallLogs (Number, Type, Duration, Time) VALUES (@number, @type, @duration, @time)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@number", row["Number"]);
                        cmd.Parameters.AddWithValue("@type", row["Type"]);
                        cmd.Parameters.AddWithValue("@duration", row["Duration"]);
                        cmd.Parameters.AddWithValue("@time", row["Time"]);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Call logs saved to database!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving call logs: " + ex.Message);
            }
        }

        // DEVICE INFO TAB
        private void button9_Click_1(object sender, EventArgs e)
        {
            // Load Device Info
            if (!isConnected)
            {
                MessageBox.Show("Please connect to a device first.");
                return;
            }

            textBox5.Text = "CPU: Octa-core\r\nProcessor: Google Tensor\r\nCores: 8";
            textBox6.Text = "RAM: 8GB\r\nStorage: 128GB\r\nFree Space: 64GB";
            button10.Enabled = true;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            // Save Device Info
            try
            {
                if (string.IsNullOrEmpty(textBox5.Text) || string.IsNullOrEmpty(textBox6.Text))
                {
                    MessageBox.Show("No device info to save.");
                    return;
                }

                using (SQLiteCommand cmd = new SQLiteCommand(dbConnection))
                {
                    cmd.CommandText = "INSERT INTO DeviceInfo (DeviceID, Model, Manufacturer, CPUInfo, MemoryInfo) VALUES (@id, @model, @manufacturer, @cpu, @memory)";
                    cmd.Parameters.AddWithValue("@id", "ABCD1234");
                    cmd.Parameters.AddWithValue("@model", "Pixel 6");
                    cmd.Parameters.AddWithValue("@manufacturer", "Google");
                    cmd.Parameters.AddWithValue("@cpu", textBox5.Text);
                    cmd.Parameters.AddWithValue("@memory", textBox6.Text);
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Device info saved to database!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving device info: " + ex.Message);
            }
        }

        // SEARCH FUNCTIONALITY
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Search contacts
            SearchDataGrid(dataGridView1, textBox2.Text);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            // Search SMS
            SearchDataGrid(dataGridView2, textBox3.Text);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            // Search call logs
            SearchDataGrid(dataGridView3, textBox4.Text);
        }

        private void SearchDataGrid(DataGridView dgv, string searchText)
        {
            if (dgv.DataSource == null || string.IsNullOrWhiteSpace(searchText)) return;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                bool visible = false;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null && cell.Value.ToString().ToLower().Contains(searchText.ToLower()))
                    {
                        visible = true;
                        break;
                    }
                }
                row.Visible = visible;
            }
        }

        // Empty event handlers to satisfy the designer
        private void label1_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox5_TextChanged(object sender, EventArgs e) { }
        private void textBox6_TextChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        // Close the database when the form closes
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            if (dbConnection != null && dbConnection.State == ConnectionState.Open)
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }
    }
}