// <copyright file="MainWindowViewModel.cs" company="Okolov Company">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Calculator.ViewModels
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Input;
    using Commands;
    using Models;

    internal class MainWindowViewModel : ViewModelBase
    {
        private DelegateCommand<string> _digitButtonPressCommand;
        private DelegateCommand<string> _getDigitCommand;
        private DelegateCommand _getResultCommand;

        private string _display;
        private string _displayExp;
        private string _displayErr;
        private int _countOpenBracket;

        public string Display
        {
            get
            {
                return _display;
            }

            set
            {
                _display = value;
                OnPropertyChanged("Display");
            }
        }

        public string DisplayExp
        {
            get
            {
                return _displayExp;
            }

            set
            {
                _displayExp = value;
                OnPropertyChanged("DisplayExp");
            }
        }

        public string DisplayErr
        {
            get
            {
                return _displayErr;
            }

            set
            {
                _displayErr = value;
                OnPropertyChanged("DisplayErr");
            }
        }

        public ICommand DigitButtonPressCommand
        {
            get
            {
                if (_digitButtonPressCommand == null)
                {
                    _digitButtonPressCommand = new DelegateCommand<string>(
                        DigitButtonPress, CanDigitButtonPress);
                }

                return _digitButtonPressCommand;
            }
        }

        public ICommand GetDigitCommand
        {
            get
            {
                if (_getDigitCommand == null)
                {
                    _getDigitCommand = new DelegateCommand<string>(GetDigit, CanDigitButtonPress);
                }

                return _getDigitCommand;
            }
        }

        public ICommand GetResultCommand
        {
            get
            {
                if (_getResultCommand == null)
                {
                    _getResultCommand = new DelegateCommand(GetResult, CanResultButtonPress);
                }

                return _getResultCommand;
            }
        }

        public void GetDigit(string button) // вводим числа
        {
            Display = _display + button;
            DisplayErr = null;
        }

        public bool DisplayNull()
        {
            if (_display == string.Empty)
            {
                Display = null;
            }

            if (Display == null)
            {
                return true;
            }

            return false;
        }

        public void DigitButtonPress(string button) // вводим операторы
        {
            switch (button)
            {
                case "C": // очищаем поле
                    Display = null;
                    _countOpenBracket = 0;
                    break;
                case "Del": // backspace
                {
                    if (Display == null)
                    {
                        break;
                    }

                    if (Display.Length == 1)
                    {
                        Display = null;
                        break;
                    }

                    char[] lastSymbol = null;
                    if (!DisplayNull())
                    {
                        lastSymbol = _display.Substring(_display.Length - 1).ToCharArray();
                    }

                    if (lastSymbol == null)
                    {
                        break;
                    }

                    if (lastSymbol[0] == ')')
                    {
                        _countOpenBracket++;
                    }

                    var doif = false;
                    if (lastSymbol[0] == '(' && _display.Length > 2 && _display.Length < 4 &&
                        char.IsLetter(_display.ElementAt(_display.Length - 2)) &&
                        char.IsLetter(_display.ElementAt(_display.Length - 3)))
                    {
                        Display = _display.Substring(0, _display.Length - 3);
                        doif = true;
                        if (DisplayNull())
                        {
                            break;
                        }

                        lastSymbol = _display.Substring(_display.Length - 1).ToCharArray();
                        _countOpenBracket--;
                    }

                    if (lastSymbol[0] == '(' && _display.Length > 3 &&
                        char.IsLetter(_display.ElementAt(_display.Length - 2)) &&
                        char.IsLetter(_display.ElementAt(_display.Length - 3)) &&
                        !char.IsLetter(_display.ElementAt(_display.Length - 4)))
                    {
                        Display = _display.Substring(0, _display.Length - 3);
                        doif = true;
                        if (DisplayNull())
                        {
                            break;
                        }

                        lastSymbol = _display.Substring(_display.Length - 1).ToCharArray();
                        _countOpenBracket--;
                    }

                    if (lastSymbol[0] == '(' && _display.Length > 3 &&
                        char.IsLetter(_display.ElementAt(_display.Length - 2)) &&
                        char.IsLetter(_display.ElementAt(_display.Length - 3)) &&
                        char.IsLetter(_display.ElementAt(_display.Length - 4)))
                    {
                        Display = _display.Substring(0, _display.Length - 4);
                        doif = true;
                        if (DisplayNull())
                        {
                            break;
                        }

                        lastSymbol = _display.Substring(_display.Length - 1).ToCharArray();
                        _countOpenBracket--;
                    }

                    if (lastSymbol[0] == '(' && _display.Length > 1 &&
                        !char.IsLetter(_display.ElementAt(_display.Length - 2)))
                    {
                        Display = _display.Substring(0, _display.Length - 2);
                        doif = true;
                        if (DisplayNull())
                        {
                            break;
                        }

                        _countOpenBracket--;
                    }

                    if (!doif)
                    {
                        Display = _display.Substring(0, _display.Length - 1);
                    }

                    break;
                }

                case "(":
                {
                    if (_display == null)
                    {
                        Display = _display + "(";
                        _countOpenBracket++;
                        break;
                    }

                    var lastSymbol = _display.Substring(_display.Length - 1).ToCharArray();
                    const string SymbolsWhereNeedMult = "0123456789)";
                    foreach (var i in SymbolsWhereNeedMult)
                    {
                        if (lastSymbol[0] != i)
                        {
                            continue;
                        }

                        Display = _display + "*";
                        break;
                    }

                    if (lastSymbol[0] == '(')
                    {
                        break;
                    }

                    _countOpenBracket++;
                    Display = _display + button;
                    break;
                }

                case ")":
                {
                    if (_display == null)
                    {
                        break;
                    }

                    var lastSymbol = _display.Substring(_display.Length - 1).ToCharArray();
                    const string Symbols = "(,+-/*^";
                    var isTrue = false;
                    if (lastSymbol[0] == '(')
                    {
                        break;
                    }

                    if (_countOpenBracket > 0)
                    {
                        foreach (var i in Symbols)
                        {
                            if (lastSymbol[0] == i)
                            {
                                isTrue = true;
                                break;
                            }
                        }

                        if (!isTrue)
                        {
                            _countOpenBracket--;
                            Display = _display + button;
                        }
                    }

                    break;
                }

                case "-":
                {
                    if (_display == null)
                    {
                        Display = _display + button;
                        break;
                    }

                    char[] lastSymbol = _display.Substring(_display.Length - 1).ToCharArray();
                    if (_display == "-")
                    {
                        break;
                    }

                    if (_display.Length == 1 || lastSymbol[0] == '(')
                    {
                        Display = _display + button;
                        break;
                    }

                    if (char.IsDigit(_display.ElementAt(_display.Length - 1)) ||
                        (_display.ElementAt(_display.Length - 2) == '(' &&
                         char.IsDigit(_display.ElementAt(_display.Length - 3))))
                    {
                        Display = _display + button;
                    }

                    break;
                }

                case "+":
                case "/":
                case "*":
                case "^":
                {
                    if (_display == null)
                    {
                        break;
                    }

                    char[] lastSymbol = _display.Substring(_display.Length - 1).ToCharArray();
                    var symbols = ",+-/*^";
                    bool isequal = false;
                    if (lastSymbol[0] == '(')
                    {
                        break;
                    }

                    foreach (var i in symbols)
                    {
                        if (lastSymbol[0] == i)
                        {
                            isequal = true;
                            DisplayErr = "введите второе число или функцию";
                            break;
                        }
                    }

                    if (isequal)
                    {
                        break;
                    }

                    Display = _display + button;
                    DisplayErr = null;
                    break;
                }

                case ",":
                {
                    if (_display == null)
                    {
                        Display = _display + "0";
                    }

                    const string Symbols = "+-/*^";
                    const string digits = "0123456789";
                    if (_display != null)
                    {
                        var reverseDisplay = new string(_display.ToCharArray().Reverse().ToArray());
                        string tempDisplay = null;
                        var isHaveDigit = false;
                        var isHaveOperation = false;
                        var isHavePoint = false;

                        for (var i = 0; i < reverseDisplay.Length; i++)
                        {
                            if (reverseDisplay[i] != ',')
                            {
                                continue;
                            }

                            isHavePoint = true;
                            tempDisplay = reverseDisplay.Substring(0, i);
                            break;
                        }

                        if (isHavePoint)
                        {
                            foreach (var i in tempDisplay)
                            {
                                if (!isHaveOperation)
                                {
                                    if (digits.Any(j => i == j))
                                    {
                                        isHaveOperation = true;
                                    }

                                    if (isHaveOperation && !isHaveDigit)
                                    {
                                        continue;
                                    }
                                }

                                if (!isHaveDigit)
                                {
                                    if (Symbols.Any(j => i == j))
                                    {
                                        isHaveDigit = true;
                                    }

                                    if (isHaveDigit && !isHaveOperation)
                                    {
                                        continue;
                                    }
                                }

                                if (isHaveDigit && isHaveOperation)
                                {
                                    var lastSymbol1 = _display.Substring(_display.Length - 1).ToCharArray();
                                    foreach (var j in Symbols)
                                    {
                                        if (lastSymbol1[0] != j)
                                        {
                                            continue;
                                        }

                                        Display = _display + "0";
                                        break;
                                    }

                                    Display = _display + button;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Display = _display + button;
                        }
                    }

                    break;
                }

                case "sin":
                {
                    CorrectInputFunc(button);
                    break;
                }

                case "cos":
                {
                    CorrectInputFunc(button);
                    break;
                }

                case "tan":
                {
                    CorrectInputFunc(button);
                    break;
                }

                case "√":
                {
                    CorrectInputFunc(button);
                    break;
                }

                case "lg":
                {
                    CorrectInputFunc(button);
                    break;
                }

                case "ln":
                {
                    CorrectInputFunc(button);
                    break;
                }
            }
        }

        public void CorrectInputFunc(string button)
        {
            if (Display != null)
            {
                var lastSymbol = _display.Substring(_display.Length - 1).ToCharArray();
                var digits = "1234567890";
                foreach (var i in digits)
                {
                    if (lastSymbol[0] == i)
                    {
                        Display = _display + "*";
                    }
                }
            }

            Display = _display + button;
            Display = _display + "(";
            _countOpenBracket++;
        }

        // вычисляет выражение
        public void GetResult()
        {
            bool isTrue;
            CorrectExpression(out isTrue);
            if (!isTrue)
            {
                return;
            }

            DisplayExp = Display;
            Display = RPN.Calculate(Display).ToString(CultureInfo.CurrentCulture);
        }

        public void CorrectExpression(out bool complete) // приводит выражение в корректный вид
        {
            complete = true;
            string subString1 = "бесконечность";
            string subString2 = "NaN";
            string subString3 = "E";
            string subString4 = "e";
            char[] lastSymbol = null;

            if (_display != null)
            {
                lastSymbol = _display.Substring(_display.Length - 1).ToCharArray();
            }

            if (lastSymbol == null)
            {
                complete = false;
                return;
            }

            if (Display.IndexOf(subString1, StringComparison.Ordinal) > -1 ||
                Display.IndexOf(subString2, StringComparison.Ordinal) > -1 ||
                Display.IndexOf(subString3, StringComparison.Ordinal) > -1 ||
                Display.IndexOf(subString4, StringComparison.Ordinal) > -1)
            {
                _countOpenBracket = 0;
                Display = null;
                DisplayErr = "Неверное выражение";
                complete = false;
                return;
            }

            var symbols = "+-/*^";
            foreach (var i in symbols)
            {
                if (lastSymbol[0] == i)
                {
                    DisplayErr = "Введите второе число или функцию";
                    complete = false;
                    return;
                }
            }

            if (lastSymbol[0] == ',')
            {
                DisplayErr = "Закончите нецелое число";
                complete = false;
                return;
            }

            if (lastSymbol[0] == '(')
            {
                DisplayErr = "Завершите выражение со скобками";
                complete = false;
                return;
            }

            if (_display == string.Empty)
            {
                Display = null;
                complete = false;
                return;
            }

            while (_countOpenBracket > 0)
            {
                Display = _display + ")";
                _countOpenBracket--;
            }

            DisplayErr = null;
        }

        private static bool CanResultButtonPress()
        {
            return true;
        }

        private static bool CanDigitButtonPress(string button)
        {
            return true;
        }
    }
}
