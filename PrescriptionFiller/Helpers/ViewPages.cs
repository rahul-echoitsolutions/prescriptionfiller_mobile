using System;
using PrescriptionFiller.Views;

namespace PrescriptionFiller
{
    public class ViewPages
    {
        private static ViewPages _instance;
        public LoginPage loginPage { get; set; }
        public HomePage homePage { get; set; }
        public RootPage rootPage { get; set; }
        public AccountInfoPage accountInfoPage { get; set; }

        public static ViewPages Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ViewPages();

                return _instance;
            }
        }

        private ViewPages()
        {
        }
    }
}

