using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for signupWindow.xaml
    /// </summary>
    public partial class SignupWindow : Window
    {
        private MainWindow parentWindow;
        public SignupWindow(MainWindow parentWindow)
        {
            InitializeComponent();
            this.parentWindow = parentWindow;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            bool error = false;
            error = emailCheck(error, emailError, emailBox);
            error = checkError(error, firstError, firstBox);
            error = checkError(error, lastError, lastBox);
            error = checkError(error, nameError, userBox);

            ////Check dob and passwords

            if (parentWindow.DB.CheckUsername(userBox.Text) && userBox.Text != "")
            {
                error = false;
                nameErrorExist.Visibility = Visibility.Visible;
            }

        }

        private bool emailCheck(bool error, Label warning, TextBox text)
        {
            var emailregex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");

            if (text.Text != "" && !emailregex.IsMatch(emailBox.Text))
                error = ShowError(warning);

            else warning.Visibility = Visibility.Hidden;
            return error;
        }

        private bool checkError(bool error, Label warning, TextBox text)
        {
            if (text.Text == "") error = ShowError(warning);

            else warning.Visibility = Visibility.Hidden;
            return error;
        }

        private static bool ShowError(Label warning)
        {
            bool error = true;
            warning.Visibility = Visibility.Visible;
            return error;
        }
    }
}
