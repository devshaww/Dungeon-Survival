using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Objects/Movement/MovementDetails")]
public class MovementDetailSO : ScriptableObject
{
    public float minMoveSpeed = 8f;
	public float maxMoveSpeed = 8f;

	public float moveSpeed = 8f;
}
