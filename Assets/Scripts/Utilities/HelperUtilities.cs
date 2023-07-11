using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtilities
{

    public static Camera mainCamera;

    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        if (stringToCheck == "")
        {
            Debug.Log(fieldName + " is empty and must contain a value in object " + thisObject.name.ToString());
            return true;
        }
        return false;
    }


	public static bool ValidateCheckNullValue(Object thisObject, string fieldName, Object objectToCheck)
	{
		if (objectToCheck == null)
		{
			Debug.Log(fieldName + " is null and must contain a value in object " + thisObject.name.ToString());
			return true;
		}
		return false;
	}


	public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, int valueToCheck, bool isZeroAllowed)
	{
		bool error = false;

		if (isZeroAllowed)
		{
			if (valueToCheck < 0)
			{
				Debug.Log(fieldName + " must contain a positive value or zero in object " + thisObject.name.ToString());
				error = true;
			}
		}
		else
		{
			if (valueToCheck <= 0)
			{
				Debug.Log(fieldName + " must contain a positive value in object " + thisObject.name.ToString());
				error = true;
			}
		}

		return error;
	}

	public static Vector3 GetSpawnPositionNearestTo(Vector3 position)
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom.instantiatedRoom.grid;

        Vector3 nearestPosition = new Vector3(10000f, 10000f, 0);

        foreach (Vector2Int spawnPositionGrid in currentRoom.spawnPositionArray)
        {
            Vector2 spawnPositionWorld = grid.CellToWorld((Vector3Int)spawnPositionGrid);
            if (Vector3.Distance(spawnPositionWorld, position) < Vector3.Distance(nearestPosition, position))
            {
                nearestPosition = spawnPositionWorld;
            }
        }

        return nearestPosition;
    }

    public static Vector3 GetMouseWorldPosition()
    {
        if (!mainCamera)
        {
            mainCamera = Camera.main;
        }

        Vector3 mouseScreenPos = Input.mousePosition;

        mouseScreenPos.x = Mathf.Clamp(mouseScreenPos.x, 0f, Screen.width);
		mouseScreenPos.y = Mathf.Clamp(mouseScreenPos.y, 0f, Screen.height);

		Vector3 worldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

        worldPos.z = 0f;

        return worldPos;
	}

    public static float GetAngleFromVector(Vector3 vector)
    {
        // in pi
        float radians = Mathf.Atan2(vector.y, vector.x);
        // angle
        float degrees = radians * Mathf.Rad2Deg;

        return degrees;
    }

	public static AimDirection GetAimDirection(float angle)
	{
		AimDirection aimDirection;

		if (angle >= 22f && angle <= 67f)
		{
			aimDirection = AimDirection.UpRight;
		}

		else if (angle > 67f && angle <= 112f)
		{
			aimDirection = AimDirection.Up;
		}

		else if (angle > 112f && angle <= 158f)
		{
			aimDirection = AimDirection.UpLeft;
		}

		else if ((angle <= 180f && angle > 158f) || (angle > -180 && angle <= -135f))
		{
			aimDirection = AimDirection.Left;
		}

		else if ((angle > -135f && angle <= -45f))
		{
			aimDirection = AimDirection.Down;
		}

		else if ((angle > -45f && angle <= 0f) || (angle > 0 && angle < 22f))
		{
			aimDirection = AimDirection.Right;
		}
		else
		{
			aimDirection = AimDirection.Right;
		}

		return aimDirection;

	}


	/// <summary>
	/// list empty or contains null value check - returns true if there is an error
	/// </summary>
	public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

        if (enumerableObjectToCheck == null)
        {
            Debug.Log(fieldName + " is null in object " + thisObject.name.ToString());
            return true;
        }


        foreach (var item in enumerableObjectToCheck)
        {

            if (item == null)
            {
                Debug.Log(fieldName + " has null values in object " + thisObject.name.ToString());
                error = true;
            }
            else
            {
                count++;
            }
        }

        if (count == 0)
        {
            Debug.Log(fieldName + " has no values in object " + thisObject.name.ToString());
            error = true;
        }

        return error;
    }

}
