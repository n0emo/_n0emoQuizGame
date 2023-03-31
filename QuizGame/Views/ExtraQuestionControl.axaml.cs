using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace QuizGame.Views;

public partial class ExtraQuestionControl : UserControl
{
    public ExtraQuestionControl()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}