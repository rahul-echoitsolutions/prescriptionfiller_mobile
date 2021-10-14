using System;
using System.IO;
using System.Threading.Tasks;
using Plugin.Media;
using Xamarin.Forms;
using PrescriptionFiller.Helpers;

namespace PrescriptionFiller.iOS.Services
{
	public class iOSCameraService : ICameraService
	{
		#region Last Id Management		
		static iOSCameraService()
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
			if (!CrossMedia.Current.IsCameraAvailable)
			{
				Console.WriteLine("No Camera", ":( No camera avaialble.", "OK");
				return null;
			}

            System.DateTime now = System.DateTime.Now;
            string year = now.Year.ToString();
            string month = now.Month.ToString();
            string day = now.Day.ToString();
            string hour = now.Hour.ToString();
            string minute = now.Minute.ToString();
            string second = now.Second.ToString();
            string millisec = now.Millisecond.ToString();
            string timeString = year + "_" + month + "_" + day + "_" + hour + "_" + minute + "_" + second + "_" + millisec;

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions {
                Directory = "Prescriptions",
                Name = "prescription_"+timeString+"_"+(++LastId)+".jpg"
            });

			if (file == null)
				return null;

            ImageHandler.MakeThumbnail (file.Path, 400f);
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

