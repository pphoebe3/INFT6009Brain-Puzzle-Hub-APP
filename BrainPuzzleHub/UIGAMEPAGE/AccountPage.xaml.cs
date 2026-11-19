using BrainPuzzleHub.ViewModels;

namespace BrainPuzzleHub.UIGAMEPAGE;

public partial class AccountPage : ContentPage
{
    private AccountViewModel _vm => BindingContext as AccountViewModel;
    public AccountPage(AccountViewModel vm)
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
                await _vm.LoadAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Data loading failed", ex.ToString(), "OK");
            //Data loading failed
        }
    }
}













