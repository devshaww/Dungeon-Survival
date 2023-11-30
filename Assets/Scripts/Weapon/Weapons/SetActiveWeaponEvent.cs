using System;
using UnityEngine;

public class SetActiveWeaponEvent : MonoBehaviour
{
    public event Action<SetActiveWeaponEvent, SetActiveWeaponEventArgs> OnSetActiveWeapon;

    public void Call(Weapon weapon)
    {
        OnSetActiveWeapon?.Invoke(this, new SetActiveWeaponEventArgs() { weapon = weapon }) ;
    }
}

public class SetActiveWeaponEventArgs : EventArgs
{
    public Weapon weapon;
}
