using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using UIKit;
using PrescriptionFiller;
using PrescriptionFiller.iOS.Renderers;
using System.ComponentModel;
using CoreAnimation;
using Foundation;
[assembly: ExportCell(typeof(CustomLineEntry), typeof(CustomLineEntryRenderer))]
namespace PrescriptionFiller.iOS.Renderers
{
    public class CustomLineEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);


            // Need to connect to Sizechanged event because first render time, Entry has no size (-1).
            if (e.NewElement != null)
                e.NewElement.SizeChanged += (obj, args) =>
                {
                    var xamEl = obj as Entry;
                    if (xamEl == null)
                        return;

                    // get native control (UITextField)
                    var entry = this.Control;

                    // Create borders (bottom only)
                    CALayer border = new CALayer();
                    float width = 1.0f;
                    border.BorderColor = new CoreGraphics.CGColor(0.73f, 0.7451f, 0.7647f);  // gray border color
                    border.Frame = new CoreGraphics.CGRect(x: 0, y: xamEl.Height - width, width: xamEl.Width, height: 1.0f);
                    border.BorderWidth = width;

                    entry.Layer.AddSublayer(border);

                    entry.Layer.MasksToBounds = true;
                    entry.BorderStyle = UITextBorderStyle.None;
                    entry.BackgroundColor = new UIColor(1, 1, 1, 1); // white
                };
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var xamEl = sender as Entry;
            if (xamEl == null)
                return;

            // get native control (UITextField)
            var entry = this.Control;

            // Create borders (bottom only)
            CALayer border = new CALayer();
            float width = 1.0f;
            border.BorderColor = new CoreGraphics.CGColor(0.73f, 0.7451f, 0.7647f);  // gray border color
            border.Frame = new CoreGraphics.CGRect(x: 0, y: xamEl.Height - width, width: xamEl.Width, height: 1.0f);
            border.BorderWidth = width;

            entry.Layer.AddSublayer(border);

            entry.Layer.MasksToBounds = true;
            entry.BorderStyle = UITextBorderStyle.None;
            entry.BackgroundColor = new UIColor(1, 1, 1, 1); // white

        }
      //protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        //{
        //    base.OnElementChanged(e);

        //    if (Control != null)
        //    {
        //        Control.BorderStyle = UITextBorderStyle.None;

        //        var view = (Element as CustomLineEntry);
        //        if (view != null)
        //        {
        //            DrawBorder(view);
        //            SetFontSize(view);
        //            SetPlaceholderTextColor(view);
        //        }
        //    }
        //}

        //protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    base.OnElementPropertyChanged(sender, e);

        //    var view = (CustomLineEntry)Element;

        //    if (e.PropertyName.Equals(view.BorderColor))
        //        DrawBorder(view);
        //    if (e.PropertyName.Equals(view.FontSize))
        //        SetFontSize(view);
        //    if (e.PropertyName.Equals(view.PlaceholderColor))
        //        SetPlaceholderTextColor(view);
        //}

        //void DrawBorder(CustomLineEntry view)
        //{
        //    var borderLayer = new CALayer();
        //    borderLayer.MasksToBounds = true;
        //    borderLayer.Frame = new CoreGraphics.CGRect(0f, Frame.Height / 2, Frame.Width, 1f);
        //    borderLayer.BorderColor = view.BorderColor.ToCGColor();
        //    borderLayer.BorderWidth = 1.0f;

        //    Control.Layer.AddSublayer(borderLayer);
        //    Control.BorderStyle = UITextBorderStyle.None;
        //}

        //void SetFontSize(CustomLineEntry view)
        //{
        //    if (view.FontSize != Font.Default.FontSize)
        //        Control.Font = UIFont.SystemFontOfSize((System.nfloat)view.FontSize);
        //    else if (view.FontSize == Font.Default.FontSize)
        //        Control.Font = UIFont.SystemFontOfSize(17f);
        //}

        //void SetPlaceholderTextColor(CustomLineEntry view)
        //{
        //    if (string.IsNullOrEmpty(view.Placeholder) == false && view.PlaceholderColor != Color.Default)
        //    {
        //        var placeholderString = new NSAttributedString(view.Placeholder,
        //                                    new UIStringAttributes { ForegroundColor = view.PlaceholderColor.ToUIColor() });
        //        Control.AttributedPlaceholder = placeholderString;
        //    }
        //}
    }
}