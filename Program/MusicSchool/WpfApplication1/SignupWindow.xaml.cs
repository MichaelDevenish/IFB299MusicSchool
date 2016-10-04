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
            bool error = errorChecking();
            if (!error) RegisterUser();
        }

        /// <summary>
        /// Registers a user and signs them in
        /// </summary>
        private void RegisterUser()
        {
            DateTime date;
            PasswordManagment passwordGen = new PasswordManagment();
            string salt = passwordGen.GenerateSalt();
            byte[] hash = passwordGen.GenerateHash(passwordBox.Password, salt);

            if (DateTime.TryParse(birthPicker.Text, out date))
            {
                parentWindow.DB.InsertUser(firstBox.Text, lastBox.Text, date, 2, hash, salt, userBox.Text);
                MessageBox.Show("User Created");
                parentWindow.IsTeacher = false;
                parentWindow.IsAdmin = false;
                parentWindow.StudentID = parentWindow.DB.GetUserID(userBox.Text);
            }
        }

        /// <summary>
        /// Checks if the supplied variables are correct
        /// </summary>
        /// <returns>true if error</returns>
        private bool errorChecking()
        {
            bool error = false;
            error = HelperFunctions.emailCheck(error, emailError, emailBox);
            error = HelperFunctions.checkError(error, firstError, firstBox.Text);
            error = HelperFunctions.checkError(error, lastError, lastBox.Text);
            error = HelperFunctions.checkError(error, nameError, userBox.Text);

            if (birthPicker.SelectedDate == null)
                HelperFunctions.ShowError(birthError);
            else birthError.Visibility = Visibility.Hidden;

            if (parentWindow.DB.CheckUsername(userBox.Text) && userBox.Text != "")
                HelperFunctions.ShowError(nameErrorExist);
            else nameErrorExist.Visibility = Visibility.Hidden;

            if (passwordBox.Password == "")
                HelperFunctions.ShowError(passwordError);
            else passwordError.Visibility = Visibility.Hidden;

            if (passwordBox.Password != confirmBox.Password && passwordBox.Password != "")
                HelperFunctions.ShowError(confirmError);
            else confirmError.Visibility = Visibility.Hidden;
            return error;
        }
    }
}
