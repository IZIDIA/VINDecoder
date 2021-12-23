using System;
using System.Collections.Generic;
using Xamarin.Forms;
using VIN_decoder.Classes;
using Acr.UserDialogs;
using Plugin.Media.Abstractions;
using Plugin.Media;
using VIN_decoder.Classes.VIN;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;

namespace VIN_decoder {
	public partial class AddCarPage : ContentPage {
		ParserWorker<string[]> parser;
		MainPage homePage;
		public AddCarPage() {
			InitializeComponent();
			ComputerVisionQuickstart.client = ComputerVisionQuickstart.Authenticate(ComputerVisionQuickstart.endpoint, ComputerVisionQuickstart.subscriptionKey);
			parser = new ParserWorker<string[]>(new VINParser());
			parser.OnCompleted += Parser_OnCompleted;
			parser.OnNewData += Parser_OnNewData;
			NavigationPage navPage = (NavigationPage)Application.Current.MainPage;
			homePage = navPage.RootPage as MainPage;
			WarningLabel.Text = string.Empty;
		}

		#region Main_methods
		private async void SelectImage_Pressed(object sender, EventArgs e) {
			WarningLabel.Text = string.Empty;
			var result = await CrossMedia.Current.PickPhotoAsync();
			if (result != null) {
				if (result.Path.Contains(".jpg") || result.Path.Contains(".jpeg") || result.Path.Contains(".png") || result.Path.Contains(".bmp")) {
					GetVinInfo(result.Path);
				}
				else {
					await DisplayAlert("Неверный формат", "Только: JPG PNG BMP", "OK");
					WarningLabel.Text = "Формат выбранного вами изображения не поддерживается. Выберите другое фото.";
				}
			}
		}
		private async void CreateImage_Pressed(object sender, EventArgs e) {
			WarningLabel.Text = string.Empty;
			var result = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions {
				Directory = "Sample",
				Name = "vin_decode.jpg"
			});
			if (result != null) {
				GetVinInfo(result.Path);
			}
		}
		public async void GetVinInfo(string result) {
			UserDialogs.Instance.ShowLoading("Загрузка");
			var text = await ComputerVisionQuickstart.StartService(result);
			var vinCode = string.Empty;
			Regex regex = new Regex("[A-HJ-NPR-Z0-9]{17}");
			MatchCollection matches = regex.Matches(text);
			if (matches.Count == 1) {
				vinCode = matches[0].Value;
				parser.Settings = new VINSettings(vinCode);
				parser.Start();
			}
			else {
				var vinIdentical = true;
				for (int i = 0; i < matches.Count - 1; i++) {
					if (matches[i].Value != matches[i + 1].Value) {
						vinIdentical = false;
					}
				}
				if (matches.Count > 1 && vinIdentical) {
					vinCode = matches[0].Value;
					parser.Settings = new VINSettings(vinCode);
					parser.Start();
				}
				else {
					if (matches.Count > 1 && !vinIdentical) {
						await DisplayAlert("Проблема с фотографией", "На фото несколько различных VIN кодов.", "OK");
						WarningLabel.Text = "На фотографии выбранной вами должен быть VIN одного авто.";
						UserDialogs.Instance.HideLoading();
					}
					else {
						LaunchPlanB(text);
					}
				}
			}
		}
		private void Parser_OnCompleted(object obj) {
		}
		private async void Parser_OnNewData(object arg1, string[] arg2) {
			var res = string.Join("\r\n", arg2);
			if (res.Length > 1) {
				var addOrNot = true;
				var decodingCar = new Car($"My car {MainPage.Counts}", "default_logo.jpg", res);
				decodingCar.Info.DecoderDate = DateTime.Now.ToString("dd.MM.yyyy");
				foreach (var car in MainPage.MyCars) {
					if (car.Info.VIN == decodingCar.Info.VIN) {
						addOrNot = false;
					}
				}
				if (addOrNot) {
					decodingCar.ImagePath = GetImagePath(decodingCar.Info.Name);
					MainPage.MyCars.Add(decodingCar);
					MainPage.Counts += 1;
					homePage.UpdateList();
					Save();
					await Navigation.PopAsync();
				}
				else {
					await DisplayAlert("Повторное декодирование", "Данное авто уже добавлено в ваш список.", "OK");
					await Navigation.PopAsync();
				}
			}
			else {
				await DisplayAlert("Проблема базы данных", "Этого VIN-кода нет в базе.", "OK");
				WarningLabel.Text = "На фотографии выбранной вами должен быть только один VIN.";
			}
			UserDialogs.Instance.HideLoading();
		}
		private async void LaunchPlanB(string text) {
			text = text.Replace(" ", "");
			var vinCode = "";
			Regex regex = new Regex("([A-H]|[J-NPR]|[S-Z]|[1-5]|[67]|[89])[0-9A-HJ-NPR-Z]([0-8A-HJ-NPR-Z]|9)[0-9A-HJ-NPR-Z]{5}([A-HJ-NPR-WYZ]|[0-9X])([1-9A-HJ-NPR-TV-Y]|[0-9A-HJ-NPR-Z])[0-9A-HJ-NPR-Z]{3}[0-9]{4}");
			MatchCollection matches = regex.Matches(text);
			if (matches.Count == 1) {
				vinCode = matches[0].Value;
				parser.Settings = new VINSettings(vinCode);
				parser.Start();
			}
			else {
				var vinIdentical = true;
				for (int i = 0; i < matches.Count - 1; i++) {
					if (matches[i].Value != matches[i + 1].Value) {
						vinIdentical = false;
					}
				}
				if (matches.Count > 1 && vinIdentical) {
					vinCode = matches[0].Value;
					parser.Settings = new VINSettings(vinCode);
					parser.Start();
				}
				else {
					if (matches.Count > 1 && !vinIdentical) {
						await DisplayAlert("Проблема с фотографией", "На фото несколько различных VIN кодов.", "OK");
						WarningLabel.Text = "На фотографии выбранной вами должен быть VIN одного авто.";
						UserDialogs.Instance.HideLoading();
					}
					else {
						await DisplayAlert("Не найден VIN", "На фото нет VIN кода.", "OK");
						WarningLabel.Text = "На выбранной фотографии нет VIN кода.";
					}
				}
				UserDialogs.Instance.HideLoading();
			}
		}
		protected string GetImagePath(string carTitle) {
			List<string> cars = new List<string>()
				{      "acura","alfa_romeo","aston_martin","audi","bentley","bmw","bugatti","buick","cadillac","chery","chevrolet","chrysler","citroen","dacia","daewoo","default_logo","dodge","ferrari","fiat","ford","gaz","holden","honda","hyundai","infiniti","jaguar","jeep","kia","lada","lamborghini","lancia","land_rover","lexus","lotus","maserati","maybach","mazda","mercedes","mercury","mini","mitsubishi","nissan","opel","pagani","peugeot","pontiac","porshe","renault","rolls_royce","rover","saab","scion","seat","skoda","ssang_yong","subaru","suzuki","toyota","vauxhall","volkswagen","volvo"
			};
			string[] words = carTitle.Split(' ');
			string variant1 = words[0].ToLower();
			string variant2 = words[0].ToLower() + "_" + words[1].ToLower();
			if (cars.Contains(variant1))
				return $"{variant1}.png";
			if (cars.Contains(variant2))
				return $"{variant2}.png";
			return "default_logo.png";
		}
		static public void Save() {
			var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			var filename = "dataCars";
			var jsonFile = JsonConvert.SerializeObject(MainPage.MyCars);
			File.WriteAllText(Path.Combine(folderPath, filename), jsonFile);
		}
		#endregion
	}
}
