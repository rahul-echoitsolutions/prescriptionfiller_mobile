using System;
using Xamarin.Forms;

namespace PrescriptionFiller
{
    public delegate void LayoutChildrenDelegate (double x, double y, double width, double height);

    //utility class to allow composing of layout behaviour
    public class SimpleLayout : AbsoluteLayout
    {
        public event LayoutChildrenDelegate OnLayoutChildren;

        public bool IsHandlingLayoutManually { get; set; }

        public SimpleLayout ()
        {
        }

        protected override void LayoutChildren (double x, double y, double width, double height)
        {
            if (!IsHandlingLayoutManually) {
                base.LayoutChildren (x, y, width, height);
            }

            if (OnLayoutChildren != null) {
                OnLayoutChildren (x, y, width, height);
            }

            Console.WriteLine(string.Format("SimpleLayout.LayoutChildren({0}, {1}, {2}, {3});", x, y, width, height));
        }

        public void InvalidateSimpleLayout()
        {
            InvalidateLayout();
        }
    }
}

