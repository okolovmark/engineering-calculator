// <copyright file="App.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Calculator
{
    using System.Windows;

    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            // Create the ViewModel and expose it using the View's DataContext
            Views.MainWindow view = new Views.MainWindow();
            view.DataContext = new ViewModels.MainWindowViewModel();
            view.Show();
        }
    }
}
