using Cinemachine;
using UnityEngine;

[DisallowMultipleComponent]
public class Minimap : MonoBehaviour
{
    [SerializeField] private GameObject minimapPlayer;

    private Transform playerTransform;

	private void Start()
	{
		playerTransform = GameManager.Instance.GetPlayer().transform;
		CinemachineVirtualCamera cinemachineVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
		cinemachineVirtualCamera.Follow = playerTransform;

		SpriteRenderer spriteRenderer = minimapPlayer.GetComponent<SpriteRenderer>();
		if (spriteRenderer != null)
		{
			spriteRenderer.sprite = GameManager.Instance.GetPlayerMinimapIcon();
		}
	}

	private void Update()
	{
		if (playerTransform != null && minimapPlayer != null)
		{
			minimapPlayer.transform.position = playerTransform.position;
		}	
	}
}
