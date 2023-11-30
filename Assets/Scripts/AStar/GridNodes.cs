using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNodes
{
    private int width;
    private int height;

    private Node[,] gridNode;
    private List<Vector2Int> obstacles;

    public GridNodes(int width, int height)
    {
        obstacles = new List<Vector2Int>();
        this.width = width;
        this.height = height;

        gridNode = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
			for (int y = 0; y < height; y++)
			{
                bool isObstacle = obstacles.Contains(new Vector2Int(x, y)); 
                gridNode[x, y] = new Node(new Vector2Int(x, y), isObstacle);
			}
		}      
    }

    public Node GetGridNode(int x, int y)
    {
        if (x < width && y < height)
        {
			return gridNode[x, y];
		}

        Debug.Log("Requested node is out of range");
        return null;
    }
}
