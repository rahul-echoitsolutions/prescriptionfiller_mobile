using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using UIKit;
using PrescriptionFiller;
using PrescriptionFiller.iOS.Renderers;
using PrescriptionFiller.Views;

[assembly: ExportCell (typeof (DarkTextCell), typeof (DarkTextCellRenderer))]

namespace PrescriptionFiller.iOS.Renderers
{

    public class DarkTextCellRenderer : ImageCellRenderer
    {
		public override UITableViewCell GetCell (Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var cellView = base.GetCell (item, reusableCell, tv);

			cellView.BackgroundColor = Color.FromHex("FFE53935").ToUIColor();
            cellView.TextLabel.TextColor = Color.FromHex("FFFFFF").ToUIColor();
            cellView.DetailTextLabel.TextColor = Color.FromHex("AAAAAA").ToUIColor();

			tv.SeparatorColor = Color.Transparent.ToUIColor();

            return cellView;
        }
    }
    
}