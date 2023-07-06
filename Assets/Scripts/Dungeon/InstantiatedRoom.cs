using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
	[HideInInspector] public Room room;
	[HideInInspector] public Grid grid;
	[HideInInspector] public Tilemap groundTilemap;
	[HideInInspector] public Tilemap decoration1Tilemap;
	[HideInInspector] public Tilemap decoration2Tilemap;
	[HideInInspector] public Tilemap frontTilemap;
	[HideInInspector] public Tilemap collisionTilemap;
	[HideInInspector] public Tilemap minimapTilemap;
	//[HideInInspector] public int[,] aStarMovementPenalty;  // use this 2d array to store movement penalties from the tilemaps to be used in AStar pathfinding
	//[HideInInspector] public int[,] aStarItemObstacles; // use to store position of moveable items that are obstacles
	[HideInInspector] public Bounds roomColliderBounds;
	//[HideInInspector] public List<MoveItem> moveableItemsList = new List<MoveItem>();

	//[Space(10)]
	//[Header("OBJECT REFERENCES")]
	//[SerializeField] private GameObject environmentGameObject;

	private BoxCollider2D boxCollider2D;

	private void Awake()
	{
		boxCollider2D = GetComponent<BoxCollider2D>();

		// Save room collider bounds
		roomColliderBounds = boxCollider2D.bounds;

	}

	//private void Start()
	//{
	//	// Update moveable item obstacles array
	//	UpdateMoveableObstacles();
	//}


	//// Trigger room changed event when player enters a room
	//private void OnTriggerEnter2D(Collider2D collision)
	//{
	//	// If the player triggered the collider
	//	if (collision.tag == Settings.playerTag && room != GameManager.Instance.GetCurrentRoom())
	//	{
	//		// Set room as visited
	//		this.room.isPreviouslyVisited = true;

	//		// Call room changed event
	//		StaticEventHandler.CallRoomChangedEvent(room);
	//	}
	//}

	public void Initialize(GameObject roomGameobject)
	{
		PopulateTilemapMemberVariables(roomGameobject);

		//BlockOffUnusedDoorWays();

		//AddObstaclesAndPreferredPaths();

		//CreateItemObstaclesArray();

		//AddDoorsToRooms();

		DisableCollisionTilemapRenderer();

	}

	private void PopulateTilemapMemberVariables(GameObject roomGameobject)
	{
		// Get the grid component.
		grid = roomGameobject.GetComponentInChildren<Grid>();

		// Get tilemaps in children.
		Tilemap[] tilemaps = roomGameobject.GetComponentsInChildren<Tilemap>();

		foreach (Tilemap tilemap in tilemaps)
		{
			if (tilemap.gameObject.tag == "groundTilemap")
			{
				groundTilemap = tilemap;
			}
			else if (tilemap.gameObject.tag == "decoration1Tilemap")
			{
				decoration1Tilemap = tilemap;
			}
			else if (tilemap.gameObject.tag == "decoration2Tilemap")
			{
				decoration2Tilemap = tilemap;
			}
			else if (tilemap.gameObject.tag == "frontTilemap")
			{
				frontTilemap = tilemap;
			}
			else if (tilemap.gameObject.tag == "collisionTilemap")
			{
				collisionTilemap = tilemap;
			}
			else if (tilemap.gameObject.tag == "minimapTilemap")
			{
				minimapTilemap = tilemap;
			}

		}
	}

	private void DisableCollisionTilemapRenderer()
	{
		// Disable collision tilemap renderer
		collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;

	}
}
