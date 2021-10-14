using PrescriptionFiller.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using API = PrescriptionFiller.Services.AccountAPIServer;

namespace PrescriptionFiller
{
    public partial class App : Application
    {
        public static int ScreenWidth;
        public static int ScreenHeight;
        public static float ScreenDensity;

        public static LoginPage loginPage { get; private set; }
        public static HomePage homePage { get; private set; }
        public static RootPage rootPage { get; private set; }
        public App()
        {
            InitializeComponent();

            rootPage = new RootPage();
            MainPage = rootPage;
            //layout.Children.Insert(layout.Children.Count,adView)
            // may need to force layout here.
            // ForceLayout();


            /************** Toolbar Color ********************/
            // Changing the Toolbar color and text color
            Resources = new ResourceDictionary();
            var navigationStyle = new Style(typeof(NavigationPage));
            var barTextColorSetter = new Setter { Property = NavigationPage.BarTextColorProperty, Value = Color.White };
            var barBackgroundColorSetter = new Setter { Property = NavigationPage.BarBackgroundColorProperty, Value = Color.FromHex("#FF0000") };

            navigationStyle.Setters.Add(barTextColorSetter);
            navigationStyle.Setters.Add(barBackgroundColorSetter);

            Resources.Add(navigationStyle);
            /************** Toolbar Color ********************/
            
        }

        private void goToHomePage()
        {
            //            StackTrace st = new StackTrace(true);
            //            rootPage.DisplayAlert("goToHomePage stack trace", st.ToString(), "Yes", "No");
            //rootPage.DisplayAlert("goToHomePage", "1 ", "Yes", "No");
            //rootPage.goToAccountInfoPage();
            if (API.Instance.Account != null && API.Instance.Account.user_info != null && API.Instance.Account.user_info.isAccountInfoEmpty())
            {
                //rootPage.DisplayAlert("goToHomePage", "2 ", "Yes", "No");
                //ViewPages.Instance.rootPage.goToAccountInfoPage();
                //ViewPages.Instance.accountInfoPage.showEditAccountInfoPage();
                //rootPage.DisplayAlert("Notification", "2 end", "Yes", "No");
            }
            if (API.Instance.Account != null && API.Instance.Account.user_info != null)
            {
                //rootPage.DisplayAlert("goToHomePage", "3 ", "Yes", "No");
                //ViewPages.Instance.homePage.Refresh();
                //try
                //{
                //rootPage.Navigation.PopModalAsync();
                //rootPage.Navigation.PopModalAsync();
                //}
                //catch (Exception e)
                //{
                //Console.WriteLine("LoginViewModel: Warning cannot pop model");
                //}
                //rootPage.DisplayAlert("Notification", "4 ", "Yes", "No");
            }
            else
            {
                rootPage.ShowLoginDialog();
            }
        }

        protected override void OnStart()
        {
            goToHomePage();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
            goToHomePage();
        }
    }
}
