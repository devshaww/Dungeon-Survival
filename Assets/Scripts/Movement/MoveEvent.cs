using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveEvent : MonoBehaviour
{
    public event Action<MoveEvent, MoveEventArgs> OnMove;

    public void Call(Vector2 moveDirection, float moveSpeed)
    {
        OnMove?.Invoke(this, new MoveEventArgs() { moveDirection = moveDirection,  moveSpeed = moveSpeed});
    }
}

public class MoveEventArgs: EventArgs
{
    public Vector2 moveDirection;
	public float moveSpeed;
}
