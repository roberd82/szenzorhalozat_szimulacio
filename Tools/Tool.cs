using System.Text.Encodings.Web;
using System.Text.Json;
namespace Tools;

public static class Tool {
	private static readonly Random Rnd = Random.Shared;
	private static readonly JsonSerializerOptions JsonOptions = new() {
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
		WriteIndented = true
	};
	
	// a kapott objektum string-ként megkapott nevű property-jére rátesz egy random int értéket
	public static void GenerateIntData<T>(T t, string propertyName, int max = int.MaxValue, int min = int.MinValue) {
		var p = typeof(T).GetProperty(propertyName);
		if (p is null) throw new ArgumentException("A megadott nevű tulajdonság nem létezik!");

		if (p.PropertyType != typeof(int)) throw new NotSupportedException("A megadott nevű tulajdonság nem int típusú!");

		if (!p.CanWrite) throw new InvalidOperationException("A megadott nevű tulajdonság nem írható!");
		
		p.SetValue(t, Rnd.Next(min, max));
	}

	// json fájlból olvasás
	public static T? JsonReader<T>(string filename) {
		return JsonSerializer.Deserialize<T>(File.ReadAllText(filename), JsonOptions);
	}

	// json fájlba írás
	public static void JsonWriter<T>(string filename, T adat) {
		File.WriteAllText(filename, JsonSerializer.Serialize(adat, JsonOptions));
	}
}
