﻿using System;
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

            LoadTimetableData(db.ReadEmptyLessons(), allTimetables);
            //all we have to do to setup the personal timetable is call
            //the above command but with a different query and refrence myTimetables
        }

        /// <summary>
        /// loads all of the rows from the supplied query into the 
        /// supplied data binding
        /// </summary>
        /// <param name="results">the results that are to be added</param>
        /// <param name="timetable">the table databinding that is to be added to</param>
        private void LoadTimetableData(List<string[]> results, List<HalfHour> timetable)
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
            int hour = (((i + 18) % 24) / 2);
            if (hour == 0) hour = 12;

            string time = hour + ":";

            if (i % 2 == 1) time += "30";
            else time += "00";

            if (i >= 6) time += "pm";
            else time += "am";

            return time;
        }
    }

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
}