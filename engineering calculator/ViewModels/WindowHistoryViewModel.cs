// <copyright file="WindowHistoryViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Calculator.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Input;
    using Calculator.Commands;
    using Newtonsoft.Json;

    internal class WindowHistoryViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _main = new MainWindowViewModel();

        public string DisplayHistory { get; } = JsonConvert.DeserializeObject<string>(File.ReadAllText("history.json"));
    }
}
