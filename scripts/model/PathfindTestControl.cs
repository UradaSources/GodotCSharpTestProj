using System.Collections.Generic;
using System.Collections;
using Godot;

namespace urd
{
	public class PathfindTestControl : BasicMoveControl
	{
		private Pathfind m_pathfind;

		private vec2i? m_target;
		private Queue<Tile> m_path = new Queue<Tile>(10);

		public vec2i? target => m_target;

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
				foreach(var tile in m_pathfind.getPath(curTile, targetTile))
					m_path.Enqueue(tile);
			}
		}
		public void clearTaget()
		{
			m_target = null;
			m_path.Clear();
		}

		public override void _update(float dt)
		{
			if (this.motion.moveProcessing) return;

			if (m_path.Count > 0)
			{
				var t = m_path.Dequeue();

				var dir = new vec2i(t.x, t.y) - this.motion.entity.coord;
				this.motion.moveDirect = dir;
			}
			else 
			{
				var rnd = new RandomNumberGenerator();

				var world = this.motion.entity.world;

				Tile tile;
				do
				{
					int i = rnd.RandiRange(0, world.tileCount - 1);
					tile = world.rawGetTile(i);
				}
				while (!tile.pass);

				this.setTarget(new vec2i(tile.x, tile.y));
			}
		}

		public PathfindTestControl(EntityMotion motion)
			:base(motion)
		{
			var world = motion.entity.world;
			m_pathfind = new Pathfind(world);
		}
	}
}
