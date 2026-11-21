using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
	public int Data { get; set; }
}

public class Measurement {
	// a mérés azonosítója
	// primary key, auto_increment
	[Key] [Column("id")] [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public required int Id { get; init; }
	
	[Column("sensor_id")] [ForeignKey("Sensor")]
	public required Sensor SensorId { get; init; }
	
	[Column("time")]
	public required DateTime Time { get; init; }
	
	[Column("data")]
	public int? Data { get; init; }
}
