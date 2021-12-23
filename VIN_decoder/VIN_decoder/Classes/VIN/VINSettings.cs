namespace VIN_decoder.Classes.VIN {
	class VINSettings : IParserSettings {
		public VINSettings(string start) {
			StartPoint = start;
		}
		public string BaseUrl { get; set; } = "https://vinru.ru";
		public string Prefix { get; set; } = "poisk?Vin={CurrentId}";
		public string StartPoint { get; set; }
	}
}
