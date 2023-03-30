using System;
using System.Text.Json.Serialization;

namespace QuizGame.Core;

#pragma warning disable CS8618
[Serializable]
public class GameQuestion
{
    [JsonPropertyName("Text")]
    public string Text { get; set; }
    
    [JsonPropertyName("AnswerVariants")]
    public string[] AnswerVariants { get; set; }
    
    [JsonPropertyName("AnswerIndex")]
    public int AnswerIndex { get; set; }
    
    [JsonPropertyName("Cost")]
    public int Cost { get; set; }
    
    [JsonPropertyName("AnswerImagePath")]
    public string? AnswerImagePath { get; set; }
}