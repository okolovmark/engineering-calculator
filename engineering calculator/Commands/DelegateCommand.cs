// <copyright file="DelegateCommand.cs" company="Okolov Company">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Calculator.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;

    public class DelegateCommand : ICommand
    {
        private readonly Action _executeMethod;
        private readonly Func<bool> _canExecuteMethod;
        private bool _isAutomaticRequeryDisabled;
        private List<WeakReference> _canExecuteChangedHandlers;

        public DelegateCommand(Action executeMethod)
            : this(executeMethod, null, false)
        {
        }

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : this(executeMethod, canExecuteMethod, false)
        {
        }

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod, bool isAutomaticRequeryDisabled)
        {
            if (executeMethod == null)
            {
                throw new ArgumentNullException("executeMethod");
            }

            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
            _isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (!_isAutomaticRequeryDisabled)
                {
                    CommandManager.RequerySuggested += value;
                }

                CommandManagerHelper.AddWeakReferenceHandler(ref _canExecuteChangedHandlers, value, 2);
            }

            remove
            {
                if (!_isAutomaticRequeryDisabled)
                {
                    CommandManager.RequerySuggested -= value;
                }

                CommandManagerHelper.RemoveWeakReferenceHandler(_canExecuteChangedHandlers, value);
            }
        }

        public bool IsAutomaticRequeryDisabled
        {
            get
            {
                return _isAutomaticRequeryDisabled;
            }

            set
            {
                if (_isAutomaticRequeryDisabled != value)
                {
                    if (value)
                    {
                        CommandManagerHelper.RemoveHandlersFromRequerySuggested(_canExecuteChangedHandlers);
                    }
                    else
                    {
                        CommandManagerHelper.AddHandlersToRequerySuggested(_canExecuteChangedHandlers);
                    }

                    _isAutomaticRequeryDisabled = value;
                }
            }
        }

        // Method to determine if the command can be executed
        public bool CanExecute()
        {
            if (_canExecuteMethod != null)
            {
                return _canExecuteMethod();
            }

            return true;
        }

        // Execution of the command
        public void Execute()
        {
            if (_executeMethod != null)
            {
                _executeMethod();
            }
        }

        // Property to enable or disable CommandManager's automatic requery on this command

        // Raises the CanExecuteChaged event
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        // ICommand.CanExecuteChanged implementation
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            Execute();
        }

        // Protected virtual method to raise CanExecuteChanged event
        protected virtual void OnCanExecuteChanged()
        {
            CommandManagerHelper.CallWeakReferenceHandlers(_canExecuteChangedHandlers);
        }
    }
}
