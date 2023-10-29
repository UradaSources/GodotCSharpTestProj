namespace urd
{
	public class CharacterView : Renderer
	{
		[BindComponent] private Entity m_entity = null;
		[BindComponent] private Movement m_movement = null;

		[BindComponent(false)] private Navigation m_navigation = null;

		private char m_graph;

		public bool drawPathfind { set; get; }

		public override void _draw()
		{
			RenderFunc.Canvas.drawCharSprite(m_entity.coord.x, m_entity.coord.y, m_graph);

			// 绘制目标格子
			if (m_movement.processing)
			{
				var target = m_entity.getNearTile(m_movement.currentDirect);
				RenderFunc.Canvas.drawCharSprite(target.x, target.y, 'x', new Color(0, 125, 0, 125));

				// 绘制寻路路径
				if (drawPathfind && m_navigation != null)
				{
					for (int i = 0; i < m_navigation.pathNodeCount; i++)
					{
						var node = m_navigation.getPathNode(i);
						RenderFunc.Canvas.drawCharSprite(node.x, node.y, 'x', new Color(255, 255, 255, 126));
					}
				}

			}
		}

		public CharacterView(char graph, bool drawPathfind)
		{
			m_graph = graph;
			this.drawPathfind = drawPathfind;
		}
	}
}