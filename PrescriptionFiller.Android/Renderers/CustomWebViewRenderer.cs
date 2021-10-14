using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;
using PrescriptionFiller.Droid.Renderers;

[assembly: ExportRenderer(typeof(WebView), typeof(CustomWebViewRenderer))]
namespace PrescriptionFiller.Droid.Renderers
{
	public class CustomWebViewRenderer : WebViewRenderer
	{

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (Control != null)
			{
				Control.Settings.SupportZoom();
				Control.Settings.BuiltInZoomControls = true;
				Control.Settings.DisplayZoomControls = true;
				Control.Settings.LoadWithOverviewMode = true;
				Control.Settings.UseWideViewPort = true;
			}
			base.OnElementPropertyChanged(sender, e);
		}

	}
}

