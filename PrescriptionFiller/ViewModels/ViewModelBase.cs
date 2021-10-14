using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.ComponentModel;

namespace PrescriptionFiller.ViewModels
{
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				//let the UI know a property changed.
				PropertyChanged(this,
					new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}

