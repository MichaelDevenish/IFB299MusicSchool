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
            fillComboBox();
            update();

        }

        private void update()
        {


            string query = "SELECT users.first_name, users.last_name, skills.skill_name FROM musicschool.users " + 
                            "JOIN musicschool.user_skills ON users.user_id = user_skills.user_id " + 
                            "JOIN musicschool.skills ON skills.skill_id = user_skills.skill_id;";
            List<string[]> data = db.simpleConnection(false, query, null);
            List<entry> entries = new List<entry>();

            
            foreach(string[] el in data)
            {
                entries.Add(new entry { FirstName = el[0], LastName = el[1], Skills = el[2]});
            }
            
            dataGrid.ItemsSource = entries;
          


        }

        public class entry
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Skills { get; set; }
            
        }

        public void fillComboBox()
        {
            string query = "SELECT  user_ID, first_name, last_name FROM users WHERE role=1";
            List<string[]> teachers = db.simpleConnection(false, query, null);
            List<string> teacherNames = new List<string>();
            foreach(string[] teach in teachers)
            {
                teacherNames.Add(teach[1] + " " + teach[2]);
            }
            teacherSelect.ItemsSource = teacherNames;

            query = "SELECT  skill_ID, skill_name FROM skills";
            List<string[]> skills = db.simpleConnection(false, query, null);
            List<string> skillnames = new List<string>();
            foreach (string[] sk in skills)
            {
                skillnames.Add(sk[1]);
            }
            skillSelect.ItemsSource = skillnames;
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void addskill_Click(object sender, RoutedEventArgs e)
        {
            string query = "DELETE FROM user_skills WHERE ";
        }
    }
}
