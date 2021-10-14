using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrescriptionFiller.ViewModels;
using API = PrescriptionFiller.Services.AccountAPIServer;
using Plugin.Permissions.Abstractions;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PrescriptionFiller.Views.Popups;

namespace PrescriptionFiller.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PharmacyListPage : ContentPage
    {
        public static INavigation classNav;
        public static PharmacyListPage instance;
        private ListView listView;
        private Button sendPrescriptionButton;
        private Button searchPharamciesButton;
        private Button searchUsingMyLocationButton;
        private PharmacyInfo selectedPharmacyInfo;
        private PharmacyListViewModel pharmacyListViewModel;
        public PharmacyListPage(PrescriptionItem prescriptionItem)
        {
            InitializeComponent();
            classNav = this.Navigation;
            instance = this;
            NavigationPage.SetHasNavigationBar(this, true);
            initView(new List<PharmacyInfo>());
            pharmacyListViewModel = new PharmacyListViewModel(this.Navigation, prescriptionItem);
            this.BindingContext = pharmacyListViewModel;
        }
        public PharmacyListPage()
        {
            InitializeComponent();
            classNav = this.Navigation;
            pharmacyListViewModel = new PharmacyListViewModel(this.Navigation, null);
            this.BindingContext = pharmacyListViewModel;
            NavigationPage.SetHasNavigationBar(this, true);
        }

        public void showPharmacyInfos(List<PharmacyInfo> pharmacyInfos)
        {
            listView.ItemsSource = pharmacyInfos;
        }

        public void initView(List<PharmacyInfo> pharmacyInfos)
        {
            StackLayout layout = (Content as StackLayout);
            StackLayout searchButtonsLayout = new StackLayout { Orientation = StackOrientation.Horizontal };
            int searchCityViewIndex = 0;
            int searchPharmNameViewIndex = 1;
            int searchButtonViewIndex = 2;
            int listScrollViewIndex = 3;
            int sendPrescriptionIndex = 4;
            List<View> views = new List<View>();
            views.AddRange(layout.Children);
            foreach (View v in views)
            {
                layout.Children.Remove(v);
            }
            //            if (layout.Children.Count > listScrollViewIndex && layout.Children [listScrollViewIndex] is ScrollView)
            //                layout.Children.RemoveAt (listScrollViewIndex);
            layout.Spacing = 10;

            Entry citySearchText = new Entry
            {
                Text = "",
                Placeholder = "City",
            };
            citySearchText.SetBinding(Entry.TextProperty, "city");
            layout.Children.Insert(searchCityViewIndex, citySearchText);

            Entry pharmacyNameSearchText = new Entry
            {
                Text = "",
                Placeholder = "Pharmacy Name",
            };
            pharmacyNameSearchText.SetBinding(Entry.TextProperty, "pharmacyName");
            layout.Children.Insert(searchPharmNameViewIndex, pharmacyNameSearchText);

            searchPharamciesButton = new Button()
            {
                Text = "Search",
                IsEnabled = true,
                //BackgroundColor = Color.FromHex("#BEBEBF"),
                BackgroundColor = Color.FromHex("#FF0000"),
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.Start,
                WidthRequest = 100
            };
            // <Button BackgroundColor="#FF0000" Text="Send Prescription" TextColor="White" Clicked="Send" BorderRadius="0"/>
            searchPharamciesButton.Clicked += (sender, e) =>
            {
                string citySearchName = pharmacyListViewModel.city;
                string pharmacySearchName = pharmacyListViewModel.pharmacyName;
                pharmacyListViewModel.SearchByCityAndName();
            };
            // add the first search button to the first element of the buttons layout
            searchButtonsLayout.Children.Insert(0, searchPharamciesButton);

            searchUsingMyLocationButton = new Button()
            {
                Text = "Use My Location",
                IsEnabled = true,
                //BackgroundColor = Color.FromHex("#BEBEBF"),
                BackgroundColor = Color.FromHex("#FF0000"),
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.Start,

                WidthRequest = 150
            };
            searchUsingMyLocationButton.Clicked += async (sender, e) =>
            {
                var hasPermission = await GeoLocationUtil.CheckPermissions(Permission.Location);
                string longitude = null;
                string latitude = null;
                if (!hasPermission)
                {
                    return;
                }
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;

                if (locator == null)
                {
                    return; //pharmacyListViewModel.currlocationStr = "locator is null!";
                }
                else
                {
                    //pharmacyListViewModel.currlocationStr = "task starting";
                    Position position = await locator.GetPositionAsync(TimeSpan.FromSeconds(5));
                    if (position == null)
                    {
                        return;
                    }
                    longitude = position.Longitude.ToString();
                    latitude = position.Latitude.ToString();
                    //pharmacyListViewModel.currlocationStr = position.Longitude.ToString()
                    //+ ", " + position.Latitude.ToString();
                }
                string citySearchName = pharmacyListViewModel.city;
                string pharmacySearchName = pharmacyListViewModel.pharmacyName;
                pharmacyListViewModel.longitude = longitude;
                pharmacyListViewModel.latitude = latitude;
                pharmacyListViewModel.SearchByLocation();
            };
            // add the second search button to the second element of the buttons layout
            searchButtonsLayout.Children.Insert(1, searchUsingMyLocationButton);

            Label locationLbl = new Label();
            locationLbl.SetBinding(Label.TextProperty, "currlocationStr");
            searchButtonsLayout.Children.Insert(2, locationLbl);

            // add the layout with the search buttons to the outermost layout of this page
            layout.Children.Insert(searchButtonViewIndex, searchButtonsLayout);

            // Initialize grid rows
            PharmacyInfo[] pharmacyInfosArr = pharmacyInfos.ToArray();
            int numberPharmacies = pharmacyInfosArr.Length;
            // important to set HasUnevenRows! Took me more than 6 hours to find out the answer for row height not updating problem!
            listView = new ListView()
            {
                ItemTemplate = new DataTemplate(typeof(PharmacyCell)),
                ItemsSource = pharmacyInfos,
                HasUnevenRows = true
            };
            listView.ItemSelected += (sender, e) =>
            {
                if (e.SelectedItem == null) return;
                selectedPharmacyInfo = (e.SelectedItem as PharmacyInfo);
                sendPrescriptionButton.IsEnabled = true;
            };

            // Accomodate iPhone status bar
            layout.Padding = new Thickness(10, Device.OnPlatform(0, 0, 0), 10, 0);

            ScrollView scroll = new ScrollView
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Content = listView
                //Content = grid,
            };
            AbsoluteLayout.SetLayoutFlags(scroll, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(scroll, new Rectangle(0, 0, 1, 1));

            layout.Children.Insert(listScrollViewIndex, scroll);

            StackLayout buttonLayoutContainer = new StackLayout();
            sendPrescriptionButton = new Button()
            {
                Text = "Send to Pharmacy",
                IsEnabled = false,
                BackgroundColor = Color.FromHex("#FF0000"),
                TextColor = Color.White
            };
            // <Button BackgroundColor="#E53935" Text="Send Prescription" TextColor="White" Clicked="Send" BorderRadius="0"/>
            sendPrescriptionButton.Clicked += (sender, e) =>
            {
                sendPrescription();
            };

            buttonLayoutContainer.Children.Insert(0, sendPrescriptionButton);
            layout.Children.Insert(sendPrescriptionIndex, buttonLayoutContainer);
            this.ForceLayout();
        }

        private async void sendPrescription()
        {
            if (API.Instance.Account != null && API.Instance.Account.user_info != null)
            {
                int userId = API.Instance.Account.user_info.id;
                int pharmacyId = selectedPharmacyInfo.pharmacyId;
                PrescriptionItem prescriptionItem = selectedPharmacyInfo.prescriptionItem;
                string description = prescriptionItem.description;
                string medicalNotes = prescriptionItem.medicalNotes;
                string extendedHealth = prescriptionItem.extendedHealth;
                string thumbnailImagePath = prescriptionItem.thumbPath;
                string imagePath = prescriptionItem.path;
                description = (description == null) ? "" : description;
                Console.WriteLine("Chosen " + selectedPharmacyInfo.pharmacyId);
                await API.Instance.CreatePrescription(this.Navigation, userId, pharmacyId,
                    description, medicalNotes, extendedHealth, thumbnailImagePath, imagePath,
                        successCallback, errorCallback);
            }
        }

        static private async Task successCallback(API.ServerResponse<API.PrescriptionRecord> response)
        {
            Console.WriteLine("created perscription server assigned prescription with id " + response.data.id);
            await LoadingPopup.Dismiss(PharmacyListPage.instance.Navigation);
            await classNav.PopToRootAsync();
            new System.Threading.Thread(new System.Threading.ThreadStart(() => {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                    HomePage.Instance.Refresh(); // this works!
                });
            })).Start();
        }

        static private async Task errorCallback(API.ServerResponseErrorCode errorCode)
        {
            if (errorCode == API.ServerResponseErrorCode.ERROR_CONNECTION_TIMEOUT)
                await MessagePopup.Show(PharmacyListPage.classNav, MessagePopup.Type.Error, MessagePopup.Icon.Connection, "Oops!", "Server is unreachable");

            else if (errorCode == API.ServerResponseErrorCode.ERROR_MISSING_REQUIRED_DATA)
                await MessagePopup.Show(PharmacyListPage.classNav, MessagePopup.Type.Warning, MessagePopup.Icon.User, "Oops!", "Missing required Data");

            else
                await MessagePopup.Show(PharmacyListPage.classNav, MessagePopup.Type.Error, MessagePopup.Icon.User, "Sorry", "An unknown error has appeared");
        }

        public class PharmacyCell : ViewCell
        {
            public PharmacyCell()
            {
                Label nameLabel = new Label()
                {
                    Text = "Name:"
                };
                Label nameTextLabel = new Label();
                nameTextLabel.SetBinding(Label.TextProperty, "name");
                Label addressLabel = new Label()
                {
                    Text = "Address:"
                };
                Label addressTextLabel = new Label();
                addressTextLabel.SetBinding(Label.TextProperty, "address");
                Label faxLabel = new Label()
                {
                    Text = "Fax:"
                };
                Label faxTextLabel = new Label();
                faxTextLabel.SetBinding(Label.TextProperty, "faxNumber");
                StackLayout testStackLayout = new StackLayout
                {
                    Spacing = 0,
                    Padding = new Thickness(10, 5, 10, 5),
                    Orientation = StackOrientation.Vertical,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Children = {nameLabel, nameTextLabel, addressLabel, addressTextLabel, faxLabel,
                        faxTextLabel }
                };
                this.View = testStackLayout;

                //MenuItem sendAction = new MenuItem { Text = "Send" };
                //sendAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
                //sendAction.Clicked += PharmacyListPage.instance.send;
                //this.ContextActions.Add(sendAction);
            }
        }
    }
}