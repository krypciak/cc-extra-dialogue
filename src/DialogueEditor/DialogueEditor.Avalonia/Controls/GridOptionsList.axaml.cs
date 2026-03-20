using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DialogueEditor.Models;
using System;

namespace DialogueEditor.Avalonia.Controls
{
    public partial class GridOptionsList : UserControl
    {
        public event EventHandler<Option>? OptionClick;

        public GridOptionsList()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Option_Click(object sender, RoutedEventArgs e)
        {
            var selectedOption = (sender as Control)?.DataContext as Option;

            if (selectedOption != null)
                OptionClick?.Invoke(sender, selectedOption);
        }
    }
}
