using BrainPuzzleHub.UIGAMEPAGE;
using BrainPuzzleHub.ViewModels;
using Microsoft.Extensions.Logging;
//ViewModel
using BrainPuzzleHub.Services;
//data
//Plugin.LocalNotification
//Daily timed reminder push
using Plugin.LocalNotification;

//Long press gesture 9.0.0!!!!!!! can't ues Microsoft.Maui.Controls


namespace BrainPuzzleHub
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
   
                //Long press
                .UseLocalNotification()
                //Daily timed reminder push

                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
            //
#endif
            builder.Services.AddSingleton<LocationService>();
            //GPS/Location
            builder.Services.AddTransient<MinesweeperViewModel>();
            builder.Services.AddTransient<Minesweeper_Page>();
            //Minesweeper /Long press
            builder.Services.AddSingleton<NotificationService>();
            //Daily timed reminder push
            //user
            builder.Services.AddSingleton<AppSession>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<AccountViewModel>();
            builder.Services.AddTransient<AccountPage>();
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<HomeViewModel>();
            //AppShell also needs to be created via DI (because it now requires injecting AppSession).
            builder.Services.AddSingleton<AppShell>();
            //home
            builder.Services.AddSingleton<MainPage>();
            //2048
            builder.Services.AddTransient<Game2048ViewModel>();
            builder.Services.AddTransient<Game2048_Page>();
            // Database service, global singleton, shared by all games.
            builder.Services.AddSingleton<DatabaseService>();
            //AddTransient needs to reset its status every time it enters the game.
            builder.Services.AddTransient<SliderViewModel>();
            builder.Services.AddTransient<Slider_Page>();
            //15puzzle




            return builder.Build();
            // return
            //Register as a singleton


        }
    }
}
