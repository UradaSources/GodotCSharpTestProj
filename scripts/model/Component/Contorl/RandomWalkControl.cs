using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

namespace urd
{
	public class RandomWalkControl : BasicMotionControl
	{
		[BindComponent] private Entity m_entity;
		[BindComponent] private Navigation m_moveToward;

		private Godot.RandomNumberGenerator m_rng;
		private float m_timer = 0;

		public override void _update(float dt)
		{
			if (m_motion.processing) return;
			if (m_timer > 0)
			{
				m_timer -= dt;
				return;
			}

			var world = m_entity.world;

			TileCell tile;
			do
			{
				int i = m_rng.RandiRange(0, world.tileCount - 1);
				tile = world.rawGetTile(i);
			}
			while (tile.type.cost < 0);

			var target = new vec2i(tile.x, tile.y);
			m_moveToward.setTarget(target);

			m_timer = m_rng.RandfRange(0.0f, 4.0f);
		}

		public RandomWalkControl()
		{
			m_rng = new Godot.RandomNumberGenerator();
		}
	}
}