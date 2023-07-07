using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]

[DisallowMultipleComponent]

public class Player : MonoBehaviour
{
	[HideInInspector] public PlayerDetailSO playerDetails;
	[HideInInspector] public SpriteRenderer spriteRenderer;
	[HideInInspector] public Animator animator;
	[HideInInspector] public Health health;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
	}

	public void Initialize(PlayerDetailSO playerDetails)
	{
		this.playerDetails = playerDetails;

		//Create player starting weapons
		//CreatePlayerStartingWeapons();


		// Set player starting health
		SetPlayerHealth();
	}

	private void SetPlayerHealth()
	{
		health.SetMaxHealth(playerDetails.health);
	}
}
