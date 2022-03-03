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
    /// Логика взаимодействия для ChangeCostWindow.xaml
    /// </summary>
    public partial class ChangeCostWindow : Window
    {
        public ChangeCostWindow()
        {
            InitializeComponent();
            this.DataContext = MainWindow.model;
            MainWindow.model.ProductListModule.ProductChangeCostCommandBindingCollection = CommandBindings;
            MainWindow.model.ProductListModule.LoadCommands();
        }
    }
}
