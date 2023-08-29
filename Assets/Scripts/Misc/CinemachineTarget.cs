using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
	private CinemachineTargetGroup cinemachineTargetGroup;
	[SerializeField] private Transform cursor;

	private void Awake()
	{
		cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
	}

	private void Start()
	{
		SetCinemachineTargetGroup();
	}

	private void SetCinemachineTargetGroup()
	{
		CinemachineTargetGroup.Target playerTarget = new CinemachineTargetGroup.Target { weight = 1f, radius = 2.5f, target = GameManager.Instance.GetPlayer().transform };
		CinemachineTargetGroup.Target cursorTarget = new CinemachineTargetGroup.Target { weight = 1f, radius = 1f, target = cursor };
		CinemachineTargetGroup.Target[] targets = new CinemachineTargetGroup.Target[] { playerTarget, cursorTarget };

		cinemachineTargetGroup.m_Targets = targets;
	}

	private void Update()
	{
		cursor.position = HelperUtilities.GetMouseWorldPosition();

	}
}
