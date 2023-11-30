using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System;

public class WeaponStatusUI : MonoBehaviour
{
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    [SerializeField] private Image weaponImage;
	[SerializeField] private Transform ammoHolderTransform;
    [SerializeField] private TextMeshProUGUI reloadText;
    [SerializeField] private TextMeshProUGUI ammoRemainingText;
	[SerializeField] private TextMeshProUGUI weaponNameText;
	[SerializeField] private Transform reloadBar;
    [SerializeField] private Image barImage;

    private Player player;
    private List<GameObject> ammoIconList = new List<GameObject>();
    private Coroutine reloadWeaponCoroutine;
    private Coroutine blinkingReloadTextCoroutine;

	private void Awake()
	{
        player = GameManager.Instance.GetPlayer();
	}

	private void Start()
	{
		SetActiveWeapon(player.activeWeapon.GetCurrentWeapon());
	}

	private void OnEnable()
	{
		player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
		player.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;
		player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;
		player.weaponReloadedEvent.OnWeaponReloaded += WeaponRelodedEvent_OnWeaponReloaded;
	}

	private void OnDisable()
	{
		player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
		player.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;
		player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;
		player.weaponReloadedEvent.OnWeaponReloaded -= WeaponRelodedEvent_OnWeaponReloaded;
	}

	private void WeaponFiredEvent_OnWeaponFired(WeaponFiredEvent @event, WeaponFiredEventArgs args)
	{
		Weapon weapon = args.weapon;
		UpdateAmmoText(weapon);
		UpdateAmmoLoadedIcons(weapon);
		UpdateReloadText(weapon);
	}

	private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent @event, ReloadWeaponEventArgs args)
	{
		// update reload bar
		Weapon weapon = args.weapon;
		if (weapon.weaponDetails.hasInfiniteClipCapacity)
		{
			return;
		}
		StopReloadWeaponCoroutine();
		UpdateReloadText(weapon);

		reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarRoutine(weapon));
	}

	private void WeaponRelodedEvent_OnWeaponReloaded(WeaponReloadedEvent @event, WeaponReloadedEventArgs args)
	{
		Weapon weapon = args.weapon;
		if (player.activeWeapon.GetCurrentWeapon() == weapon)
		{
			UpdateReloadText(weapon);
			UpdateAmmoText(weapon);
			UpdateAmmoLoadedIcons(weapon);
			ResetWeaponReloadBar();
		}
	}

	private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent @event, SetActiveWeaponEventArgs args)
	{
		SetActiveWeapon(args.weapon);
	}

	private void SetActiveWeapon(Weapon weapon)
	{
		UpdateActiveWeaponImage(weapon.weaponDetails);
		UpdateActiveWeaponName(weapon);
		UpdateAmmoText(weapon);
		UpdateAmmoLoadedIcons(weapon);

		if (weapon.isWeaponReloading)
		{
			UpdateWeaponReloadBar(weapon);
		}
		else
		{
			ResetWeaponReloadBar();
		}

		UpdateReloadText(weapon);
	}

	private void UpdateWeaponReloadBar(Weapon weapon)
	{
		if (weapon.weaponDetails.hasInfiniteClipCapacity)
			return;

		StopReloadWeaponCoroutine();
		UpdateReloadText(weapon);

		reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarRoutine(weapon));
	}

	private void UpdateActiveWeaponName(Weapon weapon)
	{
		weaponNameText.text = "(" + weapon.weaponListPosition + ")" + weapon.weaponDetails.weaponName.ToUpper();
	}

	private void UpdateActiveWeaponImage(WeaponDetailSO weaponDetails)
	{
		weaponImage.sprite = weaponDetails.weaponSprite;
	}

	private void UpdateReloadText(Weapon weapon)
	{
		if ((!weapon.weaponDetails.hasInfiniteClipCapacity) && (weapon.weaponClipRemainingAmmo <= 0 || weapon.isWeaponReloading))
		{
			// set the reload bar to red
			barImage.color = Color.red;

			StopBlinkingReloadTextCoroutine();

			blinkingReloadTextCoroutine = StartCoroutine(StartBlinkingReloadTextRoutine());
		}
		else
		{
			StopBlinkingReloadText();
		}
	}

	private void UpdateAmmoLoadedIcons(Weapon weapon)
	{
		ClearAmmoLoadedIcons();

		for (int i = 0; i < weapon.weaponClipRemainingAmmo; i++)
		{
			// Instantiate ammo icon prefab
			GameObject ammoIcon = Instantiate(GameResources.Instance.ammoIconPrefab, ammoHolderTransform);

			ammoIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Settings.uiAmmoIconSpacing * i);

			ammoIconList.Add(ammoIcon);
		}
	}

	private void ClearAmmoLoadedIcons()
	{
		// Loop through icon gameobjects and destroy
		foreach (GameObject ammoIcon in ammoIconList)
		{
			Destroy(ammoIcon);
		}

		ammoIconList.Clear();
	}

	private void UpdateAmmoText(Weapon weapon)
	{
		if (weapon.weaponDetails.hasInfiniteAmmo)
		{
			ammoRemainingText.text = "INFINITE AMMO";
		}
		else
		{
			ammoRemainingText.text = weapon.weaponRemainingAmmo.ToString() + " / " + weapon.weaponDetails.weaponAmmoCapacity.ToString();
		}
	}

	private void ResetWeaponReloadBar()
	{
		StopReloadWeaponCoroutine();

		// set bar color as green
		barImage.color = Color.green;

		// set bar scale to 1
		reloadBar.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	private void StopReloadWeaponCoroutine()
	{
		// Stop any active weapon reload bar on the UI
		if (reloadWeaponCoroutine != null)
		{
			StopCoroutine(reloadWeaponCoroutine);
		}
	}

	private IEnumerator UpdateWeaponReloadBarRoutine(Weapon currentWeapon)
	{
		// set the reload bar to red
		barImage.color = Color.red;

		// Animate the weapon reload bar
		while (currentWeapon.isWeaponReloading)
		{
			// update reloadbar
			float barFill = currentWeapon.weaponReloadTimer / currentWeapon.weaponDetails.weaponReloadTime;

			// update bar fill
			reloadBar.transform.localScale = new Vector3(barFill, 1f, 1f);

			yield return null;
		}
	}

	private void StopBlinkingReloadTextCoroutine()
	{
		if (blinkingReloadTextCoroutine != null)
		{
			StopCoroutine(blinkingReloadTextCoroutine);
		}
	}

	private IEnumerator StartBlinkingReloadTextRoutine()
	{
		while (true)
		{
			reloadText.text = "RELOAD";
			yield return new WaitForSeconds(0.3f);
			reloadText.text = "";
			yield return new WaitForSeconds(0.3f);
		}
	}

	private void StopBlinkingReloadText()
	{
		StopBlinkingReloadTextCoroutine();

		reloadText.text = "";
	}
}
