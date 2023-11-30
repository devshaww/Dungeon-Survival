using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDetails", menuName = "Scriptable Objects/Weapons/Weapon Details")]
public class WeaponDetailSO : ScriptableObject
{
    public string weaponName;
    public Sprite weaponSprite;

    public Vector3 weaponShootPosition;
    public AmmoDetailSO weaponCurrentAmmo;

    public bool hasInfiniteAmmo = false;
    public bool hasInfiniteClipCapacity = false;
    public int weaponClipAmmoCapacity = 6;
    public int weaponAmmoCapacity = 100;
    // 5 shots per second
    public float weaponFireRate = 0.2f;
    // time in seconds to hold fire button down before firing
    public float weaponPrechargeTime = 0f;
    public float weaponReloadTime = 0f;

    public SoundEffectSO weaponFiringSoundEffect;
    public SoundEffectSO weaponReloadSoundEffect;
}
