using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(AnimatePlayer))]
[RequireComponent(typeof(PlayerControl))]
[DisallowMultipleComponent]

public class Player : MonoBehaviour
{
	[HideInInspector] public PlayerDetailSO playerDetails;
	[HideInInspector] public SpriteRenderer spriteRenderer;
	[HideInInspector] public Animator animator;
	[HideInInspector] public Health health;
	[HideInInspector] public IdleEvent idleEvent;
	[HideInInspector] public AimWeaponEvent aimWeaponEvent;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		health = GetComponent<Health>();
		idleEvent = GetComponent<IdleEvent>();
		aimWeaponEvent = GetComponent<AimWeaponEvent>();
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
