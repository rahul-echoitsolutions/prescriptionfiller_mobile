using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API = PrescriptionFiller.Services.AccountAPIServer;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading;
using PrescriptionFiller.ViewModels;
using PrescriptionFiller.Views.Popups;
using PrescriptionFiller.Database;
using PrescriptionFiller.Helpers;

namespace PrescriptionFiller.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        static HomePage instance;
        private AbsoluteLayout layout;
        RefreshPrescriptionListTimer timer;
        SynchronizationContext _context;

        public class RefreshPrescriptionListTimer : System.Timers.Timer
        {
            public RefreshPrescriptionListTimer(double interval) : base(interval)
            {

            }
        }

        public enum PrescriptionsDisplayed
        {
            New,
            Sent,
            All,
        }

        public PrescriptionsDisplayed Choice
        {
            get { return m_choice; }
            set
            {
                m_choice = value;

                Refresh();
            }
        }

        private PrescriptionsDisplayed m_choice = PrescriptionsDisplayed.New;

        private static HomePage _instance;

        public static HomePage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new HomePage();
                }
                return _instance;
            }
        }


        public void CaptureCurrentContext()
        {
            _context = SynchronizationContext.Current;
        }

        private HomePage()
        {
            instance = this;
            InitializeComponent();
            //layout.Children.Insert(layout.Children.Count,adView)
            // may need to force layout here.
            // ForceLayout();
            CaptureCurrentContext();
            this.BindingContext = new HomeViewModel(this.Navigation, this);
            Refresh();
        }

        public async void Refresh()
        {
            // ######## Grid ###########
            // Getting the images sources
            if (API.Instance.Account == null || API.Instance.Account.user_info == null)
            {
                return;
            }

            int userId = API.Instance.Account.user_info.id;
            this.layout = (Content as StackLayout).Children[1] as AbsoluteLayout;
            // TODO may not be needed.
            //IEnumerable<PrescriptionItem> prescriptionItems = LocalPrescriptionDatabase.Instance.GetItems(userId);
            // comment out so that we will not have concurrency issue where the user tries to
            // resend a prescription that was previously sent before the list of prescriptions
            // is retrieved from the server 
            //showPrescriptions(instance.layout, prescriptionItems);
            await API.Instance.RetrievePrescriptionsByUserId(this.Navigation, userId,
                retrievePrescriptionsByUserIdSuccess,
                retrievePrescriptionsByUserIdFailed);
        }

        private void timedGetPrescriptions()
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
            // set timer to start task every 30 seconds
            timer = new RefreshPrescriptionListTimer(30000);
            timer.Elapsed += _timedGetPrescriptions;
            timer.Start();
        }

        private void _timedGetPrescriptions(Object sender, System.Timers.ElapsedEventArgs e)
        {
            RefreshPrescriptionListTimer timer = (RefreshPrescriptionListTimer)sender;
            new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    HomePage.Instance.Refresh(); // this works!
                });
            })).Start();
            timer.Dispose();
        }

        static private string nvl(string inString)
        {
            return (inString == null) ? "" : inString;
        }
        // the Dictionary key has truncates long strings producing wrong results
        // so I am creating my own map
        private class MyMap<K, V>
        {
            List<MyMapKeyValue<K, V>> keyValueEntries = new List<MyMapKeyValue<K, V>>();
            public void put(K key, V value)
            {
                int indexExistingKeyValue = findIndex(key);
                if (indexExistingKeyValue > 0)
                {
                    keyValueEntries.RemoveAt(indexExistingKeyValue);
                }
                keyValueEntries.Add(new MyMapKeyValue<K, V>(key, value));
            }
            public V getValue(K key)
            {
                int indexExistingKeyValue = findIndex(key);
                if (indexExistingKeyValue >= 0)
                {
                    return keyValueEntries[indexExistingKeyValue].value;
                }
                return default(V);
            }
            public int findIndex(K key)
            {
                int index = 0;
                int result = -1;
                foreach (MyMapKeyValue<K, V> keyValue in keyValueEntries)
                {
                    if (keyValue.key.Equals(key))
                    {
                        result = index;
                        return result;
                    }
                    index++;
                }
                return result;
            }
        }
        private class MyMapKeyValue<K, V>
        {
            public K key;
            public V value;
            public MyMapKeyValue(K key, V value)
            {
                this.key = key;
                this.value = value;
            }
        }
        static private async Task retrievePrescriptionsByUserIdSuccess(API.ServerResponse<API.PrescriptionRecord[]> serverResponse)
        {
            API.Instance.pharmacyInfos = new List<PharmacyInfo>();
            API.PrescriptionRecord[] userPrescriptionsResponses = serverResponse.data;
            try
            {
                await LoadingPopup.Dismiss(HomePage.Instance.Navigation);
            }
            catch (Exception e)
            {
            }

            Console.Out.WriteLine("retrievePrescriptionsByUserIdSuccess");
            //            _instance.DisplayAlert("retrievePrescriptionsByUserIdSuccess","method invoked","cancel");
            int userId = API.Instance.Account.user_info.id;
            //            _instance.DisplayAlert("retrievePrescriptionsByUserIdSuccess for user id", userId.ToString(), "cancel");
            IEnumerable<PrescriptionItem> localPrescriptionItems = LocalPrescriptionDatabase.Instance.GetItems(userId);
            MyMap<string, API.PrescriptionRecord> serverPrescriptionRecordsDict = new MyMap<string, API.PrescriptionRecord>();
            // update statuses of PrescriptionItem base on server values
            foreach (API.PrescriptionRecord serverPrescriptionRecord in userPrescriptionsResponses)
            {
                // this version of service API does not have a status assuming everything
                // from the server is SENT
                //                serverPrescriptionRecord.fax_status = "SENT";
                API.PrescriptionRecord existingPrescriptionRecord = null;
                existingPrescriptionRecord = serverPrescriptionRecordsDict.getValue(serverPrescriptionRecord.image_path);
                int existingServerId = (existingPrescriptionRecord != null) ? existingPrescriptionRecord.id : -1;
                int currentServerId = (serverPrescriptionRecord != null) ? serverPrescriptionRecord.id : -1;
                if (existingPrescriptionRecord == null)
                {
                    serverPrescriptionRecordsDict.put(serverPrescriptionRecord.image_path, serverPrescriptionRecord);
                    //serverPrescriptionRecordsDict[serverPrescriptionRecord.image_path] = serverPrescriptionRecord;
                }
                if (currentServerId > existingServerId)
                {
                    serverPrescriptionRecordsDict.put(serverPrescriptionRecord.image_path, serverPrescriptionRecord);
                }
            }
            //            _instance.DisplayAlert("localPrescriptionItems count", 
            //                                   new List<PrescriptionItem>(localPrescriptionItems).Count.ToString(),
            //                                   "cancel");
            foreach (PrescriptionItem localPrescriptionItem in localPrescriptionItems)
            {
                API.PrescriptionRecord serverPrescriptionRecord;
                serverPrescriptionRecord = serverPrescriptionRecordsDict.getValue(localPrescriptionItem.path);
                int serverPrescriptionId = (serverPrescriptionRecord == null) ? -1 : serverPrescriptionRecord.id;
                int serverPharmacyId = (serverPrescriptionRecord == null) ? -1 : serverPrescriptionRecord.pharmacy_id;
                int localPrescriptionId = localPrescriptionItem.serverPrescriptionId;
                string serverStatus = (serverPrescriptionRecord == null) ? "" : nvl(serverPrescriptionRecord.fax_status);
                string localStatus = nvl(localPrescriptionItem.status);
                string serverPrescriptionCreatedDt = (serverPrescriptionRecord == null) ? "" : nvl(serverPrescriptionRecord.created_at);
                if (serverPrescriptionRecord != null)
                {
                    if (!serverPrescriptionId.Equals(localPrescriptionId) ||
                        !serverStatus.Equals(localStatus))
                    {
                        localPrescriptionItem.status = serverStatus;
                        localPrescriptionItem.serverPrescriptionId = serverPrescriptionId;
                        localPrescriptionItem.createdDt = serverPrescriptionCreatedDt;
                        LocalPrescriptionDatabase.Instance.SaveItem(localPrescriptionItem);
                    }
                }
                // todo add call to get Pharamacy info by pharmacy id
                if (serverPharmacyId > 0 && !processedPharamcyId(serverPharmacyId, API.Instance.pharmacyInfos))
                {
                    await API.Instance.RetrievePharmaciesById(HomePage.Instance.Navigation, serverPharmacyId,
                        retrievePharmaciesByIdSuccess,
                        retrievePharmaciesByIdFail);
                }
            }
            showPrescriptions(instance.layout, localPrescriptionItems);
        }

        static private async Task retrievePharmaciesByIdSuccess(API.PharmaciesInfoResponse pharmacyInfoResponse)
        {
            if (pharmacyInfoResponse == null) return;
            API.Instance.pharmacyInfos = (API.Instance.pharmacyInfos == null) ? new List<PharmacyInfo>() : API.Instance.pharmacyInfos;
            PharmacyInfo pharmacyInfo = new PharmacyInfo();
            pharmacyInfo.address = pharmacyInfoResponse.address;
            pharmacyInfo.faxNumber = pharmacyInfoResponse.fax_number;
            pharmacyInfo.name = pharmacyInfoResponse.name;
            pharmacyInfo.pharmacyId = pharmacyInfoResponse.id;
            pharmacyInfo.phoneNumber = pharmacyInfoResponse.phone_number;
            pharmacyInfo.zipCode = pharmacyInfoResponse.zip_code;
            // !!! to be populated by the prescription details page 
            //pharmacyInfo.prescriptionItem = this.prescriptionItem;
            API.Instance.pharmacyInfos.Add(pharmacyInfo);
        }

        static private bool processedPharamcyId(int pharmacyId, List<PharmacyInfo> pharmacyInfos)
        {
            foreach (PharmacyInfo pInfo in pharmacyInfos)
            {
                if (pInfo.pharmacyId == pharmacyId) return true;
            }
            return false;
        }

        static private async Task retrievePharmaciesByIdFail(API.ServerResponseErrorCode serverErrorCode)
        {
        }

        static private async Task retrievePrescriptionsByUserIdFailed(API.ServerResponseErrorCode serverErrorCode)
        {
            try
            {
                await LoadingPopup.Dismiss(HomePage.Instance.Navigation);
            }
            catch (Exception e)
            {
            }
            serverErrorCode.ToString();
        }

        private static void showPrescriptions(AbsoluteLayout layout, IEnumerable<PrescriptionItem> prescriptionItems)
        {
            // TODO get list of sent prescriptions here and only show the ones as follows
            // if "Choice" returns PrescriptionsDisplayed.New
            //   only show ones that have not been sent
            // if "Choice" returns PrescriptionsDisplayed.Sent
            //   only show ones that have been sent
            // if "Choice" returns PrescriptionsDisplayed.All
            //   show everything
            // TODO test code remove when done
            //            API.SuccessCallback<API.PrescriptionRecord[]>
            //            retreivedUserPrescriptionsCallback =
            //                delegate(API.PrescriptionRecord[] userPrescriptionsResponses) {
            //            };
            //            API.ErrorCallback retreivedUserPrescriptionsErrorCallback =
            //                delegate(API.ServerResponseErrorCode serverErrorCode )
            //                {
            //                    serverErrorCode.ToString();
            //                };
            if (layout.Children.Count > 0 && layout.Children[0] is ScrollView)
                layout.Children.RemoveAt(0);

            List<Prescription> prescriptionsList = new List<Prescription>();
            Prescription[] prescriptions = null;
            int numPrescriptionItems = 0;
            bool loadPendingPrescriptions = false;
            //            _instance.DisplayAlert("showPrescriptions", "here","cancel");
            //            List<PrescriptionItem> testList = new List<PrescriptionItem>(prescriptionItems);
            //            _instance.DisplayAlert("num of items:", testList.Count.ToString(), "cancel");
            foreach (PrescriptionItem pItem in prescriptionItems)
            {
                string itemStatus = nvl(pItem.status);
                if (!itemStatus.Equals(PrescriptionStatusCodes.FAX_SENT.ToString()) &&
                    !itemStatus.Equals(PrescriptionStatusCodes.FAX_FAILED.ToString()) &&
                    !itemStatus.Contains("COULD_NOT_SEND") && !itemStatus.Equals(""))
                {
                    loadPendingPrescriptions = true;
                }
                if (instance.Choice == PrescriptionsDisplayed.New)
                {
                    // if only display new prescription skip the ones that has been sent
                    if (itemStatus.Equals(((int)PrescriptionStatusCodes.FAX_SENT).ToString()))
                    {
                        //||
                        //itemStatus.Equals(((int)PrescriptionStatusCodes.FAX_QUEUED).ToString())) {
                        continue;
                    }
                }
                else if (instance.Choice == PrescriptionsDisplayed.Sent)
                {
                    // if only display new prescription skipp the ones that has NOT been sent
                    if (!itemStatus.Equals(((int)PrescriptionStatusCodes.FAX_SENT).ToString()))
                    {
                        //&&
                        //!itemStatus.Equals(((int)PrescriptionStatusCodes.FAX_QUEUED).ToString())) {
                        continue;
                    }
                }
                Prescription aPrescription = new Prescription(pItem);
                prescriptionsList.Add(aPrescription);
                numPrescriptionItems++;
            }
            if (loadPendingPrescriptions)
            {
                Instance.timedGetPrescriptions();
            }
            prescriptions = prescriptionsList.ToArray();
            //          Prescription[] prescriptions = Prescription.GetAllPrescriptions ();
            // Initialize the grid with two columns
            Grid grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                ColumnDefinitions =  {
                    new ColumnDefinition {
                        Width = new GridLength(App.ScreenWidth / 2.0, GridUnitType.Star)
                    },
                    new ColumnDefinition {
                        Width = new GridLength(App.ScreenWidth / 2.0, GridUnitType.Star)
                    }
                }
            };
            // Initialize grid rows
            int numberRows = (int)Math.Ceiling(numPrescriptionItems / 2.0);
            grid.RowDefinitions = new RowDefinitionCollection();
            for (int i = 0; i < numberRows; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(200, GridUnitType.Absolute)
                });
                grid.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(50, GridUnitType.Absolute)
                });
            }
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnTapGestureRecognizerTapped;
            // Add images to the grid
            int j = 0;
            for (int i = 0; i < numPrescriptionItems; i++)
            {
                // TODO : actually filter the new and sent prescriptions
                //                if (Choice == PrescriptionsDisplayed.New && i % 2 == 0)
                //                    continue;
                //
                //                else if (Choice == PrescriptionsDisplayed.Sent && i % 2 == 1)
                //                    continue;
                Prescription p = prescriptions[i];
                p.Source = p.PrescriptionThumbImageSource;
                //                _instance.DisplayAlert("item number " + i,p.Source.ToString(),"cancel");
                string itemStatus = nvl(p.prescriptionItem.status);
                p.VerticalOptions = LayoutOptions.Center;
                p.HorizontalOptions = LayoutOptions.Center;
                p.BorderRadius = Device.OnPlatform(16, 256, 0);
                p.HeightRequest = 150;
                p.TranslationY = 20;
                p.GestureRecognizers.Add(tapGestureRecognizer);
                grid.Children.Add(p, i % 2, j);
                if (itemStatus.Equals(((int)PrescriptionStatusCodes.FAX_QUEUED).ToString()))
                {
                    grid.Children.Add(new Image()
                    {
                        Source = Device.OnPlatform(
                            iOS: ImageSource.FromFile("Icons/sending.png"),
                            Android: ImageSource.FromFile("sending_mini.png"),
                            WinPhone: ImageSource.FromFile("")),
                        WidthRequest = 100
                    }, i % 2, j);
                }

                if (itemStatus.Equals(((int)PrescriptionStatusCodes.FAX_SENT).ToString()))
                {
                    grid.Children.Add(new Label()
                    {
                        Text = p.prescriptionItem.createdDt,
                        //Text = "Test Date",
                        //Text = p.prescriptionItem.photoTakenTime,
                        HorizontalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center
                    }, i % 2, (j + 1));
                }
                else
                {
                    if (itemStatus.Equals(((int)PrescriptionStatusCodes.FAX_FAILED).ToString()))
                    {
                        grid.Children.Add(new Image()
                        {
                            Source = Device.OnPlatform(
                                                   iOS: ImageSource.FromFile("Icons/failed.png"),
                                                   Android: ImageSource.FromFile("failed_mini.png"),
                                                   WinPhone: ImageSource.FromFile("")),
                            WidthRequest = 100
                        }, i % 2, j);
                        grid.Children.Add(new Label()
                        {
                            //Text = "Fax Failed", 
                            Text = p.prescriptionItem.photoTakenTime,
                            HorizontalOptions = LayoutOptions.Center,
                            HorizontalTextAlignment = TextAlignment.Center
                        }, i % 2, (j + 1));
                    }
                    else
                    {
                        grid.Children.Add(new Label()
                        {
                            //Text = p.prescriptionItem.createdDt, 
                            //Text = "Test Date", 
                            Text = p.prescriptionItem.photoTakenTime,
                            HorizontalOptions = LayoutOptions.Center,
                            HorizontalTextAlignment = TextAlignment.Center
                        }, i % 2, (j + 1));
                    }
                }
                if (i % 2 == 1)
                {
                    j = j + 2;
                }
            }
            // Accomodate iPhone status bar
            layout.Padding = new Thickness(10, Device.OnPlatform(0, 0, 0), 10, 0);
            ScrollView scroll = new ScrollView
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Content = grid,
            };
            AbsoluteLayout.SetLayoutFlags(scroll, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(scroll, new Rectangle(0, 0, 1, 1));
            layout.Children.Insert(0, scroll);
            instance.ForceLayout();
        }

        void OnClick(object sender, EventArgs e)
        {
            this.DisplayAlert("Search", "Not yet implemented", "OK");
        }

        static void OnTapGestureRecognizerTapped(object sender, EventArgs args)
        {
            Prescription p = sender as Prescription;

            if (p != null)
            {
                PrescriptionDetailPage pDetailPage = new PrescriptionDetailPage(p);
                instance.Navigation.PushAsync(pDetailPage);
            }
        }
    }
}