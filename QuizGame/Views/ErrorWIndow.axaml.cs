using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace QuizGame.Views;

public partial class ErrorWIndow : Window
{
    public required string Message
    {
        init => this.FindControl<TextBlock>("MessageTextBlock").Text = value;
    }
    
    public ErrorWIndow()
    {
        InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ExitButtonClick(object? sender, RoutedEventArgs e) => Close();
}