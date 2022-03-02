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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PracticDemoexam.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProductList.xaml
    /// </summary>
    public partial class ProductList : Page
    {
        public ProductList()
        {
            InitializeComponent();
            this.DataContext = MainWindow.model;
            MainWindow.model.ProductListModule.CommandBindingCollection = CommandBindings;
            MainWindow.model.ProductListModule.LoadCommands();
            MainWindow.model.ProductListModule.SelectedItems = lbProducts.SelectedItems;
        }
    }
}
