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

namespace terminalHash
{
    public partial class Admin : Form
    {
        public Admin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(@"db4free.net;musicschool;ifb299admin;Cpq7BWXZsVP35nZwsUzL");
            SqlDataAdapter sda = new SqlDataAdapter(@"Select * From users", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
           // dataGridView1.DataSource = dt;

        }
    }
}
