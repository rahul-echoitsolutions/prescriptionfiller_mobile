using System;
using Xamarin.Forms;

namespace PrescriptionFiller
{
    public class TintedImage : Image
    {
        public static readonly BindableProperty TintColorProperty = BindableProperty.Create<TintedImage, Color>(p => p.TintColor, new Color(0));

        public Color TintColor
        {
            get { return (Color)GetValue (TintColorProperty); }
            set { SetValue (TintColorProperty, value); }
        }
    }
}

