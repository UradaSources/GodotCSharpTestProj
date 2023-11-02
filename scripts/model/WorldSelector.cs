using Godot;
using urd;

public partial class WorldSelector : Node2D
{
	private WorldNode m_world;

	private TileCell m_selectedTile;

	public TileCell selectedTile => m_selectedTile;

	private bool m_processing;

	public override void _Ready()
	{
		base._Ready();

		m_world = this.GetParent<WorldNode>();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		var mouse = this.GetGlobalMousePosition();
		var coord = m_world.mapToCoord(mouse);

		m_world.model.tryGetTile(coord.X, coord.Y, out m_selectedTile);

		this.QueueRedraw();
	}

	public override void _Draw()
	{
		base._Draw();

		if (this.selectedTile != null)
			m_world.drawBox(this, this.selectedTile.x, this.selectedTile.y, Colors.White, false);
	}
}
