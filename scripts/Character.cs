using System.Collections;
using System.Collections.Generic;
using Godot;

namespace urd
{
	public partial class Character : Node2D
	{
		[Export] private World m_world;
		[Export] private float m_moveSpeed;

		private Vector2I m_coord;
		private Vector2I m_moveDirect;

		private bool m_moveProcessing;

		public Vector2I coord => m_coord;
		public bool moveProcessing => m_moveProcessing;

		public override void _Process(double delta)
		{
			if (m_moveProcessing)
			{
				var targetPos = m_world.CoordMapToPosition(this.coord.X, this.coord.Y);
				this.Position = this.Position.MoveToward(targetPos, m_moveSpeed * (float)delta);

				if (this.Position == targetPos)
					m_moveProcessing = false;
			}
			else
			{
				var targetCoord = m_coord + m_moveDirect;
				if (m_world.grid.tryGetTile(targetCoord.X, targetCoord.Y, out var tile) && tile.pass)
				{
					m_coord += m_moveDirect;
					m_moveProcessing = true;
				}
			}

			if (Input.IsActionPressed("ui_up"))
				m_moveDirect = Vector2I.Up;
			else if (Input.IsActionPressed("ui_down"))
				m_moveDirect = Vector2I.Down;
			else if (Input.IsActionPressed("ui_left"))
				m_moveDirect = Vector2I.Left;
			else if (Input.IsActionPressed("ui_right"))
				m_moveDirect = Vector2I.Right;

			GD.Print($"{this.coord}, {this.Position}, {this.m_moveDirect}");

			this.QueueRedraw();
		}

		public override void _Draw()
		{
			m_world.DrawSprite(this.Position, '@', this);
		}
	}
}
