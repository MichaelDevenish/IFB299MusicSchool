using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for skills.xaml
    /// </summary>
    public partial class skills : Window
    {

        private DatabaseConnector.DatabaseConnector db = new DatabaseConnector.DatabaseConnector();

        public skills()
        {
            InitializeComponent();
            update();

        }

        private void update()
        {


            string query = "SELECT users.first_name, users.last_name, skills.skill_name FROM musicschool.users " + 
                            "JOIN musicschool.user_skills ON users.user_id = user_skills.user_id " + 
                            "JOIN musicschool.skills ON skills.skill_id = user_skills.skill_id;";
            List<string[]> data = db.simpleConnection(false, query, null);


        }
    }
}
