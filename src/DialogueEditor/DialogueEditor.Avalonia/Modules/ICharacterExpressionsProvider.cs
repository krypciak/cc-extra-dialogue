using DialogueEditor.Avalonia.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DialogueEditor.Avalonia.Modules
{
    interface ICharacterExpressionsProvider
    {
        IReadOnlyList<CharacterExpression> CharacterExpressions { get; }

        Task<bool> LoadFrom(string directory);
    }
}
