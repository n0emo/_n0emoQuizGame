using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using QuizGame.Core;

namespace QuizGame.Views;

public partial class GameWindow : Window
{
    private readonly GameSettings _gameSettings;
    private int _currentTurn;
    private int _remainingQuestions;
    
    private object? _initialContent;
    private IBrush? _initialBackground;
    private Button _currentButton;

    #pragma warning disable CS8618
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public GameWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    public GameWindow(GameSettings gameSettings)
    {
        _gameSettings = gameSettings;
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        this.FindControl<Label>("GameNameLabel").Content = _gameSettings.Name;
        Title = _gameSettings.Name;
        InitializeCategories();
        UpdateTurn();
        UpdateScores();
        SetBackground();
        SetFontFamily();

        _remainingQuestions = _gameSettings.Categories.Select(c => c.Questions.Length).Sum();
    }

    private void SetFontFamily()
    {
        if(_gameSettings.FontFamily is null) return;
        try
        {
            FontFamily = FontFamily.Parse(_gameSettings.FontFamily);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void InitializeCategories()
    {
        var grid = this.FindControl<Grid>("CategoriesGrid");
        for (int i = 0; i < _gameSettings.Categories.Length; i++)
        {
            var category = _gameSettings.Categories[i];
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            var dock = GetCategoryDockPanel(category);
            grid.Children.Add(dock);
            Grid.SetRow(dock, i);
        }
    }

    private DockPanel GetCategoryDockPanel(GameCategory category)
    {
        var nameTextBlock = GetNameTextBlock(category);

        var questionGrid = GetQuestionGrid(category);

        var dock = new DockPanel()
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            Children =
            {
                nameTextBlock,
                questionGrid
            }
        };

        DockPanel.SetDock((Control)dock.Children[0], Dock.Top);
        DockPanel.SetDock((Control)dock.Children[1], Dock.Top);
        return dock;
    }

    private Grid GetQuestionGrid(GameCategory category)
    {
        var questionGrid = new Grid()
        {
            Background = new SolidColorBrush(Colors.FloralWhite),
            VerticalAlignment = VerticalAlignment.Stretch,
            RowDefinitions = new RowDefinitions(String.Join(',', Enumerable.Repeat("*", category.GridHeigth))),
            ColumnDefinitions = new ColumnDefinitions(String.Join(',', Enumerable.Repeat("*", category.GridWidth))),
        };

        var gridBackground = GetBackgroundFromString(category.Background);
        if (gridBackground is not null) questionGrid.Background = gridBackground;

        var chunks = category.Questions.Chunk(category.GridWidth).ToArray();

        for (int i = 0; i < chunks.Length; i++)
        {
            for (int j = 0; j < chunks[i].Length; j++)
            {
                var button = GetQuestionButton(category, chunks[i][j]);

                questionGrid.Children.Add(button);
                Grid.SetRow(button, i);
                Grid.SetColumn(button, j);
            }
        }

        return questionGrid;
    }

    private TextBlock GetNameTextBlock(GameCategory category)
    {
        var nameTextBlock = new TextBlock
        {
            Text = category.Name,
            FontSize = 36,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10)
        };
        var nameColor = GetBackgroundFromString(category.NameColor);
        if (nameColor is not null) nameTextBlock.Foreground = nameColor;
        return nameTextBlock;
    }

    private Button GetQuestionButton(GameCategory category, GameQuestion question)
    {
        if(question.AnswerImagePath is not null)
            question.AnswerImagePath = _gameSettings.Path + question.AnswerImagePath;
        
        var categoryParams = new QuestionParams()
        {
            Question = question,
            Teams = _gameSettings.Teams,
            Category = category,
            Background = GetBackgroundFromString(category.QuestionBackground)
        };
        
        var button = new Button()
        {
            Content = question.Cost.ToString(),
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            FontSize = 76,
            Margin = new Thickness(5),
            Tag = categoryParams,
        };

        var buttonBackground = GetBackgroundFromString(category.ButtonsBackground);
        if (buttonBackground is not null) button.Background = buttonBackground;
        var buttonForeground = GetColorFromString(category.ButtonTextColor);
        if (buttonBackground is not null) button.Foreground = buttonForeground;
        button.Click += ShowQuestion;
        return button;
    }

    private void ExitButtonClick(object sender, RoutedEventArgs args) => Close();

