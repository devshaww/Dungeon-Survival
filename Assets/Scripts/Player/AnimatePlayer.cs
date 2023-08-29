using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class AnimatePlayer : MonoBehaviour
{
    private Player player;

	private void Awake()
	{
		player = GetComponent<Player>();
	}

	private void OnEnable()
	{
		player.idleEvent.OnIdle += IdleEvent_OnIdle;
		player.moveEvent.OnMove += MoveEvent_OnMove;
		player.aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
	}

	private void OnDisable()
	{
		player.idleEvent.OnIdle -= IdleEvent_OnIdle;
		player.moveEvent.OnMove -= MoveEvent_OnMove;
		player.aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
	}

	private void IdleEvent_OnIdle(IdleEvent idleEvent)
	{
		SetIdleAnimationParameters();
	}

	private void MoveEvent_OnMove(MoveEvent moveEvent, MoveEventArgs args)
	{
		SetMoveAnimationParameters();
	}

	private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
	{
		InitializeAimAnimationParameters();

		SetAimWeaponAnimationParameters(aimWeaponEventArgs.aimDirection);
	}

	private void SetIdleAnimationParameters()
	{
		player.animator.SetBool(Settings.isMoving, false);
		player.animator.SetBool(Settings.isIdle, true);
	}

	private void SetMoveAnimationParameters()
	{
		player.animator.SetBool(Settings.isMoving, true);
		player.animator.SetBool(Settings.isIdle, false);
	}

	private void InitializeAimAnimationParameters()
	{
		player.animator.SetBool(Settings.aimUp, false);
		player.animator.SetBool(Settings.aimUpRight, false);
		player.animator.SetBool(Settings.aimUpLeft, false);
		player.animator.SetBool(Settings.aimRight, false);
		player.animator.SetBool(Settings.aimLeft, false);
		player.animator.SetBool(Settings.aimDown, false);
	}

	private void InitializeRollAnimationParameters()
	{
		player.animator.SetBool(Settings.rollDown, false);
		player.animator.SetBool(Settings.rollRight, false);
		player.animator.SetBool(Settings.rollLeft, false);
		player.animator.SetBool(Settings.rollUp, false);
	}

	private void SetAimWeaponAnimationParameters(AimDirection aimDirection)
	{
		switch (aimDirection) {
			case AimDirection.Up:
				player.animator.SetBool(Settings.aimUp, true);
				break;
			case AimDirection.UpRight:
				player.animator.SetBool(Settings.aimUpRight, true);
				break;
			case AimDirection.UpLeft:
				player.animator.SetBool(Settings.aimUpLeft, true);
				break;
			case AimDirection.Down:
				player.animator.SetBool(Settings.aimDown, true);
				break;
			case AimDirection.Left:
				player.animator.SetBool(Settings.aimLeft, true);
				break;
			case AimDirection.Right:
				player.animator.SetBool(Settings.aimRight, true);
				break;
		}
	}

}
