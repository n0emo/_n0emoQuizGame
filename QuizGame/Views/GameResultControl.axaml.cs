using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using QuizGame.Core;

namespace QuizGame.Views;

public partial class GameResultControl : UserControl
{
    public delegate void ExitHandler(object sender, RoutedEventArgs args);

    public event ExitHandler? Exit;
    
    public GameResultControl()
    {
        InitializeComponent();
    }

    public GameResultControl(Team[] teams)
    {
        InitializeComponent();
        SetWinnerLabel(teams);
        SetResultTextBlock(teams);
    }

    private void SetResultTextBlock(Team[] teams)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("Очки команд:\n");
        foreach (var team in teams.OrderByDescending(t => t.Score).Select(t => $"{t.Name}: {t.Score}"))
            stringBuilder.Append(team).Append('\n');

        this.FindControl<TextBlock>("ResultTextBlock").Text = stringBuilder.ToString();
    }

    private void SetWinnerLabel(Team[] teams) =>
        this.FindControl<Label>("WinnerLabel").Content = $"Победитель: {teams.MaxBy(t => t.Score)!.Name}";
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ExitButtonClick(object? sender, RoutedEventArgs args) => Exit?.Invoke(this, new RoutedEventArgs());
}