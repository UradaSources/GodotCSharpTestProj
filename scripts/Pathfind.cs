using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace urd
{
	public class Pathfind
	{
		private class Node
		{
			public Node parent = null;

			public int id;
			public Tile tile;

			public float g;
			public float h;

			public float f() { return this.g + this.h; }

			public Node(int id, Tile tile)
			{
				this.id = id;
				this.tile = tile;

				g = 0;
				h = 0;
			}
		}

		private static float ManhattanDistance(Tile a, Tile b)
		{
			return Mathf.Abs(b.x - a.x) + Mathf.Abs(b.y - a.y);
		}

		
		private readonly static Vector2I[] NearDirect = new Vector2I[] { 
			Vector2I.Up, Vector2I.Down, Vector2I.Left, Vector2I.Right };

		private WorldGrid m_grid;

		private SortedList<int, Node> m_nodeSet = new SortedList<int, Node>();

		private SortedList<int, Node> m_open = new SortedList<int, Node>();
		private SortedList<int, Node> m_close = new SortedList<int, Node>();

		private bool getOrBuildNode(int x, int y, out Node result)
		{
			if (m_grid.vaildCoord(x, y))
			{
				int id = m_grid.toIndex(x, y);
				if (!m_nodeSet.TryGetValue(id, out result))
				{
					var tile = m_grid.getTile(x, y);

					Node node = new Node(id, tile);
					m_nodeSet.Add(node.id, node);

					result = node;
				}
				return true;
			}
			result = null;
			return false;
		}
		
		private IEnumerable<Node> getNearNode(Node node)
		{
			foreach (var dir in NearDirect)
			{
				int x = node.tile.x + dir.X;
				int y = node.tile.y + dir.Y;

				// 获取对应方向上的临近节点, 该节点需要存在且可用
				// 且不在close列表中
				// 若满足全部条件则将其暂存到列表中
				if (this.getOrBuildNode(x, y, out var nearNode)
					&& !m_close.ContainsKey(nearNode.id))
				{
					yield return nearNode;
				}
			}
		}

		public IEnumerable<Tile> getPath(Tile start, Tile target)
		{
			m_open.Clear();
			m_close.Clear();

			// 检查起点和终点是否可到达
			Debug.Assert(this.getOrBuildNode(start.x, start.y, out Node firstNode));
			Debug.Assert(this.getOrBuildNode(target.x, target.y, out _));

			m_open.Add(firstNode.id, firstNode);

			while (m_open.Count != 0)
			{
				// 查找open列表中f值最小的节点
				// 将其从open列表中移除并加入close列表
				float fMin = float.PositiveInfinity;
				Node fMinNode = null;

				foreach (Node node in m_open.Values)
				{
					if (node.f() <= fMin)
					{
						fMin = node.f();
						fMinNode = node;
					}
				}

				m_open.Remove(fMinNode.id);
				m_close.Add(fMinNode.id, fMinNode);

				if (fMinNode.tile != target)
				{
					// 处理全部有效的临近节点
					foreach (Node nearNode in this.getNearNode(fMinNode))
					{
						if (!nearNode.tile.pass) continue;

						// 检查当前临近节点是否在开放列表中
						if (m_open.ContainsKey(nearNode.id))
						{
							// 测试新提供的g值是否令f值更小
							float gNew = fMinNode.g + 10;
							if (nearNode.g > gNew)
							{
								// 更新父节点和g值
								nearNode.parent = fMinNode;
								nearNode.g = gNew;
							}
						}
						else
						{
							nearNode.parent = fMinNode;
							nearNode.g = fMinNode.g + 10;

							// 计算当前节点到目标的距离h值,
							// 在整个算法过程中, h总是不变
							nearNode.h = ManhattanDistance(target, nearNode.tile);

							m_open.Add(nearNode.id, nearNode);
						}
					}
				}
				else
				{
					// Debug.WriteLine("pathfind done.");

					List<Tile> buf = new List<Tile>();

					Node link = fMinNode;
					do
					{
						// Debug.WriteLine($"{link.id}:({link.tile.x},{link.tile.y})-> {link.parent?.id}");

						buf.Add(link.tile);
						link = link.parent;
					}
					while (link.parent != null);

					buf.Reverse();

					foreach(var kk in buf)
						yield return kk;

					yield break;
				}
			}

			//Debug.WriteLine("pathfind faild.");

			//Debug.WriteLine("open:");
			//foreach (var node in m_open.Values)
			//{
			//	Debug.WriteLine($"{node.id}:({node.tile.x},{node.tile.y})-> {node.parent?.id}");
			//}

			//Debug.WriteLine("close:");
			//foreach (var node in m_close.Values)
			//{
			//	Debug.WriteLine($"{node.id}:({node.tile.x},{node.tile.y})-> {node.parent?.id}");
			//}

			yield break;
		}
		
		public Pathfind(WorldGrid grid)
		{
			m_grid = grid;
		}
	}
}