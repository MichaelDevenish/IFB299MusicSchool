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

            //selectDate.SelectedDate = DateTime.Now.;

            teacherinfo = db.ReadTeacherInfo();
            List<string> teacherNames = new List<string>();
            foreach(string[] name in teacherinfo)
            {
                teacherNames.Add(name[1] + " " + name[2]);
            }
            selectTeacher.ItemsSource = teacherNames;
            string[] validTimes = new string[13] { "9:00am", "9:30am", "10:00am",
                                                     "10:30am", "11:00am", "11:30am", "12:00pm", "12:30pm",
                                                      "1:00pm", "1:30pm", "2:00pm", "2:30pm", "3:00pm"};
            selectTime.ItemsSource = validTimes;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            DateTime date = (DateTime)(selectDate.SelectedDate);

            string query = "INSERT INTO `musicschool`.`lessons` (`student_id`, `teacher_id`, `lesson_date`, `attended`, `lesson_length`) VALUES(@userID, @teacherID, @timeDate, '0', @half)";
            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@userID", this.studentID },
                { "@teacherID", teacherinfo[selectTeacher.SelectedIndex][0]},
                { "@timeDate", date.AddHours(selectTime.SelectedIndex/2 + 9).AddMinutes((selectTime.SelectedIndex % 2)*30)},
                { "@half", Convert.ToInt32(selectHalf.IsChecked)} };

            DatabaseConnector.DatabaseConnector db = new DatabaseConnector.DatabaseConnector();
            db.simpleConnection(true, query, parameters);
        }
    }
}
