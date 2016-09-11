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
        public BookWindow(MainWindow parentWindow, int studentID)
        {
            InitializeComponent();
            this.parentWindow = parentWindow;
            this.studentID = studentID;
            string[] data = { "1", "2", "3", "4", "5" };
            selectTeacher.ItemsSource = data;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string query = "INSERT INTO `musicschool`.`lessons` (`student_id`, `teacher_id`, `lesson_date`, `attended`, `lesson_length`) VALUES(@userID, @teacherID, @timeDate, '0', '1')";
            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@userID", this.studentID },
                { "@teacherID", selectTeacher.SelectedIndex},
                { "@timeDate", "2016-09-16 11:00:00" }};

            DatabaseConnector.DatabaseConnector db = new DatabaseConnector.DatabaseConnector();
            db.simpleConnection(true, query, parameters);
        }
    }
}
