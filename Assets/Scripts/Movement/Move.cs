using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MoveEvent))]
[DisallowMultipleComponent]
public class Move : MonoBehaviour
{
	private Rigidbody2D rb;
	private MoveEvent moveEvent;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		moveEvent = GetComponent<MoveEvent>();
	}

	private void OnEnable()
	{
		moveEvent.OnMove += MoveEvent_OnMove;
	}

	private void OnDisable()
	{
		moveEvent.OnMove -= MoveEvent_OnMove;
	}

	private void MoveEvent_OnMove(MoveEvent moveEvent, MoveEventArgs args)
	{
		MoveRigidbody(args.moveDirection, args.moveSpeed);
	}

	private void MoveRigidbody(Vector2 moveDirection, float moveSpeed)
	{
		rb.velocity = moveDirection * moveSpeed;
	}

}
