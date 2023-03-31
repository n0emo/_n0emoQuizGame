using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using QuizGame.Core;

namespace QuizGame.Views;

public partial class ExtraScoreWindow : Window
{
    private Team[] _teams;
    private int? _selectedTeam = 0;

    public required Team[] Teams
    {
        init
        {
            _teams = value;
            
            for (int i = 0; i < _teams.Length; i++)
            {
                var team = _teams[i];
                var button = new RadioButton()
                {
                    Content = team.Name,
                    GroupName = "Teams",
                    Tag = i,
                    Margin = new Thickness(20,0,0,0),
                    Height = 30
                };
                int teamIndex = i;
                button.Checked += delegate(object? sender, RoutedEventArgs args)
                {
                    _selectedTeam = teamIndex;
                };
                
                this.FindControl<StackPanel>("TeamsStackPanel").Children.Add(button);
            }
        }
    }

    public delegate void ExitEventHandler(object sender, ExtraScoreEventArgs args);

    public event ExitEventHandler Exit;
    
    public ExtraScoreWindow()
    {
        InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        Closed += (sender, args) => Exit?.Invoke(this, new ExtraScoreEventArgs{ TeamIndex = null, Score = 0 });
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (int.TryParse(this.FindControl<TextBox>("ScoreTextBox").Text, out var score))
        {
            Exit?.Invoke(this, new ExtraScoreEventArgs{ TeamIndex = _selectedTeam, Score = score });
            Close();
        }
    }
}

public class ExtraScoreEventArgs : EventArgs
{
    public required int? TeamIndex { get; init; }
    
    public required int Score { get; init; }
} 