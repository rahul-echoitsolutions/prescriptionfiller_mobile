using System;
using System.IO;
using System.Threading.Tasks;
using Plugin.Media;
using Xamarin.Forms;
using PrescriptionFiller.Helpers;
using Xamarin.Essentials;

namespace PrescriptionFiller.Droid.Services
{
	public class DroidCameraService : ICameraService
	{
		#region Last Id Management		
		static DroidCameraService()
		{
			lastIdPath = Path.Combine(System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), "last_id");

			// Init the file to store the lastId
			if (File.Exists (lastIdPath) == false) 
				LastId = 0;

			// Else load the file content
			else
				lastId = int.Parse(File.ReadAllText(lastIdPath));
		}

		static string lastIdPath;

		private static int lastId = 0;

		static int LastId
		{
			get 
			{ 
				return lastId;
			}
			set
			{ 
				lastId = value;
				Save ();
			}
		}
			
		// Update the file that store the last id used
		static void Save()
		{
			File.WriteAllText(lastIdPath, lastId.ToString());
		}
		#endregion

        public async Task<Photo> TakePicture(System.Action<Photo> callback = null)
		{
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsCameraAvailable)
			{
				Console.WriteLine("No Camera", ":( No camera avaialble.", "OK");
				return null;
			}
			var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
			
			if (status != PermissionStatus.Granted)
			{
				status = await Permissions.RequestAsync<Permissions.Camera>();
			}
			var status1 = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
			if (status1 != PermissionStatus.Granted)
			{
				status = await Permissions.RequestAsync<Permissions.StorageRead>();
			}
			var status2 = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
			if (status2 != PermissionStatus.Granted)
			{
				status = await Permissions.RequestAsync<Permissions.StorageWrite>();
			}
			Console.WriteLine("CrossMedia.Current.IsPickPhotoSupported: ", CrossMedia.Current.IsPickPhotoSupported);
            Console.WriteLine("CrossMedia.Current.IsTakePhotoSupported: ", CrossMedia.Current.IsTakePhotoSupported);
            Console.WriteLine("CrossMedia.Current.IsTakeVideoSupported: ", CrossMedia.Current.IsTakeVideoSupported);
            Console.WriteLine("CrossMedia.Current.IsPickVideoSupported: ", CrossMedia.Current.IsPickVideoSupported);

            System.DateTime now = System.DateTime.Now;

            string year = now.Year.ToString();
            string month = now.Month.ToString();
            string day = now.Day.ToString();
            string hour = now.Hour.ToString();
            string minute = now.Minute.ToString();
            string second = now.Second.ToString();
            string millisec = now.Millisecond.ToString();
            string timeString = year + "_" + month + "_" + day + "_" + hour + "_" + minute + "_" + second + "_" + millisec;
            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Prescriptions",
                Name = "prescription_" + timeString + "_" + (++LastId) + ".jpg"
            });
            //            var file = await CrossMedia.Current.TakePhotoAsync();


            if (file == null)
				return null;
            Console.WriteLine("took photo before thumbnail for " + file.Path);
			ImageHandler.MakeThumbnail (file.Path, 400f);
            Console.WriteLine("!!!!! after making thumbnail for " + file.Path);
            Photo photo = new Photo { Picture = file, };

			if (callback != null)
                callback (photo);
			
            return photo;
		}

		public async Task<Photo> PickPicture ()
		{
			var file = await CrossMedia.Current.PickPhotoAsync();

			if (file == null)
				return null;
			else {
				return new Photo { Picture = file, };
			}
		}
	}
}

