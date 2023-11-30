using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    public static Stack<Vector3> BuildPath(Room room, Vector3Int start, Vector3Int end)
    {
        start -= (Vector3Int)room.templateLowerBounds;
        end -= (Vector3Int)room.templateLowerBounds;

        GridNodes gridNodes = new GridNodes(room.templateUpperBounds.x - room.templateLowerBounds.x + 1, room.templateUpperBounds.y - room.templateLowerBounds.y + 1);

        Node startNode = gridNodes.GetGridNode(start.x, start.y);
        Node endNode = gridNodes.GetGridNode(end.x, end.y);

        Node endPathNode = FindShortestPath(startNode, endNode, gridNodes, room.instantiatedRoom);

        if (endPathNode != null)
        {
            return CreatePathStack(endPathNode, room);
        }

        return null;
    }

    private static Node FindShortestPath(Node start, Node end, GridNodes gridNodes, InstantiatedRoom room)
    {
		List<Node> open = new List<Node>() { start };
		List<Node> closed = new List<Node>();

        Node current = null;
        while (open.Count > 0)
        {
            open.Sort();

            current = open[0];
            open.RemoveAt(0);

            closed.Add(current);
            if (current == end)
            {
                return current;
            }
        }

        int x = start.gridPosition.x;
        int y = start.gridPosition.y;

        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                if (i == 0 && j == 0) continue;
                Node neighbor = gridNodes.GetGridNode(x + i, y + j);
                if (neighbor != null) {
                    if (neighbor.isObstacle || closed.Contains(neighbor)) continue;
                    bool isNeighborInOpenList = open.Contains(neighbor);
                    if (current.G + GetDistance(current, end) < neighbor.G || !isNeighborInOpenList) {
                        neighbor.SetG(current.G + GetDistance(current, end));
                        neighbor.SetParent(current);
                        if (!isNeighborInOpenList) {
                            open.Add(neighbor);
                        }
                    }
                }
            }
        }

        return null;
    }

    private static int GetDistance(Node start, Node target) {
        int dstX = Mathf.Abs(target.gridPosition.x - start.gridPosition.x);
        int dstY = Mathf.Abs(target.gridPosition.y - start.gridPosition.y);

        if (dstX > dstY)
            return 14 * dstY + (dstY - dstX) * 10;
        return 14 * dstX + (dstX - dstY) * 10;
    }

    private static Stack<Vector3> CreatePathStack(Node end, Room room) {
        return null;
    }
}
