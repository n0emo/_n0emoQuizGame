using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;

namespace QuizGame;

public partial class AnswerControl : UserControl
{
    public delegate void AnswerEventHandler(object? sender, EventArgs args);

    public event AnswerEventHandler? Back;

    private string _text;
    private string? _imagePath;
    
    #pragma warning disable CS8618
    public AnswerControl()
    {
        InitializeComponent();
    }

    public AnswerControl(string text, string? imagePath)
    {
        _text = text;
        _imagePath = imagePath;
        
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        this.FindControl<TextBlock>("AnswerTextBlock").Text = _text;
        
        if(_imagePath is not null)
        {
            this.FindControl<Image>("AnswerImage").Source = new Bitmap(_imagePath);
        }
    }

    private void BackButtonClick(object? sender, RoutedEventArgs args) => Back?.Invoke(sender, args);
}
