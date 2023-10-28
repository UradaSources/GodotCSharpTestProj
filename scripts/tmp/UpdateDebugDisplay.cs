using Godot;
using System.Text;

public partial class UpdateDebugDisplay : RichTextLabel
{
	public override void _Process(double delta)
	{
		var build = new StringBuilder();

		build.AppendLine($"fps: {1/delta:0.0}\tdelta: {delta:0.000}\n");

		for (int i = 0; i < DebugDisplay.Main.recordCount; i++)
		{
			var record = DebugDisplay.Main.getRecord(i);
			build.AppendLine($"[Color=green]{record.title} -{record.time.ToString("HH:mm:ss")}[/Color]\n{record.msg}");
		}

		this.Text = build.ToString();
	}
}
