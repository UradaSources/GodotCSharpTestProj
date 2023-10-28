using Godot;

namespace urd
{
	public partial class ViewDrag : Camera2D
	{
		[Export] private float m_dragFactor = 1;
		[Export] private float m_zoomSpeed = 1;

		private Vector2 m_offset;
		private Vector2 m_startPos;

		private bool m_draging;

		public override void _Process(double delta)
		{
			if (m_draging)
			{
				if (!Input.IsActionPressed("mouse_drag"))
				{
					m_draging = false;
					return;
				}

				var mousePos = this.GetLocalMousePosition();
				var dir = m_startPos - mousePos;

				this.Position = m_startPos + dir * m_dragFactor - m_offset;
			}
			else if (Input.IsActionJustPressed("mouse_drag"))
			{
				m_draging = true;

				m_startPos = this.GetLocalMousePosition();
				m_offset = m_startPos - this.Position;
			}

			if (Input.IsActionJustPressed("mouse_zoom_add"))
				this.Zoom += Vector2.One * m_zoomSpeed * (float)delta;
			else if (Input.IsActionJustPressed("mouse_zoom_sub"))
				this.Zoom -= Vector2.One * m_zoomSpeed * (float)delta;
		}
	}
}