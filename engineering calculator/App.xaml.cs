// <copyright file="App.xaml.cs" company="Okolov Company">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Calculator
{
    using System.Windows;
    using Calculator.ViewModels;
    using Calculator.Views;

    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            // Create the ViewModel and expose it using the View's DataContext
            MainWindow view = new MainWindow();
            view.DataContext = new MainWindowViewModel();
            view.Show();
        }
    }
}
