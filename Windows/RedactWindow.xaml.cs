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

namespace PracticDemoexam.Windows
{
    /// <summary>
    /// Логика взаимодействия для RedactWindow.xaml
    /// </summary>
    public partial class RedactWindow : Window
    {
        public RedactWindow()
        {
            InitializeComponent();
            DataContext = MainWindow.model;
            MainWindow.model.ProductListModule.ProductRedactCommandBindingCollection = CommandBindings;
            MainWindow.model.ProductListModule.LoadCommands();
        }
    }
}
