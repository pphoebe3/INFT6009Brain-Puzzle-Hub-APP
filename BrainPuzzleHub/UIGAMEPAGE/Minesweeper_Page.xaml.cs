
using BrainPuzzleHub.ViewModels;
namespace BrainPuzzleHub.UIGAMEPAGE;

public partial class Minesweeper_Page : ContentPage
{
    private MinesweeperViewModel _vm => BindingContext as MinesweeperViewModel;
    public Minesweeper_Page(MinesweeperViewModel vm)
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
                await _vm.LoadBestTimeAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Loading failed at optimal time", ex.ToString(), "OK");
        }
    }
}