
using BrainPuzzleHub.ViewModels;


namespace BrainPuzzleHub.UIGAMEPAGE;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel vm )
	{
		InitializeComponent();
        BindingContext = vm;
    }
}