using System;
using Xamarin.Forms.Platform.Android;
using Android.OS;
using Xamarin.Forms;
using PrescriptionFiller;

[assembly: ExportRenderer(typeof(CarouselPage), typeof(CustomCarouselPageRenderer))]
namespace PrescriptionFiller
{
    public class CustomCarouselPageRenderer : CarouselPageRenderer 
    {   
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.CarouselPage> e)
        {
            base.OnElementChanged(e);


        }

        public override bool OnTouchEvent(Android.Views.MotionEvent e)
        {
            return false;
        }
    }
}

