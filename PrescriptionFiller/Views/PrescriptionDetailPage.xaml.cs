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
	public partial class PrescriptionDetailPage : ContentPage
	{

		private Prescription prescription;
		public PrescriptionDetailPage()
		{
			InitializeComponent();
		}

		public PrescriptionDetailPage(Prescription prescription)
		{
			InitializeComponent();
			// TODO
			this.BindingContext = prescription;
			this.prescription = prescription;
			Image thumb = new Image()
			{
				Source = prescription.PrescriptionThumbImageSource,
				HeightRequest = 128,
			};

			var tapGestureRecognizer = new TapGestureRecognizer();
			tapGestureRecognizer.Tapped += (s, e) => {
				// handle the tap

				WebView webView = new WebView()
				{
					BackgroundColor = Color.Black,
					Source = new HtmlWebViewSource()
					{
						Html =
						@"<!DOCTYPE html>
						<html lang=""en-us"">
							<head>
								<meta name=""viewport"" content=""width=device-width, initial-scale=0.25, minimum-scale=0.25, maximum-scale=3.0, user-scalable=1"">
								<title></title>
								<style>
								body {
									background-color: black;
									margin: 0 20px 0 0;
									font-family: HelveticaNeue-Light, HelveticaNeue-UltraLight, Helvetica, Consolas, 'Courier New';
								}
								</style>
							</head>
							<body style=""text-align:center;"">
								<img src=""file://" + prescription.PrescriptionPath + @""" style=""width:100%;"" />
							</body>
						</html>",
					},
				};

				ContentPage fullScreenPrescription = new ContentPage()
				{
					Content = webView,
				};

				NavigationPage.SetHasNavigationBar(fullScreenPrescription, false);

				Navigation.PushAsync(new NavigationPage(fullScreenPrescription));
			};

			thumb.GestureRecognizers.Add(tapGestureRecognizer);

			(Content as StackLayout).Children.Insert(0, thumb);
		}

		async void Send(object sender, EventArgs args)
		{
			// TODO remove test code
			//await API.Instance.RetrievePharmaciesByName(this.Navigation, "test", ConnectionSuccess, ConnectionFail); 
			List<PharmacyInfo> pharmacyInfos = new List<PharmacyInfo>();
			await this.Navigation.PushAsync(new PharmacyListPage(this.prescription.prescriptionItem));
		}

		//        private async Task ConnectionSuccess(API.PharmaciesInfoResponse[] response)
		//        {
		//            //ViewPages.Instance.homePage.Refresh();
		////            await this.Navigation.PopModalAsync();
		//            List<PharmacyInfo> pharmacyInfos = new List<PharmacyInfo>();
		//            foreach(API.PharmaciesInfoResponse pharmacy in response) {
		//                PharmacyInfo pharmacyInfo = new PharmacyInfo();
		//                pharmacyInfo.address = pharmacy.address;
		//                pharmacyInfo.faxNumber = pharmacy.fax_number;
		//                pharmacyInfo.name = pharmacy.name;
		//                pharmacyInfo.pharmacyId = pharmacy.id;
		//                pharmacyInfo.phoneNumber = pharmacy.phone_number;
		//                pharmacyInfo.zipCode = pharmacy.zip_code;
		//                pharmacyInfo.prescriptionItem = this.prescription.prescriptionItem;
		//                pharmacyInfos.Add(pharmacyInfo);
		//            }
		//            await this.Navigation.PushAsync(new PharmacyListPage(pharmacyInfos));
		//            //await navigation.PopModalAsync();
		//        }
		//
		//        private async Task ConnectionFail(API.ServerResponseErrorCode errorCode)
		//        {
		//            if (errorCode == API.ServerResponseErrorCode.ERROR_CONNECTION_TIMEOUT)
		//                await MessagePopup.Show(this.Navigation, MessagePopup.Type.Error, MessagePopup.Icon.Connection, "Oops!", "Server is unreachable");
		//
		////            else if (errorCode == API.ServerResponseErrorCode.ERROR_INCORRECT_USERNAME_OR_PASSWORD)
		////                await MessagePopup.Show(this.navigation, MessagePopup.Type.Warning, MessagePopup.Icon.User, "Oops!", "Incorrect email address or password");
		//
		//            else
		//                await MessagePopup.Show(this.Navigation, MessagePopup.Type.Error, MessagePopup.Icon.User, "Sorry", "An unknown error has appeared");
		//        }
	}
}