using Godot;
using Newtonsoft.Json;
using System.Diagnostics;

public partial class Run : Node2D
{
	[System.Serializable]
	public struct Data
	{
		[JsonProperty] private int id;
		[JsonProperty] public string name;
	}

	public override void _Ready()
	{
		base._Ready();

		var json = JsonConvert.SerializeObject(new Data(), Formatting.Indented);
		Debug.WriteLine(json);
	}
}
