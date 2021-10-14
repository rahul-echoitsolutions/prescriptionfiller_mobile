using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrescriptionFiller;
using Xamarin.Forms;
using PrescriptionFiller.iOS.Renderers;
using System.ComponentModel;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;
using UIKit;

[assembly: ExportRenderer(typeof(FloatingButton), typeof(FloatingButtonRenderer))]
namespace PrescriptionFiller.iOS.Renderers
{
	public class FloatingButtonRenderer : ButtonRenderer
	{

		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
		{
			base.OnElementChanged(e);

			Control.ContentEdgeInsets = new UIKit.UIEdgeInsets (0, 10, 0, 0);


		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
		}
	}

    public class CircleView : UIView
    {
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            using (CGContext g = UIGraphics.GetCurrentContext ()) 
            {
                UIColor.Blue.SetFill ();
                UIColor.Red.SetStroke ();

                g.AddEllipseInRect(rect);
                g.FillPath();
            }
        }
        
    }
}