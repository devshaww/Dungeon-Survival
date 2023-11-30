using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DisallowMultipleComponent]
public class AimWeaponEvent : MonoBehaviour
{
    // event to be subscribed
    // add events to this property
    public event Action<AimWeaponEvent, AimWeaponEventArgs> OnWeaponAim;

    public void CallAimWeaponEvent(AimDirection aimDirection, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        OnWeaponAim?.Invoke(this, new AimWeaponEventArgs() { aimDirection = aimDirection, aimAngle = aimAngle, weaponAimAngle = weaponAimAngle, weaponAimDirectionVector = weaponAimDirectionVector });
    }
}


public class AimWeaponEventArgs: EventArgs
{
    public AimDirection aimDirection;  // for determing which animation to play
    public float aimAngle;             // angle between player pivot and mouse
    public float weaponAimAngle;       // for weapon rotation, not used
	public Vector3 weaponAimDirectionVector;  // vector after vector subtraction, not used
}