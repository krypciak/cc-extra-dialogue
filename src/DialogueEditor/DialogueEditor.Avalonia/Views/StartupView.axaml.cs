using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DialogueEditor.Avalonia.Views
{
    public partial class StartupView : Window
    {
        public StartupView()
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
    }
}
