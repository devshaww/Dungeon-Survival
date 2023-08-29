using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private Transform weaponShootPosition;
	[SerializeField] private MovementDetailSO movementDetails;
	private Player player;
    private float moveSpeed;

	private void Start()
	{
        SetPlayerAnimationSpeed();
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
        AimWeaponInput();
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

	private void AimWeaponInput()
	{
        Vector3 mouseWorldPos = HelperUtilities.GetMouseWorldPosition();
		Vector3 weaponAimDirectionVec = mouseWorldPos - weaponShootPosition.position;
        Vector3 playerDirection = mouseWorldPos - transform.position;
        float weaponAimAngle = HelperUtilities.GetAngleFromVector(weaponAimDirectionVec);
        float playerAngle = HelperUtilities.GetAngleFromVector(playerDirection);
        AimDirection aimDirection = HelperUtilities.GetAimDirection(playerAngle);

        player.aimWeaponEvent.CallAimWeaponEvent(aimDirection, playerAngle, weaponAimAngle, weaponAimDirectionVec);
	}

}
