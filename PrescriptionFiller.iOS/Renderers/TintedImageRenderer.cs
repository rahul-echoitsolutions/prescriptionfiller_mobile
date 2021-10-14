using System;
using System.Collections.Generic;
using System.Text;
using PrescriptionFiller.iOS;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;


[assembly: ExportRenderer(typeof(PrescriptionFiller.TintedImage), typeof(TintedImageRenderer))]
namespace PrescriptionFiller.iOS
{
    public class TintedImageRenderer : ImageRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Image> e)
        {
            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Control != null)
            {
                if (e.PropertyName == "TintColor")
                {
                    Control.TintColor = ((TintedImage)sender).TintColor.ToUIColor();
                    Control.Image = Control.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                }
            }
        }
    }
}