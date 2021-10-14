using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using PrescriptionFiller.Views;
using PrescriptionFiller.Models;
using API = PrescriptionFiller.Services.AccountAPIServer;

namespace PrescriptionFiller
{
    public class RootPage : SimpleMasterDetailPage
	{
		OptionItem previousItem;
        MenuPage menuPage;

		public RootPage ()
		{
            ViewPages.Instance.rootPage = this;
            var optionsPage = new MenuPage { Icon = Device.OnPlatform("Icons/pfa_icon_menu.png","pfa_icon_menu.png",""), Title = "Menu", Padding = new Thickness(0, Device.OnPlatform(20, 0, 0),0,0) };
            menuPage = optionsPage;

            optionsPage.Menu.ItemSelected += (sender, e) => NavigateTo(e.SelectedItem as OptionItem);

			Master = optionsPage;

			NavigateTo(optionsPage.Menu.ItemsSource.Cast<OptionItem>().First());

			//ShowLoginDialog();    
		}

        public void goToAccountInfoPage()
        {
            NavigateTo(menuPage.Menu.ItemsSource.Cast<OptionItem>().ElementAt(1));
        }

		public async void ShowLoginDialog()
		{
			var page = new LoginPage();
            ViewPages.Instance.loginPage = page;
			await Navigation.PushModalAsync(page);
		}

		void NavigateTo(OptionItem option)
		{
			if (previousItem != null)
				previousItem.Selected = false;

			option.Selected = true;
			previousItem = option;

			var displayPage = PageForOption(option);

			if (displayPage != null) {
				Detail = new NavigationPage (displayPage) {
					BarBackgroundColor = Color.FromHex ("#FF0000"),
					BarTextColor = Color.White
				};
			}

            IsPresented = false;
		}

		Page PageForOption (OptionItem option)
		{
			// TODO: Refactor this to the Builder pattern (see ICellFactory).
            if (option is PrescriptionsOptionItem)
            {
                HomePage homePage = HomePage.Instance;
                ViewPages.Instance.homePage = homePage;
                return homePage;
            }
            if (option is AccountInfoOptionItem)
            {
                AccountInfoPage page = AccountInfoPage.Instance;
                ViewPages.Instance.accountInfoPage = page;
                return page;
            }
            if (option is MedicalHistoryOptionItem)
            {
                MedicalHistory page = MedicalHistory.Instance;
                return page;
            }
			/*if (option is AboutOptionItem) 
				return new AboutPage();*/
			if (option is LogOutOptionItem) {
                API.Instance.Logout();
				ShowLoginDialog();
				return null;
			}
			throw new NotImplementedException (string.Format ("Unknown menu option: {0}", option.Title));
		}
	}
}

