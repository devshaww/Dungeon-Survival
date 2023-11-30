using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    [Space(10)]
    [Header("DUNGEON LEVELS")]
    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

    [SerializeField] private int currentDungeonLevelListIndex = 0;

	private Room currentRoom;
	private Room previousRoom;
	private PlayerDetailSO playerDetails;
	private Player player;

	[HideInInspector] public GameState gameState;


	protected override void Awake()
	{
        base.Awake();

        playerDetails = GameResources.Instance.currentPlayer.playerDetails;
        InstantiatePlayer();
	}

    private void InstantiatePlayer()
    {
        GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);
        player = playerGameObject.GetComponent<Player>();
        player.Initialize(playerDetails);
    }

    public Room GetCurrentRoom()
    {
        return currentRoom;
    }

	public void SetCurrentRoom(Room room)
	{
        previousRoom = currentRoom;
        currentRoom = room;
	}

	public Player GetPlayer()
	{
		return player;
	}

    public Sprite GetPlayerMinimapIcon()
    {
        return playerDetails.playerMiniMapIcon;
    }

	// Start is called before the first frame update
	private void Start()
    {
        gameState = GameState.gameStarted;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateGameState();

        // For testing
        if (Input.GetKeyDown(KeyCode.P))
        {
            gameState = GameState.gameStarted;
        }

    }

    private void UpdateGameState()
    {
        switch (gameState)
        {
            case GameState.gameStarted:

                // Play first level
                PlayDungeonLevel(currentDungeonLevelListIndex);

                gameState = GameState.playingLevel;

                break;

        }
    }


    private void PlayDungeonLevel(int dungeonLevelListIndex)
    {
        // Build dungeon for level
        bool dungeonBuiltSucessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);

        if (!dungeonBuiltSucessfully)
        {
            Debug.LogError("Couldn't build dungeon from specified rooms and node graphs");
        }

        player.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f, (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0f);
        player.gameObject.transform.position = HelperUtilities.GetSpawnPositionNearestTo(player.gameObject.transform.position);

	}


    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }

#endif

    #endregion Validation

}

