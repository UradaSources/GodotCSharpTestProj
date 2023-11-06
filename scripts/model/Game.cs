using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using urd;
using Newtonsoft.Json;

public partial class Game : Node2D
{
	private bool m_mainLoop = true;

	private WorldGrid m_mainWorld;
	private PathGenerator m_pathGenerator;

	[Export] private ColorPicker m_colorPicker;
	[Export] private LineEdit m_inputField;

	struct SerTest
	{
		private int id;
		public string name;
	}

	public override void _Ready()
	{
		var player = new Entity("player");

		player.add(new InWorld(m_mainWorld, vec2i.zero));
		player.add(new Movement(3.0f, vec2i.zero));
		player.add(new Navigation(m_pathGenerator));
		player.add(new RandomWalkControl());

		var rng = new RandomNumberGenerator();
		for (int i = 0; i < 1; i++)
		{
			int x = rng.RandiRange(0, m_mainWorld.width - 1);
			int y = rng.RandiRange(0, m_mainWorld.height - 1);
			
			var enemy = new Entity("enemy");

			enemy.add(new InWorld(m_mainWorld, new vec2i(x, y)));
			enemy.add(new Movement(2.0f, vec2i.zero));
			enemy.add(new Navigation(m_pathGenerator));
			enemy.add(new FollowWalkControl()).target = player.get<InWorld>();
		}
	}

	public override void _Process(double delta)
	{
		if (m_mainLoop)
		{
			foreach (var en in Object.IterateObject<Entity>()
				.Where((Entity e) => e.active))
				en.update((float)delta);

			foreach (var en in Object.IterateObject<Entity>()
				.Where((Entity e) => e.active))
				en.lateUpdate((float)delta);
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
		//if (m_mainLoop)
		//{
		//	for (int i = 0; i < m_mainWorld.tileCount; i++)
		//	{
		//		var tile = m_mainWorld.rawGetTile(i);
		//		this.drawCharSprite(this, tile.x, tile.y, tile.tile.graph, tile.tile.color);
		//	}

		//	foreach (var en in m_objects)
		//		en.render();
		//}
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