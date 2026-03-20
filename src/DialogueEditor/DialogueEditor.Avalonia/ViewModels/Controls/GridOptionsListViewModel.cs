using DialogueEditor.Avalonia.ViewModels;
using DialogueEditor.Models;
using System.Collections.Generic;

namespace DialogueEditor.ViewModels.Controls
{
    public class GridOptionsListViewModel : ViewModelBase
    {
        public IEnumerable<Option> Options { get; }

        public GridOptionsListViewModel(IEnumerable<Option> options)
        {
            Options = options;
        }
    }
}
