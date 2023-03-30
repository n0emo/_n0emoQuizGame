using System;
using System.Text.Json.Serialization;

namespace QuizGame.Core;

#pragma warning disable CS8618
[Serializable]
public class Team
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    public int Score { get; set; } = 0;
}