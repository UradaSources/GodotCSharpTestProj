﻿using Godot;
using System.Diagnostics;

[Tool, GlobalClass]
public partial class HorizontalLayout : Control
{
	private bool m_relayout = true;

	public void setRelayoutFlag()
		=> m_relayout = true;

	private void updateLayout()
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
	}

	public override void _EnterTree()
	{
		base._EnterTree();

		this.ChildEnteredTree += (Node child) =>
		{
			if (child is Control control) 
				control.Resized += this.setRelayoutFlag;
		};
		this.ChildExitingTree += (Node child) =>
		{
			if (child is Control control) 
				control.Resized -= this.setRelayoutFlag;
		};
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (Engine.IsEditorHint())
		{
			this.updateLayout();
		}
		else
		{
			if (m_relayout)
			{
				this.updateLayout();
				m_relayout = false;
			}
		}
	}
}