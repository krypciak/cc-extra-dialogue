using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DialogueEditor.Avalonia.ViewModels;
using DialogueEditor.Models;

namespace DialogueEditor.Avalonia.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void GridOptionsList_OptionClick(object sender, Option option)
        {
            (DataContext as MainViewModel)?.OnOptionSelected(option);
        }
    }
}
