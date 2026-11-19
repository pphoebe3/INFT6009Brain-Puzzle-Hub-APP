using BrainPuzzleHub.ViewModels;

namespace BrainPuzzleHub.UIGAMEPAGE;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel vm)
	{
        InitializeComponent();
        BindingContext = vm;
	}

}