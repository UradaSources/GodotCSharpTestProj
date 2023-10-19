using Godot;

public partial class UpdateDebugDisplay : ItemList
{
	public override void _Process(double delta)
	{
		this.Clear();

		for (int i = 0; i < DebugDisplay.Main.recordCount; i++)
		{
			var record = DebugDisplay.Main.GetRecord(i);
			this.AddItem($"{record.msg} {record.title} {record.time} {record.key}");
		}
	}
}
