using System;
using System.Collections.Generic;
using System.Text;

namespace BrainPuzzleHub.Services;

//Global Singleton: Stores the ID of the currently logged-in user and shares the login state across pages.
//Persistence is achieved through Preferences, ensuring automatic login retention after app restart.
public class AppSession
{
    private const string Key = "current_user_id";

    public int? CurrentUserId
    {
        get
        {
            int id = Preferences.Get(Key, -1);
            return id == -1 ? null : id;
        }
        set
        {
            if (value is null)
                Preferences.Remove(Key);
            else
                Preferences.Set(Key, value.Value);
        }
    }

    public bool IsLoggedIn => CurrentUserId is not null;

    public void Logout() => CurrentUserId = null;
}