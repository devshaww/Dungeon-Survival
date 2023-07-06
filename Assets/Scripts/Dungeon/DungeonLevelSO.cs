using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject
{
    [Space(10)]
    [Header("BASIC LEVEL DETAILS")]
    public string levelName;

    [Space(10)]
    [Header("ROOM TEMPLATE FOR LEVEL")]
    public List<RoomTemplateSO> roomTemplateList;

    // select a random room node graph from this list
    [Space(10)]
    [Header("ROOM NODE GRAPH FOR LEVEL")]
    public List<RoomNodeGraphSO> roomNodeGraphList;

    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplateList), roomTemplateList))
        {
            return;
        }
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomNodeGraphList), roomNodeGraphList))
        {
            return;
        }

        bool hasEWCorridor = false;
        bool hasNSCorridor = false;
        bool hasEntrance = false;

        foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
        {
            if (roomTemplateSO == null)
            {
                return;
            }

            if (roomTemplateSO.roomNodeType.isCorridorEW)
            {
                hasEWCorridor = true;
            }

            if (roomTemplateSO.roomNodeType.isCorridorNS)
            {
                hasNSCorridor = true;
            }

            if (roomTemplateSO.roomNodeType.isEntrance)
            {
                hasEntrance = true;
            }
        }

        if (!hasEWCorridor)
        {
            Debug.Log("In " + this.name.ToString() + " : No EW Corridor Room Type Specified");
        }

        if (!hasNSCorridor)
        {
            Debug.Log("In " + this.name.ToString() + " : No NS Corridor Room Type Specified");
        }

        if (!hasEntrance)
        {
            Debug.Log("In " + this.name.ToString() + " : No Entrance Room Type Specified");
        }


		foreach (RoomNodeGraphSO roomNodeGraph in roomNodeGraphList)
		{
            if (roomNodeGraph == null) {
                return;
            }

            foreach (RoomNodeSO roomNodeSO in roomNodeGraph.roomNodeList)
            {
                if (roomNodeSO == null) {
                    continue;
                }

                if (roomNodeSO.roomNodeType.isEntrance || roomNodeSO.roomNodeType.isCorridorEW || roomNodeSO.roomNodeType.isCorridorNS 
                    || roomNodeSO.roomNodeType.isCorridor || roomNodeSO.roomNodeType.isNone) {
                    continue;
                }

                bool isRoomNodeTypeFound = false;

				foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
				{

					if (roomTemplateSO == null)
						continue;

					if (roomTemplateSO.roomNodeType == roomNodeSO.roomNodeType)
					{
						isRoomNodeTypeFound = true;
						break;
					}

				}

				if (!isRoomNodeTypeFound)
					Debug.Log("In " + this.name.ToString() + " : No room template " + roomNodeSO.roomNodeType.name.ToString() + " found for node graph " + roomNodeGraph.name.ToString());

			}
		}
	}


#endif

    #endregion Validation
}
