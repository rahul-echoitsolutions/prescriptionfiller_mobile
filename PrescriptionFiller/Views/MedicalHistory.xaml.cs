using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API = PrescriptionFiller.Services.AccountAPIServer;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PrescriptionFiller.Helpers;

namespace PrescriptionFiller.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MedicalHistory : ContentPage, UserInfoListener
    {
        public static MedicalHistory _instance;
        public static MedicalHistory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MedicalHistory();
                    UserInfoListenersNotifier.addListener(_instance);
                }
                return _instance;
            }
        }
        private MedicalHistory()
        {
            refresh();
            NavigationPage.SetBackButtonTitle(this, "");
        }

        void OnClick(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(EditMedicalHistory.Instance);
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
        //void OnClick(object sender, EventArgs e)
        //{
        //    this.Navigation.PushAsync(EditAccountInfoPage.Instance);
        //}

        public void update()
        {
            //    _instance.update();
        }
    }
}