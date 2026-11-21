using Microsoft.EntityFrameworkCore;
using Tools;

using szenzorhalozat_szimulacio;

const int szenzorokSzama = 6;
const double abnormalitasiFok = 0.15;	// esélye a méréshatáron kívüli adatoknak

using (var db = new SensorDdContext()) {
	db.Database.EnsureCreated();
	// táblák előző állapotának biztonsági mentése json fájlokba
	Tool.JsonWriter(GetNextBackupFilename("Sensor"), db.Sensor);
	Tool.JsonWriter(GetNextBackupFilename("Measurement"), db.Measurement);

	// régi táblák törlése és újrakreálása
	db.Database.ExecuteSql($"DROP TABLE IF EXISTS Measurement;");
	db.Database.ExecuteSql($"DROP TABLE IF EXISTS Sensor;");
	db.Database.EnsureCreated();
}

// szenzortípusok definiálása
Sensor.MeasurementRanges.Add("hőmérséklet", (0, 100));	// víz hőmérséklete
Sensor.MeasurementRanges.Add("vízszint", (0, 500));		// folyó vízszintje (max 500m)
Sensor.MeasurementRanges.Add("sebesség", (0, 10));		// folyó folyási sebessége (m/s)
Sensor.MeasurementRanges.Add("tisztaság", (0, 100));	// 0-100%

// szenzorok létrehozása
using (var db = new SensorDdContext()) {
	var szenzorTipusok = Sensor.MeasurementRanges.Keys.ToArray();
	for (var i = 0; i < szenzorokSzama; i++) {
		db.Sensor.Add(new Sensor {
			Id = i + 1,
			Type = szenzorTipusok[i % szenzorTipusok.Length]
		});
	}
	db.SaveChanges();
}

while (true) {
	Console.WriteLine("Nyomj entert a szenzorok adatainak leolvasásához: ");
	if (Console.ReadLine() == "end") {
		break;
	}
	// mérési adatok random generálása
	using var db = new SensorDdContext();
	foreach (var sensor in db.Sensor.ToList()) {
		var min = Sensor.MeasurementRanges[sensor.Type].Item1;
		var max = Sensor.MeasurementRanges[sensor.Type].Item2;
		Tool.GenerateIntData(sensor, "Data", (int)Math.Truncate(min - abnormalitasiFok * min), (int)Math.Truncate(max + abnormalitasiFok * max));
	}

	// mérési adatok leolvasása
	var i = db.Measurement.Count();
	foreach (var sensor in db.Sensor.ToList()) {
		db.Measurement.Add(new Measurement {
			Id = ++i,
			SensorId = sensor,
			Time = DateTime.Now,
			Data = sensor.Data
		});
	}
	
	db.SaveChanges();
}

string GetNextBackupFilename(string tableName) {
	var filename = tableName + "_backup";
	if (!Directory.Exists(filename)) {
		Directory.CreateDirectory(filename);
	}

	var i = 0;
	filename = Path.Join(filename, filename + "_");
	while (File.Exists(filename + i + ".json")) { 
		i++;
	}

	return filename + i + ".json";
}
