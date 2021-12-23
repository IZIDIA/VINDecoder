using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using VIN_decoder.Classes;
using Xamarin.Essentials;
using Acr.UserDialogs;
using Xamarin.Forms.PlatformConfiguration;
using Plugin.Media.Abstractions;
using Plugin.Media;
using VIN_decoder.Classes.VIN;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;

namespace VIN_decoder {
	public partial class MainPage : ContentPage {
		public static List<Car> MyCars { get; set; }
		public static StackLayout stackLayout;
		public static int Counts = 1;
		public MainPage() {
			InitializeComponent();
			initUI();
		}
		protected void initUI() {
			Label lable_title = new Label() {
				Text = "VIN Decoder v1.0",
				FontFamily = "Timew New Roman",
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				FontAttributes = FontAttributes.Bold,
				TextColor = Color.White,
				HorizontalOptions = LayoutOptions.Center,
			};
			Button button_addCar = new Button() {
				Text = "Добавить",
				FontFamily = "Timew New Roman",
				FontSize = 17,
				FontAttributes = FontAttributes.Bold,
				TextColor = Color.White,
				BackgroundColor = Color.FromHex("#0078D7"),
				HorizontalOptions = LayoutOptions.Center,
				HeightRequest = 60,
				CornerRadius = 5,
			};
			button_addCar.Margin = 10;
			button_addCar.Clicked += OnButton1Clicked;
			Label header = new Label {
				TextColor = Color.White,
				Text = "Список авто:",
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				HorizontalOptions = LayoutOptions.Center
			};
			ListView listView = new ListView();
			MyCars = new List<Car>();
			listView.ItemsSource = MyCars;
			stackLayout = new StackLayout() {
				BackgroundColor = Color.FromHex("#191919"),
				Children = {
					//lable_title,
					button_addCar,
					header,
					listView
				}
			};
			Content = stackLayout;
			if (LoadData()) {
				UpdateList();
			}
		}
		public bool LoadData() {
			var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			var filename = "dataCars";
			if (File.Exists(Path.Combine(folderPath, filename))) {
				var jsonData = File.ReadAllText(Path.Combine(folderPath, filename));
				using (StreamReader file = File.OpenText(Path.Combine(folderPath, filename))) {
					JsonSerializer serializer = new JsonSerializer();
					MyCars = (List<Car>)serializer.Deserialize(file, typeof(List<Car>));
				}
				return true;
			}
			return false;
		}
		public void UpdateList() {
			ListView listView = new ListView {
				SelectionMode = ListViewSelectionMode.Single,
				Margin = 10,
				HasUnevenRows = true,
				RefreshControlColor = Color.AliceBlue,
				ItemsSource = MyCars,
				ItemTemplate = new DataTemplate(() => {
					var stackLayout = new StackLayout {
						HorizontalOptions = LayoutOptions.FillAndExpand,
						Margin = new Thickness(5, 0, 5, 5),
					};
					stackLayout.Orientation = StackOrientation.Horizontal;
					Image image = new Image {
						Margin = new Thickness(5, 0, 0, 0),
						WidthRequest = 65,
						HeightRequest = 65,
						HorizontalOptions = LayoutOptions.StartAndExpand,
						VerticalOptions = LayoutOptions.Center,
					};
					image.SetBinding(Image.SourceProperty, "ImagePath");
					Label labelAvtoName = new Label {
						TextColor = Color.Gold,
						FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
						FontAttributes = FontAttributes.Bold,
						VerticalTextAlignment = TextAlignment.Center,
						HorizontalTextAlignment = TextAlignment.Center,
						HorizontalOptions = LayoutOptions.CenterAndExpand,
					};
					labelAvtoName.SetBinding(Label.TextProperty, "Info.Name");
					Label labelText = new Label {
						Text = "VIN:",
						TextColor = Color.White,
						FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
						FontAttributes = FontAttributes.Italic,
						VerticalTextAlignment = TextAlignment.Center,
						HorizontalTextAlignment = TextAlignment.Center,
					};
					Label labelVin = new Label {
						TextColor = Color.White,
						FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
						FontAttributes = FontAttributes.Italic,
						VerticalTextAlignment = TextAlignment.Center,
						HorizontalTextAlignment = TextAlignment.Center,
						HorizontalOptions = LayoutOptions.CenterAndExpand,
					};
					labelVin.SetBinding(Label.TextProperty, "Info.VIN");
					var stackLayoutForVinLabel = new StackLayout {
						VerticalOptions = LayoutOptions.CenterAndExpand,
						HorizontalOptions = LayoutOptions.CenterAndExpand,
					};
					stackLayoutForVinLabel.Orientation = StackOrientation.Horizontal;
					stackLayoutForVinLabel.Children.Add(labelText);
					stackLayoutForVinLabel.Children.Add(labelVin);
					var stackLayoutForLabels = new StackLayout {
						VerticalOptions = LayoutOptions.CenterAndExpand,
						HorizontalOptions = LayoutOptions.CenterAndExpand,
					};
					stackLayoutForLabels.Orientation = StackOrientation.Vertical;
					stackLayoutForLabels.Children.Add(labelAvtoName);
					stackLayoutForLabels.Children.Add(stackLayoutForVinLabel);
					ImageButton deleteButton = new ImageButton {
						HorizontalOptions = LayoutOptions.EndAndExpand,
						VerticalOptions = LayoutOptions.Center,
						BackgroundColor = Color.Transparent,
						WidthRequest = 30,
						HeightRequest = 30,
						CornerRadius = 2,
						Margin = new Thickness(0, 0, 5, 0),
						Source = "trash.png",
					};
					deleteButton.Clicked += deleteItem;
					stackLayout.Children.Add(image);
					stackLayout.Children.Add(stackLayoutForLabels);
					stackLayout.Children.Add(deleteButton);
					return new ViewCell { View = stackLayout };
				})
			};
			listView.ItemTapped += OnItemTapped;
			stackLayout.Children.RemoveAt(stackLayout.Children.Count - 1);
			stackLayout.Children.Add(listView);
		}
		public async void OnItemTapped(object sender, ItemTappedEventArgs e) {
			Car selectedCar = e.Item as Car;
			if (selectedCar != null) {
				await DisplayAlert("Выбранная модель", $"{selectedCar.Info.ToString()}", "OK");
				if (sender is ListView lv) {
					lv.SelectedItem = null;
				}
			}
		}
		private async void OnButton1Clicked(object sender, System.EventArgs e) {
			UserDialogs.Instance.ShowLoading("Загрузка");
			bool internetCheck = false;
			await Task.Run(() => { internetCheck = InternetCheck.CheckForInternetConnection(); });
			UserDialogs.Instance.HideLoading();
			if (internetCheck) {
				await Navigation.PushAsync(new AddCarPage());
			}
			else {
				await DisplayAlert("Ошибка", "Отсутствует интернет соединение", "OK");
			}
		}
		public async void deleteItem(object sender, System.EventArgs e) {
			if ((sender as ImageButton).BindingContext is Car product) {
				var answer = await DisplayAlert("Удаление", "Вы действительно хотите удалить запись об авто:\n" + product.Info.Name + "\nVIN: " + product.Info.VIN, "Да", "Нет");
				if (answer) {
					UserDialogs.Instance.ShowLoading("Удаление");
					await Task.Run(() => {
						MyCars.Remove(product);
						AddCarPage.Save();
					});
					UserDialogs.Instance.HideLoading();
					UpdateList();
				}
			}
		}
	}
}
