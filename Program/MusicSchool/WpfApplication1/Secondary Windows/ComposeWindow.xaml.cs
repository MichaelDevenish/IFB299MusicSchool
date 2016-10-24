using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ComposeWindow.xaml
    /// </summary>
    public partial class ComposeWindow : Window
    {
        private MainWindow parentWindow;

        private BackgroundWorker loadUsers;
        public List<User> userList;
        private List<string[]> users;

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="parentWindow">the window that this window came from</param>
        public ComposeWindow(MainWindow parentWindow)
        {
            InitializeComponent();
            this.Icon = BitmapFrame.Create(new Uri(Environment.CurrentDirectory + "/images/logo.ico"));
            this.parentWindow = parentWindow;
            InitializeComponent();

            users = new List<string[]>();

            loadUsers = new BackgroundWorker();
            loadUsers.DoWork += loadUsers_DoWork;
            loadUsers.RunWorkerAsync();
        }

        private void SearchUsers()
        {
            userList = new List<User>();

            foreach (string[] user in users)
            {
                string name = user[1] + " " + user[2];
                if (name.ToLower().Contains(searchBox.Text.ToLower()))
                    userList.Add(new User() { Name = name, ID = int.Parse(user[0]) });
            }
            listBox.ItemsSource = userList;
        }

        #region Event Handelers
        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem == null)
                MessageBox.Show("you must select a recipient");
            else {
                BackgroundWorker sendMessageWorker = new BackgroundWorker();
                sendMessageWorker.DoWork += sendMessage_DoWork;
                sendMessageWorker.RunWorkerAsync();
            }
        }

        private void textBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchUsers();
        }
        #endregion
        #region Background Workers
        private void loadUsers_DoWork(object sender, DoWorkEventArgs e)
        {
            DatabaseConnector.DatabaseConnector backgroundload = new DatabaseConnector.DatabaseConnector();

            if (parentWindow.IsAdmin || parentWindow.IsTeacher) users = new DatabaseConnector.DatabaseConnector().GetUsers();
            else users = new DatabaseConnector.DatabaseConnector().ReadTeacherInfo();


            this.Dispatcher.Invoke((Action)(() =>
            {
                SearchUsers();
            }));
        }

        private void sendMessage_DoWork(object sender, DoWorkEventArgs e)
        {
            string title = "";
            int userID = -1;
            string content = "";
            this.Dispatcher.Invoke((Action)(() =>
            {
                title = titleBox.Text;
                content = textBox.Text;
                userID = ((User)listBox.SelectedItem).ID;

            }));

            if (parentWindow.IsAdmin || parentWindow.IsTeacher)
                new DatabaseConnector.DatabaseConnector().SendMessage(parentWindow.StudentID, userID, DateTime.Now, title, content, 1);
            else new DatabaseConnector.DatabaseConnector().SendMessage(userID, parentWindow.StudentID, DateTime.Now, title, content, 2);

            this.Dispatcher.Invoke((Action)(() =>
            {
                this.Close();
            }));
        }
        #endregion
    }
    public class User
    {
        public string Name { get; set; }

        public int ID { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
