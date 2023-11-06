using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

namespace urd
{
	public class RandomWalkControl : BasicMotionControl
	{
		[BindComponent] private InWorld m_inWorld = null;
		[BindComponent] private Navigation m_moveToward = null;

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

			var world = m_inWorld.world;

			TileCell tile;
			do
			{
				int i = m_rng.RandiRange(0, world.tileCount - 1);
				tile = world.rawGetTile(i);
			}
			while (tile.tile.cost < 0);

			var target = new vec2i(tile.x, tile.y);
			m_moveToward.setTarget(target);

			m_timer = m_rng.RandfRange(0.0f, 4.0f);
		}

		public RandomWalkControl() : base("RandomWalkControl")
		{
			m_rng = new Godot.RandomNumberGenerator();
		}
	}
}