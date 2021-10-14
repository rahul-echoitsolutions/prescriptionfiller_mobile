using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API = PrescriptionFiller.Services.AccountAPIServer;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PrescriptionFiller.Views.Popups;

namespace PrescriptionFiller.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditAccountInfoPage : ContentPage
    {
        private static EditAccountInfoPage _instance;

        public static EditAccountInfoPage Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EditAccountInfoPage();

                return _instance;
            }
        }
        public PrescriptionFiller.Model.UserInfo editUserInfo;
        private EditAccountInfoPage()
        {
            InitializeComponent();
            editUserInfo = new PrescriptionFiller.Model.UserInfo();
            PrescriptionFiller.Model.UserInfo currentUserInfo = API.Instance.Account.user_info;
            currentUserInfo.copyTo(editUserInfo);
            editUserInfo.password = API.Instance.password;
            this.BindingContext = editUserInfo;
        }
        async void sendUserInfoUpdate(object sender, EventArgs args)
        {
            PrescriptionFiller.Model.UserInfo userInfo = EditAccountInfoPage.Instance.editUserInfo;
            if (userInfo.password == null || userInfo.password.Trim().Equals(""))
            {
                await MessagePopup.Show(EditAccountInfoPage.Instance.Navigation, MessagePopup.Type.Error, MessagePopup.Icon.User, "Oops!", "Password is required.");
                return;
            }
            await API.Instance.UpdateInfo(this.Navigation, EditAccountInfoPage.Instance.editUserInfo, successCallback, errorCallback);

            //public async Task<UserInfoValidationErrors> UpdateInfo(INavigation navigation, UserInfo newUserInfo, SuccessCallback<UpdateInfoResponse> successCallback, ErrorCallback errorCallback) {
            //CreatePrescription(this.Navigation ,userId, pharmacyId, 
            //    description, imagePath, 
            //  successCallback, errorCallback);
        }

        static private async Task successCallback(API.ServerResponse<API.UpdateInfoResponse> response)
        {
            Console.WriteLine("perscription server has updated user info with id " + response.data.id);
            EditAccountInfoPage.Instance.editUserInfo.copyTo(API.Instance.Account.user_info);
            await LoadingPopup.Dismiss(EditAccountInfoPage.Instance.Navigation);
            AccountInfoPage.Instance.refresh();
            await EditAccountInfoPage.Instance.Navigation.PopToRootAsync();
        }

        static private async Task errorCallback(API.ServerResponseErrorCode errorCode)
        {
            if (errorCode == API.ServerResponseErrorCode.ERROR_CONNECTION_TIMEOUT)
                await MessagePopup.Show(EditAccountInfoPage.Instance.Navigation, MessagePopup.Type.Error, MessagePopup.Icon.Connection, "Oops!", "Server is unreachable");

            else if (errorCode == API.ServerResponseErrorCode.ERROR_MISSING_REQUIRED_DATA)
                await MessagePopup.Show(EditAccountInfoPage.Instance.Navigation, MessagePopup.Type.Error, MessagePopup.Icon.User, "Oops!", "Missing required Data");

            else
                await MessagePopup.Show(EditAccountInfoPage.Instance.Navigation, MessagePopup.Type.Error, MessagePopup.Icon.User, "Sorry", "An unknown error has appeared");
        }
    }
}