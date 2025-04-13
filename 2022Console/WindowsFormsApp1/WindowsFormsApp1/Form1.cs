using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private const string ServerIp = "127.0.0.1";
        private const int Port = 5000;
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string command = textBox1.Text;

            if (string.IsNullOrWhiteSpace(command))
            {
                MessageBox.Show("Введите команду.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                TcpClient client = new TcpClient(ServerIp, Port);
                NetworkStream stream = client.GetStream();

                byte[] request = Encoding.UTF8.GetBytes(command);
                stream.Write(request, 0, request.Length);

                byte[] buffer = new byte[4096];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                textBox2.Text = response;
                //var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(response);
                //var result = JsonConvert.DeserializeObject<List<JsonResult>>(input);

                //dataGridView1.DataSource = dict;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
