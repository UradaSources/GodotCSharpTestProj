using Godot;
using System.Text;

public partial class UpdateDebugDisplay : RichTextLabel
{
	public override void _Process(double delta)
	{
		var build = new StringBuilder();

		build.AppendLine($"fps: {1/delta:0.0}\tdelta: {delta:0.000}\n");

		for (int i = 0; i < DebugWatch.Main.recordCount; i++)
		{
			var record = DebugWatch.Main.getRecord(i);
			build.AppendLine($"[color=green][b]{record.tag} -{record.time}[/b][/color]\n{record.constnet}");
		}

		this.Text = build.ToString();
	}
}
