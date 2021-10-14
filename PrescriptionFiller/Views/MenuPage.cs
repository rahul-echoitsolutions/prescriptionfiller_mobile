using PrescriptionFiller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace PrescriptionFiller.Views
{
	public class MenuPage : ContentPage
	{
		readonly List<OptionItem> OptionItems = new List<OptionItem>();

		public ListView Menu { get; set; }

		public MenuPage()
		{
			OptionItems.Add(new PrescriptionsOptionItem());
			OptionItems.Add(new AccountInfoOptionItem());
			//OptionItems.Add(new AboutOptionItem());
			OptionItems.Add(new MedicalHistoryOptionItem());
			OptionItems.Add(new LogOutOptionItem());

			//BackgroundColor = Color.FromHex("#FFE53935");
			BackgroundColor = Color.FromHex("#FF0000");

			var layout = new StackLayout { Spacing = 0, VerticalOptions = LayoutOptions.FillAndExpand };

			Menu = new ListView
			{
				ItemsSource = OptionItems,
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.Transparent,
				SeparatorColor = Color.Transparent,
			};

			var cell = new DataTemplate(typeof(DarkTextCell));
			cell.SetBinding(TextCell.TextProperty, "Title");
			cell.SetBinding(ImageCell.ImageSourceProperty, "IconSource");
			cell.SetValue(VisualElement.BackgroundColorProperty, Color.Transparent);

			Menu.ItemTemplate = cell;

			layout.Children.Add(Menu);

			Content = layout;
		}
	}


	public class DarkTextCell : ImageCell
	{
		public DarkTextCell()
		{
			TextColor = Color.White;
		}
	}
}