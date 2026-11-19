using System;
using System.Collections.Generic;
using System.Text;
using BrainPuzzleHub.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace BrainPuzzleHub.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private readonly AppSession _session;
    private readonly NotificationService _notification;
    //Schedule a reminder after successful login/registration

    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;

    public LoginViewModel(DatabaseService db, AppSession session, NotificationService notification)
    {
        _db = db;
        _session = session;
        _notification = notification;
    }

    [RelayCommand]
    private async Task Login()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter your username and password.";
            return;
        }

        var user = await _db.LoginAsync(Username.Trim(), Password);
        if (user is null)
        {
            ErrorMessage = "Username or password incorrect.";
            return;
        }

        _session.CurrentUserId = user.Id;

        //daily challenge reminders scheduled after successful login
        await _notification.ScheduleDailyReminderAsync(18, 0);

        await Shell.Current.GoToAsync("//MainPage");
    }

    [RelayCommand]
    private async Task GoToRegister()
    {
        await Shell.Current.GoToAsync("RegisterPage");
    }
}
