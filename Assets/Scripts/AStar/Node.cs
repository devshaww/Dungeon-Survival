using System;
using UnityEngine;

public class Node : IComparable<Node>
{
	public readonly Vector2Int gridPosition;
	public Node parent { get; private set; }
	public int G { get; private set; }     // left, distance from starting node to the current node
	public int H { get; private set; }     // right, distance from end node to the current node
	public int F => G + H;  // read-only
	public readonly bool isObstacle;

	public Node(Vector2Int gridPosition, bool isObstacle)
	{
		this.gridPosition = gridPosition;
		this.isObstacle = isObstacle;
		this.parent = null;
	}

	public int CompareTo(Node other)
	{
		int compare = F.CompareTo(other.F);

		if (compare == 0)
		{
			compare = H.CompareTo(other.H);
		}

		return compare;
	}

	public void SetG(int g)
	{
		G = g;
	}

	public void SetH(int h)
	{
		H = h;
	}

	public void SetParent(Node parent)
	{
		this.parent = parent;
	}
}
