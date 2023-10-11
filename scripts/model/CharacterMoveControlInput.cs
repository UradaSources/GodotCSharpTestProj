using Godot;

namespace urd
{
	public class CharacterMoveControlInput: IComponent
	{
		[Export] private Character m_character;

		private vec2i m_cacheMoveDirect;

		public void _update(float dt)
		{
			// 缓存移动方向的输入
			if (Input.IsActionPressed("ui_up"))
				m_cacheMoveDirect = vec2i.up;
			else if (Input.IsActionPressed("ui_down"))
				m_cacheMoveDirect = vec2i.down;
			else if (Input.IsActionPressed("ui_left"))
				m_cacheMoveDirect = vec2i.left;
			else if (Input.IsActionPressed("ui_right"))
				m_cacheMoveDirect = vec2i.right;

			// 尝试切换移动方向到目标移动方向
			vec2i peekTargetCoord = m_character.coord + m_cacheMoveDirect;
			if (m_character.world.tryGetTile(peekTargetCoord.x, peekTargetCoord.y, out var tile) && tile.pass)
			{
				m_character.moveDirect = m_cacheMoveDirect;
			}
		}
	}
}
