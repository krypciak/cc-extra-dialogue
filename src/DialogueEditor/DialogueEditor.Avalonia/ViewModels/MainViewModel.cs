using DialogueEditor.Avalonia.Models;
using DialogueEditor.Avalonia.Views;
using DialogueEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using System.Reactive;
using Avalonia;

namespace DialogueEditor.Avalonia.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        List<Option> _options = new List<Option>();

        string _characterFilter = string.Empty;
        string _expressionFilter = string.Empty;
        bool _showStatic = true;
        bool _showAnimations = true;

        Dictionary<string, List<Option>> _characterExpressionOptionsDictionary = new Dictionary<string, List<Option>>();

        public List<string> CharactersList { get; } = new List<string>();

        public string CharacterFilter
        {
            get { return _characterFilter; }
            set { this.RaiseAndSetIfChanged(ref _characterFilter, value); }
        }

        public string ExpressionFilter
        {
            get { return _expressionFilter; }
            set { this.RaiseAndSetIfChanged(ref _expressionFilter, value); }
        }

        public bool ShowStatic
        {
            get { return _showStatic; }
            set { this.RaiseAndSetIfChanged(ref _showStatic, value); }
        }

        public bool ShowAnimations
        {
            get { return _showAnimations; }
            set { this.RaiseAndSetIfChanged(ref _showAnimations, value); }
        }

        public List<Option> FilteredOptionsList
        {
            get
            {
                return GetFilteredOptionsList();
            }
        }

        public ReactiveCommand<Unit, Unit> OpenNewWindowCommand { get; }

        public MainViewModel(IEnumerable<Option> options)
        {
            _options.AddRange(options);

            var characters = _options.Select(o => (o.OptionData as CharacterExpression).Character).Distinct();

            if (characters.Any())
            {
                CharactersList.AddRange(characters);
                foreach (var character in CharactersList)
                {
                    var characterExpressions = _options.Where(o => (o.OptionData as CharacterExpression).Character == character);
                    _characterExpressionOptionsDictionary[character] = new List<Option>(characterExpressions);
                }
            }
            else
            {
                var emptyCharacter = "None";
                CharactersList.Add(emptyCharacter);
                _characterExpressionOptionsDictionary[emptyCharacter] = new List<Option>();
                for (int i = 0; i < 20; i++)
                {
                    _characterExpressionOptionsDictionary[emptyCharacter].Add(
                        new Option(new CharacterExpression(emptyCharacter, $"expression{i}", null), $"option{i}", null));
                }
            }

            var lea = CharactersList.FirstOrDefault((c) => string.Equals(c, "lea", StringComparison.OrdinalIgnoreCase));
            CharacterFilter = lea ?? CharactersList.First();

            this.WhenAnyValue(x => x.CharacterFilter, x => x.ExpressionFilter, x => x.ShowStatic, x => x.ShowAnimations)
                .Subscribe(x => this.RaisePropertyChanged(nameof(FilteredOptionsList)));

            OpenNewWindowCommand = ReactiveCommand.Create(OpenNewWindow);
        }

        List<Option> GetFilteredOptionsList()
        {
            return _characterExpressionOptionsDictionary[CharacterFilter]
                .Where(o => (o.OptionData as CharacterExpression).Expression.LastIndexOf(' ') > 0
                && char.IsDigit((o.OptionData as CharacterExpression).Expression.Last()) ? ShowAnimations : ShowStatic)
                .Where(o => (o.OptionData as CharacterExpression).Expression.ToLowerInvariant().Contains(ExpressionFilter.ToLowerInvariant()))
                .ToList();
        }

        internal void OnOptionSelected(Option option)
        {
            var expression = (option.OptionData as CharacterExpression);
            if (expression != null)
            {
                // Todo: Could probably be simplified
                var textToCopy = $"{expression.Character} > {expression.Expression}";

                // Remote index if its a frame from animation
                if (textToCopy.LastIndexOf(' ') > 0 && char.IsDigit(textToCopy.Last()))
                    textToCopy.Substring(0, textToCopy.LastIndexOf(' ') - 1);

                textToCopy = $"{textToCopy}: ";

                Application.Current.Clipboard.SetTextAsync(textToCopy);
            }
        }

        void OpenNewWindow()
        {
            var optionsListCopy = new List<Option>(_options);
            var vm = new MainViewModel(optionsListCopy)
            {
                // Copy current filtering settings
                CharacterFilter = CharacterFilter,
                ExpressionFilter = ExpressionFilter,
                ShowStatic = ShowStatic,
                ShowAnimations = ShowAnimations,
            };

            var newWindow = new MainView()
            {
                DataContext = vm,
            };
            newWindow.ShowActivated = false; // Lack of activation allows to quickly open multiple windows without loosing focus
            newWindow.Show();
        }
    }
}
