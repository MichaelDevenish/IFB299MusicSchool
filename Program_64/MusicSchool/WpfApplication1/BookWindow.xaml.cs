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
    /// A window that the user uses to book lessons
    /// </summary>
    public partial class BookWindow : Window
    {
        private MainWindow parentWindow;
        private int studentID;
        List<string[]> teacherinfo;
        public BookWindow(MainWindow parentWindow, int studentID)
        {
            DatabaseConnector.DatabaseConnector db = new DatabaseConnector.DatabaseConnector();
            InitializeComponent();
            this.parentWindow = parentWindow;
            this.studentID = studentID;

            teacherinfo = db.ReadTeacherInfo();
            List<string> teacherNames = new List<string>();
            foreach(string[] name in teacherinfo)
            {
                teacherNames.Add(name[1] + " " + name[2]);
            }
            selectTeacher.ItemsSource = teacherNames;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string query = "INSERT INTO `musicschool`.`lessons` (`student_id`, `teacher_id`, `lesson_date`, `attended`, `lesson_length`) VALUES(@userID, @teacherID, @timeDate, '0', '1')";
            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@userID", this.studentID },
                { "@teacherID", teacherinfo[selectTeacher.SelectedIndex][0]},
                { "@timeDate", selectDate.SelectedDate}};

            DatabaseConnector.DatabaseConnector db = new DatabaseConnector.DatabaseConnector();
            db.simpleConnection(true, query, parameters);
        }
    }
}
