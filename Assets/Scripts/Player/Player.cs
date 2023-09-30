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
[RequireComponent(typeof(MoveEvent))]
[RequireComponent(typeof(Move))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(AnimatePlayer))]
[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(ActiveWeapon))]
[DisallowMultipleComponent]

public class Player : MonoBehaviour
{
	[HideInInspector] public PlayerDetailSO playerDetails;
	[HideInInspector] public SpriteRenderer spriteRenderer;
	[HideInInspector] public Animator animator;
	[HideInInspector] public Health health;
	[HideInInspector] public IdleEvent idleEvent;
	[HideInInspector] public MoveEvent moveEvent;
	[HideInInspector] public AimWeaponEvent aimWeaponEvent;
	[HideInInspector] public FireWeaponEvent fireWeaponEvent;
	[HideInInspector] public WeaponFiredEvent weaponFiredEvent;
	[HideInInspector] public SetActiveWeaponEvent setActiveWeaponEvent;
	[HideInInspector] public ReloadWeaponEvent reloadWeaponEvent;
	[HideInInspector] public ActiveWeapon activeWeapon;

	public List<Weapon> weaponList = new List<Weapon>();

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		health = GetComponent<Health>();
		idleEvent = GetComponent<IdleEvent>();
		moveEvent = GetComponent<MoveEvent>();
		aimWeaponEvent = GetComponent<AimWeaponEvent>();
		setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
		activeWeapon = GetComponent<ActiveWeapon>();
		weaponFiredEvent = GetComponent<WeaponFiredEvent>();
		fireWeaponEvent = GetComponent<FireWeaponEvent>();
		reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
	}

	public void Initialize(PlayerDetailSO playerDetails)
	{
		this.playerDetails = playerDetails;

		// Create player starting weapons
		CreatePlayerStartingWeapons();

		// Set player starting health
		SetPlayerHealth();
	}

	private void CreatePlayerStartingWeapons()
	{
		weaponList.Clear();
		foreach (WeaponDetailSO weaponDetails in playerDetails.startingWeaponList)
		{
			AddWeaponToPlayer(weaponDetails);
		}
	}

	private Weapon AddWeaponToPlayer(WeaponDetailSO weaponDetails)
	{
		Weapon weapon = new Weapon() { weaponDetails = weaponDetails, weaponReloadTimer = 0f, weaponClipRemainingAmmo = weaponDetails.weaponClipAmmoCapacity, weaponRemainingAmmo = weaponDetails.weaponAmmoCapacity, isWeaponReloading = false };
		weaponList.Add(weapon);

		weapon.weaponListPosition = weaponList.Count;

		setActiveWeaponEvent.Call(weapon);

		return weapon;
	}

	private void SetPlayerHealth()
	{
		health.SetMaxHealth(playerDetails.health);
	}
}
