using System;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace QuizGame.Core;

#pragma warning disable CS8618
[Serializable]
public class GameSettings
{
    [JsonPropertyName("Name")]
    public string Name { get; set;  }
    
    [JsonPropertyName("Categories")]
    public GameCategory[] Categories { get; set; }
    
    [JsonPropertyName("Teams")]
    public Team[] Teams { get; set; }
    
    [JsonPropertyName("Background")]
    public string? Background { get; set; }
    
    [JsonPropertyName("InfoBackground")]
    public string? InfoBackground { get; set; }
    
    [JsonPropertyName("CategoriesBackground")]
    public string? CategoriesBackground { get; set; }
    
    [JsonPropertyName("FontFamily")]
    public string? FontFamily { get; set; }
    
    public string? Path { get; set; } 

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append($"Название: {Name}.\n\n");
        
        stringBuilder.Append("Категории:\n");
        foreach (var category in Categories)
        {
            stringBuilder.Append($"{category.Name}: {category.Questions.Length} вопрос{GetPluralEnding(category.Questions.Length)}.\n");
        }
        
        stringBuilder.Append('\n');
        
        stringBuilder.Append("Команды:\n");
        stringBuilder.Append(String.Join(", ", Teams.Select(t => t.Name)));
        stringBuilder.Append('.');
        
        stringBuilder.Append('\n');
        return stringBuilder.ToString();
    }

    private string GetPluralEnding(int number)
    {
        int mod10 = number % 10;
        int mod100 = number % 100;
        if (mod10 == 1 && mod100 / 10 != 1) return "";
        if (mod10 >= 2 && mod10 <= 4 && mod100 % 10 != 1) return "а";
        return "ов";
    }
}