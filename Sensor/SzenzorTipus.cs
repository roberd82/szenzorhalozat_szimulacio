namespace Sensor;

public class SzenzorTipus {
	public required string Nev { get; init; }

	[NonSerialized]
	public int MinErtek = int.MinValue;
	[NonSerialized]
	public int MaxErtek = int.MaxValue;
}
