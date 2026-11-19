using BrainPuzzleHub.ViewModels;

namespace BrainPuzzleHub.UIGAMEPAGE;


public partial class Slider_Page : ContentPage
{
    private SliderViewModel _vm => BindingContext as SliderViewModel;

    public Slider_Page(SliderViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            if (_vm is not null)
                await _vm.LoadBestMovesAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Failed to load the optimal number of steps", ex.ToString(), "OK");
        }
    }

}