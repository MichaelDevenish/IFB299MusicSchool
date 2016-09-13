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
    /// A window that the admin uses to add lessons to the timetable
    /// </summary>
    ///
    public partial class AdminTimetable : Window
    {
        private MainWindow parentWindow;
        private List<string[]> teacherInfo;
        public AdminTimetable(MainWindow parentWindow, List<string[]> teacherInfo)
        {
            InitializeComponent();
            this.teacherInfo = teacherInfo;
            this.parentWindow = parentWindow;
            SetupSelectors();
        }

        private void SetupSelectors()
        {
            for (int i = 0; i < 20; i++)
                repeatBox.Items.Add(i + 1);

            lengthBox.Items.Add("30 minutes");
            lengthBox.Items.Add("1 hour");

            for (int i = 0; i <= 16; i++)
                hourOfBox.Items.Add(Get9to5TimeFrom16Int(i));

            lengthBox.SelectedIndex = 0;
            hourOfBox.SelectedIndex = 0;
            repeatBox.SelectedIndex = 0;
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

        private void button_Click(object sender, RoutedEventArgs e)
        {
            int userID = int.Parse(teacherInfo[teacherBox.SelectedIndex][0]);
            int repeats = repeatBox.SelectedIndex + 1;
            bool length = lengthBox.SelectedIndex != 0;


            string hours = hourOfBox.SelectedItem.ToString();
            MessageBox.Show(hours);

            string date = firstDatePicker.SelectedDate.ToString();

            //check the formating and convert to datetime


            // DateTime date = firstDatePicker.SelectedDate;
            //date = date.addhours
        }
    }
}
