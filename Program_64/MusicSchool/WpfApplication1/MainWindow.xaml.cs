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

        public MainWindow()
        {
            
            InitializeComponent();
        }

        private void tab_changed(object sender, SelectionChangedEventArgs e)
        {
            if (logged_in)
            {
                errorMessage.Visibility = System.Windows.Visibility.Hidden;
                timetable_ret.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                errorMessage.Visibility = System.Windows.Visibility.Visible;
                timetable_ret.Visibility = System.Windows.Visibility.Hidden;

            }
        }
    }
}
