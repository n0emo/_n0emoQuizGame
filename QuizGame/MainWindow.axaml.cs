using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using QuizGame.Core;
using MessageBoxSlim.Avalonia;
using MessageBoxSlim.Avalonia.DTO;
using MessageBoxSlim.Avalonia.Enums;

namespace QuizGame;

public partial class MainWindow : Window
{
    private GameSettings? game;
    
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void FileButtonClick(object sender, RoutedEventArgs args)
    {
        var openFileDialog = new OpenFileDialog();
        openFileDialog.Filters?.Add(new FileDialogFilter { Name = "Select game file", Extensions = { "json" } });
        var result = await openFileDialog.ShowAsync(this);
        var selectedFile = result?.FirstOrDefault();
        if (selectedFile == null) return;
        
        game = GameLoader.Load(selectedFile);
        if (game == null) return;
        FileLabel.Content = GetFileName(selectedFile);
        GameInfoLabel.Content = game.ToString();
    }

    private async void StartGameClick(object sender, RoutedEventArgs args)
    {
        if (game is null)
        {
            await BoxedMessage.Create(new MessageBoxParams
            {
                Buttons = ButtonEnum.Ok,
                ContentTitle = "Ошибка",
                ContentMessage = "Для начала игры нужно выбрать\nподходящий файл с настройками игры.",
            }).ShowDialogAsync(this);
            return;
        }
        
        Hide();
        GameWindow gameWindow = new GameWindow(game);
        gameWindow.Closed += (_, _) => { Close(); };
        gameWindow.Show();
    }

    private string GetFileName(string Path) => Path.Split('\\')[^1];
}