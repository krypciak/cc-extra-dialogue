using Avalonia.Media.Imaging;
using System;

namespace DialogueEditor.Avalonia.Models
{
    class CharacterExpression
    {
        public string Id { get { return $"{Character}-{Expression}"; } }

        public string Character { get; }

        public string Expression { get; }

        public IBitmap Image { get; }

        public CharacterExpression(string character, string expression, IBitmap image)
        {
            Character = character ?? throw new ArgumentNullException(nameof(character));
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            Image = image;
        }
    }
}
