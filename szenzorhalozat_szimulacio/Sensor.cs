using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace szenzorhalozat_szimulacio;

public class SensorDdContext : DbContext {
	public DbSet<Sensor> Sensor => Set<Sensor>();
	public DbSet<Measurement> Measurement => Set<Measurement>();
	
	protected override void OnConfiguring(DbContextOptionsBuilder options) {
		var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
		if (connectionString is null) {
			throw new NullReferenceException("A connection string nem lett megadva!");
		}
		options.UseMySQL(connectionString);
	}
}

public class Sensor {
	// a lehetséges szenzortípusok listája a méréshatárukkal
	// "név": (min, max)
	public static Dictionary<string, (int, int)> MeasurementRanges { get; set; } = [];
	
	// a szenzor azonosítója
	// primary key, nem auto_increment
	[Key] [Column("id")] [DatabaseGenerated(DatabaseGeneratedOption.None)]
	public required int Id { get; init; }

	[Column("type", TypeName = "VARCHAR(64)")]
	public required string Type {
		get;
		init {
			if (!MeasurementRanges.ContainsKey(value)) {
				throw new KeyNotFoundException("Ilyen típusú szenzor nem létezik: " + value);
			}
			field = value;
		}
	}
	
	// a szenzorról egy adott pillanatban leolvasható adat
	[NotMapped]
	public int Data {
		get;
		// ReSharper disable once PropertyCanBeMadeInitOnly.Global
		set {
			if (value < MeasurementRanges[Type].Item1 || value > MeasurementRanges[Type].Item2) {
				// eseménykezelés méréshatáron túli érték esetén
			}
			field = value;
		}
	}
}

public class Measurement {
	// a mérés azonosítója
	// primary key, auto_increment
	[Key] [Column("id")] [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public required int Id { get; init; }
	
	[ForeignKey("Sensor")] [JsonIgnore] //[Column("sensor_id")] 
	public required Sensor SensorId { get; init; }

	[NotMapped] [JsonPropertyName("SensorId")]
	public int SensorKey => SensorId.Id;
	
	[Column("time")]
	public required DateTime Time { get; init; }
	
	[Column("data")]
	public int? Data { get; init; }
}
