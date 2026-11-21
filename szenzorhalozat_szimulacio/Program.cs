using Microsoft.EntityFrameworkCore;
using Tools;

using szenzorhalozat_szimulacio;

const int szenzorokSzama = 4;

using (var db = new SensorDdContext()) {
	// táblák előző állapotának biztonsági mentése json fájlokba
	Tool.JsonWriter(GetNextBackupFilename("Sensor"), db.Sensor);
	Tool.JsonWriter(GetNextBackupFilename("Measurement"), db.Measurement);

	// régi táblák törlése és újrakreálása
	db.Database.ExecuteSql($"DROP TABLE IF EXISTS Measurement;");
	db.Database.ExecuteSql($"DROP TABLE IF EXISTS Sensor;");
	db.Database.EnsureCreated();
}

// szenzortípusok definiálása
// "név": (min, max)
Sensor.MeasurementRanges.Add("hőmérséklet", (0, 100));	// víz hőmérséklete
Sensor.MeasurementRanges.Add("vízszint", (0, 500));		// folyó vízszintje (max 500m)
Sensor.MeasurementRanges.Add("sebesség", (0, 10));		// folyó folyási sebessége (m/s)
Sensor.MeasurementRanges.Add("tisztaság", (0, 100));	// 0-100%

// szenzorok létrehozása
using (var db = new SensorDdContext()) {
	var szenzorTipusok = Sensor.MeasurementRanges.Keys.ToList();
	for (var i = 0; i < szenzorokSzama; i++) {
		db.Sensor.Add(new Sensor {
			Id = i,
			Type = szenzorTipusok[i % szenzorokSzama]
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
	filename = Path.Join(filename, filename + "_" + i);
	while (File.Exists(filename + ".json")) {
		filename = filename[..^1] + "" + ++i;
	}

	return filename + ".json";
}
