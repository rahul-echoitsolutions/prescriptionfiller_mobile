using System;
using Xamarin.Forms;
using System.Windows.Input;
using API = PrescriptionFiller.Services.AccountAPIServer;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading.Tasks;
using PrescriptionFiller.Views;
using System.Diagnostics;
using System.Timers;

using PrescriptionFiller.Helpers;
using PrescriptionFiller.Views.Popups;

namespace PrescriptionFiller.ViewModels
{
    public class SignUpViewModel : ViewModelBase
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

        private string _confirmPassword;

        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                _confirmPassword = value;
                OnPropertyChanged("ConfirmPassword");
            }
        }

        private DateTime _dateOfBirth;

        public DateTime DateOfBirth
        {
            get { return _dateOfBirth; }
            set
            {
                _dateOfBirth = value;
                OnPropertyChanged("DateOfBirth");
            }
        }

        // initialized as 'M' to handle the case that the user sees the Male option is selected and decides not to select a sex
        private char _sex = 'M';

        public char Sex
        {
            get { return _sex; }
            set
            {
                _sex = value;
                OnPropertyChanged("Sex");
            }
        }

        private bool showPassword = false;

        private string _firstName;

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                OnPropertyChanged("FirstName");
            }
        }

        private string _lastName;

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                OnPropertyChanged("LastName");
            }
        }

        private string _phoneNumber;

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                _phoneNumber = value.Length > 12 ? value.Substring(0, 12) : value;
                OnPropertyChanged("PhoneNumber");
            }
        }

        private string _notes;

        public string Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                OnPropertyChanged("Notes");
            }
        }

        public ICommand FemaleCommand { get; set; }
        public ICommand MaleCommand { get; set; }
        public ICommand ContinueCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand SignInCommand { get; set; }
        public ICommand toggleShowPassword { get; set; }

        private Page signUpPage;

        private Grid page1, page2, page3;
            
        public SignUpViewModel( INavigation navigation, Page signUpPage)
        {
            this.navigation = navigation;
            this.signUpPage = signUpPage;
            DateOfBirth = DateTime.Now;

            float pageWidth = App.ScreenWidth / App.ScreenDensity;
            float pageHeight = App.ScreenHeight / App.ScreenDensity;

            ContinueCommand = new Command(Continue);

            CancelCommand = new Command(Cancel);

            SignInCommand = new Command(async () => {
                await navigation.PopModalAsync();
            });
                
            MaleCommand = new Command(() =>
                {
                    Sex = 'M';

                    Device.BeginInvokeOnMainThread(() =>
                        {
                            Button maleB = signUpPage.FindByName<Button>("MaleButton");
                            maleB.Image = (FileImageSource)ImageSource.FromFile(Device.OnPlatform("Icons/pfa_icon_check_on.png", "pfa_icon_check_on.png", ""));

                            Button femaleB = signUpPage.FindByName<Button>("FemaleButton");
                            femaleB.Image = (FileImageSource)ImageSource.FromFile(Device.OnPlatform("Icons/pfa_icon_check_off.png", "pfa_icon_check_off.png", ""));
                        });
                });
            FemaleCommand = new Command((object obj) =>
                {
                    Sex = 'F';

                    Device.BeginInvokeOnMainThread(() =>
                        {
                            Button maleB = signUpPage.FindByName<Button>("MaleButton");
                            maleB.Image = (FileImageSource)ImageSource.FromFile(Device.OnPlatform("Icons/pfa_icon_check_off.png", "pfa_icon_check_off.png", ""));

                            Button femaleB = signUpPage.FindByName<Button>("FemaleButton");
                            femaleB.Image = (FileImageSource)ImageSource.FromFile(Device.OnPlatform("Icons/pfa_icon_check_on.png", "pfa_icon_check_on.png", ""));
                        });
                });
            toggleShowPassword = new Command((object obj) =>
            {
                showPassword = !showPassword;

                Device.BeginInvokeOnMainThread(() =>
                {
                    
                    Button showPasswordButton = signUpPage.FindByName<Button>("showPasswordButton");
                    if (showPassword)
                    {
                        showPasswordButton.Image = (FileImageSource)ImageSource.FromFile(Device.OnPlatform("Icons/pfa_icon_check_on.png", "pfa_icon_check_on.png", ""));
                    }
                    else
                    {
                        showPasswordButton.Image = (FileImageSource)ImageSource.FromFile(Device.OnPlatform("Icons/pfa_icon_check_off.png", "pfa_icon_check_off.png", ""));
                    }
                });
                Entry passwordTextEntry = signUpPage.FindByName<Entry>("passwordText");
                Entry conformPasswordTextEntry = signUpPage.FindByName<Entry>("confirmPasswordText");
                passwordTextEntry.IsPassword = !showPassword;
                conformPasswordTextEntry.IsPassword = !showPassword;
            });
        }

        async Task HandleErrorCallback(API.ServerResponseErrorCode errorCode)
        {
            if (errorCode == API.ServerResponseErrorCode.ERROR_CONNECTION_TIMEOUT)
                await MessagePopup.Show(this.navigation, MessagePopup.Type.Error, MessagePopup.Icon.Connection, "Oops!", "Server is unreachable");

            else if (errorCode == API.ServerResponseErrorCode.ERROR_EMAIL_ADDRESS_EXISTS)
                await MessagePopup.Show(this.navigation, MessagePopup.Type.Warning, MessagePopup.Icon.User, "Sorry!", "This address email is already in use");

            else if (errorCode == API.ServerResponseErrorCode.ERROR_INVALID_EMAIL_ADDRESS)
                await MessagePopup.Show(this.navigation, MessagePopup.Type.Warning, MessagePopup.Icon.User, "Oops!", "Address email has not a valid format");

            else if (errorCode == API.ServerResponseErrorCode.ERROR_INVALID_PHONE_NUMBER)
                await MessagePopup.Show(this.navigation, MessagePopup.Type.Warning, MessagePopup.Icon.User, "Oops!", "Phone number has not a valid format");

            else if (errorCode == API.ServerResponseErrorCode.ERROR_INVALID_SEX)
                await MessagePopup.Show(this.navigation, MessagePopup.Type.Warning, MessagePopup.Icon.User, "Oops!", "Sex has not a valid format");

            else if (errorCode == API.ServerResponseErrorCode.ERROR_MISSING_REQUIRED_DATA)
                await MessagePopup.Show(this.navigation, MessagePopup.Type.Warning, MessagePopup.Icon.User, "Oops!", "You are missing a required field"); 

            else 
                await MessagePopup.Show(this.navigation, MessagePopup.Type.Error, MessagePopup.Icon.User, "Error!", "An unknown error happened");

        }

        async Task HandleSuccessCallback(API.ServerResponse<API.UserInfoResponseData> response)
        {
            //API.Instance.SaveToken(response.token);
            if (response.error)
            {
                API.ServerResponseErrorCode errorCode = response.error_code;
                if (errorCode == API.ServerResponseErrorCode.ERROR_CONNECTION_TIMEOUT)
                    await MessagePopup.Show(this.navigation, MessagePopup.Type.Error, MessagePopup.Icon.Connection, "Oops!", "Server is unreachable");

                else if (errorCode == API.ServerResponseErrorCode.ERROR_EMAIL_ADDRESS_EXISTS)
                    await MessagePopup.Show(this.navigation, MessagePopup.Type.Warning, MessagePopup.Icon.User, "Sorry!", "This address email is already in use");

                else if (errorCode == API.ServerResponseErrorCode.ERROR_INVALID_EMAIL_ADDRESS)
                    await MessagePopup.Show(this.navigation, MessagePopup.Type.Warning, MessagePopup.Icon.User, "Oops!", "Address email has not a valid format");

                else if (errorCode == API.ServerResponseErrorCode.ERROR_INVALID_PHONE_NUMBER)
                    await MessagePopup.Show(this.navigation, MessagePopup.Type.Warning, MessagePopup.Icon.User, "Oops!", "Phone number has not a valid format");

                else if (errorCode == API.ServerResponseErrorCode.ERROR_INVALID_SEX)
                    await MessagePopup.Show(this.navigation, MessagePopup.Type.Warning, MessagePopup.Icon.User, "Oops!", "Sex has not a valid format");

                else if (errorCode == API.ServerResponseErrorCode.ERROR_MISSING_REQUIRED_DATA)
                    await MessagePopup.Show(this.navigation, MessagePopup.Type.Warning, MessagePopup.Icon.User, "Oops!", "You are missing a required field");

                else
                    await MessagePopup.Show(this.navigation, MessagePopup.Type.Error, MessagePopup.Icon.User, "Error!", "An unknown error happened");
            } else {

                Page page = (navigation.ModalStack.Count == 0) ? null :
                navigation.ModalStack[navigation.ModalStack.Count - 1];
                while (navigation.ModalStack.Count > 1 && page.GetType() != typeof(LoginPage))
                {
                    await navigation.PopModalAsync();
                }
                ViewPages.Instance.loginPage.showSiginSuccessDialog();
                //if (navigation.ModalStack[] > 1)
                //  await navigation.PopModalAsync(); // Close sign up 

                //if (navigation.NavigationStack.Count > 1)
                //  await navigation.PopModalAsync(); // Close login 
            }
        }

        private async void Continue()
        {
            UserInfoValidationErrors errors = await API.Instance.SignUp(navigation, EmailAddress, Password, ConfirmPassword, DateOfBirth.ToString("yyyy-MM-dd"), Sex, FirstName, LastName, PhoneNumber, "todo remove me when field is removed", HandleSuccessCallback, HandleErrorCallback);
            if (errors == UserInfoValidationErrors.InconsistentPassword)
                await MessagePopup.Show(navigation, MessagePopup.Type.Warning, MessagePopup.Icon.User, "Oops!", "You have entered inconsistent Passwords");
            else if (errors != UserInfoValidationErrors.None)
                await MessagePopup.Show(navigation, MessagePopup.Type.Warning, MessagePopup.Icon.User, "Oops!", "You are missing some fields");
            return;
        }

        private async void Cancel()
        {
            //await HomePage.Instance.DisplayAlert("SignUpViewModel Cancel", "invoked", "cancel");
            Page page = (navigation.ModalStack.Count == 0) ? null :
                navigation.ModalStack[navigation.ModalStack.Count - 1];
            while (navigation.ModalStack.Count > 1 && page.GetType() != typeof(LoginPage))
            {
                await navigation.PopModalAsync();
            }
            return;
        }


         

    }
}

