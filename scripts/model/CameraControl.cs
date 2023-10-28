using Godot;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace urd
{
	public partial class CameraControl : Camera2D
	{
		const string CameraDataFilePath = "./save/camera.json";

		[System.Serializable]
		private struct SerializeData
		{
			[JsonInclude] public float pos_x;
			[JsonInclude] public float pos_y;
			[JsonInclude] public float zoom_x;
			[JsonInclude] public float zoom_y;
		}

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

			if (Input.IsActionJustPressed("mouse_zoom_in"))
				this.Zoom += Vector2.One * m_zoomSpeed * (float)delta;
			else if (Input.IsActionJustPressed("mouse_zoom_out"))
				this.Zoom -= Vector2.One * m_zoomSpeed * (float)delta;
		}

		public override void _EnterTree()
		{
			if (System.IO.File.Exists(CameraDataFilePath))
			{
				var json = System.IO.File.ReadAllText(CameraDataFilePath);
				var data = JsonSerializer.Deserialize<SerializeData>(json);

				this.Position = new Vector2(data.pos_x, data.pos_y);
				this.Zoom = new Vector2(data.zoom_x, data.zoom_y);
			}
		}
		public override void _ExitTree()
		{
			var data = new SerializeData {
				pos_x = this.Position.X,
				pos_y = this.Position.Y,
				zoom_x = this.Zoom.X,
				zoom_y = this.Zoom.Y
			};
			var json = JsonSerializer.Serialize(data);
			System.IO.File.WriteAllText(CameraDataFilePath, json);
		}
	}
}