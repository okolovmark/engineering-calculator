// <copyright file="CommandReference.cs" company="Okolov Company">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Calculator.Commands
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    public class CommandReference : Freezable, ICommand
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(CommandReference), new PropertyMetadata(OnCommandChanged));

        public event EventHandler CanExecuteChanged;

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public bool CanExecute(object parameter)
        {
            return Command != null && Command.CanExecute(parameter);
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
            var commandReference = d as CommandReference;
            var oldCommand = e.OldValue as ICommand;
            var newCommand = e.NewValue as ICommand;

            if (oldCommand != null)
            {
                oldCommand.CanExecuteChanged -= commandReference?.CanExecuteChanged;
            }

            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += commandReference?.CanExecuteChanged;
            }
        }
    }
}
