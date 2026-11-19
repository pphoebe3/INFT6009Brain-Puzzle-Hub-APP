using Microsoft.Extensions.DependencyInjection;

namespace BrainPuzzleHub
{
    public partial class App : Application
    {
        //public App()
        //{
        //    InitializeComponent();
        //}

        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    return new Window(new AppShell());
        //}
        private readonly AppShell _appShell;
        //Receive the injected instance

        // 
        //AppShell is injected via a DI container, ensuring that its internally dependent AppSession can be correctly resolved.
        public App(AppShell appShell)
        {
            InitializeComponent();
            _appShell = appShell;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(_appShell);
        }




    }
}