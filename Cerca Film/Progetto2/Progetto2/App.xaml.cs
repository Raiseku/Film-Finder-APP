using Xamarin.Forms;

namespace Progetto2
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // Imposto la NavigationPage per permettere lo spostamento tra varie pagine.
            // La prima pagina che sarà caricata sarà la MainPage.
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
