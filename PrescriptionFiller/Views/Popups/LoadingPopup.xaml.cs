using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrescriptionFiller.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingPopup : PopupPage
    {
        private static LoadingPopup Instance;

        public static async Task Show(INavigation nav)
        {
            if (Instance == null)
            {
                Instance = new LoadingPopup();
                new System.Threading.Thread(new System.Threading.ThreadStart(() => {
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                        nav.PushModalAsync(Instance);

                        //Instance.AnimateAsync();
                    });
                })).Start();
            }
        }

        public static async Task Dismiss(INavigation nav)
        {
            if (Instance != null)
            {
                Instance.Closing = true;
                try
                {
                    await nav.PopModalAsync();
                }
                catch (Exception e)
                {
                };
                Instance = null;
            }
        }

        private Button BG1, BG2, BG3;
        private FloatingButton BG;

        public bool Closing
        {
            get;
            set;
        }

        public LoadingPopup()
        {
            InitializeComponent();
            MainLayout.OnLayoutChildren += MainLayout_OnLayoutChildren;

            InitButton(BG1 = new Button());
            InitButton(BG2 = new Button());
            InitButton(BG3 = new Button());

            InitButton(BG = new FloatingButton());
        }

        ~LoadingPopup()
        {
            MainLayout.OnLayoutChildren += MainLayout_OnLayoutChildren;
        }

        private void InitButton(Button b)
        {
            b.BackgroundColor = Color.White;
            b.TextColor = Color.Transparent;
            b.HorizontalOptions = LayoutOptions.CenterAndExpand;
            b.VerticalOptions = LayoutOptions.CenterAndExpand;
            b.WidthRequest = b.HeightRequest = 48;
            b.BorderRadius = Device.OnPlatform(24, 256, 0);

            if (b is FloatingButton)
            {
                FloatingButton fb = (FloatingButton)b;
                fb.PaddingLeft = 24;
                fb.Image = (FileImageSource)FileImageSource.FromFile(Device.OnPlatform("Icons/pfa_icon_loader.png", "pfa_icon_loader.png", ""));
                fb.Command = new Command(delegate ()
                {
                    Navigation.PopModalAsync();
                });
            }

            MainLayout.Children.Add(b);
            float pageWidth = App.ScreenWidth / App.ScreenDensity;
            float pageHeight = App.ScreenHeight / App.ScreenDensity;
            b.Layout(new Rectangle((pageWidth - b.WidthRequest) / 2f, (pageHeight - b.HeightRequest) / 2f, b.WidthRequest, b.HeightRequest));
        }

        void MainLayout_OnLayoutChildren(double x, double y, double width, double height)
        {

        }

        public async Task AnimateAsync()
        {
            //uint length = 1000;

            //while (Closing == false)
            //{
            //    await Task.WhenAll(
            //       BG.RotateTo(360, length * 2, Easing.CubicInOut),
            //       BG1.ScaleTo(1.8, length, Easing.Linear),
            //       BG2.ScaleTo(1.8, (uint)(length * 1.5), Easing.Linear),
            //       BG3.ScaleTo(1.8, length * 2, Easing.Linear),
            //       BG1.FadeTo(0, length, Easing.Linear),
            //       BG2.FadeTo(0, (uint)(length * 1.5), Easing.Linear),
            //       BG3.FadeTo(0, length * 2, Easing.Linear));
            //    BG1.Scale = BG2.Scale = BG3.Scale = 1;
            //    BG1.Opacity = BG2.Opacity = BG3.Opacity = 1;
            //    BG.Rotation = 0;
            //}
        }
    }
}