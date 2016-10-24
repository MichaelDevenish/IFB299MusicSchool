using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using WpfApplication1;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        SqlConnection con;
        DatabaseConnector.DatabaseConnector database = new DatabaseConnector.DatabaseConnector();
        public Form1()
        {
            
            //establishDBConnection();
            InitializeComponent();
           
            
        }

        private void establishDBConnection()
        {
            con = new SqlConnection("SERVER=db4free.net;DATABASE=musicschool;UID=ifb299admin;PASSWORD=Cpq7BWXZsVP35nZwsUzL;");
            con.Open();
        }

        private bool OpenConnection()
        {
            try
            {
                con.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            dataGridView2.DataSource = database.getInstruments();       
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = database.getUsers();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            database.updateFields();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            database.updateFields();
            
        }

        private void button7_Click(object sender, EventArgs e)
        {

            PasswordManagment pWord = new PasswordManagment();

            string firstName = textBox1.Text;
            string lastName = textBox2.Text;
            string username = textBox3.Text;
            string email = textBox4.Text;
            DateTime dob = dateTimePicker1.Value.Date;
            int role = 3;


            if (comboBox1.Text == "0" || comboBox1.Text == "1" || comboBox1.Text == "2")
            {
                role = Int32.Parse(comboBox1.Text);
            }


            string password = textBox5.Text;
            string confPassword = textBox6.Text;

            string salt;
            byte[] hash;

            if(password == confPassword && role != 3)
            {
                salt = pWord.GenerateSalt();
                hash = pWord.GenerateHash(password, salt);

                database.InsertUser(firstName, lastName, dob, role, hash, salt, username);
            }
            else if(password != confPassword)
            {
                MessageBox.Show("Password and Confirm Password does not match.");
            }
            else if (role == 3)
            {
                MessageBox.Show("Please select a role.");
            }
            else
            {
                MessageBox.Show("Ivalid Input. Please check the fields and try again.");
            }

            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string instName = textBox7.Text;
            string instType = textBox8.Text;
            string quality = richTextBox1.Text;
            database.InsertInstrument(quality, instName, instType);
        }

    }
}
