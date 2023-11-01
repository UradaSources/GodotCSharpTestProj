using Godot;
using System.Diagnostics;

public partial class HorizontalLayout : Control
{

	private bool m_relayout = true;

	public void setRelayoutFlag()
		=> m_relayout = true;

	public override void _EnterTree()
	{
		base._EnterTree();

		this.ChildEnteredTree += (Node child) =>
		{
			Debug.WriteLine($"get {child.Name}");
			if (child is Control control)
				control.Resized += this.setRelayoutFlag;
		};
		this.ChildExitingTree += (Node child) =>
		{
			Debug.WriteLine($"lost {child.Name}");
			if (child is Control control)
				control.Resized -= this.setRelayoutFlag;
		};
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (m_relayout)
		{
			var pos = new Vector2();
			for (int i = 0; i < this.GetChildCount(); i++)
			{
				if (this.GetChild(i) is Control child)
				{
					child.Position = pos;
					pos += new Vector2(child.Size.X, 0);
				}
			}
			m_relayout = false;
		}
	}
}
