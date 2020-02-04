using System;
using System.Windows.Forms;
using System.Data.SqlClient; // SQL Server local DB
using System.Net; // Это система библиотеки Windows
using System.Net.Mail;
using System.Drawing;

// ДЛЯ ТЕСТА РАБОТОСПОСОБНОСТИ МОЕЙ ПРОГРАММЫ (github: https://github.com/kayweba)
// НЕОБХОДИМО УКАЗАТЬ СВОИ ПАРОЛЬ И ПОЧТУ В private void SendMessage - ПРЕДПОСЛЕДНИЙ МЕТОД В ЭТОМ ФАЙЛЕ.

namespace LocalDB
{
    public partial class Form1 : Form
    {
        SqlConnection cn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Database1.mdf;Integrated Security=True;");
        SqlCommand cmd = new SqlCommand();
        SqlDataReader dr;
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmd.Connection = cn;
            loadlist();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (txtId.Text != "" && txtName.Text != "" && insuranceNum.Text != "" && phoneNumber.Text != "" && emailAdress.Text != "")
            {
                cn.Open();
                cmd.CommandText = "insert into [Table] (id, name, chinumber, phonenumber, email) values ('" + txtId.Text + "','" + txtName.Text + "', '"+ insuranceNum.Text + "', '"+ phoneNumber.Text +"', '"+ emailAdress.Text + "')";
                cmd.ExecuteNonQuery();
                cmd.Clone();
                MessageBox.Show("Record inserted!");
                cn.Close();
                loadlist();
                clearTextBox();

            }
        }

        private void loadlist()
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            listBox4.Items.Clear();
            listBox5.Items.Clear();
            cn.Open();
            cmd.CommandText = "select * from [Table]";
            dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    listBox1.Items.Add(dr[0].ToString());
                    listBox2.Items.Add(dr[1].ToString());
                    listBox3.Items.Add(dr[2].ToString());
                    listBox4.Items.Add(dr[3].ToString());
                    listBox5.Items.Add(dr[4].ToString());
                }
            }

            cn.Close();
        }

        private void clearTextBox()
        {
            txtId.Text = "";
            txtName.Text = "";
            insuranceNum.Text = "";
            phoneNumber.Text = "";
            emailAdress.Text = "";
        }

        private void listBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            ListBox l = sender as ListBox;
            if (l.SelectedIndex != -1)
            {
                listBox1.SelectedIndex = l.SelectedIndex;
                listBox2.SelectedIndex = l.SelectedIndex;
                listBox3.SelectedIndex = l.SelectedIndex;
                listBox4.SelectedIndex = l.SelectedIndex;
                listBox5.SelectedIndex = l.SelectedIndex;

                txtId.Text = listBox1.SelectedItem.ToString();
                txtName.Text = listBox2.SelectedItem.ToString();
                insuranceNum.Text = listBox3.SelectedItem.ToString();
                phoneNumber.Text = listBox4.SelectedItem.ToString();
                emailAdress.Text = listBox5.SelectedItem.ToString();

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (txtId.Text != "" && txtName.Text != "" && insuranceNum.Text != "" && phoneNumber.Text != "" && emailAdress.Text != "")
            {
                cn.Open();
                cmd.CommandText = "delete from [Table] where id='" + txtId.Text + "' and name='" + txtName.Text + "' and chinumber='"+insuranceNum.Text+ "' and phonenumber='"+phoneNumber.Text+"' and email='"+ emailAdress.Text+"' ";
                cmd.ExecuteNonQuery();
                cn.Close();
                MessageBox.Show("Record Deleted");
                loadlist();
                clearTextBox();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (txtId.Text != "" && txtName.Text != "" && listBox1.SelectedIndex != -1)
            {
                cn.Open();
                cmd.CommandText = "update [Table] set id='"+txtId.Text+"', name='"+txtName.Text+ "', chinumber='"+insuranceNum.Text+ "', phonenumber='"+phoneNumber.Text+ "', email='" +emailAdress.Text + "'  where id='" + listBox1.SelectedItem.ToString()+"' and name='"+listBox2.SelectedItem.ToString()+ "' and chinumber='"+ listBox3.SelectedItem.ToString() + "' and phonenumber='"+listBox4.SelectedItem.ToString() + "' and email='" + listBox5.SelectedItem.ToString()+"'";
                cmd.ExecuteNonQuery();
                cn.Close();
                MessageBox.Show("Record Updated");
                loadlist();
                clearTextBox();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(seacrhBox.Text != "")
            {
                cn.Open();
                cmd.CommandText = "select * from [Table] where name='" + seacrhBox.Text + "'";
                dr = cmd.ExecuteReader();
                //cmd.ExecuteNonQuery();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        label7.Visible = true;
                        label9.Text += dr[0].ToString() + " " + dr[1].ToString() + "\r\n" + dr[2].ToString() + "\r\n" + dr[3].ToString() + "\r\n" + dr[4].ToString();
                    }

                }
                cn.Close();

            }
        }

        private void SendMessage(string userName, string adressTo, string messageSubject, string messageText)
        {
            try
            {
                string from = @""; // Адрес отправителя
                string pass = ""; // Пароль отправителя
                MailMessage mess = new MailMessage();
                mess.To.Add(adressTo); // Адрес получателя
                mess.From = new MailAddress(from);
                mess.Subject = messageSubject; // Тема письма
                mess.Body = messageText;
                SmtpClient client = new SmtpClient();
                client.Host = "smtp.mail.ru";
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(from.Split('@')[0], pass);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mess); // Отправка пользователю
                label10.ForeColor = Color.Green;
                label10.Text = "Сообщение доставлено!";
                mess.To.Remove(mess.To[0]);
                mess.To.Add(from); // для сообщения на свой адрес
                mess.Subject = "Отправлено сообщение";
                mess.Body = "Пользователю " + userName + " отправлено сообщение";
                client.Send(mess); // Отправка себе сообщения, высланное пользователю
                mess.Dispose();
                
            }

            catch (Exception e)
            {
                label10.ForeColor = Color.Red;
                label10.Text = "Сообщение не было доставлено. Проверьте введенную информацию!";
                throw new Exception("Mail.Send: " + e.Message);
            }
            

        }
        private void button5_Click(object sender, EventArgs e)
        {
            string userName = nameUser.Text; // Имя пользователя
            string addressTo = mailAddress.Text; // Адрес пользователя
            string messageSubject = mailSubject.Text; // Тема письма
            string messageText = mailBody.Text;
            SendMessage(userName, addressTo, messageSubject, messageText);
        }

        private void mailClear_Click(object sender, EventArgs e)
        {
            nameUser.Text = "";
            mailAddress.Text = "";
            mailSubject.Text = "";
            mailBody.Text = "";
        }

        
    }
}
