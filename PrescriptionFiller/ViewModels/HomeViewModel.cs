using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Xamarin.Forms;
using API = PrescriptionFiller.Services.AccountAPIServer;
using P = System.IO.Path;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions.Abstractions;
using PrescriptionFiller.Views;
using PrescriptionFiller.Database;

namespace PrescriptionFiller.ViewModels
{
    public class HomeViewModel : ViewModelBase
	{
		private readonly INavigation navigation;
		public ICommand TakePhotoCommand { get; set; }
		public ICommand GoToGalleryCommand { get; set; }
		public ICommand GoToPharmacyListCommand { get; set; }

        public ICommand NewPrescriptionsCommand { get; set; }
        public ICommand SentPrescriptionsCommand { get; set; }
        public ICommand AllPrescriptionsCommand { get; set; }

        public ICommand getLocationCommand { get; set;  }

        //public string test_location { 
        //    get { return _test_location; } 
        //    set {
        //        _test_location = value;
        //        OnPropertyChanged("test_location");
        //    } 
        //}

        //public string _test_location;

        private Color _newPrescriptionsColor;

        public Color NewPrescriptionsColor
        {
            get { return _newPrescriptionsColor; }
            set
            {
                _newPrescriptionsColor = value;
                OnPropertyChanged("NewPrescriptionsColor");
            }
        }

        private Color _sentPrescriptionsColor;

        public Color SentPrescriptionsColor
        {
            get { return _sentPrescriptionsColor; }
            set
            {
                _sentPrescriptionsColor = value;
                OnPropertyChanged("SentPrescriptionsColor");
            }
        }

        private Color _allPrescriptionsColor;

        public Color AllPrescriptionsColor
        {
            get { return _allPrescriptionsColor; }
            set
            {
                _allPrescriptionsColor = value;
                OnPropertyChanged("AllPrescriptionsColor");
            }
        }

        public static String getTimestring() {
            System.DateTime now = System.DateTime.Now;
            string year = now.Year.ToString();
            string month = now.Month.ToString();
            string day = now.Day.ToString();
            string hour = now.Hour.ToString();
            string minute = now.Minute.ToString();
            string second = now.Second.ToString();
            string millisec = now.Millisecond.ToString();
            // yyyy-MM-dd HH:mm:ss
            string timeString = year + "-" + month + "-" + day + "\n" + hour + ":" + minute + ":" + second;
            //string timeString = hour + ":" + minute + ":" + second;
            return timeString;
        }

        public HomeViewModel(INavigation _navigation, HomePage page)
        {
            //var hasPermission = await Utils.CheckPermissions(Permission.Location);
            //if (!hasPermission)
              //  return;
            //test_location = "location init";
            //getLocationCommand = new Command(async () =>
            //{
            //    var hasPermission = await GeoLocationUtil.CheckPermissions(Permission.Location);
            //    if (!hasPermission) {
            //        return;
            //    }
            //    var locator = CrossGeolocator.Current;
            //    locator.DesiredAccuracy = 50;

            //    if (locator == null)
            //    {
            //        test_location = "locator is null!";
            //    }
            //    else
            //    {
            //        test_location = "task starting";
            //        Position position = await locator.GetPositionAsync(TimeSpan.FromSeconds(5));
            //        test_location = "Longitude: " + position.Longitude.ToString()
            //                                                  + position.Latitude.ToString();
            //    }

            //});
  
			navigation = _navigation;
			TakePhotoCommand = new Command(() => {
				ICameraService cameraService = DependencyService.Get<ICameraService>();
                Task<Photo> takePictureTask = cameraService.TakePicture(delegate(Photo photo) {
                    //page.DisplayAlert("saving photo", "here 1", "cancel");
                    int userId = API.Instance.Account.user_info.id;
                    PrescriptionItem prescriptionItem = new PrescriptionItem();
                    prescriptionItem.path = photo.Picture.Path;
                    //page.DisplayAlert("saving photo", "here 2", "cancel");
                    string thumbPath = P.Combine (P.GetDirectoryName (photo.Picture.Path), P.GetFileNameWithoutExtension (photo.Picture.Path)) + "_thumb.jpg";
                    prescriptionItem.thumbPath = thumbPath;
                    prescriptionItem.photoTakenTime = HomeViewModel.getTimestring();
                    prescriptionItem.userId = userId;
                    //page.DisplayAlert("saving photo","here 3 prescriptionItem.thumbPath: " + prescriptionItem.thumbPath,"cancel");
                    //page.DisplayAlert("saving photo", "here 3 prescriptionItem.path: " + prescriptionItem.path, "cancel");
                    LocalPrescriptionDatabase.Instance.SaveItem(prescriptionItem);
                    page.Refresh();
                    Console.WriteLine("saved photo at: " + photo.Picture.Path);
//                    IEnumerable<PrescriptionItem> prescriptionItems = LocalPrescriptionDatabase.Instance.GetItems(userId);
//                    foreach (PrescriptionItem pItem in prescriptionItems) {
//                        Console.WriteLine("pItem.userId " + pItem.userId);
//                        Console.WriteLine("pItem.path " + pItem.path);
//                        Console.WriteLine("pItem.thumbPath " + pItem.thumbPath);
//                    }
				});
			});

			GoToGalleryCommand = new Command(() => {
				ICameraService cameraService = DependencyService.Get<ICameraService>();
				cameraService.PickPicture();
			});

			GoToPharmacyListCommand = new Command(() => {
				navigation.PushAsync(new PharmacyListPage());
			});

            NewPrescriptionsCommand = new Command(() => {
                Device.BeginInvokeOnMainThread(() => {
                    page.Choice = HomePage.PrescriptionsDisplayed.New;
                    NewPrescriptionsColor = Color.White;
                    AllPrescriptionsColor = SentPrescriptionsColor = Color.FromHex("#AAAAAAAA");
                });
            });

            SentPrescriptionsCommand = new Command(() => {
                Device.BeginInvokeOnMainThread(() => {
                    page.Choice = HomePage.PrescriptionsDisplayed.Sent;
                    SentPrescriptionsColor = Color.White;
                    AllPrescriptionsColor = NewPrescriptionsColor = Color.FromHex("#AAAAAAAA");
                });
            });

            AllPrescriptionsCommand = new Command(() => {
                Device.BeginInvokeOnMainThread(() => {
                    page.Choice = HomePage.PrescriptionsDisplayed.All;
                    AllPrescriptionsColor = Color.White;
                    NewPrescriptionsColor = SentPrescriptionsColor = Color.FromHex("#AAAAAAAA");
                });
            });

            //test_location = "location init 2";
            NewPrescriptionsColor = Color.White;
            AllPrescriptionsColor = SentPrescriptionsColor = Color.FromHex("#AAAAAAAA");
		}
    }
}


