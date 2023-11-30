using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFireable
{
    void InitializeAmmo(AmmoDetailSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirection, bool overrideAmmoMovement=false);

    GameObject GetGameObject();
}
