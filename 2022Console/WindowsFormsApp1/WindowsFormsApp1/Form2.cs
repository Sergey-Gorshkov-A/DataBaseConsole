using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        private const string ServerIp = "127.0.0.1";
        private const int Port = 5000;
        public Form2()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {
        }

        private void buttonAuth_Click(object sender, EventArgs e)
        {
            string command = "AUTH:" + loginBox.Text + ";" + passwordBox.Text;

            if (string.IsNullOrWhiteSpace(command))
            {
                MessageBox.Show("Введите логин или пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                if (response == "Вход успешен")
                {
                    Form1 form1 = new Form1();
                    form1.Show();
                }
                else 
                {
                    MessageBox.Show($"Ошибка: неверно введён пароль или логин", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonRegistration_Click(object sender, EventArgs e)
        {
            Form3 form = new Form3();
            form.Show();
        }
    }
}
