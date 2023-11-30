using UnityEngine;

[DisallowMultipleComponent]
public class Ammo : MonoBehaviour, IFireable
{
	[SerializeField]
	private TrailRenderer trailRenderer;

	private float ammoRange = 0f;
	private float ammoSpeed;
	private Vector3 fireDirectionVector;
	private float fireDirectionAngle;
	private SpriteRenderer spriteRenderer;
	private AmmoDetailSO ammoDetails;
	private float ammoChargeTimer;
	private bool isAmmoMaterialSet = false;
	private bool overrideAmmoMovement;

	private void Awake()
	{
		// cache sprite renderer
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Update ()
	{
		if (ammoChargeTimer > 0f) {
			ammoChargeTimer -= Time.deltaTime;
			return;
		} else if (!isAmmoMaterialSet) {
			SetAmmoMaterial(ammoDetails.ammoMaterial);
			isAmmoMaterialSet = true;
		}

		Vector3 distanceVector = fireDirectionVector * ammoSpeed * Time.deltaTime;
		transform.position += distanceVector;

		ammoRange -= distanceVector.magnitude;

		if (ammoRange < 0)
		{
			DisableAmmo();
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// TODO: - Add hit particle VFX
		DisableAmmo();
	}

	public GameObject GetGameObject()
	{
		return gameObject;
	}

	private void SetAmmoMaterial(Material material)
	{
		spriteRenderer.material = material;
	}

	public void InitializeAmmo(AmmoDetailSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirection, bool overrideAmmoMovement = false)
	{
		this.ammoDetails = ammoDetails;
		SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirection);

		spriteRenderer.sprite = ammoDetails.ammoSprite;

		if (ammoDetails.ammoChargeTime > 0f)
		{
			ammoChargeTimer = ammoDetails.ammoChargeTime;
			SetAmmoMaterial(ammoDetails.ammoMaterial);
			isAmmoMaterialSet = false;
		}
		else
		{
			ammoChargeTimer = 0f;
			SetAmmoMaterial(ammoDetails.ammoMaterial);
			isAmmoMaterialSet = true;
		}

		ammoRange = ammoDetails.ammoRange;
		this.ammoSpeed = ammoSpeed;
		this.overrideAmmoMovement = overrideAmmoMovement;

		gameObject.SetActive(true);

		if (ammoDetails.isAmmoTrail)
		{
			trailRenderer.gameObject.SetActive(true);
			trailRenderer.emitting = true;
			trailRenderer.material = ammoDetails.ammoTrailMaterial;
			trailRenderer.startWidth = ammoDetails.ammoTrailStartWidth;
			trailRenderer.endWidth = ammoDetails.ammoTrailEndWidth;
			trailRenderer.time = ammoDetails.ammoTrailTime;
		}
		else
		{
			trailRenderer.emitting = false;
			trailRenderer.gameObject.SetActive(false);
		}
	}

	private void SetFireDirection(AmmoDetailSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirection)
	{
		float randomSpeed = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);

		//// [-1, 1)
		int spreadToggle = Random.Range(0, 1) * 2 - 1;

		fireDirectionAngle = weaponAimAngle;

		//if (weaponAimDirection.magnitude > Settings.useAimAngleDistance) {
		//	fireDirectionAngle = weaponAimAngle;
		//} else {
		//	fireDirectionAngle = aimAngle;
		//}

		fireDirectionAngle += spreadToggle * randomSpeed;

		transform.eulerAngles = new Vector3(0f, 0f, fireDirectionAngle);

		fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
	}	

	private void DisableAmmo()
	{
		gameObject.SetActive(false);
	}

}
