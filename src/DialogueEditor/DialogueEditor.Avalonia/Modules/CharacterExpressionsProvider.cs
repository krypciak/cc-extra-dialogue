using DialogueEditor.Avalonia.Models;
using DialogueEditor.Avalonia.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DialogueEditor.Avalonia.Modules
{
    class CharacterExpressionsProvider : ICharacterExpressionsProvider
    {
        List<CharacterExpression> _characterExpressions = new List<CharacterExpression>();

        public IReadOnlyList<CharacterExpression> CharacterExpressions { get { return _characterExpressions.AsReadOnly(); } }

        public async Task<bool> LoadFrom(string directory)
        {
            try
            {
                var loadedCharacterExpressions = new List<CharacterExpression>();
                var characterDirectories = Directory.GetDirectories(directory);

                object loadingLock = new object();

                var loadingTasks = new List<Task<List<CharacterExpression>>>();

                foreach (var characterDirectory in characterDirectories)
                {
                    var task = LoadCharacterExpressions(characterDirectory);
                    loadingTasks.Add(task);
                    task.Start();
                }

                await Task.WhenAll(loadingTasks);

                foreach (var task in loadingTasks)
                    loadedCharacterExpressions.AddRange(task.Result);

                _characterExpressions = loadedCharacterExpressions;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{nameof(CharacterExpressionsProvider)} - {ex.Message}");
                return false;
            }

            return true;
        }

        Task<List<CharacterExpression>> LoadCharacterExpressions(string characterDirectoryPath)
        {
            return new Task<List<CharacterExpression>>(() =>
            {
                var characterExpressions = new List<CharacterExpression>();
                var characterName = Path.GetFileNameWithoutExtension(characterDirectoryPath);
                var expressionFiles = Directory.GetFiles(characterDirectoryPath);
                foreach (var expressionFile in expressionFiles)
                {
                    var expressionName = Path.GetFileNameWithoutExtension(expressionFile);
                    
                    try
                    {
                        var expressionBitmap = ImageLoader.CreateCroppedBitmapFromFile(expressionFile);
                        characterExpressions.Add(new CharacterExpression(characterName, expressionName, expressionBitmap));
                    }
                    catch (Exception) { }
                }

                return characterExpressions;
            });
        }
    }
}
