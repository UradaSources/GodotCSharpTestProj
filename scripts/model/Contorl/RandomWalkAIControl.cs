using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using Godot;
using urd;

public class EntityMoveToward
{
	private Pathfind m_pathfind;
	private EntityMotion m_motion;


}

public class RandomWalkAIControl : BasicMoveControl
{
	private Pathfind m_pathfind;

	private vec2i? m_target;
	private List<TileCell> m_path = new List<TileCell>(10);

	public bool walk { set; get; }
	public vec2i? target => m_target;

	public int pathNodeCount => m_path.Count;
	
	public TileCell getPathNode(int index)
	{
		Debug.Assert(index >= 0 && index < m_path.Count, $"invaild pathnode index: {index}");
		return m_path[index];
	}

	public void setTarget(vec2i target)
	{
		if (m_target != target)
		{
			GD.Print($"set path target {target}");

			var entity = this.motion.entity;
			var world = entity.world;

			var curTile = world.getTile(entity.coord.x, entity.coord.y);
			var targetTile = world.getTile(target.x, target.y);

			// 清除旧目标
			this.clearTaget();

			m_target = target;
			foreach (var tile in m_pathfind.getPath(curTile, targetTile))
				m_path.Add(tile);
		}
	}
	public void clearTaget()
	{
		m_target = null;
		m_path.Clear();
	}

	public override void _update(float dt)
	{
		DebugDisplay.Main.outObject(this);
		DebugDisplay.Main.outString("pathcount", $"{m_path.Count}");

		if (this.motion.processing) return;

		if (m_path.Count > 0)
		{
			var t = m_path[m_path.Count - 1];
			m_path.RemoveAt(m_path.Count - 1);

			var dir = new vec2i(t.x, t.y) - this.motion.entity.coord;
			this.motion.moveDirect = dir;
		}
		else if(false)
		{
			var rnd = new RandomNumberGenerator();

			var world = this.motion.entity.world;

			TileCell tile;
			do
			{
				int i = rnd.RandiRange(0, world.tileCount - 1);
				tile = world.rawGetTile(i);
			}
			while (tile.type.cost < 0);

			this.setTarget(new vec2i(tile.x, tile.y));
		}
		else
		{
			this.motion.moveDirect = vec2i.zero;
		}
	}

	public RandomWalkAIControl(EntityMotion motion)
		: base(motion)
	{
		var world = motion.entity.world;
		m_pathfind = new Pathfind(world);
	}
}
