using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PrescriptionFiller.Renderers;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using PrescriptionFiller;
// https://forums.xamarin.com/discussion/23562/change-the-text-color-for-xamarin-forms-datepicker

[assembly: ExportRenderer(typeof(CustomDatePicker), typeof(CustomDatePickerRender))]
namespace PrescriptionFiller.Renderers
{
    class CustomDatePickerRender : DatePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.DatePicker> e)
        {
            base.OnElementChanged(e);

            CustomDatePicker datePicker = (CustomDatePicker)Element;

            if (datePicker != null)
            {
                SetTextColor(datePicker);
            }

            if (e.OldElement == null)
            {
                //Wire events
            }

            if (e.NewElement == null)
            {
                //Unwire events
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Control == null)
            {
                return;
            }

            CustomDatePicker datePicker = (CustomDatePicker)Element;

            if (e.PropertyName == CustomDatePicker.TextColorProperty.PropertyName)
            {
                this.Control.SetTextColor(datePicker.TextColor.ToAndroid());
            }
        }

        void SetTextColor(CustomDatePicker datePicker)
        {
            this.Control.SetTextColor(datePicker.TextColor.ToAndroid());
        }
    }
}