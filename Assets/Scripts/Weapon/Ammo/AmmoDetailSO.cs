using UnityEngine;

[CreateAssetMenu(fileName = "AmmoDetails", menuName = "Scriptable Objects/Weapons/Ammo Details")]
public class AmmoDetailSO : ScriptableObject
{
	[Header("AMMO BASE DETAILS")]
	public string ammoName;
	public Sprite ammoSprite;
	public GameObject[] ammoPrefabArray;
	public Material ammoMaterial;
	public float ammoChargeTime = 0.1f;
	public Material ammoChargeMaterial;

	[Space(10)]
	[Header("AMMO BASE PARAMETERS")]
	public int ammoDamage = 1;
	public float ammoSpeedMin = 20f;
	public float ammoSpeedMax = 20f;
	public float ammoRange = 20f;
	public float ammoRotationSpeed = 1f;

	[Space(10)]
	[Header("AMMO SPREAD DETAILS")]
	public float ammoSpreadMin = 0f;
	public float ammoSpreadMax = 0f;

	[Space(10)]
	[Header("AMMO SPAWN DETAILS")]
	public int ammoSpawnAmountMin = 1;
	public int ammoSpawnAmountMax = 1;
	public float ammoSpawnIntervalMin = 0f;
	public float ammoSpawnIntervalMax = 0f;

	[Space(10)]
	[Header("AMMO TRAIL DETAILS")]
	public bool isAmmoTrail = false;
	public float ammoTrailTime = 3f;
	public Material ammoTrailMaterial;
	[Range(0f, 1f)] public float ammoTrailStartWidth;
	[Range(0f, 1f)] public float ammoTrailEndWidth;
}
