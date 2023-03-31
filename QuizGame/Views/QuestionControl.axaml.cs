using System;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using QuizGame.Core;

namespace QuizGame.Views;

public partial class QuestionControl : UserControl
{
    private GameQuestion _question;
    private Team[] _teams;
    private int _currentTurn;
    
    // ReSharper disable once NotAccessedField.Local
    // TODO: add category
    private GameCategory _category;

    public delegate void EventHandler(object sender, AnswerEventArgs args);

    public event EventHandler? Answered;
    
#pragma warning disable CS8618 
    public QuestionControl()
    {
        InitializeComponent();
    }
    
    public QuestionControl(QuestionParams questionParams)
    {
        InitializeComponent();
        _question = questionParams.Question;
        _teams = questionParams.Teams;
        _currentTurn = questionParams.CurrentTurn;
        _category = questionParams.Category;

        SetQuestionText();
        SetOtherTeamsButtons();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void SetQuestionText()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(_question.Text);
        stringBuilder.Append("\n\n");
        for (int i = 0; i < _question.AnswerVariants.Length; i++)
        {
            stringBuilder.Append(i + 1);
            stringBuilder.Append(") ");
            stringBuilder.Append(_question.AnswerVariants[i]);
            stringBuilder.Append('\n');
        }

        this.FindControl<TextBlock>("QuestionTextBlock").Text = stringBuilder.ToString();
    }

    private void SetOtherTeamsButtons()
    {
        var otherTeamGrid = new Grid
        {
            Name = "OtherTeamsGrid",
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            ColumnDefinitions = new ColumnDefinitions("*")
        };
        var otherTeamsDock =  this.FindControl<DockPanel>("OtherTeamsPanel");
        otherTeamsDock.Children.Add(otherTeamGrid);

        int row = 0;
        for (int teamIndex = 0; teamIndex < _teams.Length; teamIndex++)
        {
            if (teamIndex != _currentTurn)
            {
                otherTeamGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
                var button = new Button()
                {
                    MinHeight = 50,
                    FontSize = 24,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Content = _teams[teamIndex].Name,
                    Margin = new Thickness(0, 0, 5, 0),
                    Background = new SolidColorBrush(Colors.LemonChiffon)
                };
                var answeredTeam = teamIndex;
                button.Click += (sender,  _) =>
                    Answered?.Invoke(sender!, new AnswerEventArgs(answeredTeam, _question.Cost / 2, AnswerEventArgs.AnswerResult.Successed));
                
                otherTeamsDock.Children.Add(button);
                Grid.SetRow(button, row);
                row++;
            }
        }
        
        DockPanel.SetDock((Control)otherTeamsDock.Children[1], Dock.Bottom);
    }

    private void AnswerButtonClick(object sender, RoutedEventArgs args) =>
        Answered?.Invoke(sender, new AnswerEventArgs(_currentTurn, _question.Cost, AnswerEventArgs.AnswerResult.Successed));

    private void NoTeamAnsweredButtonClick(object sender, RoutedEventArgs args) =>
        Answered?.Invoke(sender, new AnswerEventArgs(_currentTurn, 0, AnswerEventArgs.AnswerResult.Successed));

    private void CancelButtonClick(object sender, RoutedEventArgs args) =>
        Answered?.Invoke(sender, new AnswerEventArgs(_currentTurn, 0, AnswerEventArgs.AnswerResult.Canceled));

    private object? _initialContent;
    
    private void ViewAnswerClick(object sender, RoutedEventArgs args)
    {
        _initialContent = Content;
        var answerControl = new Views.AnswerControl(_question.AnswerVariants[_question.AnswerIndex], _question.AnswerImagePath);
        answerControl.Back += OnBackFromAnswer;
        Content = answerControl;
    }

    private void OnBackFromAnswer(object? sender, EventArgs eventArgs)
    {
        Content = _initialContent;
    }
}