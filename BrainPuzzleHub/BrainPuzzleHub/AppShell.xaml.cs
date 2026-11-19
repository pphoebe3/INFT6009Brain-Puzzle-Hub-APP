
using BrainPuzzleHub.Services;
using BrainPuzzleHub.UIGAMEPAGE;


namespace BrainPuzzleHub;

public partial class AppShell : Shell
{
    //public AppShell()
    //{
    //    InitializeComponent();
    //    //Register routes using code
    //    //Game page route registration
    //    Routing.RegisterRoute("Game2048Page", typeof(Game2048_Page));
    //    // Routing.RegisterRoute("MinesweeperPage", typeof(MinesweeperPage));
    //    // Routing.RegisterRoute("SliderPage", typeof(SliderPage));
    //    // Routing.RegisterRoute("SudokuPage", typeof(SudokuPage));
    //}
    private readonly AppSession _session;

    public AppShell(AppSession session)
    // injection AppSession
    {
        InitializeComponent();
        _session = session;

        
        Routing.RegisterRoute("SliderPage", typeof(Slider_Page));
        Routing.RegisterRoute("MinesweeperPage", typeof(Minesweeper_Page));
        Routing.RegisterRoute("Game2048Page", typeof(Game2048_Page));
        Routing.RegisterRoute("LoginPage", typeof(LoginPage));
        Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
        Routing.RegisterRoute("AccountPage", typeof(AccountPage));
        //Only after the shell has fully loaded should the login status be checked; at this point, Shell.Current will be effective.
        this.Loaded += (s, e) => CheckLoginStatus();
        //CheckLoginStatus();
        //Instead of calling `CheckLoginStatus()` directly, subscribe to `this.Loaded += (s, e) => CheckLoginStatus()`.
        //This calls Shell.Current.GoToAsync.

    }

    //Upon startup, the system checks the login status; if the user is not logged in, the system redirects to the login page.
    private async void CheckLoginStatus()
    {
        if (!_session.IsLoggedIn)
            await Shell.Current.GoToAsync("LoginPage");
        //Change the absolute path .////LoginPage to a relative path LoginPage
    }





}
