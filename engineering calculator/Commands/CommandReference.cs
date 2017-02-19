// <copyright file="CommandReference.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Calculator.Commands
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    public class CommandReference : Freezable, ICommand
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(CommandReference), new PropertyMetadata(new PropertyChangedCallback(OnCommandChanged)));

        public event EventHandler CanExecuteChanged;

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public bool CanExecute(object parameter)
        {
            if (Command != null)
            {
                return Command.CanExecute(parameter);
            }

            return false;
        }

        public void Execute(object parameter)
        {
            Command.Execute(parameter);
        }

        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CommandReference commandReference = d as CommandReference;
            ICommand oldCommand = e.OldValue as ICommand;
            ICommand newCommand = e.NewValue as ICommand;

            if (oldCommand != null)
            {
                oldCommand.CanExecuteChanged -= commandReference.CanExecuteChanged;
            }

            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += commandReference.CanExecuteChanged;
            }
        }
    }
}
