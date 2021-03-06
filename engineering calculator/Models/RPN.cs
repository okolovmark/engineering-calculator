﻿// <copyright file="RPN.cs" company="Okolov Company">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Calculator.Models
{
    using System;
    using System.Collections.Generic;

    public class RPN
    {
        // Метод Calculate принимает выражение в виде строки и возвращает результат, в своей работе использует другие методы класса
        public static double Calculate(string input)
        {
            var output = GetExpression(input); // Преобразовываем выражение в постфиксную запись
            var result = Counting(output); // Решаем полученное выражение
            return result; // Возвращаем результат
        }

        // Метод, преобразующий входную строку с выражением в постфиксную запись
        private static string GetExpression(string input1)
        {
            var input = input1;
            var output = string.Empty; // Строка для хранения выражения
            var operStack = new Stack<char>(); // Стек для хранения операторов
            input = input1.Replace("sin", "s");
            var temp = input;
            input = temp.Replace("cos", "c");
            temp = input;
            input = temp.Replace("tan", "t");
            temp = input;
            input = temp.Replace("√", "q");
            temp = input;
            input = temp.Replace("lg", "l");
            temp = input;
            input = temp.Replace("ln", "n");
            temp = input;
            input = temp.Replace("fct", "f");
            temp = input;
            input = temp.Replace("π", "3,14159265359");
            temp = input;
            input = temp.Replace("e", "2,71828182846");

            // Для каждого символа в входной строке
            for (var i = 0; i < input.Length; i++)
            {
                // Разделители пропускаем
                if (IsDelimeter(input[i]))
                {
                    continue; // Переходим к следующему символу
                }

                if (input[i] == '-' && (i == 0 || (i > 0 && (!char.IsDigit(input[i - 1]) || input[i - 1] == '('))))
                {
                    i++;
                    output += "-"; // в переменную для чисел добавляется знак "-"
                }

                // Если символ - цифра, то считываем все число

                // Если цифра
                if (char.IsDigit(input[i]))
                {
                    // Читаем до разделителя или оператора, что бы получить число
                    while (!IsDelimeter(input[i]) && !IsOperator(input[i]))
                    {
                        output += input[i]; // Добавляем каждую цифру числа к нашей строке
                        i++; // Переходим к следующему символу

                        if (i == input.Length)
                        {
                            break; // Если символ - последний, то выходим из цикла
                        }
                    }

                    output += " "; // Дописываем после числа пробел в строку с выражением
                    i--; // Возвращаемся на один символ назад, к символу перед разделителем
                }

                // Если символ - оператор
                if (!IsOperator(input[i]))
                {
                    continue;
                }

                switch (input[i])
                {
                    case '(':
                        operStack.Push(input[i]); // Записываем её в стек
                        break;
                    case ')':
                        // Выписываем все операторы до открывающей скобки в строку
                        var s = operStack.Pop();

                        while (s != '(')
                        {
                            output += s.ToString() + ' ';
                            s = operStack.Pop();
                        }

                        break;
                    default: // Если любой другой оператор

                        // Если в стеке есть элементы
                        if (operStack.Count > 0)
                        {
                            if (GetPriority(input[i]) <= GetPriority(operStack.Peek()))
                            {
                                // И если приоритет нашего оператора меньше или равен приоритету оператора на вершине стека
                                output += operStack.Pop() + " ";
                            }
                        }

                        // То добавляем последний оператор из стека в строку с выражением
                        operStack.Push(char.Parse(input[i].ToString()));

                            // Если стек пуст, или же приоритет оператора выше - добавляем операторов на вершину стека
                        break;
                }
            }

            // Когда прошли по всем символам, выкидываем из стека все оставшиеся там операторы в строку
            while (operStack.Count > 0)
            {
                output += operStack.Pop() + " ";
            }

            do
            {
                output = output.Replace("--", "-");
            }
            while (output.Contains("--"));

            return output; // Возвращаем выражение в постфиксной записи
        }

        // Метод, вычисляющий значение выражения, уже преобразованного в постфиксную запись
        private static double Counting(string input)
        {
            double result = 0; // Результат
            var temp = new Stack<double>(); // Временный стек для решения

            // Для каждого символа в строке
            for (var i = 0; i < input.Length; i++)
            {
                var x = string.Empty;

                // если минус первый и после него цифра, то число отрицательное
                if (input[i] == '-' && char.IsDigit(input[i + 1]))
                {
                    x += "-";
                    i++;
                }

                // Если символ - цифра, то читаем все число и записываем на вершину стека
                if (char.IsDigit(input[i]))
                {
                    // Пока не разделитель
                    while (!IsDelimeter(input[i]) && !IsOperator(input[i]))
                    {
                        x += input[i]; // Добавляем
                        i++;
                        if (i == input.Length)
                        {
                            break;
                        }
                    }

                    temp.Push(double.Parse(x)); // Записываем в стек
                    // x = string.Empty;
                    i--;
                }

                // Если символ - оператор
                else if (IsOperator(input[i]))
                {
                    if (temp.Count == 1)
                    {
                        var h = temp.Pop();
                        temp.Push(1);
                        temp.Push(h);
                    }

                    // Берем два последних значения из стека
                    var a = temp.Pop();
                    var b = temp.Pop();

                    // И производим над ними действие, согласно оператору
                    switch (input[i])
                    {
                        case '+':
                            result = b + a;
                            break;
                        case '-':
                            result = b - a;
                            break;
                        case '*':
                            result = b * a;
                            break;
                        case '/':
                            result = b / a;
                            break;
                        case '^':
                            result = Math.Pow(b, a);
                            break;
                        case 's':
                            result = Math.Sin(a);
                            temp.Push(b);
                            break;
                        case 'c':
                            result = Math.Cos(a);
                            temp.Push(b);
                            break;
                        case 't':
                            result = Math.Tan(a);
                            temp.Push(b);
                            break;
                        case 'q':
                            result = Math.Sqrt(a);
                            temp.Push(b);
                            break;
                        case 'l':
                            result = Math.Log10(a);
                            temp.Push(b);
                            break;
                        case 'n':
                            result = Math.Log(a);
                            temp.Push(b);
                            break;
                        case 'f':
                            double f = 1;
                            for (var s = a; s > 1; s--)
                            {
                                f = f * s;
                            }

                            result = f;
                            temp.Push(b);
                            break;
                    }

                    temp.Push(result); // Результат вычисления записываем обратно в стек
                }
            }

            return temp.Peek(); // Забираем результат всех вычислений из стека и возвращаем его
        }

        // Метод возвращает true, если проверяемый символ - разделитель ("пробел" или "равно")
        private static bool IsDelimeter(char c)
        {
            return " =".IndexOf(c) != -1;
        }

        // Метод возвращает true, если проверяемый символ - оператор
        private static bool IsOperator(char с)
        {
            return "+-/*^()sctqlnf".IndexOf(с) != -1;
            /**
             * s-sin
             * c-cos
             * t-tan
             * q-sqrt
             * l-log
             * n-ln
             * f-fct
             */
        }

        // Метод возвращает приоритет оператора
        private static byte GetPriority(char s)
        {
            switch (s)
            {
                case '(':
                    return 0;
                case ')':
                    return 1;
                case '+':
                    return 2;
                case '-':
                    return 3;
                case '*':
                    return 4;
                case '/':
                    return 4;
                case '^':
                    return 5;
                case 's':
                    return 5;
                case 'c':
                    return 5;
                case 't':
                    return 5;
                case 'q':
                    return 5;
                case 'l':
                    return 5;
                case 'n':
                    return 5;
                case 'f':
                    return 5;
                default:
                    return 6;
            }
        }
    }
}
