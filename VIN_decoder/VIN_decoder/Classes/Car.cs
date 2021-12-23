using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace VIN_decoder.Classes {
	public class Car {
		public Car() {
		}
		public string Title { get; set; }
		public string ImagePath { get; set; }
		public Info Info { get; set; }
		public Car(string title, string imagePath, string info) {
			Title = title;
			ImagePath = imagePath;

			Info = Construct(info);
		}
		public Info Construct(string info) {
			Info arr = new Info();
			string[] elements = info.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
			LilInfo lilInfo = new LilInfo(elements);
			arr.Name = lilInfo.name;
			arr.VIN = lilInfo.vin;
			arr.Year = lilInfo.year;
			arr.Category = lilInfo.category;
			arr.Power = lilInfo.power;
			arr.Capacity = lilInfo.capacity;
			arr.Registration = lilInfo.registration;
			return arr;
		}
	}
	public struct LilInfo {
		public string name;
		public string vin;
		public string year;
		public string category;
		public string power;
		public string capacity;
		public string registration;
		public LilInfo(string[] elemements) : this() {
			string[] subElem = elemements[0].Split(',');
			name = subElem[0];
			for (int i = 1; i < elemements.Length; i++) {
				subElem = elemements[i].Split(':');
				switch (i) {
					case 1:
						vin = subElem[1].Trim();
						break;
					case 2:
						year = subElem[1];
						break;
					case 3:
						category = subElem[1];
						break;
					case 4:
						power = subElem[1];
						break;
					case 5:
						capacity = subElem[1];
						break;
					case 6:
						registration = subElem[1];
						break;
				}
			}
		}
	}
	public partial class Info {
		[JsonProperty("Name")]
		public string Name { get; set; }

		[JsonProperty("VIN")]
		public string VIN { get; set; }

		[JsonProperty("Year")]
		public string Year { get; set; }

		[JsonProperty("Capacity")]
		public string Capacity { get; set; }

		[JsonProperty("Category")]
		public string Category { get; set; }

		[JsonProperty("Power")]
		public string Power { get; set; }

		[JsonProperty("Registration")]
		public string Registration { get; set; }
		[JsonProperty("DecoderDate")]
		public string DecoderDate { get; set; }
		public override string ToString() {
			return new string($"Авто: {Name}\nVIN: {VIN}\nГод: {Year}\nКатегория: {Category}\nМощность двигателя: {Power}\n" +
				$"Объем двигателя: {Capacity}\nМесто регистрации: {Registration}\nДата декодирования: {DecoderDate}");
		}
	}

}
