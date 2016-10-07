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


        public ComposeWindow(MainWindow parentWindow)
        {
            InitializeComponent();
            this.parentWindow = parentWindow;
            InitializeComponent();

            users = new List<string[]>();

            loadUsers = new BackgroundWorker();
            loadUsers.DoWork += loadUsers_DoWork;
            loadUsers.RunWorkerAsync();


        }

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

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //do database query
            this.Close();
        }

        private void textBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchUsers();
        }
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
