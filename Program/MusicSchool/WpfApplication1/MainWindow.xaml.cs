using System;
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
using WpfApplication1.Secondary_Windows;

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
        private BackgroundWorker refreshMessage;
        private BackgroundWorker update_loginscreen;
        private List<Messages> messages;

        //Variables for lesson list on main account tab
        private List<string> lesson_list;
        private List<string> comments;
        private List<string> lesson_ids;

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
            this.Icon = BitmapFrame.Create(new Uri(Environment.CurrentDirectory + "/images/logo.ico"));

            BackgroundWorker firstLoad = new BackgroundWorker();
            firstLoad.DoWork += info_DoWork;
            firstLoad.RunWorkerAsync();

            worker = new BackgroundWorker();
            worker.DoWork += timetable_DoWork;
            worker.RunWorkerAsync();

            System.Windows.Threading.DispatcherTimer wallpaperIncrement = new System.Windows.Threading.DispatcherTimer();
            wallpaperIncrement.Tick += new EventHandler(wallpaperIncrement_Tick);
            wallpaperIncrement.Interval = new TimeSpan(0, 1, 0);
            wallpaperIncrement.Start();

            System.Windows.Threading.DispatcherTimer sessionClock = new System.Windows.Threading.DispatcherTimer();
            sessionClock.Tick += new EventHandler(sessionClock_Tick);
            sessionClock.Interval = new TimeSpan(0, 0, 1);
            sessionClock.Start();

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

                if (teacherThread != null) teacherInfo = teacherThread;
                else teacherInfo = new List<string[]>();
                if (instrumentThread != null) instrumentInfo = instrumentThread;
                else instrumentInfo = new List<string[]>();
                for (int i = 0; i < teacherInfo.Count; i++)
                    cmbRecipient.Items.Add(teacherInfo[i][1] + " " + teacherInfo[i][2]);
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
            manageSkills.Visibility = Visibility.Hidden;
            bookButton.IsEnabled = false;
            if (logged_in)
            {
                if (!isAdmin && !IsTeacher && studentID != -1)
                {
                    bookButton.IsEnabled = true;
                }
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
                    manageSkills.Visibility = Visibility.Visible;
                }

                errorMessage.Visibility = System.Windows.Visibility.Hidden;
            }
            //timetable_ret.Visibility = System.Windows.Visibility.Visible;

            else
            {
                errorMessage.Visibility = System.Windows.Visibility.Visible;
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
            bool executedFirst = false;
            bool executedSecond = false;

            allTimetables = GenerateTimetableDataBindings();
            myTimetables = GenerateTimetableDataBindings();

            while (!executedFirst) executedFirst = LoadTimetableData(new DatabaseConnector.DatabaseConnector().ReadEmptyLessons(), allTimetables);
            while (!executedSecond) executedSecond = LoadTimetableData(new DatabaseConnector.DatabaseConnector().ReadUserLessons(studentID), myTimetables);

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
        private bool LoadTimetableData(List<string[]> results, ObservableCollection<HalfHour> timetable)
        {
            bool executed = true;
            if (results != null)
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
            else this.Dispatcher.Invoke((() =>
            {
                if (MessageBox.Show("There was an error when loading the timetable, Do you wish to retry?",
                "Error", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    executed = false;
            }));

            return executed;
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

        #endregion
        #region Login
        /// <summary>
        /// Does the database query to log a user in and alters the global variables relating
        /// to user information
        /// </summary>
        private void Login()
        {
            List<string[]> result = db.ReadLoginCheckValid(usernameBox.Text, passwordBox.Password);
            if (result != null)
            {
                if (bool.Parse(result[0][0]))
                {
                    isAdmin = bool.Parse(result[0][1]);
                    isTeacher = bool.Parse(result[0][2]);
                    studentID = int.Parse(result[0][3]);
                    checkAbilities();

                    refreshMessage = new BackgroundWorker();
                    refreshMessage.DoWork += refreshMessage_DoWork;
                    refreshMessage.RunWorkerAsync();
                    MessageBox.Show("You have logged in successfuly");

                    //show confirmation and change user screen
                    //load relavant Info
                    loginPrompt.Visibility = Visibility.Hidden;
                    accountInfo.Visibility = Visibility.Visible;

                    update_loginscreen = new BackgroundWorker();
                    update_loginscreen.DoWork += update_loginscreen_DoWork;
                    update_loginscreen.RunWorkerAsync();
                }
                else loginError.Visibility = Visibility.Visible;
            }
            else MessageBox.Show("Could not contact database");
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


        /// <summary>
        /// Series of actions that update information in the login screen
        /// </summary>
        private void update_loginscreen_DoWork(object sender, DoWorkEventArgs e)
        {
            //Get basic name info
            string query = "SELECT users.username, users.first_name, users.last_name FROM users WHERE user_id = @student_id";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("@student_id", studentID + "");
            List<string[]> result = db.simpleConnection(false, query, param);

            this.Dispatcher.Invoke(() => username_label.Content = result[0][0]);
            this.Dispatcher.Invoke(() => name_label.Content = result[0][1] + " " + result[0][2]);

            if (IsTeacher)
            {
                query = "SELECT lessons.lesson_id, lessons.lesson_date, lessons.comments, lessons.attended, " +
                        "users.first_name , users.last_name FROM musicschool.lessons " +
                        "INNER JOIN users ON users.user_id = lessons.student_id WHERE lessons.teacher_id = @user_id";
                param = new Dictionary<string, object>();
                param.Add("@user_id", studentID + "");
                result = db.simpleConnection(false, query, param);

                lesson_list = new List<string>();
                comments = new List<string>();
                lesson_ids = new List<string>();
                foreach (string[] les in result)
                {
                    lesson_list.Add(les[1] + " - " + les[4] + " " + les[5]);
                    string com_temp = "";
                    if (bool.Parse(les[3])) com_temp = "Attended - ";
                    com_temp = com_temp + les[2];
                    comments.Add(com_temp);
                    lesson_ids.Add(les[0]);
                }
                this.Dispatcher.Invoke(() => lesson_box.ItemsSource = lesson_list);
            }
            else
            {
                //get lesson comments
                query = "SELECT lessons.lesson_date, lessons.comments, lessons.attended, " +
                        "users.first_name , users.last_name FROM musicschool.lessons " +
                        "INNER JOIN users ON users.user_id = lessons.teacher_id WHERE student_id = @student_id";
                param = new Dictionary<string, object>();
                param.Add("@student_id", studentID + "");
                result = db.simpleConnection(false, query, param);

                int lessons_attended = 0;
                int lessons_upcoming = 0;
                int lessons_prior = 0;

                foreach (string[] les in result)
                {

                    //update the comments box
                    lesson_list.Add(les[0] + " - " + les[3] + " " + les[4]);
                    string com_temp = "";
                    if (bool.Parse(les[2])) com_temp = "Attended - ";
                    com_temp = com_temp + les[1];
                    comments.Add(com_temp);

                    //get lessions attended vs total lessons
                    DateTime currentTime = DateTime.Now;
                    DateTime lessonTime = Convert.ToDateTime(les[0]);
                    if (DateTime.Compare(lessonTime, currentTime) < 0)
                    {
                        lessons_prior++;
                    }
                    else
                    {
                        lessons_upcoming++;
                    }
                    if (bool.Parse(les[2])) lessons_attended++;
                }
                this.Dispatcher.Invoke(() => lesson_attend_number.Content = lessons_attended + "/" + lessons_prior);
                this.Dispatcher.Invoke(() => upcoming_lesson_number.Content = lessons_upcoming);
            }

        }

        /// <summary>
        /// When the uses selects a new lesson from the listbox, show the comments for that lesson in the comments box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lesson_box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lesson_comments.Text = comments[lesson_box.SelectedIndex];
        }

        /// <summary>
        /// What to do when the user clicks the logout button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logOut_btn_Click(object sender, RoutedEventArgs e)
        {
            checkAbilities();
            isAdmin = false;
            isTeacher = false;
            studentID = -1;
            clear_account_screen();
            loginPrompt.Visibility = Visibility.Visible;
            accountInfo.Visibility = Visibility.Hidden;
        }

        private void clear_account_screen()
        {
            lesson_comments.Text = "";
            username_label.Content = "...";
            name_label.Content = "...";
            lesson_attend_number.Content = "...";
            upcoming_lesson_number.Content = "...";
        }


        #endregion
        #region Event Handeling
        private void cmbRecipient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtTeacherInfo.Text = String.Empty;
            int teacherID = int.Parse(teacherInfo[cmbRecipient.SelectedIndex][0]);
            txtTeacherInfo.Text = "Teacher Name: " + teacherInfo[teacherID][1] + " " + teacherInfo[teacherID][2];
            //txtTaecherSkill.Text = "Teacher's Instruments: " + "Example";
        }

        private void cmbInstrument_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtInstrumentInfo.Text = String.Empty;
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

        private void refreshbutton_Click_1(object sender, RoutedEventArgs e)
        {
            if (studentID != -1)
                if (!refreshMessage.IsBusy)
                    refreshMessage.RunWorkerAsync();
                else MessageBox.Show("You must be logged in to use this feature");

        }

        private void composebutton_Click(object sender, RoutedEventArgs e)
        {
            if (studentID != -1)
            {
                new ComposeWindow(this).ShowDialog();
                if (!refreshMessage.IsBusy)
                    refreshMessage.RunWorkerAsync();
            }
            else MessageBox.Show("You must be logged in to use this feature");
        }
        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.SelectedIndex <= listBox.Items.Count && listBox.SelectedItem != null)
            {
                ListBox list = (ListBox)sender;
                Messages message = (Messages)list.SelectedItem;
                richTextBox = message.formatMessage(richTextBox);
            }
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            if (studentID != -1)
            {
                if (listBox.SelectedItem != null)
                {
                    BackgroundWorker sendMessage = new BackgroundWorker();
                    sendMessage.DoWork += replyMessage_DoWork;
                    sendMessage.RunWorkerAsync();
                }
            }
            else MessageBox.Show("You must be logged in to use this feature");
        }

        private void modify_lesson_Click(object sender, RoutedEventArgs e)
        {
            lesson_comments comments_window = new lesson_comments(lesson_ids[lesson_box.SelectedIndex]);
            comments_window.ShowDialog();
        }

        #endregion
        #region messaging
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
            DateTime time = DateTime.Now;
            this.Dispatcher.Invoke((Action)(() =>
            {
                title = titleBox.Text;
                content = replyBox.Text;
                message = (Messages)listBox.SelectedItem;
                message.AddMessage(title, time, content, studentID);
                richTextBox = message.formatMessage(richTextBox);
                replyBox.Text = "";
                titleBox.Text = "";

            }));
            DatabaseConnector.DatabaseConnector replyConnection = new DatabaseConnector.DatabaseConnector();
            replyConnection.SendMessage(message.TeacherID, message.StudentID, time, title, content, role);

            LoadMessages(replyConnection);

        }
        /// <summary>
        /// Sends a reply message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshMessage_DoWork(object sender, DoWorkEventArgs e)
        {
            LoadMessages(new DatabaseConnector.DatabaseConnector());
        }

        /// <summary>
        /// Loads the messages corresponding to the current user
        /// </summary>
        private void LoadMessages(DatabaseConnector.DatabaseConnector data)
        {
            //do data processing 
            int remember = -1;
            bool selected = false;
            this.Dispatcher.Invoke((Action)(() =>
            {
                selected = listBox.SelectedItem != null;
                if (selected)
                {
                    if (((Messages)listBox.SelectedItem).UserType == 2)
                    {
                        remember = ((Messages)listBox.SelectedItem).TeacherID;
                    }
                    else
                    {
                        remember = ((Messages)listBox.SelectedItem).StudentID;
                    }
                }
            }));
            messages = new DatabaseConnector.DatabaseConnector().ReceiveMessages((IsTeacher || isAdmin), studentID);
            int selectIndex = -1;
            if (selected)
            {
                foreach (Messages message in messages)
                {
                    if (message.UserType == 2 && message.TeacherID == remember)
                        selectIndex = messages.IndexOf(message);
                    else if (message.UserType != 2 && message.StudentID == remember)
                        selectIndex = messages.IndexOf(message);
                }
            }
            this.Dispatcher.Invoke((Action)(() =>
            {
                listBox.ItemsSource = messages;
                listBox.SelectedIndex = selectIndex;
            }));
        }
        #endregion




        private void wallpaperIncrement_Tick(object sender, EventArgs e)
        {
            // wallpaper array index increment
        }

        private void sessionClock_Tick(object sender, EventArgs e)
        {
            this.clockTimer.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtInstrumentInfo_Copy_TextChanged(object sender, TextChangedEventArgs e)
        {

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