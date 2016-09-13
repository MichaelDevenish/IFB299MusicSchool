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
        public AdminTimetable(MainWindow parentWindow)
        {
            InitializeComponent();
            this.parentWindow = parentWindow;
            SetupSelectors();
        }

        private void SetupSelectors()
        {
            for (int i = 0; i < 20; i++)
                repeatBox.Items.Add(i + 1);

            for (int i = 0; i < parentWindow.TeacherInfo.Count; i++)
                teacherBox.Items.Add(parentWindow.TeacherInfo[i][1] + " " + parentWindow.TeacherInfo[i][2]);

            lengthBox.Items.Add("30 minutes");
            lengthBox.Items.Add("1 hour");

            for (int i = 0; i <= 16; i++)
                hourOfBox.Items.Add(Get9to5TimeFrom16Int(i));

            lengthBox.SelectedIndex = 0;
            hourOfBox.SelectedIndex = 0;
            repeatBox.SelectedIndex = 0;
            teacherBox.SelectedIndex = 0;
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

            string time = "";
            if (hour > 9) time += hour + ":";
            else time += "0" + hour + ":";

            if (i % 2 == 1) time += "30:00";
            else time += "00:00";

            if (i >= 6) time += " PM";
            else time += " AM";

            return time;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            int userID = int.Parse(parentWindow.TeacherInfo[teacherBox.SelectedIndex][0]);
            bool length = lengthBox.SelectedIndex != 0;

            DateTime day;
            if (DateTime.TryParse(firstDatePicker.SelectedDate.ToString(), out day))
            {
                string dateString = day.ToString("dd/MM/yyyy") + " " + hourOfBox.SelectedItem.ToString();
                DateTime dateof = DateTime.ParseExact(dateString
                   , "dd/MM/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);

                for (int i = 0; i < repeatBox.SelectedIndex + 1; i++)
                {
                    if (i != 0) dateof = dateof.AddDays(7);

                    parentWindow.DB.InsertLesson(userID, dateof, length);
                }
                MessageBox.Show("Insertion Complete");
            }
            else MessageBox.Show("The date entered was invalid");
        }
    }
}
