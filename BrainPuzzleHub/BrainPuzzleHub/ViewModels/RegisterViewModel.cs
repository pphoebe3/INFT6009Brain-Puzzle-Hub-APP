//using Android.App;
using BrainPuzzleHub.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrainPuzzleHub.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private readonly AppSession _session;
    private NotificationService _notification;

    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _confirmPassword = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;

    public RegisterViewModel(DatabaseService db, AppSession session, NotificationService notification)
    {
        _db = db;
        _session = session;
        _notification = notification;

    }

    [RelayCommand]
    private async Task Register()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Username) || Username.Trim().Length < 3)
        {
            ErrorMessage = "Username must be at least 3 characters long";
            return;
        }
        if (Password.Length < 6)
        {
            ErrorMessage = "Password must be at least 6 characters long";
            return;
        }
        if (Password != ConfirmPassword)
        {
            ErrorMessage = "The two passwords did not match.";
            return;
        }

        var user = await _db.RegisterAsync(Username.Trim(), Password);
        if (user is null)
        {
            ErrorMessage = "Username is already taken.";
            return;
        }

        _session.CurrentUserId = user.Id;
        await _notification.ScheduleDailyReminderAsync(15, 45);
        //Daily Challenge Reminder
        await Shell.Current.GoToAsync("//MainPage");
    }

    [RelayCommand]
    private async Task GoToLogin()
    {
        await Shell.Current.GoToAsync("LoginPage");
    }
}
