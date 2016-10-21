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

namespace WpfApplication1.Secondary_Windows
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class lesson_comments : Window
    {
        DatabaseConnector.DatabaseConnector db = new DatabaseConnector.DatabaseConnector();
        string lesson_id;
        public lesson_comments(string l_id)
        {
            InitializeComponent();
            lesson_id = l_id;
        }

        private void submit_btn_Click(object sender, RoutedEventArgs e)
        {
            string query = "UPDATE musicschool.lessons SET comments= @comment, attended=@attended WHERE lesson_id= @lesson_id";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("@comment", comment_box.Text);
            param.Add("@attended", attended_chkbx.IsChecked);
            param.Add("@lesson_id", lesson_id);
            db.simpleConnection(true, query, param);
        }
    }
}
