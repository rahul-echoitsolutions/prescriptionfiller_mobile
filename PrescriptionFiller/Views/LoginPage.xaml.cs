using PrescriptionFiller.ViewModels;
using PrescriptionFiller.Views.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrescriptionFiller.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            this.BindingContext = new LoginViewModel(this.Navigation);
            NavigationPage.SetHasNavigationBar(this, false);
        }

        // Cancel the back button
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
        public async void showSiginSuccessDialog()
        {
            var page = new MessagePopup(MessagePopup.Type.Success, MessagePopup.Icon.Success, "Sign up", "You have been registered successfully ");
            await this.Navigation.PushModalAsync(page);

        }
    }
}