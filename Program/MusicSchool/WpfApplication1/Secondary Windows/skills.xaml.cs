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
        Dictionary<string, string> teachers_inverse = new Dictionary<string, string>();
        Dictionary<string, string> skillList;
        Dictionary<string, string> skillList_inverse = new Dictionary<string, string>();
        List<string[]> skillInfo;
        List<string[]> userSkills;

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

            string query = "SELECT user_id, skill_id FROM user_skills";
            userSkills = db.simpleConnection(false, query, null);
            tableData = new List<entry>();
            foreach (string[] el in userSkills)
            {
                try {
                    tableData.Add(new entry { Name = teachers_inverse[el[0]], Skill = skillList_inverse[el[1]] });
                }
                catch
                {

                }
            }

            //manipulate the table synchroniously
            this.Dispatcher.Invoke(()=>  dataGrid.ItemsSource = tableData);

        }

        /// <summary>
        /// This class takes the info from the database and stores it so that the datagrid lays it out nicely
        /// </summary>
        public class entry
        {
            public string Name { get; set; }
            public string Skill { get; set; }
            
        }


        /// <summary>
        /// populate the combobox selectors as well as store information as bijective dictionary
        /// for later use
        /// </summary>
        public void fillComboBox()
        {

            //Get info on teachers
            string query = "SELECT  user_ID, first_name, last_name FROM users WHERE role=1";
            List<string[]> data = db.simpleConnection(false, query, null);
            teachers = new Dictionary<string, string>();
            foreach(string[] teach in data)
            {
                teachers.Add(teach[1] + " " + teach[2], teach[0] + "");
                teachers_inverse.Add(teach[0] + "", teach[1] + " " + teach[2]);
            }

            this.Dispatcher.Invoke(() => teacherSelect.ItemsSource = teachers.Keys);
            
            //Get the info on skills
            query = "SELECT  skill_ID, skill_name FROM skills";
            skillInfo = db.simpleConnection(false, query, null);
            skillList = new Dictionary<string, string>();
            foreach (string[] sk in skillInfo)
            {
                skillList.Add(sk[1], sk[0]);
                skillList_inverse.Add(sk[0], sk[1]);
            }

            this.Dispatcher.Invoke(() => skillSelect.ItemsSource = skillList.Keys);
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// Run when the user clicks add skill button. Reads information from the combobox selectors.
        /// Uses the bijective skill and teacher dictionaries to obtain database friendly values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addskill_Click(object sender, RoutedEventArgs e)
        {
            string addSkill = skillList[(string)skillSelect.SelectedItem];
            string addUser = teachers[(string)teacherSelect.SelectedItem];

            //use the dictionaries to quickly check if value isn't already in database
            foreach(string[] us in userSkills)
            {
                if(us[0] == addUser && us[1] == addSkill)
                {
                    System.Windows.MessageBox.Show("User skill already exists in database");
                    return;
                }
            }

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
                {"skillID", skillList[delete.Skill] },
                {"userID", teachers[delete.Name] }
            };
            db.simpleConnection(true, query, param);

            ThreadStart upd = new ThreadStart(update);
            Thread updThread = new Thread(upd);
            updThread.Start();
        }
    }
}
