using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PrescriptionFiller.Droid;
using PrescriptionFiller.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(ResizingAndroid))]
namespace PrescriptionFiller.Droid
{
    public class ResizingAndroid : IResizing
    {
        public static BitmapFactory.Options GetBitmapOptionsOfImageAsync(byte[] imageData)
        {
            BitmapFactory.Options options = new BitmapFactory.Options
            {
                InJustDecodeBounds = true
            };

            // The result will be null because InJustDecodeBounds == true.
            //Bitmap result=  BitmapFactory.DecodeResourceAsync(Resources, Resource.Drawable.samoyed, options);
            Bitmap result = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length, options);

            int imageHeight = options.OutHeight;
            int imageWidth = options.OutWidth;

            Console.WriteLine(string.Format("Original Size= {0}x{1}", imageWidth, imageHeight));

            return options;
        }
        public static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            float height = options.OutHeight;
            float width = options.OutWidth;
            double inSampleSize = 1D;

            if (height > reqHeight || width > reqWidth)
            {
                int halfHeight = (int)(height / 2);
                int halfWidth = (int)(width / 2);

                // Calculate a inSampleSize that is a power of 2 - the decoder will use a value that is a power of two anyway.
                while ((halfHeight / inSampleSize) > reqHeight && (halfWidth / inSampleSize) > reqWidth)
                {
                    inSampleSize *= 2;
                }
            }

            return (int)inSampleSize;
        }

        public byte[] ResizeImage(byte[] imageData, float width, float height)
        {
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);

            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, false);

            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                return ms.ToArray();
            }
        }

        public byte[] ResizeImage(byte[] imageData, float sizeDesired)
        {
            BitmapFactory.Options options = GetBitmapOptionsOfImageAsync(imageData);

            // Calculate inSampleSize
            int width = 0, height = 0;

            if (options.OutWidth > options.OutHeight)
            {
                height = (int)(options.OutHeight * sizeDesired / options.OutWidth);
                width = (int)sizeDesired;
            }
            else
            {
                width = (int)(options.OutWidth * sizeDesired / options.OutHeight);
                height = (int)sizeDesired;
            }

            options.InSampleSize = CalculateInSampleSize(options, width, height);

            // Decode bitmap with inSampleSize set
            options.InJustDecodeBounds = false;

            //return BitmapFactory.DecodeResourceAsync(res, Resource.Drawable.samoyed, options);
            Bitmap resizedImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length, options);
            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                return ms.ToArray();
            }
        }
    }
}