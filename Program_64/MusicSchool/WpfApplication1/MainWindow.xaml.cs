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

    //TODO find out what row and column the result goes in (LINE 85)

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

            foreach (string[] result in db.ReadEmptyLessons())
            {
                MessageBox.Show(result[1]);
                //first get the timeslot by first getting the minutes and if equal to 30 add one
                //then get the hours multiply by 2 and minus the offset.
                //Then put it in the corresponding result for that day by getting the current day of
                //the week and get finding the lessons offset using  TimeSpan ts = newDate - oldDate; 
                //if it is within an acceptable timespan depending of th day of the week add it to the
                //required position 

                //possible ways to reduce data use, get only the lessons within a timespan
            }

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

