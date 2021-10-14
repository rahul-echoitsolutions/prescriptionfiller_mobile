using System;
using Xamarin.Forms;

namespace PrescriptionFiller
{

    // https://gist.github.com/davidtavarez/e3580c98357edd89de6f
    // davidtavarez/LineEntry.cs in gist.github.com
    // another example at https://kampeki-factory.blogspot.com/2017/06/xamarin-forms-custom-bottom-bordered.html

    public class CustomLineEntry : Entry
    {
        public static readonly BindableProperty BorderColorProperty =
            BindableProperty.Create<CustomLineEntry, Color>(p => p.BorderColor, Color.Black);

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create<CustomLineEntry, double>(p => p.FontSize, Font.Default.FontSize);

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static readonly BindableProperty PlaceholderColorProperty =
            BindableProperty.Create<CustomLineEntry, Color>(p => p.PlaceholderColor, Color.Default);

        public Color PlaceholderColor
        {
            get { return (Color)GetValue(PlaceholderColorProperty); }
            set { SetValue(PlaceholderColorProperty, value); }
        }

    }
}