    private void NextTurn()
    {
        _currentTurn++;
        if (_currentTurn == _gameSettings.Teams.Length)
        {
            _currentTurn = 0;
        }
    }

    private void UpdateTurn()
    {
        this.FindControl<Label>("CurrentTurnLabel").Content = _gameSettings.Teams[_currentTurn].Name;
    }

    private void UpdateScores()
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var team in _gameSettings.Teams)
        {
            stringBuilder.Append(team.Name);
            stringBuilder.Append(": ");
            stringBuilder.Append(team.Score.ToString());
            stringBuilder.Append('\n');
        }

        this.FindControl<TextBlock>("ScoresTextBlock").Text = stringBuilder.ToString();
    }

    private void ShowQuestion(object? sender, RoutedEventArgs routedEventArgs)
    {
        if(sender is not Button button) return;
        _currentButton = button;
        var questionParams = (QuestionParams)button.Tag!;
        questionParams.CurrentTurn = _currentTurn;
        var questionControl = new QuestionControl(questionParams);
        questionControl.Answered += OnTeamAnswered;
        _initialContent = Content;
        _initialBackground = Background;
        Content = questionControl;
        Background = questionParams.Background;
    }

    private void OnTeamAnswered(object? sender, AnswerEventArgs args)
    {
        Content = _initialContent;
        Background = _initialBackground;
        if(args.Result == AnswerEventArgs.AnswerResult.Successed)
        {
            _gameSettings.Teams[args.AnsweredTeam].Score += args.Score;
            NextTurn();
            UpdateTurn();
            HideCurrentButton();
            ShowResultsIfNoQuestionsRemain();
        }
        
        UpdateScores();
    }

    private void SetBackground()
    {
        Background = GetBackgroundFromString(_gameSettings.Background);
        this.FindControl<Grid>("InfoGrid").Background = GetBackgroundFromString(_gameSettings.InfoBackground);
        this.FindControl<Grid>("CategoriesGrid").Background =
            GetBackgroundFromString(_gameSettings.CategoriesBackground);
    }

    private Brush? GetBackgroundFromString(string? background)
    {
        if (background is null) return null;
        
        if (background.StartsWith('#'))
        {
            return GetColorFromString(background);
        }

        return GetImageFromString(background);
    }

    private Brush? GetImageFromString(string background)
    {
        try
        {
            return new ImageBrush(new Bitmap(_gameSettings.Path + background))
            {
                Stretch = Stretch.UniformToFill
            };
        }
        catch (Exception)
        {
            ShowErrorWindow($"Ошибка при задании заднего фона картинкой: \"{background}\" - нет такого файла.");
            return null;
        }
    }

    private SolidColorBrush? GetColorFromString(string? color)
    {
        if (color is null) return null;
        try
        {
            return (SolidColorBrush)new BrushConverter().ConvertFrom(color)!;
        }
        catch (Exception)
        {
            ShowErrorWindow($"Ошибка при задании цвета: \"{color}\" - неправильный формат цвета");
            return null;
        }
    }

    private static void ShowErrorWindow(string message)
    {
        var errorWIndow = new ErrorWIndow()
        {
            Message = message,
            ShowActivated = true,
        };
        errorWIndow.Show();
    }

    private void HideCurrentButton()
    {
        _currentButton.IsVisible = false;
        _remainingQuestions--;
    }

    private void ShowResultsIfNoQuestionsRemain()
    {
        if (_remainingQuestions > 0) return;

        var gameResultControl = new GameResultControl(_gameSettings.Teams);
        gameResultControl.Exit += delegate { Close(); };

        Content = gameResultControl;
        
    }

    private void ExtraScoreButtonClick(object? sender, RoutedEventArgs e)
    {
        var extraWindow = new ExtraScoreWindow()
        {
            Teams = _gameSettings.Teams,
            Background =  this.FindControl<Grid>("InfoGrid").Background,
            FontFamily = FontFamily
        };
        extraWindow.Exit += (o, args) =>
        {
            if (!args.TeamIndex.HasValue) return;
            _gameSettings.Teams[args.TeamIndex.Value].Score += args.Score;
            UpdateScores();
        };
        extraWindow.Show();
    }
}

public class QuestionParams : RoutedEventArgs
{
    public required GameQuestion Question { get; init; }
    
    public required GameCategory Category { get; init; }
    
    public required Team[] Teams { get; init; }
    
    public Brush? Background { get; init; }
    
    public int CurrentTurn { get; set; }
}
