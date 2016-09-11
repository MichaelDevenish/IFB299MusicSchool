﻿using System;
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
    /// A window that the user uses to book lessons
    /// </summary>
    public partial class BookWindow : Window
    {
        private MainWindow parentWindow;
        private int studentID;
        public BookWindow(MainWindow parentWindow, int studentID)
        {
            InitializeComponent();
            this.parentWindow = parentWindow;
            this.studentID = studentID;
        }
    }
}
