using BrainPuzzleHub.ViewModels;
using BrainPuzzleHub.Services;
//Replace Preferences with SQLite


namespace BrainPuzzleHub.UIGAMEPAGE;

public partial class Game2048_Page : ContentPage
{
    private Game2048ViewModel _vm => BindingContext as Game2048ViewModel;

    public Game2048_Page(Game2048ViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    //The highest score is read asynchronously every time the page is accessed.
    protected override async void OnAppearing()
    {
        //base.OnAppearing();


        //if (_vm is not null)
        //    await _vm.LoadBestScoreAsync();
        base.OnAppearing();
        try
        {
            if (_vm is not null)
                await _vm.LoadBestScoreAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Failed to load highest score", ex.ToString(), "OK");
        }

    }


    //Receive XAML swipe gestures and forward them to the ViewModel.
    private void OnSwiped(object sender, SwipedEventArgs e)
    {
        _vm?.OnSwiped(e.Direction);
    }
}