using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using DialogueEditor.Avalonia.Modules;
using DialogueEditor.Avalonia.Views;
using DialogueEditor.Models;
using MessageBox.Avalonia;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace DialogueEditor.Avalonia.ViewModels
{
    internal class StartupViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        public ReactiveCommand<Window, Unit> SearchCurrentDirectoryCommand { get; }
        public ReactiveCommand<Window, Unit> SelectDirectoryCommand { get; }

        public StartupViewModel()
        {
            CancelCommand = ReactiveCommand.Create(OnCancel);
            SearchCurrentDirectoryCommand = ReactiveCommand.CreateFromTask<Window>(OnSearchCurrentDirectory);
            SelectDirectoryCommand = ReactiveCommand.CreateFromTask<Window>(OnSelectDirectory);
        }

        void OnCancel()
        {
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.Shutdown();
        }

        async Task OnSearchCurrentDirectory(Window window)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            await LaunchMainViewProc(window, currentDirectory);
        }

        async Task OnSelectDirectory(Window window)
        {
            var ofd = new OpenFolderDialog()
            {
                Directory = Directory.GetCurrentDirectory(),
                Title = "Select directory with compiled expressions",
            };

            var selectedDirectory = await ofd.ShowAsync(window);
            if (!string.IsNullOrWhiteSpace(selectedDirectory))
                await LaunchMainViewProc(window, selectedDirectory);
        }

        async Task LaunchMainViewProc(Window window, string expressionsDirectoryPath)
        {
            // Load character expressions into memory
            var expressionsProvider = new CharacterExpressionsProvider();
            var loadResult = await expressionsProvider.LoadFrom(expressionsDirectoryPath);

            if (loadResult != true)
            {
                await MessageBoxManager
                    .GetMessageBoxStandardWindow("Error", $"Failed to load expressions from: {Environment.NewLine}{expressionsDirectoryPath}")
                    .ShowDialog(window);
                return;
            }

            var optionsList = expressionsProvider.CharacterExpressions.Select(ce => new Option(ce, ce.Expression, ce.Image));
            if (optionsList.Count() == 0)
            {
                await MessageBoxManager
                    .GetMessageBoxStandardWindow("Error", $"Loaded 0 expressions from: {Environment.NewLine}{expressionsDirectoryPath}")
                    .ShowDialog(window);
                return;
            }

            ShowMainView(window, optionsList);
        }

        void ShowMainView(Window window, IEnumerable<Option> optionsList)
        {
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainView()
                {
                    DataContext = new MainViewModel(optionsList),
                };
                desktop.MainWindow.Show();
                desktop.MainWindow.Activate();

                window.Close();
            }

        }
    }
}
