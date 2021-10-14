using PrescriptionFiller.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API = PrescriptionFiller.Services.AccountAPIServer;


using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrescriptionFiller.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountInfoPage : ContentPage, UserInfoListener
    {
        public static AccountInfoPage _instance;
        public static AccountInfoPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AccountInfoPage();
                    UserInfoListenersNotifier.addListener(_instance);
                }
                return _instance;
            }
        }
        private AccountInfoPage()
        {
            refresh();
            NavigationPage.SetBackButtonTitle(this, "");
        }

        public void refresh()
        {
            //            this.layout = (Content as StackLayout).Children[1] as AbsoluteLayout;
            //            if (layout.Children.Count > 0 && layout.Children [0] is ScrollView)
            //  layout.Children.RemoveAt (0);
            ToolbarItems.Clear();
            InitializeComponent();
            if (API.Instance.Account != null && API.Instance.Account.user_info != null)
            {
                this.BindingContext = API.Instance.Account.user_info;
            }
        }

        void OnClick(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(EditAccountInfoPage.Instance);
        }

        public void showEditAccountInfoPage()
        {
            this.Navigation.PushAsync(EditAccountInfoPage.Instance);
        }

        void ViewMedicalHistory(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(MedicalHistory.Instance);
        }

        public void update()
        {
            // _instance.update();
        }
    }
}