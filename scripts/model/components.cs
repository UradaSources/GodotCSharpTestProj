using System.Collections;
using System.Collections.Generic;

namespace urd.model
{
	public struct com_cacheMoveControlInput { public vec2i target_move_direct; }

	public struct com_worldRef { public WorldGrid world; }
	public struct com_coord { public int x, y; }

	public struct com_moveProcess
	{
		public bool processing;
		public vec2 position;
	}

	public struct com_moveSpeed { public float speed; }
	public struct com_moveDirect { public vec2i direct; }
}
