using System;
using System.Collections.Generic;
using System.Text;
using BrainPuzzleHub.Models;
using BrainPuzzleHub.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BrainPuzzleHub.ViewModels;

public partial class AccountViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private readonly AppSession _session;
    private User? _currentUser;

    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _avatar = "🙂";
    [ObservableProperty] private bool _soundEnabled = true;
    [ObservableProperty] private bool _vibrationEnabled = true;

    //A summary of the highest scores for each game, linked to the account page.
    public ObservableCollection<GameScore> ScoreSummary { get; } = new();

    public AccountViewModel(DatabaseService db, AppSession session)
    {
        _db = db;
        _session = session;
    }

    //Called when the page is in the OnAppearing state,
    //this function loads the current user's profile and score.
    public async Task LoadAsync()
    {
        if (_session.CurrentUserId is not int userId) return;

        _currentUser = await _db.GetUserByIdAsync(userId);
        if (_currentUser is null) return;

        Username = _currentUser.Username;
        Avatar = _currentUser.Avatar;
        SoundEnabled = _currentUser.SoundEnabled;
        VibrationEnabled = _currentUser.VibrationEnabled;

        ScoreSummary.Clear();
        var scores = await _db.GetAllScoresAsync();
        foreach (var s in scores)
            ScoreSummary.Add(s);
    }

    //NOO__Automatically save when sound effects are switched on or off.
    partial void OnSoundEnabledChanged(bool value) => _ = SaveSettingsAsync();

    //Automatically save when vibration switch changes
    partial void OnVibrationEnabledChanged(bool value) => _ = SaveSettingsAsync();

    private async Task SaveSettingsAsync()
    {
        if (_currentUser is null) return;
        _currentUser.SoundEnabled = SoundEnabled;
        _currentUser.VibrationEnabled = VibrationEnabled;
        await _db.UpdateUserAsync(_currentUser);
    }

    [RelayCommand]
    private async Task Logout()
    {
        _session.Logout();
        await Shell.Current.GoToAsync("LoginPage");
        //Change the absolute path //LoginPage to a relative path

    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}
