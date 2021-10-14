using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.Graphics;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PrescriptionFiller.Droid;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(PrescriptionFiller.TintedImage), typeof(TintedImageRenderer))]
namespace PrescriptionFiller.Droid
{
    public class TintedImageRenderer : ImageRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Image> e)
        {
            base.OnElementChanged(e);
            var image = (PrescriptionFiller.TintedImage)e.NewElement;

            if (Control != null)
            {
                Control.SetColorFilter(image.TintColor.ToAndroid());
            }
        }
    }
}