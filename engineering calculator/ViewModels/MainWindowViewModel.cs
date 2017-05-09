// <copyright file="MainWindowViewModel.cs" company="Okolov Company">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Calculator.ViewModels
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows.Input;
    using Commands;
    using Models;
    using Newtonsoft.Json;
    using Views;

    internal class MainWindowViewModel : ViewModelBase
    {
        private DelegateCommand<string> _digitButtonPressCommand;
        private DelegateCommand<string> _getDigitCommand;
        private DelegateCommand _getResultCommand;
        private DelegateCommand _showHistoryCommand;

        private string _display;
        private string _displayExp;
        private string _displayHistory;
        private string _displayErr;
        private int _countOpenBracket;
        private string _specialSymbols = "πe";

        public string Display
        {
            get => _display;

            set
            {
                _display = value;
                OnPropertyChanged("Display");
            }
        }

        public string DisplayExp
        {
            get => _displayExp;

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

        public ICommand ShowHistoryCommand
        {
            get
            {
                if (_showHistoryCommand == null)
                {
                    _showHistoryCommand = new DelegateCommand(ShowHistory, CanShowHistoryButtonPress);
                }

                return _showHistoryCommand;
            }
        }

        private string DisplayHistory
        {
            get => _displayHistory;

            set
            {
                _displayHistory = value;
                OnPropertyChanged("DisplayHistory");
            }
        }

        private static bool CanResultButtonPress()
        {
            return true;
        }

        private static bool CanShowHistoryButtonPress()
        {
            return true;
        }

        private static bool CanDigitButtonPress(string button)
        {
            return true;
        }

        private void GetDigit(string button)
        {
            if (_display != null && _display.Length > 32)
            {
                DisplayErr = "Введено максимальное количество знаков";
                return;
            }

            char[] lastSymbol = null;
            if (!DisplayNull())
            {
                lastSymbol = _display.Substring(_display.Length - 1).ToCharArray();
            }

            if (lastSymbol == null)
            {
                Display = _display + button;
                DisplayErr = null;
                return;
            }

            foreach (var ss in _specialSymbols)
            {
                if (lastSymbol[0] == ss)
                {
                    DisplayErr = "Введите математическое действие";
                    return;
                }
            }

            Display = _display + button;
            DisplayErr = null;
        }

        private void DigitButtonPress(string button)
        {
            switch (button)
            {
                case "C":
                    Display = null;
                    DisplayErr = null;
                    _countOpenBracket = 0;
                    break;
                case "Del":
                    CorrectInputDel();
                    break;
                case "(":
                    CorrectFirstBracket();
                    break;
                case ")":
                    CorrectLastBracket();
                    break;
                case "-":
                    CorrectInputMinus();
                    break;
                case "+":
                case "/":
                case "*":
                case "^":
                    CorrectInputOperator(button);
                    break;
                case ",":
                    CorrectInputPoint(button);
                    break;
                case "e":
                case "π":
                    CorrectInputSpecialSymbol(button);
                    break;
                case "sin":
                case "cos":
                case "tan":
                case "√":
                case "lg":
                case "fct":
                case "ln":
                    CorrectInputFunc(button);
                    break;
            }
        }

        private bool CheckMaxLength()
        {
            if (_display != null && _display.Length > 32)
            {
                DisplayErr = "Введено максимальное количество знаков";
                return true;
            }

            return false;
        }

        private bool DisplayNull()
        {
            if (_display == string.Empty)
            {
                Display = null;
            }

            return Display == null;
        }

        private void CorrectInputDel()
        {
            if (Display == null)
            {
                return;
            }

            if (Display.Length == 1)
            {
                DisplayErr = null;
                Display = null;
                return;
            }

            char[] lastSymbol = null;
            if (!DisplayNull())
            {
                lastSymbol = _display.Substring(_display.Length - 1).ToCharArray();
            }

            if (lastSymbol == null)
            {
                return;
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
                DisplayErr = null;
                doif = true;
                if (DisplayNull())
                {
                    return;
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
                DisplayErr = null;
                doif = true;
                if (DisplayNull())
                {
                    return;
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
                DisplayErr = null;
                doif = true;
                if (DisplayNull())
                {
                    return;
                }

                lastSymbol = _display.Substring(_display.Length - 1).ToCharArray();
                _countOpenBracket--;
            }

            if (lastSymbol[0] == '(' && _display.Length > 1 &&
                !char.IsLetter(_display.ElementAt(_display.Length - 2)))
            {
                Display = _display.Substring(0, _display.Length - 2);
                DisplayErr = null;
                doif = true;
                if (DisplayNull())
                {
                    return;
                }

                _countOpenBracket--;
            }

            if (!doif)
            {
                Display = _display.Substring(0, _display.Length - 1);
                DisplayErr = null;
            }
        }

        private void CorrectInputOperator(string button)
        {
            if (_display == null)
            {
                DisplayErr = "Введите число или функцию";
                return;
            }

            if (CheckMaxLength())
            {
                return;
            }

            var btn = button.ToCharArray();
            char[] lastSymbol = _display.Substring(_display.Length - 1).ToCharArray();
            var symbols = "+-/*^";
            if (lastSymbol[0] == '(')
            {
                return;
            }

             if (lastSymbol[0] == btn[0])
            {
                DisplayErr = "Введите число или функцию";
                return;
            }

            if (lastSymbol[0] == ',')
            {
                DisplayErr = "Закончите число";
                return;
            }

            char[] prelastSymbol = null;
            if (_display.Length > 1)
            {
                prelastSymbol = _display.Substring(_display.Length - 2).ToCharArray();
            }

            foreach (var i in symbols)
            {
                if (lastSymbol[0] == i && prelastSymbol == null)
                {
                    Display = _display.Substring(0, _display.Length - 1) + button;
                    DisplayErr = null;
                    return;
                }

                if (lastSymbol[0] == i && char.IsDigit(prelastSymbol[0]))
                {
                    Display = _display.Substring(0, _display.Length - 1) + button;
                    DisplayErr = null;
                    return;
                }

                if (lastSymbol[0] == '-' && prelastSymbol == null)
                {
                    DisplayErr = "Введите число или функцию";
                    return;
                }

                if (lastSymbol[0] == '-' && prelastSymbol[0] == i)
                {
                    DisplayErr = "Введите число или функцию";
                    return;
                }
            }

            Display = _display + button;
            DisplayErr = null;
        }

        private void CorrectInputMinus()
        {
            if (_display == null)
            {
                Display = _display + '-';
                DisplayErr = null;
                return;
            }

            if (CheckMaxLength())
            {
                return;
            }

            var symbols = "+/*^";
            var lastSymbol = _display.Substring(_display.Length - 1).ToCharArray();
            if (lastSymbol[0] == '-')
            {
                DisplayErr = "Введите число или функцию";
                return;
            }

            if (lastSymbol[0] == ',')
            {
                DisplayErr = "Закончите число";
                return;
            }

            if ((_display.Length == 1 || lastSymbol[0] == '(') || lastSymbol[0] == ')')
            {
                Display = _display + '-';
                DisplayErr = null;
                return;
            }

            foreach (var sy in symbols)
            {
                if (lastSymbol[0] == sy)
                {
                    Display = _display.Substring(0, _display.Length - 1) + '-';
                    DisplayErr = null;
                    return;
                }
            }

            if (char.IsDigit(_display.ElementAt(_display.Length - 1)) ||
                (_display.ElementAt(_display.Length - 1) == '(' &&
                 (_display.ElementAt(_display.Length - 3) == ')' ||
                  char.IsDigit(_display.ElementAt(_display.Length - 3)) ||
                  char.IsLetter(_display.ElementAt(_display.Length - 3)))))
            {
                Display = _display + '-';
                DisplayErr = null;
            }
        }

        private void CorrectFirstBracket()
        {
            if (_display == null)
            {
                Display = _display + "(";
                _countOpenBracket++;
                return;
            }

            if (CheckMaxLength())
            {
                return;
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

            if (lastSymbol[0] == '(' || lastSymbol[0] == ',')
            {
                return;
            }

            _countOpenBracket++;
            Display = _display + '(';
        }

        private void CorrectLastBracket()
        {
            if (_display == null)
            {
                return;
            }

            if (CheckMaxLength())
            {
                return;
            }

            var lastSymbol = _display.Substring(_display.Length - 1).ToCharArray();
            const string Symbols = "(,+-/*^";
            var isTrue = false;
            if (lastSymbol[0] == '(')
            {
                return;
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
                    Display = _display + ')';
                }
            }
        }

        private void CorrectInputPoint(string button)
        {
            if (_display == null)
            {
                Display = _display + "0";
            }

            if (CheckMaxLength())
            {
                return;
            }

            char[] lastSymbol = _display.Substring(_display.Length - 1).ToCharArray();
            foreach (var ss in _specialSymbols)
            {
                if (lastSymbol[0] == ss)
                {
                    DisplayErr = "К специальным символам нельзя ставить точку";
                    return;
                }
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

                if (!isHavePoint)
                {
                    var lastSymbol1 = _display.Substring(_display.Length - 1).ToCharArray();
                    foreach (var j in Symbols)
                    {
                        if (lastSymbol1[0] == j)
                        {
                            Display = _display + "0";
                            break;
                        }
                    }
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
                                if (lastSymbol1[0] == j)
                                {
                                    Display = _display + "0";
                                    break;
                                }
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
        }

        private void CorrectInputSpecialSymbol(string button)
        {
            if (Display == null)
            {
                Display = _display + button;
                return;
            }

            if (CheckMaxLength())
            {
                return;
            }

            string digits = "1234567890" + _specialSymbols;
            const string Symbols = "+-/*^(";
            var lastSymbol = _display.Substring(_display.Length - 1);
            foreach (var ss in digits)
            {
                if (lastSymbol[0] == ss)
                {
                    Display = _display + '*' + button;
                    return;
                }
            }

            foreach (var sy in Symbols)
            {
                if (lastSymbol[0] == sy)
                {
                    Display = _display + button;
                    return;
                }
            }
        }

        private void CorrectInputFunc(string button)
        {
            if (CheckMaxLength())
            {
                return;
            }

            if (Display != null)
            {
                var lastSymbol = _display.Substring(_display.Length - 1).ToCharArray();
                var digits = "1234567890" + _specialSymbols;
                if (lastSymbol[0] == '-' && _display.Length == 1)
                {
                    DisplayErr = "Введите число";
                    return;
                }

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

        private void AddDisplayHistory()
        {
            if (_displayHistory != null)
            {
                DisplayHistory += "\n" + DisplayExp + " = " + Display;
            }
            else
            {
                DisplayHistory = DisplayExp + " = " + Display;
            }

            var json = JsonConvert.SerializeObject(DisplayHistory);
            File.WriteAllText("history.json", json);
        }

        private void GetResult()
        {
            if (CheckMaxLength())
            {
                return;
            }

            bool isTrue;
            CorrectExpression(out isTrue);
            if (!isTrue)
            {
                return;
            }

            DisplayExp = Display;
            Display = RPN.Calculate(Display).ToString(CultureInfo.CurrentCulture);
            AddDisplayHistory();
        }

        private void ShowHistory()
        {
            var windowHistory = new WindowHistory();
            windowHistory.Show();
        }

        private void CorrectExpression(out bool complete)
        {
            complete = true;
            string subString1 = "бесконечность";
            string subString2 = "NaN";
            string subString3 = "E";
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
                Display.IndexOf(subString3, StringComparison.Ordinal) > -1)
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
    }
}
