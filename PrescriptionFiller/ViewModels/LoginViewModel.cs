using System;
using System.Net;
using System.Net.Http;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Windows.Input;
using Plugin.Media;
using PrescriptionFiller.Model;
using PrescriptionFiller.Services;
using API = PrescriptionFiller.Services.AccountAPIServer;
using PrescriptionFiller.Views;
using PrescriptionFiller.Views.Popups;

namespace PrescriptionFiller.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private INavigation navigation;

        private string _emailAddress;

        public string EmailAddress
        {
            get { return _emailAddress; }
            set
            {
                _emailAddress = value;
                OnPropertyChanged("EmailAddress");
            }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }
            
        public ICommand LoginCommand { get; set; }
        public ICommand SignUpCommand { get; set; }
        public ICommand ForgotPasswordCommand { get; set; }

        public LoginViewModel( INavigation p_navigation )
        {
            navigation = p_navigation;
            _emailAddress = "";
            _password = "";

            LoginCommand = new Command(async (object o) =>
                {
                    await API.Instance.Login(this.navigation, EmailAddress, Password, ConnectionSuccess, ConnectionFail);
                });

            SignUpCommand = new Command(async () =>
                {
                    var page = new SignUpPage();

                    await navigation.PushModalAsync(page);
                });

            ForgotPasswordCommand = new Command(async () =>
                {
                    await API.Instance.ResetPassword(this.navigation, EmailAddress, ResetPasswordSuccess, ResetPasswordFail);
                });
        }
                    
        private async Task ConnectionSuccess(API.LoginResponse response)
        {
            API.Instance.SaveAccount(response.token, response.user);
            ViewPages.Instance.homePage.Refresh();
            if (response.user != null && response.user.isAccountInfoEmpty())
            {
                ViewPages.Instance.rootPage.goToAccountInfoPage();
                ViewPages.Instance.accountInfoPage.showEditAccountInfoPage();
            }
            try {
            await navigation.PopModalAsync();
            await navigation.PopModalAsync();
            } catch (Exception e) {
                Console.WriteLine("LoginViewModel: Warning cannot pop model");
            }
        }

        private async Task ConnectionFail(API.ServerResponseErrorCode errorCode)
        {
            if (errorCode == API.ServerResponseErrorCode.ERROR_CONNECTION_TIMEOUT)
                await MessagePopup.Show(this.navigation, MessagePopup.Type.Error, MessagePopup.Icon.Connection, "Oops!", "Server is unreachable");
            
            else if (errorCode == API.ServerResponseErrorCode.ERROR_INCORRECT_USERNAME_OR_PASSWORD)
                await MessagePopup.Show(this.navigation, MessagePopup.Type.Error, MessagePopup.Icon.User, "Oops!", "Incorrect email address or password");

            else
                await MessagePopup.Show(this.navigation, MessagePopup.Type.Error, MessagePopup.Icon.User, "Sorry", "An unknown error has appeared");
        }

        private async Task ResetPasswordSuccess(API.ResetPasswordResponse response)
        {
            await MessagePopup.Show(this.navigation, MessagePopup.Type.Success, MessagePopup.Icon.User, "Password Request Sent", "You will receive an email with instructions.");
            //            try
            //            {
            //await navigation.PopModalAsync();
            //await navigation.PopModalAsync();
            //}
            //catch (Exception e)
            //{
            //Console.WriteLine("LoginViewModel: Warning cannot pop model");
            //}
        }

        private async Task ResetPasswordFail(API.ServerResponseErrorCode errorCode)
        {
            if (errorCode == API.ServerResponseErrorCode.ERROR_INCORRECT_USERNAME_OR_PASSWORD)
                await MessagePopup.Show(this.navigation, MessagePopup.Type.Error, MessagePopup.Icon.User, "Oops!", "Incorrect email address.");
            else
                await MessagePopup.Show(this.navigation, MessagePopup.Type.Success, MessagePopup.Icon.User, "Password Request Sent", "You will receive an email with instructions.");
        }
    }
}

