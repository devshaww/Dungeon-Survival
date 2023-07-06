using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonobehavior<DungeonBuilder>
{
	public Dictionary<string, Room> dungeonBuilderRoomDictionary = new Dictionary<string, Room>();
	private Dictionary<string, RoomTemplateSO> roomTemplateDictionary = new Dictionary<string, RoomTemplateSO>();
	private List<RoomTemplateSO> roomTemplateList = null;
	private RoomNodeTypeListSO roomNodeTypeList;
	private bool dungeonBuildSuccessful;

	private void OnEnable()
	{
		// Set dimmed material to off
		GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 0f);
	}

	private void OnDisable()
	{
		// Set dimmed material to fully visible
		GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
	}

	protected override void Awake()
	{
		base.Awake();

		LoadRoomNodeTypeList();
		GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
	}

	private void LoadRoomNodeTypeList()
	{
		roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
	}

	public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel)
	{
		roomTemplateList = currentDungeonLevel.roomTemplateList;

		// Load the scriptable object room templates into the dictionary
		LoadRoomTemplatesIntoDictionary();

		dungeonBuildSuccessful = false;
		int dungeonBuildAttempts = 0;

		while (!dungeonBuildSuccessful && dungeonBuildAttempts < Settings.maxDungeonBuildAttempts)
		{
			dungeonBuildAttempts++;

			// Select a random room node graph from the list
			RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphList);

			int dungeonRebuildAttemptsForNodeGraph = 0;
			dungeonBuildSuccessful = false;

			// Loop until dungeon successfully built or more than max attempts for node graph
			while (!dungeonBuildSuccessful && dungeonRebuildAttemptsForNodeGraph <= Settings.maxDungeonRebuildAttemptsForRoomGraph)
			{
				// Clear dungeon room gameobjects and dungeon room dictionary
				ClearDungeon();

				dungeonRebuildAttemptsForNodeGraph++;

				// Attempt To Build A Random Dungeon For The Selected room node graph
				dungeonBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph);
			}


			if (dungeonBuildSuccessful)
			{
				// Instantiate Room Gameobjects
				Debug.Log("dungeon build successful!");
				InstantiateRoomGameobjects();
			}
		}

		return dungeonBuildSuccessful;
	}

	private void InstantiateRoomGameobjects()
	{
		// Iterate through all dungeon rooms.
		foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
		{
			Room room = keyvaluepair.Value;

			// Calculate room position (remember the room instantiatation position needs to be adjusted by the room template lower bounds)
			// Don't understand why
			Vector3 roomPosition = new Vector3(room.lowerBounds.x - room.templateLowerBounds.x, room.lowerBounds.y - room.templateLowerBounds.y, 0f);
			Debug.Log("roomPosition: " + roomPosition);

			// Instantiate room
			GameObject roomGameobject = Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);

			// Get instantiated room component from instantiated prefab.
			InstantiatedRoom instantiatedRoom = roomGameobject.GetComponentInChildren<InstantiatedRoom>();

			instantiatedRoom.room = room;

			// Initialise The Instantiated Room
			instantiatedRoom.Initialize(roomGameobject);

			// Save gameobject reference.
			room.instantiatedRoom = instantiatedRoom;

			//// Demo code to set rooms as cleared - except for boss
			//if (!room.roomNodeType.isBossRoom)
			//{
			//    room.isClearedOfEnemies = true;
			//}
		}
	}

	private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
	{

		// Create Open Room Node Queue
		Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>();

		// Add Entrance Node To Room Node Queue From Room Node Graph
		RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));

		if (entranceNode != null)
		{
			openRoomNodeQueue.Enqueue(entranceNode);
		}
		else
		{
			Debug.LogError("No Entrance Node");
			return false;  // Dungeon Not Built
		}

		// Start with no room overlaps
		bool noRoomOverlaps = true;


		// Process open room nodes queue
		noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

		// If all the room nodes have been processed and there hasn't been a room overlap then return true
		if (openRoomNodeQueue.Count == 0 && noRoomOverlaps)
		{
			return true;
		}
		else
		{
			return false;
		}

	}

	private void ClearDungeon()
	{
		// Destroy instantiated dungeon gameobjects and clear dungeon manager room dictionary
		if (dungeonBuilderRoomDictionary.Count > 0)
		{
			foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
			{
				Room room = keyvaluepair.Value;

				if (room.instantiatedRoom != null)
				{
					Destroy(room.instantiatedRoom.gameObject);
				}
			}

			dungeonBuilderRoomDictionary.Clear();
		}
	}


	private RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphList)
	{
		if (roomNodeGraphList.Count > 0)
		{
			int idx = UnityEngine.Random.Range(0, roomNodeGraphList.Count);
			Debug.Log("selected node graph idx: " + idx);
			return roomNodeGraphList[idx];
		}
		else
		{
			Debug.Log("No room node graphs in list");
			return null;
		}
	}

	private void LoadRoomTemplatesIntoDictionary()
	{
		// Clear room template dictionary
		roomTemplateDictionary.Clear();

		// Load room template list into dictionary
		foreach (RoomTemplateSO roomTemplate in roomTemplateList)
		{
			if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid))
			{
				roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
			}
			else
			{
				Debug.Log("Duplicate Room Template Key In " + roomTemplateList);
			}
		}
	}

	private bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps)
	{
		// While room nodes in open room node queue & no room overlaps detected.
		while (openRoomNodeQueue.Count > 0 && noRoomOverlaps == true)
		{
			// Get next room node from open room node queue.
			RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();

			// Add child Nodes to queue from room node graph (with links to this parent Room)
			foreach (RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
			{
				Debug.Log("Enqueue child room node");
				openRoomNodeQueue.Enqueue(childRoomNode);
			}

			// if the room is the entrance mark as positioned and add to room dictionary
			if (roomNode.roomNodeType.isEntrance)
			{
				RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);

				Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

				room.isPositioned = true;

				// Add room to room dictionary
				dungeonBuilderRoomDictionary.Add(room.id, room);
			}

			// else if the room type isn't an entrance
			else
			{
				// Else get parent room for node
				Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];

				// See if room can be placed without overlaps
				noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);
				Debug.Log("noRoomOverlaps: " + noRoomOverlaps);
			}

		}

		return noRoomOverlaps;

	}

	private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom)
	{
		// initialise and assume overlap until proven otherwise.
		bool roomOverlaps = true;

		// Do While Room Overlaps - try to place against all available doorways of the parent until
		// the room is successfully placed without overlap.
		while (roomOverlaps)
		{
			// Select random unconnected available doorway for Parent
			List<Doorway> unconnectedAvailableParentDoorways = GetUnconnectedAvailableDoorways(parentRoom.doorWayList).ToList();

			Debug.Log("unconnectedAvailableParentDoorways Count: " + unconnectedAvailableParentDoorways.Count);

			if (unconnectedAvailableParentDoorways.Count == 0)
			{
				// If no more doorways to try then overlap failure.
				return false; // room overlaps
			}

			Doorway doorwayParent = unconnectedAvailableParentDoorways[UnityEngine.Random.Range(0, unconnectedAvailableParentDoorways.Count)];

			// Get a random room template for room node that is consistent with the parent door orientation
			RoomTemplateSO roomtemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, doorwayParent);

			// Create a room
			Room room = CreateRoomFromRoomTemplate(roomtemplate, roomNode);

			// Place the room - returns true if the room doesn't overlap
			if (PlaceTheRoom(parentRoom, doorwayParent, room))
			{
				// If room doesn't overlap then set to false to exit while loop
				roomOverlaps = false;

				// Mark room as positioned
				room.isPositioned = true;

				// Add room to dictionary
				dungeonBuilderRoomDictionary.Add(room.id, room);

			}
			else
			{
				roomOverlaps = true;
			}

		}

		return true;  // no room overlaps

	}

	private bool PlaceTheRoom(Room parentRoom, Doorway doorwayParent, Room room)
	{
		Debug.Log("Place The Room");
		// Get current room doorway position
		Doorway doorway = GetOppositeDoorway(doorwayParent, room.doorWayList);

		// Return if no doorway in room opposite to parent doorway
		if (doorway == null)
		{
			// Just mark the parent doorway as unavailable so we don't try and connect it again
			doorwayParent.isUnavailable = true;

			return false;
		}

		// Calculate 'world' grid parent doorway position
		Vector2Int parentDoorwayPosition = parentRoom.lowerBounds + doorwayParent.position - parentRoom.templateLowerBounds;

		Vector2Int adjustment = Vector2Int.zero;

		// Calculate adjustment position offset based on room doorway position that we are trying to connect (e.g. if this doorway is west then we need to add (1,0) to the east parent doorway)

		switch (doorway.orientation)
		{
			case Orientation.north:
				adjustment = new Vector2Int(0, -1);
				break;

			case Orientation.east:
				adjustment = new Vector2Int(-1, 0);
				break;

			case Orientation.south:
				adjustment = new Vector2Int(0, 1);
				break;

			case Orientation.west:
				adjustment = new Vector2Int(1, 0);
				break;

			case Orientation.none:
				break;

			default:
				break;
		}

		// Calculate room lower bounds and upper bounds based on positioning to align with parent doorway
		room.lowerBounds = parentDoorwayPosition + adjustment + room.templateLowerBounds - doorway.position;
		room.upperBounds = room.lowerBounds + room.templateUpperBounds - room.templateLowerBounds;

		Room overlappingRoom = CheckForRoomOverlap(room);

		if (overlappingRoom == null)
		{
			// mark doorways as connected & unavailable
			doorwayParent.isConnected = true;
			doorwayParent.isUnavailable = true;

			doorway.isConnected = true;
			doorway.isUnavailable = true;

			// return true to show rooms have been connected with no overlap
			Debug.Log("Placed!");
			return true;
		}
		else
		{
			// Just mark the parent doorway as unavailable so we don't try and connect it again
			Debug.Log("Overlap");
			doorwayParent.isUnavailable = true;

			return false;
		}

	}

	private Doorway GetOppositeDoorway(Doorway parentDoorway, List<Doorway> doorwayList)
	{

		foreach (Doorway doorwayToCheck in doorwayList)
		{
			if (parentDoorway.orientation == Orientation.east && doorwayToCheck.orientation == Orientation.west)
			{
				return doorwayToCheck;
			}
			else if (parentDoorway.orientation == Orientation.west && doorwayToCheck.orientation == Orientation.east)
			{
				return doorwayToCheck;
			}
			else if (parentDoorway.orientation == Orientation.north && doorwayToCheck.orientation == Orientation.south)
			{
				return doorwayToCheck;
			}
			else if (parentDoorway.orientation == Orientation.south && doorwayToCheck.orientation == Orientation.north)
			{
				return doorwayToCheck;
			}
		}

		return null;

	}

	private IEnumerable<Doorway> GetUnconnectedAvailableDoorways(List<Doorway> roomDoorwayList)
	{
		// Loop through doorway list
		foreach (Doorway doorway in roomDoorwayList)
		{
			if (!doorway.isConnected && !doorway.isUnavailable)
				yield return doorway;
		}
	}

	private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
	{
		// Initialise room from template
		Room room = new Room();

		room.templateID = roomTemplate.guid;
		room.id = roomNode.id;
		room.prefab = roomTemplate.prefab;
		//room.battleMusic = roomTemplate.battleMusic;
		//room.ambientMusic = roomTemplate.ambientMusic;
		room.roomNodeType = roomTemplate.roomNodeType;
		room.lowerBounds = roomTemplate.lowerBounds;
		room.upperBounds = roomTemplate.upperBounds;
		room.spawnPositionArray = roomTemplate.spawnPositionArray;
		//room.enemiesByLevelList = roomTemplate.enemiesByLevelList;
		//room.roomLevelEnemySpawnParametersList = roomTemplate.roomEnemySpawnParametersList;
		room.templateLowerBounds = roomTemplate.lowerBounds;
		room.templateUpperBounds = roomTemplate.upperBounds;
		room.childRoomIDList = CopyStringList(roomNode.childRoomNodeIDList);
		room.doorWayList = CopyDoorwayList(roomTemplate.doorwayList);

		// Set parent ID for room
		if (roomNode.parentRoomNodeIDList.Count == 0) // Entrance
		{
			room.parentRoomID = "";
			room.isPreviouslyVisited = true;

			// Set entrance in game manager
			//GameManager.Instance.SetCurrentRoom(room);

		}
		else
		{
			room.parentRoomID = roomNode.parentRoomNodeIDList[0];
		}


		// If there are no enemies to spawn then default the room to be clear of enemies
		//if (room.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel()) == 0)
		//{
		//	room.isClearedOfEnemies = true;
		//}


		return room;

	}

	private List<Doorway> CopyDoorwayList(List<Doorway> oldDoorwayList)
	{
		List<Doorway> newDoorwayList = new List<Doorway>();

		foreach (Doorway doorway in oldDoorwayList)
		{
			Doorway newDoorway = new Doorway();

			newDoorway.position = doorway.position;
			newDoorway.orientation = doorway.orientation;
			newDoorway.doorPrefab = doorway.doorPrefab;
			newDoorway.isConnected = doorway.isConnected;
			newDoorway.isUnavailable = doorway.isUnavailable;
			newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
			newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;
			newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;

			newDoorwayList.Add(newDoorway);
		}

		return newDoorwayList;
	}

	private List<string> CopyStringList(List<string> oldStringList)
	{
		List<string> newStringList = new List<string>();

		foreach (string stringValue in oldStringList)
		{
			newStringList.Add(stringValue);
		}

		return newStringList;
	}


	private RoomTemplateSO GetRandomTemplateForRoomConsistentWithParent(RoomNodeSO roomNode, Doorway doorwayParent)
	{
		RoomTemplateSO roomtemplate = null;

		// If room node is a corridor then select random correct Corridor room template based on
		// parent doorway orientation
		if (roomNode.roomNodeType.isCorridor)
		{
			switch (doorwayParent.orientation)
			{
				case Orientation.north:
				case Orientation.south:
					roomtemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorNS));
					break;


				case Orientation.east:
				case Orientation.west:
					roomtemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorEW));
					break;


				case Orientation.none:
					break;

				default:
					break;
			}
		}
		// Else select random room template
		else
		{
			roomtemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
		}


		return roomtemplate;
	}

	private Room CheckForRoomOverlap(Room roomToTest)
	{
		// Iterate through all rooms
		foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
		{
			Room room = keyvaluepair.Value;

			// skip if same room as room to test or room hasn't been positioned
			if (room.id == roomToTest.id || !room.isPositioned)
				continue;

			// If room overlaps
			if (IsOverLappingRoom(roomToTest, room))
			{
				return room;
			}
		}


		// Return
		return null;

	}

	private bool IsOverLappingInterval(int imin1, int imax1, int imin2, int imax2)
	{
		if (Mathf.Max(imin1, imin2) <= Mathf.Min(imax1, imax2))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	private bool IsOverLappingRoom(Room room1, Room room2)
	{
		bool isOverlappingX = IsOverLappingInterval(room1.lowerBounds.x, room1.upperBounds.x, room2.lowerBounds.x, room2.upperBounds.x);

		bool isOverlappingY = IsOverLappingInterval(room1.lowerBounds.y, room1.upperBounds.y, room2.lowerBounds.y, room2.upperBounds.y);

		if (isOverlappingX && isOverlappingY)
		{
			return true;
		}
		else
		{
			return false;
		}

	}

	private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType)
	{
		List<RoomTemplateSO> matchingRoomTemplateList = new List<RoomTemplateSO>();

		// Loop through room template list
		foreach (RoomTemplateSO roomTemplate in roomTemplateList)
		{
			// Add matching room templates
			if (roomTemplate.roomNodeType == roomNodeType)
			{
				matchingRoomTemplateList.Add(roomTemplate);
			}
		}

		// Return null if list is zero
		if (matchingRoomTemplateList.Count == 0)
			return null;

		// Select random room template from list and return
		return matchingRoomTemplateList[UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)];

	}


}
