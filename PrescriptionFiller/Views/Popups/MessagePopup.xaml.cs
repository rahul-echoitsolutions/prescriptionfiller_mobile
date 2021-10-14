using PrescriptionFiller.ViewModels;
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
    public partial class MessagePopup : PopupPage
    {
        public class Type
        {
            public static Type Success = new Type(Color.FromHex("#3FC246"));
            public static Type Warning = new Type(Color.FromHex("#FF0000"));
            public static Type Error = new Type(Color.FromHex("#FF0000"));

            private Color color;

            private Type(Color c)
            {
                color = c;
            }

            public Color Color
            {
                get { return color; }
            }

            public string ButtonImageName
            {
                get
                {
                    string path = string.Empty;
                    if (Xamarin.Essentials.DeviceInfo.Platform == Xamarin.Essentials.DevicePlatform.iOS)
                    {
                        path = "Icons/";
                    }

                    if (this == Success)
                    {
                        path += "pfa_icon_confirm.png";
                    }
                    else if (this == Warning)
                    {
                        path += "pfa_icon_x_close_orange.png";
                    }
                    else
                    {
                        path += "pfa_icon_x_close_red.png";
                    }

                    return path;
                }
            }

            public override bool Equals(object obj)
            {
                return color.Equals(((Type)obj).color);
            }

            public override int GetHashCode()
            {
                return color.GetHashCode();
            }
        }

        public new class Icon
        {
            public static Icon Printer = new Icon("pfa_icon_printer.png");
            public static Icon Connection = new Icon("pfa_icon_connection.png");
            public static Icon Success = new Icon("pfa_icon_success.png");
            public static Icon User = new Icon("pfa_icon_user.png");

            private ImageSource source;

            public ImageSource ImageSource
            {
                get { return source; }
            }

            private Icon(string path)
            {
#if __IOS__
                path = "Icons/" + path;
#endif

                source = FileImageSource.FromFile(path);
            }
        }

        public MessagePopup(Type type, Icon icon, string title, string message)
        {
            InitializeComponent();

            MessagePopupViewModel vm = new MessagePopupViewModel(this.Navigation);

            this.BindingContext = vm;

            vm.Title = title;
            vm.Message = message;
            vm.PopupColor = type.Color;
            vm.ButtonImage = FileImageSource.FromFile(type.ButtonImageName);
            vm.Icon = icon.ImageSource;
        }

        public static async Task Show(INavigation navigation, Type type, Icon icon, string title, string message)
        {
            await LoadingPopup.Dismiss(navigation);

            var page = new MessagePopup(type, icon, title, message);

            await navigation.PushModalAsync(page);
        }
    }
}