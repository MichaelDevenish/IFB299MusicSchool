using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication1
{
    public static class HelperFunctions
    {
        /// <summary>
        /// converts a number between 0 and 16 to a time string between 9:00am 
        /// and 5:00pm in half hour intervals
        /// </summary>
        /// <param name="i"> the current time as a number between 0 and 16</param>
        /// <returns>a time string between 9:00am and 5:00pm in half hour intervals</returns>
        public static string Get9to5TimeFrom16Int(int i)
        {
            string time = null;
            if (i >= 0 && i <= 16)
            {
                int hour = (((i + 18) % 24) / 2);
                if (hour == 0) hour = 12;

                time = hour + ":";

                if (i % 2 == 1) time += "30";
                else time += "00";

                if (i >= 6) time += "pm";
                else time += "am";
            }

            return time;
        }
        /// <summary>
        /// Compares two byte arrays together and returns true if they match and false if they dont
        /// </summary>
        /// <param name="byte1"></param>
        /// <param name="byte2"></param>
        /// <returns></returns>
        public static bool compareByteArrays(byte[] byte1, byte[] byte2)
        {
            if (byte1 == null && byte2 == null)
                return true;
            else if (byte1 == null || byte2 == null)
                return false;

            bool hashComparison = true;
            if (byte1.Length != byte2.Length)
                hashComparison = false;

            else for (int i = 0; i < byte1.Length; i++)
                    if (byte1[i] != byte2[i])
                        hashComparison = false;

            return hashComparison;
        }

        public static bool emailCheck(bool error, Label warning, TextBox text)
        {
            var emailregex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");

            if (text.Text != "" && !emailregex.IsMatch(text.Text))
                error = ShowError(warning);

            else warning.Visibility = Visibility.Hidden;
            return error;
        }

        public static bool checkError(bool error, Label warning, string text)
        {
            if (text == "") error = ShowError(warning);

            else warning.Visibility = Visibility.Hidden;
            return error;
        }

        public static bool ShowError(Label warning)
        {
            bool error = true;
            warning.Visibility = Visibility.Visible;
            return error;
        }
    }
}
