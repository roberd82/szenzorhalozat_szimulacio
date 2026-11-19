using System.Text.Encodings.Web;
using System.Text.Json;
using Sensor;
namespace Tools;

public class Eszkozok {
	private static readonly Random Rnd = new();
	private static JsonSerializerOptions JsonOptions = new() {
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
	};

	static void AdatGeneral(Szenzor sz) {
		sz.Meres = Rnd.Next(sz.Tipus.MinErtek, sz.Tipus.MaxErtek);
	}

	static T? JsonOlvaso<T>(string filename) {
		return JsonSerializer.Deserialize<T>(File.ReadAllText(filename), JsonOptions);
	}

	static void JsonIro<T>(string filename, T adat) {
		File.WriteAllText(filename, JsonSerializer.Serialize(adat, JsonOptions));
	}
}