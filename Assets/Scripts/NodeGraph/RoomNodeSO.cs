using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
     public List<string> parentRoomNodeIDList = new List<string>();
     public List<string> childRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

    #region Editor Code

    // the following code should only be run in the Unity Editor
#if UNITY_EDITOR

    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;


    public void Initialize(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;

        // Load room node type list
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    // Draw node with the nodestyle
    // Will be called at some frequency because this gets called in OnGUI()
    public void Draw(GUIStyle nodeStyle, GUIStyle labelStyle)
    {
        //  Draw Node Box Using Begin Area
        GUILayout.BeginArea(rect, nodeStyle);

        // Start Region To Detect Popup Selection Changes
        EditorGUI.BeginChangeCheck();

        // if the room node has a parent or is of type entrance then display a label else display a popup
        if (parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance)
        {
            // Display a label that can't be changed
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName, labelStyle);
        }
        else
        {
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

            roomNodeType = roomNodeTypeList.list[selection];

            // If the room type selection has changed making child connections potentially invalid
            if (roomNodeTypeList.list[selected].isCorridor && !roomNodeTypeList.list[selection].isCorridor || !roomNodeTypeList.list[selected].isCorridor && roomNodeTypeList.list[selection].isCorridor || !roomNodeTypeList.list[selected].isBossRoom && roomNodeTypeList.list[selection].isBossRoom)
            {
                // If a room node type has been changed and it already has children then delete the parent child links since we need to revalidate any
                if (childRoomNodeIDList.Count > 0)
                {
                    for (int i = childRoomNodeIDList.Count - 1; i >= 0; i--)
                    {
                        // Get child room node
                        RoomNodeSO childRoomNode = roomNodeGraph.GetRoomNode(childRoomNodeIDList[i]);

                        // If the child room node is not null
                        if (childRoomNode != null)
                        {
                            // Remove childID from parent room node
                            RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);

                            // Remove parentID from child room node
                            childRoomNode.RemoveParentRoomNodeIDFromRoomNode(id);
                        }
                    }
                }
            }
        }
    

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(this);
        }
            
        GUILayout.EndArea();
    }

    // Populate a string array with the room node types to display that can be selected
    public string[] GetRoomNodeTypesToDisplay()
    {
        string[] roomArray = new string[roomNodeTypeList.list.Count];

        for (int i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }

        return roomArray;
    }

    // Process events for the node
    public void ProcessEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            // Process Mouse Down Events
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;

            // Process Mouse Up Events
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;

            // Process Mouse Drag Events
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
            default:
                break;
        }
    }

    // Process mouse down events
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // left click down
        if (currentEvent.button == 0)
        {
            ProcessLeftClickDownEvent();
        }
        // right click down
        else if (currentEvent.button == 1)
        {
            ProcessRightClickDownEvent(currentEvent);
        }
    }

    // Process left click down event
    private void ProcessLeftClickDownEvent()
    {
        // Select that object in Project Hierarchy so its property inspector appears
        Selection.activeObject = this;

        // Toggle node selection
        isSelected = !isSelected;
    }

    // Process right click down
    private void ProcessRightClickDownEvent(Event currentEvent)
    {
        roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
    }

    // Process mouse up event
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        // If left click up
        if (currentEvent.button == 0)
        {
            ProcessLeftClickUpEvent();
        }
    }

    // Process left click up event
    private void ProcessLeftClickUpEvent()
    {
        if (isLeftClickDragging)
        {
            isLeftClickDragging = false;
        }
    }

    // Process mouse drag event
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        // process left click drag event
        if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent);
        }
    }

    // Process left mouse drag event
    private void ProcessLeftMouseDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;

        DragNode(currentEvent.delta);
        GUI.changed = true;
    }

    // Drag node
    public void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    // Add childID to the node (returns true if the node has been added, false otherwise)
    public bool AddChildRoomNodeIDToRoomNode(string childID)
    {
        // Check child node can be added validly to parent
        if (IsChildRoomValid(childID))
        {
            Debug.Log("allow");
            childRoomNodeIDList.Add(childID);
            return true;
    }

        return false;
    }

    // Check the child node can be validly added to the parent node - return true if it can otherwise return false
    public bool IsChildRoomValid(string childID)
    {
        bool isConnectedBossNodeAlready = false;
        RoomNodeSO childRoomNode = roomNodeGraph.GetRoomNode(childID);
        // Check if there is there already a connected boss room in the node graph
        foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
        {
            if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)
                isConnectedBossNodeAlready = true;
        }

        // if the child node has a type of boss room and there is already a connected boss room node then return false
        if (childRoomNode.roomNodeType.isBossRoom && isConnectedBossNodeAlready)
        {
            Debug.Log("1");
            return false;
        } 

        // If the child node has a type of none then return false
        if (childRoomNode.roomNodeType.isNone)
        {
            Debug.Log("2");
            return false;
        }

        // If the node already has a child with this child ID return false
        if (childRoomNodeIDList.Contains(childID))
        {
            Debug.Log("3");
            return false;
        }

        // If this node ID and the child ID are the same return false
        if (id == childID)
        {
            Debug.Log("4");
            return false;
        }

        // If this childID is already in the parentID list return false
        if (parentRoomNodeIDList.Contains(childID))
        {
            Debug.Log("5");
            return false;
        }

        // If the child node already has a parent return false
        if (childRoomNode.parentRoomNodeIDList.Count > 0)
        {
            Debug.Log("6");
            return false;
        }

        // If child is a corridor and this node is a corridor return false
        if (childRoomNode.roomNodeType.isCorridor && roomNodeType.isCorridor)
        {
            Debug.Log("7");
            return false;
        }

        // If child is not a corridor and this node is not a corridor return false
        if (!childRoomNode.roomNodeType.isCorridor && !roomNodeType.isCorridor)
        {
            Debug.Log("8");
            return false;
        }

        // If adding a corridor check that this node has < the maximum permitted child corridors
        if (childRoomNode.roomNodeType.isCorridor && childRoomNodeIDList.Count >= Settings.maxChildCorridors)
        {
            Debug.Log("9");
            return false;
        }

        // if the child room is an entrance return false - the entrance must always be the top level parent node
        if (childRoomNode.roomNodeType.isEntrance)
        {
            Debug.Log("10");
            return false;
        }

        // If adding a room to a corridor check that this corridor node doesn't already have a room added
        // in this case, the current room node must be a room instead of corridor
        // because there's already a (!child.iscorridor && !iscorridor) situation handled above
        if (!childRoomNode.roomNodeType.isCorridor && childRoomNodeIDList.Count > 0)
        {
            Debug.Log("11");
            return false;
        }

        return true;
    }

    //// Add parentID to the node (returns true if the node has been added, false otherwise)
    public bool AddParentRoomNodeIDToRoomNode(string parentID)
    {
        parentRoomNodeIDList.Add(parentID);
        return true;
    }

    // Remove childID from the node (returns true if the node has been removed, false otherwise)
    public bool RemoveChildRoomNodeIDFromRoomNode(string childID)
    {
        // if the node contains the child ID then remove it
        if (childRoomNodeIDList.Contains(childID))
        {
            childRoomNodeIDList.Remove(childID);
            return true;
        }
        return false;
    }

    // Remove parentID from the node (returns true if the node has been remove, false otherwise)
    public bool RemoveParentRoomNodeIDFromRoomNode(string parentID)
    {
        // if the node contains the parent ID then remove it
        if (parentRoomNodeIDList.Contains(parentID))
        {
            parentRoomNodeIDList.Remove(parentID);
            return true;
        }
        return false;
    }

#endif

    #endregion Editor Code
}