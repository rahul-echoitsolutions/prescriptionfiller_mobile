//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

[assembly: global::Xamarin.Forms.Xaml.XamlResourceIdAttribute("PrescriptionFiller.Views.HomePage.xaml", "Views/HomePage.xaml", typeof(global::PrescriptionFiller.Views.HomePage))]

namespace PrescriptionFiller.Views {
    
    
    [global::Xamarin.Forms.Xaml.XamlFilePathAttribute("Views\\HomePage.xaml")]
    public partial class HomePage : global::Xamarin.Forms.ContentPage {
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "2.0.0.0")]
        private global::Xamarin.Forms.Button NewPrescriptions;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "2.0.0.0")]
        private global::Xamarin.Forms.Button SentPrescriptions;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "2.0.0.0")]
        private global::PrescriptionFiller.FloatingButton CameraButton;
        
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "2.0.0.0")]
        private void InitializeComponent() {
            global::Xamarin.Forms.Xaml.Extensions.LoadFromXaml(this, typeof(HomePage));
            NewPrescriptions = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Button>(this, "NewPrescriptions");
            SentPrescriptions = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Button>(this, "SentPrescriptions");
            CameraButton = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::PrescriptionFiller.FloatingButton>(this, "CameraButton");
        }
    }
}
