using PrescriptionFiller.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrescriptionFiller.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpPage : ContentPage
    {
        public SignUpPage()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            InitializeComponent();
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            this.BindingContext = new SignUpViewModel(this.Navigation, this);
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public void Remove(View v)
        {
            SimpleLayout s = (Content as SimpleLayout);
            int index = s.Children.IndexOf(v);
            s.Children.RemoveAt(index);
        }

        // Cancel the back button
        //        protected override bool OnBackButtonPressed ()
        //        {
        //            return true;
        //        }
    }
}