using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        List<string[]> teacherinfo;
        DatabaseConnector.DatabaseConnector db = new DatabaseConnector.DatabaseConnector();

        List<string> teacherNames = new List<string>();
        List<string> teacherIDs = new List<string>();

        int[,] bookArray = new int[90, 13];


        public BookWindow(MainWindow parentWindow)
        {

            InitializeComponent();
            this.parentWindow = parentWindow;

            selectDate.SelectedDate = DateTime.Now.Date;

            ThreadStart updateTS = new ThreadStart(update);
            Thread initialThread = new Thread(updateTS);
            initialThread.Start();

        }

        private void update()
        {
            //Get the names of the teachers and populate the teacher combobox
            teacherinfo = db.ReadTeacherInfo();

            //get user skills from database so we can display skills next to name
            string query = "SELECT lesson_date, lesson_length FROM lessons";
            foreach (string[] name in teacherinfo)
            {
                teacherNames.Add(name[1] + " " + name[2]);
                teacherIDs.Add(name[0]);
            }
            ;
            this.Dispatcher.Invoke(() => selectTeacher.ItemsSource = teacherNames);


            //Access the databse and determine which timeslots for the next 90 days are
            //valid, and populate bookarray
            string query = "SELECT lesson_date, lesson_length FROM lessons";
            List<string[]> lessons = db.simpleConnection(false, query, null);

            foreach (string[] day in lessons)
            {
                DateTime currentDate = Convert.ToDateTime(day[0]);
                int diff = currentDate.Day - DateTime.Now.Day;
                if (diff >= 0 && (currentDate.Hour - 9) * 2 < 13 && (currentDate.Hour - 9) * 2 >= 0)
                {

                    bookArray[diff, (currentDate.Hour - 9) * 2 + currentDate.Minute / 30] = 1;
                    if (day[1] == "True" && (currentDate.Hour - 9) * 2 != 12) bookArray[diff, (currentDate.Hour - 9) * 2 + 1] = 1;
                }

            }

            List<TimeSpan> validTimes = new List<TimeSpan>();
            for (int i = 0; i < 13; i++)
            {
                if (bookArray[0, i] == 0)
                {
                    validTimes.Add(new TimeSpan(i / 2 + 9, (i % 2) * 30, 0));
                }
            }

            
            this.Dispatcher.Invoke(() => selectTime.ItemsSource = validTimes);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            DateTime date = (DateTime)(selectDate.SelectedDate);

            string query = "INSERT INTO `musicschool`.`lessons` (`student_id`, `teacher_id`, `lesson_date`, `attended`, `lesson_length`) VALUES(@userID, @teacherID, @timeDate, '0', @half)";
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object> {
                    { "@userID", parentWindow.StudentID },
                    { "@teacherID", teacherinfo[selectTeacher.SelectedIndex][0]},
                    { "@timeDate", date.Add((TimeSpan)selectTime.SelectedItem)},
                    { "@half", Convert.ToInt32(selectHalf.IsChecked)} };

                DatabaseConnector.DatabaseConnector db = new DatabaseConnector.DatabaseConnector();
                db.simpleConnection(true, query, parameters);
                MessageBoxResult success = MessageBox.Show("Booking successful", "Success", MessageBoxButton.OK);
                this.Close();
            }
            catch
            {
                MessageBoxResult failure = MessageBox.Show("Booking failed", "Failure", MessageBoxButton.OK);
                this.Close();
            }
        }

        private void changeDate(object sender, SelectionChangedEventArgs e)
        {
            List<TimeSpan> validTimes = new List<TimeSpan>();
            for (int i = 0; i < 13; i++)
            {
                int diff = ((DateTime)(selectDate.SelectedDate)).Day - DateTime.Now.Day;
                if (diff >= 0 && bookArray[diff, i] == 0)
                {
                    validTimes.Add(new TimeSpan(i / 2 + 9, (i % 2) * 30, 0));
                }
            }
            selectTime.ItemsSource = validTimes;
        }
    }
}
