using System;
using System.Collections.Generic;
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

    //TODO find out what row and column the result goes in (LINE 165)

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public bool logged_in = false;
        public List<HalfHour> allTimetables;
        public List<HalfHour> myTimetables;
        public DatabaseConnector.DatabaseConnector db;

        public MainWindow()
        {
            db = new DatabaseConnector.DatabaseConnector();

            InitializeComponent();
            SetupTimetable();

            System.Diagnostics.Debug.WriteLine("aaa");
            //test
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("@userID", "1");
            List<string>[] list = db.simpleConnection(false, "SELECT first_name,last_name,dob FROM users WHERE user_id = @userID", param);
        }

        //Each time the user changes the tab, check if the login status has changes and make the appropriate
        //adjustments
        private void tab_changed(object sender, SelectionChangedEventArgs e)
        {
            if (logged_in)
            {
                //get data for timetable info
                if (timetableTab.IsSelected)
                {
                    //List<string>[] info = db.getTimetableInfo();
                }
            }
            //errorMessage.Visibility = System.Windows.Visibility.Hidden;
            //timetable_ret.Visibility = System.Windows.Visibility.Visible;

            else
            {
                //errorMessage.Visibility = System.Windows.Visibility.Visible;
                //timetable_ret.Visibility = System.Windows.Visibility.Hidden;

            }
        }

        /// <summary>
        /// Sets up everything related to the Timetable tab
        /// Does this by creating blank data bindings for the tables
        /// and calling a function to populate the Date Selectors
        /// </summary>
        private void SetupTimetable()
        {
            allTimetables = GenerateTimetableDataBindings();
            myTimetables = GenerateTimetableDataBindings();
            allClassesTable.ItemsSource = allTimetables;
            myClassesTable.ItemsSource = myTimetables;

            SetupDateSelector();
        }

        /// <summary>
        /// Initialises the date selectors by setting them up with the 
        /// required values and setting their default to the current date
        /// </summary>
        private void SetupDateSelector()
        {
            for (int i = 0; i < 31; i++)
                dayBox.Items.Add(i + 1);
            for (int i = 0; i < 12; i++)
                monthBox.Items.Add(i + 1);
            for (int i = 1900; i <= DateTime.Today.Year; i++)
                yearBox.Items.Add(i);

            dayBox.SelectedIndex = DateTime.Today.Day - 1;
            monthBox.SelectedIndex = DateTime.Today.Month - 1;
            yearBox.SelectedIndex = yearBox.Items.Count - 1;
        }

        /// <summary>
        /// Generates a blank databinding for the timetables
        /// </summary>
        /// <returns>a blank array of object HalfHour that is all blank except for Time</returns>
        private List<HalfHour> GenerateTimetableDataBindings()
        {
            List<HalfHour> Days = new List<HalfHour>();
            for (int i = 0; i <= 16; i++)
            {
                Days.Add(new HalfHour()
                {
                    Time = Get9to5TimeFrom16Int(i),
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

        /// <summary>
        /// converts a number between 0 and 16 to a time string between 9:00am 
        /// and 5:00pm in half hour intervals
        /// </summary>
        /// <param name="i"> the current time as a number between 0 and 16</param>
        /// <returns>a time string between 9:00am and 5:00pm in half hour intervals</returns>
        private static string Get9to5TimeFrom16Int(int i)
        {
            string time = "";
            bool halfhour = false;
            bool am = true;

            if (i % 2 == 1) halfhour = true;
            if (i >= 6) am = false;

            int hour = (((i + 18) % 24) / 2);
            if (hour == 0) hour = 12;

            if (halfhour && am) time = hour + ":30am";
            else if (halfhour && !am) time = hour + ":30pm";
            else if (!halfhour && am) time = hour + ":00am";
            else if (!halfhour && !am) time = hour + ":00pm";

            return time;
        }

        /// <summary>
        /// Event handeler for when the user searches the timetable for a date, it searches the timetable for that
        /// date and propogates the tables with the results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Click(object sender, RoutedEventArgs e)
        {

            allTimetables = GenerateTimetableDataBindings();
            myTimetables = GenerateTimetableDataBindings();

            //now we need to check what column and row the result goes into and then place it there

            try
            {
                foreach (string[] result in db.ReadEmptyLessons(GetDate()))
                {
                    MessageBox.Show(result[1]);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("The date supplied does not exist.");
            }
        }

        /// <summary>
        /// Returns the current date selected in the timetable tab represented as a DateTime
        /// </summary>
        /// <returns>DateTime representing the current selected date in the timetable tab</returns>
        private DateTime GetDate()
        {
            string date = yearBox.SelectedItem + "-";

            if (((int)monthBox.SelectedItem) < 10) date += "0" + monthBox.SelectedItem + "-";
            else date += monthBox.SelectedItem + "-";

            if (((int)dayBox.SelectedItem) < 10) date += "0" + dayBox.SelectedItem;
            else date += dayBox.SelectedItem;

            return DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }

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
}
