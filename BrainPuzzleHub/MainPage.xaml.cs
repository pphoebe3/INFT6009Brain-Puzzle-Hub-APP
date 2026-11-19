using BrainPuzzleHub.ViewModels;
//r!ViewModels！！
//Page initialization includes the VM.



namespace BrainPuzzleHub;

public partial class MainPage : ContentPage
{
    public MainPage(HomeViewModel vm)
    {
        InitializeComponent();
        //Parse XAML and draw the interface.
        BindingContext = vm;
        //Tell XAML that the data source is this VM
        //MainPage.xaml connects the two: HomeViewModel.cs
    }
}