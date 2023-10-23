using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace urd
{
	public class EntityMoveToward : Component
	{
		private Pathfind m_pathfind;
		private EntityMotion m_motion;

		private vec2i? m_target;
		private List<TileCell> m_pathNodeList;

		public vec2i? target => m_target;
		public int pathNodeCount => m_pathNodeList.Count;

		public TileCell getPathNode(int index)
		{
			Debug.Assert(index >= 0 && index < m_pathNodeList.Count, $"invaild pathnode index: {index}");
			return m_pathNodeList[index];
		}

		public void setTarget(vec2i target)
		{
			if (m_target != target)
			{
				GD.Print($"set path target {target}");

				var entity = m_motion.entity;
				var world = entity.world;

				var curTile = world.getTile(entity.coord.x, entity.coord.y);
				var targetTile = world.getTile(target.x, target.y);

				// 清除旧目标
				this.clearData();

				m_target = target;
				foreach (var tile in m_pathfind.getPath(curTile, targetTile))
					m_pathNodeList.Add(tile);
			}
		}
		public void clearData()
		{
			m_target = null;
			m_pathNodeList.Clear();
		}

		public override void _init()
		{
			m_motion = this.container.getComponent<EntityMotion>();
			Debug.Assert(m_motion != null);
		}
		public override void _update(float delta)
		{
			DebugDisplay.Main.outObject(this);
			DebugDisplay.Main.outString("pathcount", $"{m_pathNodeList.Count}");

			if (m_motion.processing) return;

			if (m_pathNodeList.Count > 0)
			{
				var t = m_pathNodeList[m_pathNodeList.Count - 1];
				if (t.type.cost < 0)
				{
					this.clearData();
					m_motion.moveDirect = vec2i.zero;

					return;
				}
				m_pathNodeList.RemoveAt(m_pathNodeList.Count - 1);

				var dir = new vec2i(t.x, t.y) - m_motion.entity.coord;
				m_motion.moveDirect = dir;
			}
			else
			{
				m_motion.moveDirect = vec2i.zero;
			}
		}

		public EntityMoveToward(Pathfind pathfind)
		{
			m_pathfind = pathfind;
			m_pathNodeList = new List<TileCell>();
		}
	}
}