using System;
using System.Linq;
using Calculator.ViewModels;
using engineering_calculator.Models;
using System.Globalization;
using System.Windows.Input;
using engineering_calculator.Commands;


namespace engineering_calculator.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        private DelegateCommand<string> _digitButtonPressCommand;
        private DelegateCommand<string> _getDigitCommand;
        private DelegateCommand _getResultCommand;

        private string _display;
        private string _displayExp;
        private string _displayErr;
        int _countOpenBracket;

        public string Display
        {
            get { return _display; }
            set
            {
                _display = value;
                OnPropertyChanged("Display");
            }
        }

        public string DisplayExp
        {
            get { return _displayExp; }
            set
            {
                _displayExp = value;
                OnPropertyChanged("DisplayExp");
            }
        }

        public string DisplayErr
        {
            get { return _displayErr; }
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

        private static bool CanResultButtonPress()
        {
            return true;
        }

        private static bool CanDigitButtonPress(string button)
        {
            return true;
        }

        public void GetDigit(string button) //вводим числа
        {
            Display = _display + button;
            DisplayErr = null;
        }

        public bool DisplayNull()
        {
            if (_display == "")
                Display = null;
            if (Display == null)
                return true;
            return false;
        }

        public void DigitButtonPress(string button) //вводим операторы
        {
            switch (button)
            {
                case "C": //очищаем поле
                    Display = null;
                    _countOpenBracket = 0;
                    break;
                case "Del": //backspace
                {
                    if (Display.Length == 1)
                    {
                        Display = null;
                        break;
                    }
                    char[] lastsimvol = null;
                    if (!DisplayNull())
                        lastsimvol = _display.Substring(_display.Length - 1).ToCharArray();
                    if (lastsimvol == null)
                    {
                        break;
                    }
                    if (lastsimvol[0] == ')')
                    {
                        _countOpenBracket++;
                    }
                    var doif = false;
                    if (lastsimvol[0] == '(' && _display.Length > 2 && _display.Length < 4 &&
                        char.IsLetter(_display.ElementAt(_display.Length - 2)) &&
                        char.IsLetter(_display.ElementAt(_display.Length - 3)))
                    {
                        Display = _display.Substring(0, _display.Length - 3);
                        doif = true;
                        if (DisplayNull())
                            break;
                        lastsimvol = _display.Substring(_display.Length - 1).ToCharArray();
                        _countOpenBracket--;
                    }

                    if (lastsimvol[0] == '(' && _display.Length > 3 &&
                        char.IsLetter(_display.ElementAt(_display.Length - 2)) &&
                        char.IsLetter(_display.ElementAt(_display.Length - 3)) &&
                        !char.IsLetter(_display.ElementAt(_display.Length - 4)))
                    {
                        Display = _display.Substring(0, _display.Length - 3);
                        doif = true;
                        if (DisplayNull())
                            break;
                        lastsimvol = _display.Substring(_display.Length - 1).ToCharArray();
                        _countOpenBracket--;
                    }
                    if (lastsimvol[0] == '(' && _display.Length > 3 &&
                        char.IsLetter(_display.ElementAt(_display.Length - 2)) &&
                        char.IsLetter(_display.ElementAt(_display.Length - 3)) &&
                        char.IsLetter(_display.ElementAt(_display.Length - 4)))
                    {
                        Display = _display.Substring(0, _display.Length - 4);
                        doif = true;
                        if (DisplayNull())
                            break;
                        lastsimvol = _display.Substring(_display.Length - 1).ToCharArray();
                        _countOpenBracket--;
                    }
                    if (lastsimvol[0] == '(' && _display.Length > 1 &&
                        !char.IsLetter(_display.ElementAt(_display.Length - 2)))
                    {
                        Display = _display.Substring(0, _display.Length - 2);
                        doif = true;
                        if (DisplayNull())
                            break;
                        _countOpenBracket--;
                    }

                    if (!doif)
                        Display = _display.Substring(0, _display.Length - 1);

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
                    var lastsimvol = _display.Substring(_display.Length - 1).ToCharArray();
                    const string digits = "0123456789";
                    foreach (var i in digits)
                    {
                        if (lastsimvol[0] != i) continue;
                        Display = _display + "*";
                        break;
                    }

                    if (lastsimvol[0] == '(')
                        break;
                    _countOpenBracket++;
                    Display = _display + button;
                    break;
                }
                case ")":
                {
                    if (_display == null)
                        break;
                    var lastsimvol = _display.Substring(_display.Length - 1).ToCharArray();
                    const string simvols = "(,+-/*^";
                    var isTrue = false;
                    if (lastsimvol[0] == '(')
                        break;
                    if (_countOpenBracket > 0)
                    {
                        foreach (var i in simvols)
                        {
                            if (lastsimvol[0] == i)
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
                    char[] lastsimvol = _display.Substring(_display.Length - 1).ToCharArray();
                    if (_display == "-")
                    {
                        break;
                    }
                    if (_display.Length == 1 || lastsimvol[0] == '(')
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
                        break;
                    char[] lastsimvol = _display.Substring(_display.Length - 1).ToCharArray();
                    var simvols = ",+-/*^";
                    bool isequal = false;
                    if (lastsimvol[0] == '(')
                        break;
                    foreach (var i in simvols)
                    {
                        if (lastsimvol[0] == i)
                        {
                            isequal = true;
                            DisplayErr = "введите второе число или функцию";
                            break;
                        }
                    }
                    if (isequal)
                        break;
                    Display = _display + button;
                    DisplayErr = null;
                    break;
                }
                case ",":
                {
                    if (_display == null)
                        Display = _display + "0";
                    const string simvols = "+-/*^";
                    const string digits = "0123456789";
                    var reverseDisplay = new string(_display.ToCharArray().Reverse().ToArray());
                    string tempDisplay = null;
                    var isHaveDigit = false;
                    var isHaveOperation = false;
                    var isHavePoint = false;

                    for (var i = 0; i < reverseDisplay.Length; i++)
                    {
                        if (reverseDisplay[i] != ',') continue;
                        isHavePoint = true;
                        tempDisplay = reverseDisplay.Substring(0, i);
                        break;
                    }
                    if (isHavePoint)
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
                                if (simvols.Any(j => i == j))
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
                                var lastsimvol1 = _display.Substring(_display.Length - 1).ToCharArray();
                                foreach (var j in simvols)
                                {
                                    if (lastsimvol1[0] != j) continue;
                                    Display = _display + "0";
                                    break;
                                }

                                Display = _display + button;
                                break;
                            }
                        }
                    else
                        Display = _display + button;
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
                var lastsimvol = _display.Substring(_display.Length - 1).ToCharArray();
                var digits = "1234567890";
                foreach (var i in digits)
                {
                    if (lastsimvol[0] == i)
                    {
                        Display = _display + "*";
                    }
                }
            }
            Display = _display + button;
            Display = _display + "(";
            _countOpenBracket++;
        }

        //вычисляет выражение
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

        public void CorrectExpression(out bool complete) //приводит выражение в корректный вид
        {
            complete = true;
            string subString1 = "бесконечность";
            string subString2 = "NaN";
            char[] lastsimvol = null;

            if (_display != null)
                lastsimvol = _display.Substring(_display.Length - 1).ToCharArray();
            if (lastsimvol == null)
            {
                complete = false;
                return;
            }
            if (Display.IndexOf(subString1, StringComparison.Ordinal) > -1 ||
                Display.IndexOf(subString2, StringComparison.Ordinal) > -1)
            {
                Display = null;
                complete = false;
                return;
            }
            var simvols = "+-/*^";
            foreach (var i in simvols)
            {
                if (lastsimvol[0] == i)
                {
                    DisplayErr = "введите второе число или функцию";
                    complete = false;
                    return;
                }
            }
            if (lastsimvol[0] == ',')
            {
                DisplayErr = "закончите нецелое число";
                complete = false;
                return;
            }
            // bool again;
            //do
            //{
            //    var doif = false;
            //    again = false;
            //    if (lastsimvol[0] == '(' && _display.Length > 2 && _display.Length < 4 &&
            //        char.IsLetter(_display.ElementAt(_display.Length - 2)) &&
            //        char.IsLetter(_display.ElementAt(_display.Length - 3)))
            //    {
            //        Display = _display.Substring(0, _display.Length - 3);
            //        doif = true;
            //        if (DisplayNull())
            //        {
            //            complete=false;
            //            return;
            //        }
            //        lastsimvol=_display.Substring(_display.Length-1).ToCharArray();
            //        _countOpenBracket--;
            //    }

            //    if (lastsimvol[0] == '(' && _display.Length > 3 &&
            //        char.IsLetter(_display.ElementAt(_display.Length - 2)) &&
            //        char.IsLetter(_display.ElementAt(_display.Length - 3)) &&
            //        !char.IsLetter(_display.ElementAt(_display.Length - 4)))
            //    {
            //        Display = _display.Substring(0, _display.Length - 3);
            //        doif = true;
            //        if (DisplayNull())
            //        {
            //            complete=false;
            //            return;
            //        }
            //        lastsimvol=_display.Substring(_display.Length-1).ToCharArray();
            //        _countOpenBracket--;
            //    }
            //    if (lastsimvol[0] == '(' && _display.Length > 3 &&
            //        char.IsLetter(_display.ElementAt(_display.Length - 2)) &&
            //        char.IsLetter(_display.ElementAt(_display.Length - 3)) &&
            //        char.IsLetter(_display.ElementAt(_display.Length - 4)))
            //    {
            //        Display = _display.Substring(0, _display.Length - 4);
            //        doif = true;
            //        if (DisplayNull())
            //        {
            //            complete=false;
            //            return;
            //        }
            //        lastsimvol=_display.Substring(_display.Length-1).ToCharArray();
            //        _countOpenBracket--;
            //    }
            //    if (lastsimvol[0] == '(' && _display.Length > 1 &&
            //        !char.IsLetter(_display.ElementAt(_display.Length - 2)))
            //    {
            //        Display = _display.Substring(0, _display.Length - 2);
            //        doif = true;
            //        if (DisplayNull())
            //        {
            //            complete=false;
            //            return;
            //        }
            //        lastsimvol=_display.Substring(_display.Length-1).ToCharArray();
            //        _countOpenBracket--;
            //    }

            //    if (lastsimvol[0] == '(' && _display.Length == 1)
            //    {
            //        Display = null;
            //        complete = false;
            //        return;
            //    }
            //    if (doif)
            //        again = true;

            //} while (again);
            if (lastsimvol[0] == '(')
            {
                DisplayErr = "завершите выражение со скобками";
                complete = false;
                return;
            }

            if (_display == "")
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
            if (complete)
            {
                DisplayErr = null;
            }
        }
    }
}
