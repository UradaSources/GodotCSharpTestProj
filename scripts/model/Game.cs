using System.Collections;
using System.Diagnostics;
using System.Linq;
using Godot;
using urd;

public partial class Game : Node2D
{
	private bool m_mainLoop = true;

	[Export] private World m_mainWorld;

	public override void _Ready()
	{
		var player = new ComponentContainer();

		player.addComponent(new Entity(m_mainWorld.model, vec2i.zero));
		player.addComponent(new Movement(3.0f, vec2i.zero));
		player.addComponent(new Navigation(m_mainWorld.pathGenerator));
		player.addComponent(new RandomWalkControl());

		player._init();

		var rng = new RandomNumberGenerator();
		for (int i = 0; i < 0; i++)
		{
			var enemy = new ComponentContainer();

			int x = rng.RandiRange(0, m_mainWorld.model.width - 1);
			int y = rng.RandiRange(0, m_mainWorld.model.height - 1);

			enemy.addComponent(new Entity(m_mainWorld.model, new vec2i(x, y)));
			enemy.addComponent(new Movement(2.0f, vec2i.zero));
			enemy.addComponent(new Navigation(m_mainWorld.pathGenerator));
			enemy.addComponent(new FollowWalkControl()).target = player.getComponent<Entity>();

			enemy._init();
		}
	}

	public override void _Process(double delta)
	{
		if (m_mainLoop)
		{
			foreach (var entity in Entity.IterateInstance()
				.Where((Entity en)=>en.container != null))
			{ 
				entity.container._update((float)delta);
			}
		}

		var mousePos = this.GetLocalMousePosition();
		// this.TrySelectTile(mousePos);

		//// 设置目的地
		//if (Input.IsActionJustPressed("mouse_right"))
		//{
		//	if (m_selectedTile != null && m_player.findComponent(typeof(BasicMotionControl)) == null)
		//	{
		//		var navigation = m_player.getComponent<Navigation>();
		//		navigation.setTarget(new vec2i(m_selectedTile.x, m_selectedTile.y));
		//	}
		//}

		// 暂停或是运行游戏
		if (Input.IsActionJustPressed("ui_stop"))
			m_mainLoop = !m_mainLoop;

		this.QueueRedraw();
	}
	public override void _Draw()
	{
		//foreach (var entity in Entity.IterateInstance())
		//{
		//	// 绘制角色本身
		//	this.DrawCharacterSprite(entity.coord.x, entity.coord.y, entity.name[0]);

		//	// 绘制目标格子
		//	var motion = entity.container.getComponent<Movement>();
		//	if (motion.processing)
		//	{
		//		var target = entity.getNearTile(motion.currentDirect);
		//		this.DrawCharacterSprite(target.x, target.y, 'x', new Godot.Color(0, 0.5f, 0, 0.5f));

		//		//// 绘制寻路路径
		//		//var navigation = entity.container.getComponent<Navigation>();
		//		//for (int i = 0; i < navigation.pathNodeCount; i++)
		//		//{
		//		//	var node = navigation.getPathNode(i);
		//		//	this.DrawCharacterSprite(node.x, node.y, 'x', new Color(1, 1, 1, 0.5f));
		//		//}
		//	}
		//}

		//if (m_selectedTile != null)
		//	this.DrawSelectBox(m_selectedTile.x, m_selectedTile.y, Colors.Red);
	}
}