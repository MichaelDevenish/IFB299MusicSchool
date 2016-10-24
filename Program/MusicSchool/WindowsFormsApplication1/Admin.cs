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
    public partial class Admin : Form
    {
        SqlConnection con;
        DatabaseConnector.DatabaseConnector database = new DatabaseConnector.DatabaseConnector();
        public Admin()
        {
            
            //establishDBConnection();
            InitializeComponent();
            this.Text = "Admin Application";
           
            
        }

        private void establishDBConnection()
        {
            con = new SqlConnection("SERVER=db4free.net;DATABASE=musicschool;UID=ifb299admin;PASSWORD=Cpq7BWXZsVP35nZwsUzL;");
            con.Open();
        }

        /// <summary>
        /// calls a method that returns a datatable contianing the instrument data
        /// loads the data into the datagridview object
        /// </summary>
        private void button6_Click(object sender, EventArgs e)
        {
            dataGridView2.DataSource = database.getInstruments();       
        }

        /// <summary>
        /// calls a method that returns a datatable contianing the user data
        /// loads the data into the datagridview object
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = database.getUsers();
        }

        /// <summary>
        /// calls a method to update the instrument information in the database
        /// as per the changes made to the data in the datagridview object
        /// </summary>
        private void button5_Click(object sender, EventArgs e)
        {
            database.updateFields();
        }

        /// <summary>
        /// calls a method to update the user information in the database
        /// as per the changes made to the data in the datagridview object
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            database.updateFields();
            
        }

        /// <summary>
        /// handles the event for the add user button.
        /// checks if inputs are valid
        /// then calls a method to add the new user
        /// </summary>
        private void button7_Click(object sender, EventArgs e)
        {

            PasswordManagment pWord = new PasswordManagment();

            //fetch the information from the input fields
            string firstName = textBox1.Text;
            string lastName = textBox2.Text;
            string username = textBox3.Text;
            string email = textBox4.Text;
            DateTime dob = dateTimePicker1.Value.Date;
            int role = 3;
            string password = textBox5.Text;
            string confPassword = textBox6.Text;

            //if there is valid information selected
            if (comboBox1.Text == "0" || comboBox1.Text == "1" || comboBox1.Text == "2")
            {
                //change value of role to selected value
                role = Int32.Parse(comboBox1.Text);
            }

            //needed for hashing of password
            string salt;
            byte[] hash;

            //if inputs are valid
            if(password == confPassword && role != 3)
            {
                //create the password hash
                salt = pWord.GenerateSalt();
                hash = pWord.GenerateHash(password, salt);

                //call method to add the new user to the database
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

        /// <summary>
        /// handles the event for the add instrument button.
        /// then calls a method to add the new instrument
        /// </summary>
        private void button8_Click(object sender, EventArgs e)
        {
            //fetch the information from the input fields
            string instName = textBox7.Text;
            string instType = textBox8.Text;
            string quality = richTextBox1.Text;
            //call a method to add the information to the database
            database.InsertInstrument(quality, instName, instType);
        }

    }
}
