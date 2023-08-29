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
    [HideInInspector] public Bounds roomColliderBounds;

    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();

        // Save room collider bounds
        roomColliderBounds = boxCollider2D.bounds;

    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == Settings.playerTag || collision.tag == Settings.playerWeapon)
        {
            this.room.isPreviouslyVisited = true;
            StaticEventHandler.CallRoomChangedEvent(room);
        }
	}

	// Initialize The Instantiated Room
	public void Initialize(GameObject roomGameobject)
    {
        PopulateTilemapMemberVariables(roomGameobject);

        DisableCollisionTilemapRenderer();

        BlockOffUnusedDoorWays();

        AddDoorsToRooms();
	}

    public void AddDoorsToRooms()
    {
        if (room.roomNodeType.isCorridorEW || room.roomNodeType.isCorridorNS) { return; }

        foreach (Doorway doorway in room.doorWayList)
        {
            if (doorway.doorPrefab != null && doorway.isConnected)
            {
                float tileDistance = Settings.tileSizePixels / Settings.pixelsPerUnit;

                GameObject door;

                if (doorway.orientation == Orientation.north)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f, doorway.position.y + tileDistance, 0f);
                }
                else if (doorway.orientation == Orientation.south)
                {
					door = Instantiate(doorway.doorPrefab, gameObject.transform);
					door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f, doorway.position.y, 0f);
				}
                else if (doorway.orientation == Orientation.east)
                {
					door = Instantiate(doorway.doorPrefab, gameObject.transform);
					door.transform.localPosition = new Vector3(doorway.position.x + tileDistance, doorway.position.y + tileDistance * 1.25f, 0f);
				}
				else if (doorway.orientation == Orientation.west)
				{
					door = Instantiate(doorway.doorPrefab, gameObject.transform);
					door.transform.localPosition = new Vector3(doorway.position.x, doorway.position.y + tileDistance * 1.25f, 0f);
				}
			}
        }
    }


    /// <summary>
    /// Populate the tilemap and grid memeber variables.
    /// </summary>
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

    /// <summary>
    /// Disable collision tilemap renderer
    /// </summary>
    private void DisableCollisionTilemapRenderer()
    {
        // Disable collision tilemap renderer
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;

    }

    private void BlockOffUnusedDoorWays()
    {
        foreach (Doorway doorway in room.doorWayList)
        {
            if (doorway.isConnected)
                continue;

            if (collisionTilemap != null) {
                BlockDoorwayOnTilemapLayer(collisionTilemap, doorway);
            }

			if (minimapTilemap != null)
			{
				BlockDoorwayOnTilemapLayer(minimapTilemap, doorway);
			}

			if (decoration1Tilemap != null)
			{
				BlockDoorwayOnTilemapLayer(decoration1Tilemap, doorway);
			}

			if (decoration2Tilemap != null)
			{
				BlockDoorwayOnTilemapLayer(decoration2Tilemap, doorway);
			}

			if (groundTilemap != null)
			{
				BlockDoorwayOnTilemapLayer(groundTilemap, doorway);
			}

			if (frontTilemap != null)
			{
				BlockDoorwayOnTilemapLayer(frontTilemap, doorway);
			}
		}
    }

    private void BlockDoorwayOnTilemapLayer(Tilemap tilemap, Doorway doorway)
    {
		switch (doorway.orientation)
		{
			case Orientation.north:
			case Orientation.south:
				BlockDoorwayHorizontally(tilemap, doorway);
				break;

			case Orientation.east:
			case Orientation.west:
				BlockDoorwayVertically(tilemap, doorway);
				break;

			case Orientation.none:
				break;
		}

	}

	private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
        {
			for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
			{
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));
                tilemap.SetTile(new Vector3Int(startPosition.x + xPos + 1, startPosition.y - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + xPos + 1, startPosition.y - yPos, 0), transformMatrix);
			}
		}
    }

	private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
	{
		Vector2Int startPosition = doorway.doorwayStartCopyPosition;

		for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
		{
			for (int xPos = 0; xPos < doorway.doorwayCopyTileHeight; xPos++)
			{
				Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));
				tilemap.SetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos - 1, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));
				tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos - 1, 0), transformMatrix);
			}
		}
	}

}
