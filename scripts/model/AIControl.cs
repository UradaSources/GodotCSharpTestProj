using System.Collections.Generic;
using System.Collections;

namespace urd
{
	public class AIControl : IComponent
	{
		private Character m_character;
		private Pathfind m_pathfind;

		private vec2i? m_target;
		private IEnumerator<Tile> m_process;

		public void setTarget(vec2i target)
		{
			if (m_target != target)
			{
				var curTile = m_character.world.getTile(m_character.coord.x, m_character.coord.y);
				var targetTile = m_character.world.getTile(target.x, target.y);
				
				m_target = target;

				m_process?.Dispose();
				m_process = m_pathfind.getPath(curTile, targetTile).GetEnumerator();
			}
		}
		public void clearTaget()
		{
			m_target = null;
			m_process?.Dispose();
		}

		public void _update(float dt)
		{
			if (m_character.moveProcessing) return;

			m_process.MoveNext();
			var t = m_process.Current;

			var dir = new vec2i(t.x, t.y) - m_character.coord;
			m_character.moveDirect = dir;
		}

		public AIControl(Character character, Pathfind pathfind)
		{
			m_character = character;
			m_pathfind = pathfind;
		}
		~AIControl()
		{
			m_process?.Dispose();
		}
	}
}
