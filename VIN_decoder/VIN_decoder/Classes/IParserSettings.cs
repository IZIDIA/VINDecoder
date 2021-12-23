using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIN_decoder.Classes {
	interface IParserSettings {
		string BaseUrl { get; set; }
		string Prefix { get; set; }
		string StartPoint { get; set; }
	}
}
