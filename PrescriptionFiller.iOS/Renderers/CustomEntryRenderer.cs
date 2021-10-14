using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using UIKit;
using PrescriptionFiller.iOS.Renderers;

[assembly: ExportCell (typeof (Entry), typeof (CustomEntryRenderer))]

namespace PrescriptionFiller.iOS.Renderers
{
    public class CustomEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Entry> e)
        {
            base.OnElementChanged(e);

            if (this.Control == null) return;

            Control.BorderStyle = UITextBorderStyle.None;

        }
    }
}
    

