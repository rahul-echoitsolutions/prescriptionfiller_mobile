using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using UIKit;
using PrescriptionFiller.iOS.Renderers;
using PrescriptionFiller;

[assembly: ExportCell (typeof (CustomDatePicker), typeof (CustomDatePickerRenderer))]

namespace PrescriptionFiller.iOS.Renderers
{
    public class CustomDatePickerRenderer : DatePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                DatePicker dpicker = (e.NewElement == null) ? e.OldElement : e.NewElement;
                Control.TextColor = (dpicker as CustomDatePicker).TextColor.ToUIColor();
                Control.BorderStyle = UITextBorderStyle.None;
            }
        }
    }
}

