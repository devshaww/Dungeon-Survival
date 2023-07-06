using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehavior<GameManager>
{
	//[Space(10)]
	//[Header("GAMEOBJECT REFERENCES")]
	//[SerializeField] private GameObject pauseMenu;

	//[SerializeField] private TextMeshProUGUI messageTextTMP;

	//[SerializeField] private CanvasGroup canvasGroup;


	[Space(10)]
	[Header("DUNGEON LEVELS")]
	[SerializeField] private List<DungeonLevelSO> dungeonLevelList;

	[SerializeField] private int currentDungeonLevelListIndex = 0;
	//private Room currentRoom;
	//private Room previousRoom;
	//private PlayerDetailsSO playerDetails;
	//private Player player;

	[HideInInspector] public GameState gameState;
	[HideInInspector] public GameState previousGameState;
	//private long gameScore;
	//private int scoreMultiplier;
	//private InstantiatedRoom bossRoom;
	//private bool isFading = false;

	private void Start()
    {
		gameState = GameState.gameStarted;
    }

    private void Update()
    {
		UpdateGameState();

		if (Input.GetKeyDown(KeyCode.R))
		{
			gameState = GameState.gameStarted;
		}
    }

	private void UpdateGameState()
	{
		switch (gameState)
		{
			case GameState.gameStarted:
				PlayDungeonLevel(currentDungeonLevelListIndex);
				gameState = GameState.playingLevel;
				break;
		}
	}

	private void PlayDungeonLevel(int idx)
	{
		bool dungeonBuiltSuccessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[idx]);

		if (!dungeonBuiltSuccessfully)
		{
			Debug.LogError("Couldn't build dungeon from specified rooms and node graphs");
		}
	}
}
