using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
	[SerializeField] private MovementDetailSO movementDetails;
	private Player player;
	private bool leftMouseDownPreviousFrame = false;
	private float moveSpeed;
    private int currentWeaponIndex = 1;

	private void Start()
	{
        SetPlayerAnimationSpeed();

        SetStartingWeapon();
	}

	private void Awake()
	{
        player = GetComponent<Player>();

        moveSpeed = movementDetails.moveSpeed;
	}

    // Update is called once per frame
    void Update()
    {
        MovementInput();
        WeaponInput();
	}

    // Set index of the starting weapon
	private void SetStartingWeapon()
    {
        int index = 0;
		foreach (Weapon weapon in player.weaponList)
		{
			if (weapon.weaponDetails == player.playerDetails.startingWeapon)
            {
                SetWeaponByIndex(index);
                break;
            }
            index++;
		}
	}

	private void SetWeaponByIndex(int index)
    {
        if (index < player.weaponList.Count)
        {
            currentWeaponIndex = index;
            player.setActiveWeaponEvent.Call(player.weaponList[index]);
        }
    }

	private void SetPlayerAnimationSpeed()
    {
        player.animator.speed = moveSpeed / Settings.baseSpeedForPlayerAnimations;
    }

	private void MovementInput()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
		float verticalInput = Input.GetAxisRaw("Vertical");
        Vector2 moveDirection = new Vector2(horizontalInput, verticalInput);
        moveDirection.Normalize();

        if (moveDirection != Vector2.zero)
        {
            player.moveEvent.Call(moveDirection, movementDetails.moveSpeed);
        }

        else
        {
            player.idleEvent.CallIdleEvent();
        }
        
    }

	private void WeaponInput()
	{
		Vector3 weaponDirection;
		float weaponAngle, playerAngle;
		AimDirection aimDirection;

		// Aim weapon input
		AimWeaponInput(out weaponDirection, out weaponAngle, out playerAngle, out aimDirection);

		// Fire weapon input
		FireWeaponInput(weaponDirection, weaponAngle, playerAngle, aimDirection);

		// Reload weapon input
		//ReloadWeaponInput();
	}

	private void ReloadWeaponInput()
	{
		Weapon currentWeapon = player.activeWeapon.GetCurrentWeapon();

		// if current weapon is reloading return
		if (currentWeapon.isWeaponReloading) return;

		// remaining ammo is less than clip capacity then return and not infinite ammo then return
		if (currentWeapon.weaponRemainingAmmo < currentWeapon.weaponDetails.weaponClipAmmoCapacity && !currentWeapon.weaponDetails.hasInfiniteAmmo) return;

		// if ammo in clip equals clip capacity then return
		if (currentWeapon.weaponClipRemainingAmmo == currentWeapon.weaponDetails.weaponClipAmmoCapacity) return;

		if (Input.GetKeyDown(KeyCode.R))
		{
			// Call the reload weapon event
			player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), 0);
		}

	}

	private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngle, out float playerAngle, out AimDirection aimDirection)
	{
		// Get mouse world position
		Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();

		// Calculate direction vector of mouse cursor from weapon shoot position
		weaponDirection = (mouseWorldPosition - player.activeWeapon.GetShootPosition());

		// Calculate direction vector of mouse cursor from player transform position
		Vector3 playerDirection = (mouseWorldPosition - transform.position);

		// Get weapon to cursor angle
		weaponAngle = HelperUtilities.GetAngleFromVector(weaponDirection);

		// Get player to cursor angle
		playerAngle = HelperUtilities.GetAngleFromVector(playerDirection);

		// Set player aim direction
		aimDirection = HelperUtilities.GetAimDirection(playerAngle);

		// Trigger weapon aim event
		player.aimWeaponEvent.CallAimWeaponEvent(aimDirection, playerAngle, weaponAngle, weaponDirection);
	}

	//private void AimWeaponInput()
	//{
	//       Vector3 mouseWorldPos = HelperUtilities.GetMouseWorldPosition();
	//       Vector3 weaponAimDirectionVec = mouseWorldPos - player.activeWeapon.GetShootPosition();
	//       Vector3 playerDirection = mouseWorldPos - transform.position;
	//       float weaponAimAngle = HelperUtilities.GetAngleFromVector(weaponAimDirectionVec);
	//       float playerAngle = HelperUtilities.GetAngleFromVector(playerDirection);
	//       AimDirection aimDirection = HelperUtilities.GetAimDirection(playerAngle);

	//       player.aimWeaponEvent.CallAimWeaponEvent(aimDirection, playerAngle, weaponAimAngle, weaponAimDirectionVec);
	//}

	private void FireWeaponInput(Vector3 weaponDirection, float weaponAngle, float playerAngle, AimDirection aimDirection)
	{
		// Fire when left mouse button is clicked
		if (Input.GetMouseButton(0))
		{
			// Trigger fire weapon event
			player.fireWeaponEvent.CallFireWeaponEvent(true, leftMouseDownPreviousFrame, aimDirection, playerAngle, weaponAngle, weaponDirection);
			leftMouseDownPreviousFrame = true;
		}
		else
		{
			leftMouseDownPreviousFrame = false;
		}
	}

}
