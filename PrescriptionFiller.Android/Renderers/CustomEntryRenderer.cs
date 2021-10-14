using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using PrescriptionFiller.Droid.Renderers;
using Android.Graphics.Drawables;

[assembly: ExportRenderer (typeof (EntryRenderer), typeof (CustomEntryRenderer))]
namespace PrescriptionFiller.Droid.Renderers
{
	public class CustomEntryRenderer : EntryRenderer
	{
		protected override void OnElementChanged (ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged (e);

            if (this.Control == null) return;

            this.Control.SetBackgroundColor(Color.Transparent.ToAndroid());
		}
	}
}