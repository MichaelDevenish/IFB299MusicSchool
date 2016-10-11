using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
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
        List<entry> tableData;
        Dictionary<string, string> teachers;
        Dictionary<string, string> teachers_inverse;
        Dictionary<string, string> skillList;
        Dictionary<string, string> skillList_inverse;
        List<string[]> skillInfo;

        public skills()
        {
            InitializeComponent();
            ThreadStart initialize = new ThreadStart(getInfo);
            Thread initialThread = new Thread(initialize);
            initialThread.Start();

        }

        /// <summary>
        /// Run the function to update the table and populate the comboboxes one after the other
        /// Makes threading easier.
        /// </summary>
        private void getInfo()
        {
            fillComboBox();
            update();
        }


        /// <summary>
        /// Update the skills table
        /// </summary>
        private void update()
        {
            /*
            string query = "SELECT users.first_name, users.last_name, skills.skill_name FROM musicschool.users " + 
                            "JOIN musicschool.user_skills ON users.user_id = user_skills.user_id " + 
                            "JOIN musicschool.skills ON skills.skill_id = user_skills.skill_id;";
            List<string[]> data = db.simpleConnection(false, query, null);
            tableData = new List<entry>();
            */

            string query = "SELECT user_id, skill_id FROM user_skills";
            List<string[]> data = db.simpleConnection(false, query, null);
            tableData = new List<entry>();
            foreach (string[] el in data)
            {
                tableData.Add(new entry { UserID = el[0], SkillID = el[1]});
            }

            //manipulate the table synchroniously
            this.Dispatcher.Invoke(()=>  dataGrid.ItemsSource = tableData);

        }

        /// <summary>
        /// This class takes the info from the database and stores it so that the datagrid lays it out nicely
        /// </summary>
        public class entry
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Skills { get; set; }
            public string UserID { get; set; }
            public string SkillID { get; set; }
            
        }


        /// <summary>
        /// populate the combobox selectors. Only needs to be run once as opposed to the update function
        /// </summary>
        public void fillComboBox()
        {
            string query = "SELECT  user_ID, first_name, last_name FROM users WHERE role=1";
            List<string[]> data = db.simpleConnection(false, query, null);
            teachers = new Dictionary<string, string>();
            List<string> teacherNames = new List<string>();
            foreach(string[] teach in data)
            {
                teachers.Add(teach[1] + " " + teach[2], teach[0]);
            }

            this.Dispatcher.Invoke(() => teacherSelect.ItemsSource = teachers.Keys);
            

            query = "SELECT  skill_ID, skill_name FROM skills";
            skillInfo = db.simpleConnection(false, query, null);
            skillList = new Dictionary<string, string>();
            foreach (string[] sk in skillInfo)
            {
                skillList.Add(sk[1], sk[0]);
            }

            this.Dispatcher.Invoke(() => skillSelect.ItemsSource = skillList.Keys);
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void addskill_Click(object sender, RoutedEventArgs e)
        {
            string query = "INSERT INTO user_skills(skill_ID, user_ID) VALUES (@skillID, @userID)";
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                {"skillID", skillList[(string)skillSelect.SelectedItem]},
                {"userID", teachers[(string)teacherSelect.SelectedItem] }
            };
            db.simpleConnection(true, query, param);

            ThreadStart upd = new ThreadStart(update);
            Thread updThread = new Thread(upd);
            updThread.Start();
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            entry delete = (entry)dataGrid.SelectedItem;
            string query = "DELETE FROM user_skills WHERE skill_ID = @skillID AND user_ID = @userID";
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                {"skillID", delete.SkillID },
                {"userID", delete.UserID }
            };
            db.simpleConnection(true, query, param);

            ThreadStart upd = new ThreadStart(update);
            Thread updThread = new Thread(upd);
            updThread.Start();
        }
    }
}
