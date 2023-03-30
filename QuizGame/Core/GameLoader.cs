using System.IO;
using System.Text.Json;

namespace QuizGame.Core;

public static class GameLoader
{
    public static GameSettings Load(string path)
    {
        var game = JsonSerializer.Deserialize<GameSettings>(File.ReadAllText(path))
            ?? throw new InvalidDataException();
        game.Path = path.Replace(path.Split('\\')[^1], "");
        return game;
    }
}