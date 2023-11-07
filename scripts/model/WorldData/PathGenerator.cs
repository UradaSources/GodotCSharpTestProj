using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace urd
{
	public class PathGenerator
	{
		private class Node
		{
			public readonly int id;
			public readonly TileCell tile;

			public Node parent = null;
			public float g;
			public float h;

			public float f => this.g + this.h;

			public Node(int id, TileCell tile)
			{
				this.id = id;
				this.tile = tile;

				g = 0;
				h = 0;
			}

			public override string ToString()
			{
				return $"{id} :({tile.x},{tile.y}) :g={g} h={h} :f={this.f} :parent={parent?.id}";
			}
		}

		private readonly static Vector2I[] NearDirect = new Vector2I[] {
			Vector2I.Up, Vector2I.Down, Vector2I.Left, Vector2I.Right };

		private WorldGrid m_world;

		private SortedList<int, Node> m_nodeSet = new SortedList<int, Node>();

		private SortedList<int, Node> m_open = new SortedList<int, Node>();
		private HashSet<int> m_closedNodeIndex = new HashSet<int>();

		private bool getOrBuildNode(int x, int y, out Node result)
		{
			if (m_world.vaildCoord(x, y))
			{
				int id = m_world.toIndex(x, y);
				if (!m_nodeSet.TryGetValue(id, out result))
				{
					var tile = m_world.getTile(x, y);

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
					&& !m_closedNodeIndex.Contains(nearNode.id))
				{
					yield return nearNode;
				}
			}
		}

		public int generatePath(vec2i start, vec2i target, ref List<TileCell> path, IPathfindCost costCalculator)
		{
			m_open.Clear();
			m_closedNodeIndex.Clear();

			costCalculator.init(m_world, start, target);

			// 检查起点和终点是否可到达
			if (!this.getOrBuildNode(start.x, start.y, out Node firstNode))
			{
				Debug.WriteLine($"invaild start({start})", $"{this.GetType().Name}.Warn");
				return 0;
			}
			if (!this.getOrBuildNode(target.x, target.y, out Node targetNode))
			{ 
				Debug.WriteLine($"invaild target({target})", $"{this.GetType().Name}.Warn");
				return 0;
			}

			firstNode.g = 0;
			firstNode.h = 0;
			firstNode.parent = null;

			m_open.Add(firstNode.id, firstNode);

			while (m_open.Count != 0)
			{
				// 查找open列表中f值最小的节点
				// 将其从open列表中移除并加入close列表
				float fMin = float.PositiveInfinity;
				Node fMinNode = null;

				foreach (Node node in m_open.Values)
				{
					if (node.f <= fMin)
					{
						fMin = node.f;
						fMinNode = node;
					}
				}

				m_open.Remove(fMinNode.id);
				m_closedNodeIndex.Add(fMinNode.id);

				if (fMinNode != targetNode)
				{
					// 处理全部有效的临近节点
					foreach (Node nearNode in this.getNearNode(fMinNode))
					{
						// 检查瓦片是否可通行
						var cost = costCalculator.tileCost(fMinNode.tile, nearNode.tile);
						if (cost < 0) continue;

						// 检查当前临近节点是否在开放列表中
						if (m_open.ContainsKey(nearNode.id))
						{
							// 测试新提供的g值是否令f值更小
							// 在整个寻路过程中, h应该保持不变, 所以简单的测试g值是否更小
							float gNew = fMinNode.g + cost;
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
							nearNode.g = fMinNode.g + cost;

							// 计算当前节点到目标的距离h值,
							// 在整个算法过程中, h总是不变
							nearNode.h = costCalculator.hValue(nearNode.tile, targetNode.tile);

							m_open.Add(nearNode.id, nearNode);
						}
					}
				}
				else
				{
#if false && DEBUG
					Debug.WriteLine($"path generate success({start}-{target}):\n" +
						$"open: {{{string.Join(", ", m_open.Values)}}}\n" +
						$"closed index: {{{string.Join(", ", m_closedNodeIndex)}}}\n",
						$"{this.GetType().Name}.Info");
#endif

					int count = path.Count;

					Node link = fMinNode;
					while (link != null)
					{
						path.Add(link.tile);
						link = link.parent;
					}

					return path.Count - count;
				}
			}

#if false && DEBUG
			Debug.WriteLine($"path generate faild({start}-{target}): \n" +
				$"open: {{{string.Join(", ", m_open.Values)}}}\n" +
				$"closed index: {{{string.Join(", ", m_closedNodeIndex)}}}\n",
				$"{this.GetType().Name}.Warn");
#endif

			return 0;
		}

		public PathGenerator(WorldGrid grid)
		{
			Debug.Assert(grid != null);

			m_world = grid;
		}
	}

}