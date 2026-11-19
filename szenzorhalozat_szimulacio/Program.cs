using Sensor;
var SZENZOROK_SZAMA = 4;

SzenzorTipus[] SZENZOR_TIPUSOK = [
	new() { Nev = "hőmérséklet", MinErtek = 0, MaxErtek = 100 },// víz hőmérséklete
	new() { Nev = "vízszint", MinErtek = 0, MaxErtek = 500 },	// folyó vízszintje (max 500m)
	new() { Nev = "sebesség", MinErtek = 0, MaxErtek = 10 },	// folyó folyási sebessége (m/s)
	new() { Nev = "tisztaság", MinErtek = 0, MaxErtek = 100 }	// 0-100%
];

var szenzorok = new Szenzor[SZENZOROK_SZAMA];
for (var i = 0; i < szenzorok.Length; i++) {
	szenzorok[i] = new Szenzor {
		Id = i,
		Tipus = SZENZOR_TIPUSOK[i % SZENZOR_TIPUSOK.Length]
	};
}








