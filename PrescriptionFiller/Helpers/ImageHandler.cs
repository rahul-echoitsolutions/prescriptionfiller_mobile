using PrescriptionFiller.Interfaces;
using System;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;
using P = System.IO.Path;

//this is no longer a way to call native
//we use DependencyService now
//#if __IOS__
//using System.Drawing;
//using UIKit;
//using CoreGraphics;
//#endif

//#if __ANDROID__
//using Android.Graphics;
//#endif

namespace PrescriptionFiller.Helpers
{
	public static class ImageHandler
	{
		static ImageHandler()
		{
		}

		public static byte[] ResizeImage (byte[] imageData, float width, float height)
		{
			return DependencyService.Get<IResizing>().ResizeImage(imageData, width, height);
		}

        static byte[] ReadAllFile(string filename)
        {
            byte[] buff = null;
            FileStream fs = null;
            try {
                fs = File.OpenRead(filename);
                Console.WriteLine("fs.Length: " + fs.Length);
                buff = new byte[fs.Length];
                fs.Read(buff, 0, Convert.ToInt32(fs.Length));
            }
            finally {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
            return buff;
        }

        public static string ImageToBase64(string imagepath) {
            byte[] imageData = ReadAllFile(imagepath);
            return System.Convert.ToBase64String(imageData);
        }

        public static void MakeThumbnail(string originalPath, float sizeDesired = 256f)
		{
			//byte[] imageData = File.ReadAllBytes(originalPath);
            byte[] imageData = ReadAllFile(originalPath);
            int tries = 1;
            // TODO fix if there is a better way. a hack... sometimes it is empty...
            while (imageData.Length == 0 && tries < 13)
            {
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine("MakeThumbnail ReadAllBytes: retry " + tries++);
                imageData = ReadAllFile(originalPath);
            }


			byte[] resizedImage = DependencyService.Get<IResizing>().ResizeImage(imageData, sizeDesired);
			

			string thumbPath = P.Combine (P.GetDirectoryName (originalPath), P.GetFileNameWithoutExtension (originalPath)) + "_thumb.jpg";

			File.WriteAllBytes(thumbPath, resizedImage);
		}

		
		//public static byte[] ResizeImageIOS (byte[] imageData, float sizeDesired)
		//{
		//	UIImage originalImage = ImageFromByteArray (imageData);

		//	int width = 0, height = 0;

		//	if (originalImage.Size.Width > originalImage.Size.Height) 
		//	{
		//		height = (int)(originalImage.Size.Height * sizeDesired / originalImage.Size.Width);
		//		width = (int)sizeDesired;
		//	} 
		//	else 
		//	{
		//		width = (int)(originalImage.Size.Width * sizeDesired / originalImage.Size.Height);
		//		height = (int)sizeDesired;
		//	}

		//	Console.WriteLine("@@@@@@@@@@@@#################" + originalImage.Size.Height + " " + originalImage.Size.Width);
		//	Console.WriteLine("@@@@@@@@@@@@#################" + height + " " + width);

		//	//create a 24bit RGB image
		//	using (CGBitmapContext context = new CGBitmapContext (IntPtr.Zero,
		//	(int)width, (int)height, 8,
		//	(int)(4 * width), CGColorSpace.CreateDeviceRGB (),
		//	CGImageAlphaInfo.PremultipliedLast)) {

		//		RectangleF imageRect = new RectangleF(0, 0, width, height);

		//		if (originalImage.Orientation == UIImageOrientation.Left) 
		//		{
		//			context.RotateCTM((nfloat)(Math.PI / 2f)); // 90 deg
		//			context.TranslateCTM(0, -height);

		//		} 
		//		else if (originalImage.Orientation == UIImageOrientation.Right) 
		//		{
		//			context.RotateCTM((nfloat)(-Math.PI / 2f)); // -90 deg
		//			context.TranslateCTM(-width, 0);

		//		} 
		//		else if (originalImage.Orientation == UIImageOrientation.Up) 
		//		{
		//			// NOTHING
		//		} 
		//		else if (originalImage.Orientation == UIImageOrientation.Down) 
		//		{
		//			context.RotateCTM((nfloat)(-Math.PI)); // -180 deg
		//			context.TranslateCTM(width, height);

		//		}

		//		// draw the image
		//		context.DrawImage (imageRect, originalImage.CGImage);

		//		UIKit.UIImage resizedImage = UIKit.UIImage.FromImage (context.ToImage ());

		//		// save the image as a jpeg
		//		return resizedImage.AsJPEG ().ToArray ();
		//	}
		//}

		//public static byte[] ResizeImageIOS (byte[] imageData, float width, float height)
		//{
		//	UIImage originalImage = ImageFromByteArray (imageData);

		//	//create a 24bit RGB image
		//	using (CGBitmapContext context = new CGBitmapContext (IntPtr.Zero,
		//	(int)width, (int)height, 8,
		//	(int)(4 * width), CGColorSpace.CreateDeviceRGB (),
		//	CGImageAlphaInfo.PremultipliedFirst)) {

		//		RectangleF imageRect = new RectangleF (0, 0, width, height);

		//		// draw the image
		//		context.DrawImage (imageRect, originalImage.CGImage);

		//		UIKit.UIImage resizedImage = UIKit.UIImage.FromImage (context.ToImage ());

		//		// save the image as a jpeg
		//		return resizedImage.AsJPEG ().ToArray ();
		//	}
		//}

		//public static UIKit.UIImage ImageFromByteArray(byte[] data)
		//{
		//	if (data == null) {
		//		return null;
		//	}

		//	UIKit.UIImage image;
		//	try {
		//		image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
		//	} catch (Exception e) {
		//		Console.WriteLine ("Image load failed: " + e.Message);
		//		return null;
		//	}
		//	return image;
		//}
		

		
        //public static BitmapFactory.Options GetBitmapOptionsOfImageAsync(byte[] imageData)
        //{
        //    BitmapFactory.Options options = new BitmapFactory.Options
        //        {
        //            InJustDecodeBounds = true
        //        };
            
        //    // The result will be null because InJustDecodeBounds == true.
        //    //Bitmap result=  BitmapFactory.DecodeResourceAsync(Resources, Resource.Drawable.samoyed, options);
        //    Bitmap result = BitmapFactory.DecodeByteArray (imageData, 0, imageData.Length, options);

        //    int imageHeight = options.OutHeight;
        //    int imageWidth = options.OutWidth;

        //    Console.WriteLine(string.Format("Original Size= {0}x{1}", imageWidth, imageHeight));

        //    return options;
        //}
        //public static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        //{
        //    // Raw height and width of image
        //    float height = options.OutHeight;
        //    float width = options.OutWidth;
        //    double inSampleSize = 1D;

        //    if (height > reqHeight || width > reqWidth)
        //    {
        //        int halfHeight = (int)(height / 2);
        //        int halfWidth = (int)(width / 2);

        //        // Calculate a inSampleSize that is a power of 2 - the decoder will use a value that is a power of two anyway.
        //        while ((halfHeight / inSampleSize) > reqHeight && (halfWidth / inSampleSize) > reqWidth)
        //        {
        //            inSampleSize *= 2;
        //        }
        //    }

        //    return (int)inSampleSize;
        //}
        //public static byte[] ResizeImageAndroid (byte[] imageData, float sizeDesired)
        //{
        //    BitmapFactory.Options options = GetBitmapOptionsOfImageAsync(imageData);

        //    // Calculate inSampleSize
        //    int width = 0, height = 0;

        //    if (options.OutWidth > options.OutHeight) 
        //    {
        //        height = (int)(options.OutHeight * sizeDesired / options.OutWidth);
        //        width = (int)sizeDesired;
        //    } 
        //    else 
        //    {
        //        width = (int)(options.OutWidth * sizeDesired / options.OutHeight);
        //        height = (int)sizeDesired;
        //    }

        //    options.InSampleSize = CalculateInSampleSize(options, width, height);

        //    // Decode bitmap with inSampleSize set
        //    options.InJustDecodeBounds = false;

        //    //return BitmapFactory.DecodeResourceAsync(res, Resource.Drawable.samoyed, options);
        //    Bitmap resizedImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length, options);
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        resizedImage.Compress (Bitmap.CompressFormat.Jpeg, 100, ms);
        //        return ms.ToArray ();
        //    }
        //}
		//public static byte[] _origResizeImageAndroid (byte[] imageData, float sizeDesired)
		//{
		//	// Load the bitmap
		//	Bitmap originalImage = BitmapFactory.DecodeByteArray (imageData, 0, imageData.Length);

		//	int width = 0, height = 0;

		//	if (originalImage.Width > originalImage.Height) 
		//	{
		//		height = (int)(originalImage.Height * sizeDesired / originalImage.Width);
		//		width = (int)sizeDesired;
		//	} 
		//	else 
		//	{
		//		width = (int)(originalImage.Width * sizeDesired / originalImage.Height);
		//		height = (int)sizeDesired;
		//	}
				
		//	Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, true);

		//	using (MemoryStream ms = new MemoryStream())
		//	{
		//		resizedImage.Compress (Bitmap.CompressFormat.Jpeg, 100, ms);
		//		return ms.ToArray ();
		//	}
		//}

		//public static byte[] ResizeImageAndroid (byte[] imageData, float width, float height)
		//{
		//	// Load the bitmap
		//	Bitmap originalImage = BitmapFactory.DecodeByteArray (imageData, 0, imageData.Length);
			
		//	Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, false);

		//	using (MemoryStream ms = new MemoryStream())
		//	{
		//		resizedImage.Compress (Bitmap.CompressFormat.Jpeg, 100, ms);
		//		return ms.ToArray ();
		//	}
		//}
		
	}
}