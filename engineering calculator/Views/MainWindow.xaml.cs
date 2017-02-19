using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using engineering_calculator.Models;

namespace engineering_calculator.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private engineering_calculatorViewModels _viewModel;
        //internal engineering_calculatorViewModels ViewModel
        //{
        //    get { return _viewModel; }
        //    set { _viewModel=value; }
        //}
        public MainWindow()
        {
          //  _viewModel=new engineering_calculatorViewModels();
           InitializeComponent();
        }
    }
}
