using System;
using Xamarin.Forms;
using PrescriptionFiller.Droid.Renderers;
using Xamarin.Forms.Platform.Android;
using PrescriptionFiller;

[assembly: ExportCell (typeof (CustomDatePicker), typeof (CustomDatePickerRenderer))]

namespace PrescriptionFiller.Droid.Renderers
{
    public class CustomDatePickerRenderer : DatePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.SetTextColor(((CustomDatePicker)e.NewElement).TextColor.ToAndroid());
            }
        }
    }
}

