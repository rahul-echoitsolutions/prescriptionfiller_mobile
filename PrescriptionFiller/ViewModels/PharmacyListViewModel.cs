using System;
using System.Collections.Generic;
using Xamarin.Forms;
using PrescriptionFiller.Model;
using PrescriptionFiller.Services;
using API = PrescriptionFiller.Services.AccountAPIServer;
using System.Threading.Tasks;
using PrescriptionFiller.Views;

namespace PrescriptionFiller.ViewModels
{
	public class PharmacyListViewModel : ViewModelBase
	{
        public class SearchTimer : System.Timers.Timer {
            public SearchTimer(double interval) : base(interval) {

            }
            public PharmacyListViewModel pharmacyListViewModel { get; set; }
        }
        SearchTimer timer;

        private string _city;
        public string city
        {
            get { return _city; }
            set {
                _city = value;
            }
        }

        private string _longitude;
        public string longitude
        {
            get { return _longitude; }
            set {
                _longitude = value;
            }
        }

        private string _latitude;
        public string latitude
        {
            get { return _latitude; }
            set
            {
                _latitude = value;
            }
        }

        private string _pharmacyName;
        public string pharmacyName
        {
            get { return _pharmacyName; }
            set {
                _pharmacyName = value;
            }
        }
        //private string _currlocationStr;
        //public string currlocationStr
        //{
        //    get { return _currlocationStr; }
        //    set
        //    {
        //        _currlocationStr = value;
        //        OnPropertyChanged("currlocationStr");
        //    }
        //}


        //private string _searchText;
        //public string searchText
        //{ 
        //    get { return _searchText; }
        //    set {
        //        bool changed = false;
        //        if (!nvl(this._searchText).Equals(nvl(value)))
        //        {
        //            changed = true;
        //        }
        //        _searchText = value; 
        //        if (changed && !nvl(this._searchText).Equals(""))
        //        {
        //            timedSearch(); 
        //        }
        //    }
        //}
        private string nvl(string inString) {
            return (inString == null) ? "" : inString;
        }
		private readonly INavigation navigation;
//		public PharmacyListModel pharmacyList { get; set; }
//		public List<PharmacyInfo> list {
//			get { return pharmacyList.list; }
//			set { pharmacyList.list = value; }
//		}

        private PrescriptionItem prescriptionItem;

        public PharmacyListViewModel (INavigation _navigation, PrescriptionItem prescriptionItem){
			navigation = _navigation;
            this.prescriptionItem = prescriptionItem;
            if (API.Instance.pharmacyInfos != null && API.Instance.pharmacyInfos.Count > 0) {
                foreach (PharmacyInfo pharmacyInfo in API.Instance.pharmacyInfos)
                {
                    pharmacyInfo.prescriptionItem = this.prescriptionItem;
                }
                PharmacyListPage.instance.showPharmacyInfos(API.Instance.pharmacyInfos);
            }
        }
        //private void timedSearch() {
        //    if (timer != null)
        //    {
        //        timer.Dispose();
        //        timer = null;
        //    }
        //    timer = new SearchTimer(500);
        //    timer.pharmacyListViewModel = this;
        //    timer.Elapsed += searchPharmacies;
        //    timer.Start();
        //}
        //private void searchPharmacies(Object sender, System.Timers.ElapsedEventArgs e) {
        //    SearchTimer pharmacySearchTimer = (SearchTimer)sender;
        //    Search();
        //    Console.WriteLine("Searching for pharcies with " + this.searchText);
        //    timer.Dispose();
        //}
        public async void SearchByCityAndName()
        {
            await API.Instance.RetrievePharmaciesByCityAndName(this.navigation, this.city, this.pharmacyName, ConnectionSuccess, ConnectionFail); 
        }
        public async void SearchByLocation()
        {
            await API.Instance.RetrievePharmaciesByLocation(this.navigation, this.longitude, this.latitude, ConnectionSuccess, ConnectionFail);
        }
        private async Task ConnectionSuccess(API.ServerResponse<API.PharmaciesInfoResponse[]> response)
        {
            API.PharmaciesInfoResponse[] pharmacyData = response.data;
            List<PharmacyInfo> pharmacyInfos = new List<PharmacyInfo>();
            foreach(API.PharmaciesInfoResponse pharmacy in pharmacyData) {
                PharmacyInfo pharmacyInfo = new PharmacyInfo();
                pharmacyInfo.address = pharmacy.address;
                pharmacyInfo.faxNumber = pharmacy.fax_number;
                pharmacyInfo.name = pharmacy.name;
                pharmacyInfo.pharmacyId = pharmacy.id;
                pharmacyInfo.phoneNumber = pharmacy.phone_number;
                pharmacyInfo.zipCode = pharmacy.zip_code;
                pharmacyInfo.prescriptionItem = this.prescriptionItem;
                pharmacyInfos.Add(pharmacyInfo);
            }
            PharmacyListPage.instance.showPharmacyInfos(pharmacyInfos);
        }

        private async Task ConnectionFail(API.ServerResponseErrorCode errorCode)
        {
            List<PharmacyInfo> pharmacyInfos = new List<PharmacyInfo>();
            PharmacyListPage.instance.showPharmacyInfos(pharmacyInfos);
        }
	}
}

