using PracticDemoexam.Models;
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
using PracticDemoexam.Pages;

namespace PracticDemoexam
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ViewModel model { get; set; } = new ViewModel();
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Content = new ProductList();
            model.ProductListModule.Window = this;
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            foreach(Window w in Application.Current.Windows)
            {
                w.Close();
            }
        }
    }
}
