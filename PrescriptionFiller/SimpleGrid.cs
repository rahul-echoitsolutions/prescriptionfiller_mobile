using System;
using Xamarin.Forms;

namespace PrescriptionFiller
{
    public class SimpleGrid : Grid
    {
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            base.LayoutChildren(x, y, width, height);
            Console.WriteLine(string.Format("SimpleGrid.LayoutChildren({0}, {1}, {2}, {3});", x, y, width, height));
        }

        protected override void InvalidateMeasure()
        {
            base.InvalidateMeasure();
            Console.WriteLine("SimpleGrid.InvalidateMeasure()");
        }
    }
}

