using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using Arch.Core;
using Godot;

namespace urd.model
{
	public static class Time
	{
		public static float deltaTime;
	}

	public struct sys_updateMoveProcess 
		: IForEachWithEntity<com_moveProcess, com_coord, com_moveSpeed>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Update(in Entity entity, 
			ref com_moveProcess process, 
			ref com_coord coord, 
			ref com_moveSpeed motionSpeed)
		{
			if (!process.processing) return;

			float posDelta = motionSpeed.speed * Time.deltaTime;

			vec2 targetPos = new vec2i(coord.x, coord.y);
			process.position = vec2.MoveTowards(process.position, targetPos, posDelta);

			if (process.position == targetPos)
				process.processing = false;
		}
	}
	public struct sys_updateMoveDirect 
		: IForEachWithEntity<com_cacheMoveControlInput, com_worldRef, com_coord, com_moveDirect>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Update(in Entity entity,
			ref com_cacheMoveControlInput inp, 
			ref com_worldRef worldRef,
			ref com_coord coord,
			ref com_moveDirect motion)
		{
			int x = coord.x + inp.target_move_direct.x;
			int y = coord.y + inp.target_move_direct.y;

			GD.Print($"{entity.Id}, {coord.x}, {worldRef.world}.");

			if (worldRef.world.tryGetTile(x, y, out var tile) && tile.pass)
				motion.direct = inp.target_move_direct;
		}
	}

	public struct sys_startMoveProcess 
		: IForEachWithEntity<com_moveProcess, com_worldRef, com_coord, com_moveDirect>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Update(in Entity entity, 
			ref com_moveProcess process, 
			ref com_worldRef worldRef,
			ref com_coord coord, 
			ref com_moveDirect moveDirect)
		{
			if (process.processing) return;

			int xNext = coord.x + moveDirect.direct.x;
			int yNext = coord.y + moveDirect.direct.y;

			if (worldRef.world.tryGetTile(xNext, yNext, out var tile) && tile.pass)
			{
				coord.x = xNext;
				coord.y = yNext;

				process.processing = true;
				process.position = new vec2(coord.x, coord.y);
			}
		}
	}

	public struct sys_updateMoveControlInput 
		: IForEachWithEntity<com_cacheMoveControlInput>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Update(in Entity entity, 
			ref com_cacheMoveControlInput cache)
		{
			if (Godot.Input.IsActionPressed("ui_up"))
				cache.target_move_direct = vec2i.up;
			else if (Godot.Input.IsActionPressed("ui_down"))
				cache.target_move_direct = vec2i.down;
			else if (Godot.Input.IsActionPressed("ui_left"))
				cache.target_move_direct = vec2i.left;
			else if (Godot.Input.IsActionPressed("ui_right"))
				cache.target_move_direct = vec2i.right;
			else
				cache.target_move_direct = vec2i.zero;
		}
	}
}
