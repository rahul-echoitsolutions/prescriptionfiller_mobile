using System;
using Xamarin.Forms;

namespace PrescriptionFiller
{
    public class SimpleMasterDetailPage : MasterDetailPage
    {
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            base.LayoutChildren(x, y, width, height);
            Console.WriteLine(string.Format("MyMasterDetailPage.LayoutChildren({0}, {1}, {2}, {3});", x, y, width, height));
        }

        protected override void InvalidateMeasure()
        {
            base.InvalidateMeasure();
            Console.WriteLine("MyMasterDetailPage.InvalidateMeasure()");
        }

        public override SizeRequest GetSizeRequest(double widthConstraint, double heightConstraint)
        {
            Console.WriteLine(string.Format("MyMasterDetailPage.GetSizeRequest({0}, {1});", widthConstraint, heightConstraint));
            return base.GetSizeRequest(widthConstraint, heightConstraint);
        }
    }
}

