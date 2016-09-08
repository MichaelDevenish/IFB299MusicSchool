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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
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
            allTimetables = GenerateTimetableDataBindings();
            myTimetables = GenerateTimetableDataBindings();
            allClassesTable.ItemsSource = allTimetables;
            myClassesTable.ItemsSource = myTimetables;

            myTimetables[1].Monday = "hello";

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
                if(timetableTab.IsSelected)
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

        private List<HalfHour> GenerateTimetableDataBindings()
        {
            //do database code to get the all timetable slots

            List<HalfHour> Days = new List<HalfHour>();
            for (int i = 0; i <= 16; i++)
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

                Days.Add(new HalfHour()
                {
                    Time = time,
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
