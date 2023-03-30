using System;
using System.Text.Json.Serialization;

namespace QuizGame.Core;

#pragma warning disable CS8618
[Serializable]
public class GameCategory
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }
    
    [JsonPropertyName("Questions")]
    public GameQuestion[] Questions { get; set; }

    [JsonPropertyName("GridWidth")]
    public int GridWidth { get; set; }

    [JsonPropertyName("GridHeigth")]
    public int GridHeigth  { get; set; }

    [JsonPropertyName("Background")]
    public string? Background { get; set; }
    
    [JsonPropertyName("NameColor")]
    public string? NameColor { get; set; }
    
    [JsonPropertyName("ButtonTextColor")]
    public string? ButtonTextColor { get; set; }
    
    [JsonPropertyName("ButtonsBackground")]
    public string? ButtonsBackground { get; set; }
    
    [JsonPropertyName("QuestionBackground")]
    public string? QuestionBackground { get; set; }
}