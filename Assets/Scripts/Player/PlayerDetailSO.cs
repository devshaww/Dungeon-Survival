using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDetails_", menuName = "Scriptable Objects/Player/Player Details")]
public class PlayerDetailSO : ScriptableObject
{
	public string characterName;
	public GameObject playerPrefab;
	public Sprite playerHandSprite;
	public Sprite playerMiniMapIcon;
	public RuntimeAnimatorController runtimeAnimatorController;
	public int health;


	#region Validation
#if UNITY_EDITOR
	private void OnValidate()
	{
		HelperUtilities.ValidateCheckEmptyString(this, nameof(characterName), characterName);
		HelperUtilities.ValidateCheckNullValue(this, nameof(playerPrefab), playerPrefab);
		HelperUtilities.ValidateCheckPositiveValue(this, nameof(health), health, false);
		//HelperUtilities.ValidateCheckNullValue(this, nameof(startingWeapon), startingWeapon);
		HelperUtilities.ValidateCheckNullValue(this, nameof(playerMiniMapIcon), playerMiniMapIcon);
		HelperUtilities.ValidateCheckNullValue(this, nameof(playerHandSprite), playerHandSprite);
		HelperUtilities.ValidateCheckNullValue(this, nameof(runtimeAnimatorController), runtimeAnimatorController);
		//HelperUtilities.ValidateCheckEnumerableValues(this, nameof(startingWeaponList), startingWeaponList);
	}
#endif
	#endregion
}

