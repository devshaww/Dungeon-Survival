using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
	[SerializeField] private SpriteRenderer weaponSpriteRenderer;
	[SerializeField] private PolygonCollider2D weaponPolygonCollider2D;
	[SerializeField] private Transform weaponShootPositionTransform;
	[SerializeField] private Transform weaponEffectPositionTransform;

	private SetActiveWeaponEvent setWeaponEvent;
	private Weapon currentWeapon;

	private void Awake()
	{
		// Load components
		setWeaponEvent = GetComponent<SetActiveWeaponEvent>();
	}

	private void OnEnable()
	{
		setWeaponEvent.OnSetActiveWeapon += SetWeaponEvent_OnSetActiveWeapon;
	}

	private void OnDisable()
	{
		setWeaponEvent.OnSetActiveWeapon -= SetWeaponEvent_OnSetActiveWeapon;
	}

	private void SetWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
	{
		SetWeapon(setActiveWeaponEventArgs.weapon);
	}

	private void SetWeapon(Weapon weapon)
	{
		currentWeapon = weapon;

		// Set current weapon sprite
		weaponSpriteRenderer.sprite = currentWeapon.weaponDetails.weaponSprite;

		// If the weapon has a polygon collider and a sprite then set it to the weapon sprite physics shape
		if (weaponPolygonCollider2D != null && weaponSpriteRenderer.sprite != null)
		{
			// Get sprite physics shape - this returns the sprite physics shape points as a list of Vector2s
			List<Vector2> spritePhysicsShapePointsList = new List<Vector2>();
			weaponSpriteRenderer.sprite.GetPhysicsShape(0, spritePhysicsShapePointsList);

			// Set polygon collider on weapon to pick up physics shap for sprite - set collider points to sprite physics shape points
			weaponPolygonCollider2D.points = spritePhysicsShapePointsList.ToArray();

		}

		// Set weapon shoot position
		weaponShootPositionTransform.localPosition = currentWeapon.weaponDetails.weaponShootPosition;
	}

	public AmmoDetailSO GetCurrentAmmo()
	{
		return currentWeapon.weaponDetails.weaponCurrentAmmo;
	}

	public Weapon GetCurrentWeapon()
	{
		return currentWeapon;
	}

	public Vector3 GetShootPosition()
	{
		return weaponShootPositionTransform.position;
	}

	public Vector3 GetShootEffectPosition()
	{
		return weaponEffectPositionTransform.position;
	}

	public void RemoveCurrentWeapon()
	{
		currentWeapon = null;
	}

}
