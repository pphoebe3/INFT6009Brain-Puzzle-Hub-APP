//
//
using BrainPuzzleHub.Models;
using BrainPuzzleHub.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
//

namespace BrainPuzzleHub.ViewModels;
//Where does the data and logic come from? What does clicking the button do
//Managing data and behavior
//Home UI view model


public partial class HomeViewModel : ObservableObject
{
    private readonly LocationService _location;
    //GPS/Location Dependency Injection!!!
    private const string DailyChallengeDateKey = "daily_challenge_last_date";
    //DailyChallengeDependency Injection
    private readonly Random _rng = new();
    //Random game selection
    //Whether you have completed today's daily challenge, displayed on the banner.
    [ObservableProperty] private bool _isDailyChallengeDone;

    public ObservableCollection<GameItem> Games { get; } = new()
    //Games collection
    {//GameItem class
        new GameItem { Name = "2048", CardColor = "#FF6B6B", Route = "Game2048Page" },
        new GameItem { Name = "15puzzle", CardColor = "#4D96FF", Route = "SliderPage" },
        new GameItem { Name = "Minesweeper", CardColor = "#6BCB77",  Route = "MinesweeperPage" },
       
        
        //preparation
    };
    public HomeViewModel(LocationService location)
    //The constructor adds a new parameter, location.
    {
        _location = location;
        
    
        RefreshDailyChallengeStatus();
        // Request GPS permission and location once upon entering the homepage,
        // similar to the "get location on startup" prompt commonly seen in apps.
        _ = _location.RequestLocationOnceAsync();
    }
    //Check if the "last check-in date" saved in Preferences is today.
    private void RefreshDailyChallengeStatus()
    {
        string lastDate = Preferences.Get(DailyChallengeDateKey, string.Empty);
        IsDailyChallengeDone = lastDate == DateTime.Today.ToString("yyyy-MM-dd");
    }



    [RelayCommand]
    //RelayCommand will automatically generate command statements for XAML to bind to button !click events.
    private async Task NavigateToGame(GameItem game)
    //Navigation games
    {
        if (game is null) return;
        //await Shell.Current.GoToAsync(game.Route);
        //game.Route Decision Navigation
        try
        {
            await Shell.Current.GoToAsync(game.Route);
        }
        catch (Exception ex)
        {
            //The actual exception is displayed directly on the screen without needing to consult the logs.
            await Shell.Current.DisplayAlert("Navigation failed", ex.ToString(), "OK");
        }


    }



    [RelayCommand]
    private async Task NavigateToDailyChallenge()
    //Go to Daily Challenge
    {
        if (Games.Count == 0) return;

        var picked = Games[_rng.Next(Games.Count)];

        // 
        Preferences.Set(DailyChallengeDateKey, DateTime.Today.ToString("yyyy-MM-dd"));
        IsDailyChallengeDone = true;

        try
        {
            await Shell.Current.GoToAsync(picked.Route);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("导航失败", ex.ToString(), "OK");
        }


        //await Shell.Current.GoToAsync("DailyChallengePage");
    }

    [RelayCommand]
    private async Task ExitApp()//??!!
    {
        bool confirm = await Shell.Current.DisplayAlert(
            "Exit", "Are you sure you want to exit？", "Confirm", "Cancel");
        if (confirm)
            Application.Current?.Quit();
    }

    [RelayCommand]
    private async Task OpenAccount()
    {
        await Shell.Current.GoToAsync("AccountPage");
    }







}


