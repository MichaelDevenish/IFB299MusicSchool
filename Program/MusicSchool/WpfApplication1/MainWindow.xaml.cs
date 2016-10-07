﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{

    //TODO find out what row and column the result goes in (LINE 85)

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region globals
        private bool logged_in = true;
        private bool isAdmin = false;
        private bool isTeacher = false;
        private int studentID = -1;

        public ObservableCollection<HalfHour> allTimetables;
        public ObservableCollection<HalfHour> myTimetables;
        private List<string[]> teacherInfo;
        private List<string[]> instrumentInfo;
        private DatabaseConnector.DatabaseConnector db;
        private BackgroundWorker worker;
        #endregion
        #region Properties
        public List<string[]> TeacherInfo { get { return teacherInfo; } }
        public List<string[]> InstrumentInfo { get { return instrumentInfo; } }
        public DatabaseConnector.DatabaseConnector DB { get { return db; } }
        public int StudentID { get { return studentID; } set { studentID = value; } }
        public bool IsAdmin { get { return isAdmin; } set { isAdmin = value; } }
        public bool IsTeacher { get { return isTeacher; } set { isTeacher = value; } }

        #endregion
        #region general
        public MainWindow()
        {
            db = new DatabaseConnector.DatabaseConnector();

            InitializeComponent();
            imgMusicSchool.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "/images/header.jpg"));

            BackgroundWorker firstLoad = new BackgroundWorker();
            firstLoad.DoWork += info_DoWork;
            firstLoad.RunWorkerAsync();

            worker = new BackgroundWorker();
            worker.DoWork += timetable_DoWork;
            worker.RunWorkerAsync();

            List<Messages> messages = new List<Messages>();
            Messages conversation1 = new Messages(1, 3, 4, "john doe");
            conversation1.AddMessage("test1", DateTime.Now, "i am testing", 3);
            conversation1.AddMessage("test2", DateTime.Now, "i am replying to the test", 4);
            conversation1.AddMessage("test3", DateTime.Now, "cool", 3);
            messages.Add(conversation1);
            Messages conversation2 = new Messages(2, 3, 4, "john doe");
            conversation2.AddMessage("test1", DateTime.Now, "i am testing", 3);
            conversation2.AddMessage("test2", DateTime.Now, "i am replying to the test", 4);
            conversation2.AddMessage("test3", DateTime.Now, "cool", 3);
            messages.Add(conversation2);

            listBox.ItemsSource = messages;
        }

        /// <summary>
        /// Gets the initial information from the database that is needed for the application to run
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void info_DoWork(object sender, DoWorkEventArgs e)
        {
            DatabaseConnector.DatabaseConnector data = new DatabaseConnector.DatabaseConnector();
            List<string[]> teacherThread = data.ReadTeacherInfo();
            List<string[]> instrumentThread = data.ReadInstrumentInfo();
            this.Dispatcher.Invoke((Action)(() =>
            {
                teacherInfo = teacherThread;
                instrumentInfo = instrumentThread;
                //for (int i = 0; i < teacherInfo.Count; i++)
                //    cmbRecipient.Items.Add(teacherInfo[i][1] + " " + teacherInfo[i][2]);
                for (int i = 0; i < instrumentInfo.Count; i++)
                    cmbInstrument.Items.Add(instrumentInfo[i][1]);
                if (isAdmin)
                    adminLessonButton.Visibility = Visibility.Visible;
            }));
        }

        //Each time the user changes the tab, check if the login status has changes and make the appropriate
        //adjustments
        private void tab_changed(object sender, SelectionChangedEventArgs e)
        {
            checkAbilities();
        }

        private void checkAbilities()
        {
            adminLessonButton.Visibility = Visibility.Hidden;
            if (logged_in)
            {
                //get data for timetable info
                if (timetableTab.IsSelected)
                {
                    if (!worker.IsBusy)
                        worker.RunWorkerAsync();

                    //RefreshTimetables();
                }
                if (isAdmin && teacherInfo != null)
                {
                    adminLessonButton.Visibility = Visibility.Visible;
                }

                errorMessage.Visibility = System.Windows.Visibility.Hidden;
                bookButton.IsEnabled = true;
            }
            //timetable_ret.Visibility = System.Windows.Visibility.Visible;

            else
            {
                errorMessage.Visibility = System.Windows.Visibility.Visible;
                bookButton.IsEnabled = false;
                //timetable_ret.Visibility = System.Windows.Visibility.Hidden;

            }
        }
        #endregion
        #region timetable
        /// <summary>
        /// Refreshes the timetables with data from the database
        /// </summary>
        void timetable_DoWork(object sender, DoWorkEventArgs e)
        {
            DatabaseConnector.DatabaseConnector data = new DatabaseConnector.DatabaseConnector();
            allTimetables = GenerateTimetableDataBindings();
            LoadTimetableData(data.ReadEmptyLessons(), allTimetables);
            myTimetables = GenerateTimetableDataBindings();
            LoadTimetableData(data.ReadUserLessons(studentID), myTimetables);

            this.Dispatcher.Invoke((Action)(() =>
            {
                allClassesTable.ItemsSource = allTimetables;
                allClassesTable.Items.Refresh();
                myClassesTable.ItemsSource = myTimetables;
                myClassesTable.Items.Refresh();
            }));
        }

        /// <summary>
        /// loads all of the rows from the supplied query into the 
        /// supplied data binding
        /// </summary>
        /// <param name="results">the results that are to be added</param>
        /// <param name="timetable">the table databinding that is to be added to</param>
        private void LoadTimetableData(List<string[]> results, ObservableCollection<HalfHour> timetable)
        {
            foreach (string[] result in results)
            {
                DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                System.Globalization.Calendar cal = dfi.Calendar;

                DateTime lessonDate = DateTime.Parse(result[2]);
                DateTime today = DateTime.Today;

                int hours = (lessonDate.Hour * 2) - 18;
                if (lessonDate.Minute == 30)
                    hours++;

                bool length = bool.Parse(result[3]);

                if (cal.GetWeekOfYear(today, dfi.CalendarWeekRule, dfi.FirstDayOfWeek)
                    == cal.GetWeekOfYear(lessonDate, dfi.CalendarWeekRule, dfi.FirstDayOfWeek)
                    && lessonDate.Year == today.Year)
                {
                    ShowLessonOnTimetable(result, timetable[hours]);
                    if (length) ShowLessonOnTimetable(result, timetable[hours + 1]);
                }
                //possible ways to reduce data use, get only the lessons within a timespan
            }
        }

        /// <summary>
        /// A function that places the supplied lesson on the table.
        /// Messy code since it relies on object refrences.
        /// </summary>
        /// <param name="result">the database result for that lesson</param>
        /// <param name="hour">the HalfHour object that the item is being placed in</param>
        private void ShowLessonOnTimetable(string[] result, HalfHour hour)
        {
            DateTime lessonDate = DateTime.Parse(result[2]);
            switch ((int)lessonDate.DayOfWeek)
            {
                case 0:
                    if (hour.Sunday.Length == 0)
                        hour.Sunday = result[0] + " " + result[1];
                    else hour.Sunday += "\n" + result[0] + " " + result[1];
                    break;
                case 1:
                    if (hour.Monday.Length == 0)
                        hour.Monday = result[0] + " " + result[1];
                    else hour.Monday += "\n" + result[0] + " " + result[1];
                    break;
                case 2:
                    if (hour.Tuesday.Length == 0)
                        hour.Tuesday = result[0] + " " + result[1];
                    else hour.Tuesday += "\n" + result[0] + " " + result[1];
                    break;
                case 3:
                    if (hour.Wednesday.Length == 0)
                        hour.Wednesday = result[0] + " " + result[1];
                    else hour.Wednesday += "\n" + result[0] + " " + result[1];
                    break;
                case 4:
                    if (hour.Thursday.Length == 0)
                        hour.Thursday = result[0] + " " + result[1];
                    else hour.Thursday += "\n" + result[0] + " " + result[1];
                    break;
                case 5:
                    if (hour.Friday.Length == 0)
                        hour.Friday = result[0] + " " + result[1];
                    else hour.Friday += "\n" + result[0] + " " + result[1];
                    break;
                case 6:
                    if (hour.Saturday.Length == 0)
                        hour.Saturday = result[0] + " " + result[1];
                    else hour.Saturday += "\n" + result[0] + " " + result[1];
                    break;
            }
        }

        /// <summary>
        /// Generates a blank databinding for the timetables
        /// </summary>
        /// <returns>a blank array of object HalfHour that is all blank except for Time</returns>
        private ObservableCollection<HalfHour> GenerateTimetableDataBindings()
        {
            ObservableCollection<HalfHour> Days = new ObservableCollection<HalfHour>();
            for (int i = 0; i <= 16; i++)
            {
                Days.Add(new HalfHour()
                {
                    Time = HelperFunctions.Get9to5TimeFrom16Int(i),
                    Monday = "",
                    Tuesday = "",
                    Wednesday = "",
                    Thursday = "",
                    Friday = "",
                    Saturday = "",
                    Sunday = ""
                });
            }
            return Days;
        }


        private void bookButton_Click(object sender, RoutedEventArgs e)
        {
            BookWindow book = new BookWindow(this);
            book.ShowDialog();
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (!worker.IsBusy)
                worker.RunWorkerAsync();
        }

        private void adminLessonButton_Click(object sender, RoutedEventArgs e)
        {
            AdminTimetable admin = new AdminTimetable(this);
            admin.ShowDialog();
        }
        #endregion
        #region Login
        /// <summary>
        /// Does the database query to log a user in and alters the global variables relating
        /// to user information
        /// </summary>
        private void Login()
        {
            List<string[]> result = db.ReadLoginCheckValid(usernameBox.Text, passwordBox.Password);
            if (bool.Parse(result[0][0]))
            {
                isAdmin = bool.Parse(result[0][1]);
                isTeacher = bool.Parse(result[0][2]);
                studentID = int.Parse(result[0][3]);
                checkAbilities();
                //show confirmation and change user screen
                //load relavant Info

            }
            else loginError.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Checks if the login credientials are correctly entered
        /// </summary>
        /// <returns>true if error</returns>
        private bool LoginErrorCheck()
        {
            bool error = false;
            error = HelperFunctions.checkError(error, usernameError, usernameBox.Text);
            error = HelperFunctions.checkError(error, passwordError, passwordBox.Password);
            return error;
        }
        #endregion
        #region Event Handeling
        private void cmbRecipient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //int teacherID = int.Parse(teacherInfo[cmbRecipient.SelectedIndex][0]);
            //txtTeacherName.Text = "Teacher Name: " + teacherInfo[teacherID - 1][1] + " " + teacherInfo[teacherID - 1][2];
            //txtTeacherSkill.Text = "Teacher's Instruments: " + "Example";
        }

        private void cmbInstrument_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int instrumentID = int.Parse(instrumentInfo[cmbInstrument.SelectedIndex][0]);
            txtInstrumentInfo.Text = "Instrument Info: " + instrumentInfo[instrumentID][3] + ", " + instrumentInfo[instrumentID][1];
        }

        private void signupButton_Click(object sender, RoutedEventArgs e)
        {
            SignupWindow signup = new SignupWindow(this);
            signup.ShowDialog();
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            bool error = LoginErrorCheck();
            if (!error) Login();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void open_skills_window(object sender, RoutedEventArgs e)
        {
            skills skillsInst = new skills();
            skillsInst.ShowDialog();
        }



        #endregion

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox list = (ListBox)sender;
            Messages message = (Messages)list.SelectedItem;
            richTextBox = message.formatMessage(richTextBox);
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                BackgroundWorker sendMessage = new BackgroundWorker();
                sendMessage.DoWork += replyMessage_DoWork;
                sendMessage.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Sends a reply message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void replyMessage_DoWork(object sender, DoWorkEventArgs e)
        {
            int role = 2;
            if (isAdmin) role = 0;
            if (IsTeacher) role = 1;

            Messages message = null;
            string title = "";
            string content = "";
            this.Dispatcher.Invoke((Action)(() =>
            {
                title = titleBox.Text;
                content = replyBox.Text;
                message = (Messages)listBox.SelectedItem;
            }));
            DatabaseConnector.DatabaseConnector replyConnection = new DatabaseConnector.DatabaseConnector();
            replyConnection.SendMessage(message.TeacherID, message.StudentID, DateTime.Now, title, content, role);
            LoadMessages(replyConnection);
        }

        /// <summary>
        /// Loads the messages corresponding to the current user
        /// </summary>
        private void LoadMessages(DatabaseConnector.DatabaseConnector data)
        {
            //do data processing 
            this.Dispatcher.Invoke((Action)(() =>
            {
                MessageBox.Show("success");
            }));
        }

        private void refreshbutton_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void composebutton_Click(object sender, RoutedEventArgs e)
        {
            ComposeWindow compose = new ComposeWindow(this);
            compose.ShowDialog();
        }
    }

    #region Timetable Layout
    /// <summary>
    /// An object that defines the layout of the timetables
    /// </summary>
    public class HalfHour
    {
        public string Time { get; set; }
        public string Sunday { get; set; }
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }
    }
    #endregion
}