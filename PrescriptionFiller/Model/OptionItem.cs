using System;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PrescriptionFiller.Models
{
	public class PrescriptionsOptionItem : OptionItem
	{
		public override string Icon { get { return (Device.OS == TargetPlatform.iOS ? "Icons/" : "") + "pfa_icon_edit_small.png"; } }
	}


	public class AccountInfoOptionItem : OptionItem
	{
        public override string Icon { get { return (Device.OS == TargetPlatform.iOS ? "Icons/" : "") + "pfa_icon_user_small.png"; } }
		public override string Title { get { return "Account info"; } }
	}

    public class MedicalHistoryOptionItem : OptionItem
    {
        public override string Icon { get { return (Device.OS == TargetPlatform.iOS ? "Icons/" : "") + "pfa_icon_home_small.png"; } }
        public override string Title { get { return "Medical History"; } }
    }


    public class AboutOptionItem : OptionItem
	{
		public override string Icon { get { return (Device.OS == TargetPlatform.iOS ? "Icons/" : "") + "pfa_icon_user_small.png"; } }
	}

	public class LogOutOptionItem : OptionItem
	{
		public override string Icon { get { return (Device.OS == TargetPlatform.iOS ? "Icons/" : "") + "pfa_icon_lock_small.png"; } }
		public override string Title { get { return "Log out"; } }
	}

	public  abstract class OptionItem
	{
		public virtual string Title { get { var n = GetType().Name; return n.Substring(0, n.Length - 10); } }
		public virtual int Count { get; set; }
		public virtual bool Selected { get; set; }
		public virtual string Icon { get { return 
				Title.ToLower().TrimEnd('s') + ".png" ; } }
		public ImageSource IconSource { get { return ImageSource.FromFile(Icon); } }
	}
}

