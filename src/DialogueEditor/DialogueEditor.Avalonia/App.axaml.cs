using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DialogueEditor.Avalonia.ViewModels;
using DialogueEditor.Avalonia.Views;

namespace DialogueEditor.Avalonia
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var view = new StartupView()
                {
                    DataContext = new StartupViewModel(),
                };
                view.Show();
                view.Activate();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
