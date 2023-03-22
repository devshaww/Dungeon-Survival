using UnityEditor.Callbacks;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private GUIStyle roomNodeStyleLabel;
    private GUIStyle roomNodeStyleActive;
    //private bool isRectangleSelectionToolActivated = false;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeSO currentRoomNode = null;
    private RoomNodeTypeListSO roomNodeTypeList;

    private const float nodeWidth= 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    #region GRID
    private Vector2 graphOffset;
    private Vector2 graphDrag;

    // Spacing
    private const float gridLarge = 100f;
    private const float gridSmall = 25f;
    #endregion

    //

    private const float connectingLineWidth = 3f;
    private const float connectingLineArrowSize = 6f;

    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon/Room Node Graph Editor")]

    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    private void OnEnable()
    {
        Selection.selectionChanged += InspectorSelectionChanged;
        roomNodeStyleLabel = new GUIStyle();
        roomNodeStyleLabel.normal.textColor = Color.white;

        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        roomNodeStyleActive = new GUIStyle();
        roomNodeStyleActive.normal.textColor = Color.white;
        roomNodeStyleActive.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        roomNodeStyleActive.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyleActive.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;

        //if (currentRoomNodeGraph == null || currentRoomNodeGraph.roomNodeList.Count == 0)
        //{
        //    CreateRoomNode(new Vector2(200f, 200f), roomNodeTypeList.list.Find(x => x.isEntrance));
        //}

    }

    private void OnDisable()
    {
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    [OnOpenAsset(0)]
    private static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;

        if (roomNodeGraph != null)
        {
            OpenWindow();
            currentRoomNodeGraph = roomNodeGraph;
            
            return true;
        }

        return false;
    }

    // not working
    //private void DetectSelectAllInput(Event currentEvent)
    //{
    //    if (currentEvent.type == EventType.KeyDown
    //        && (currentEvent.modifiers & EventModifiers.Command) != 0)        
    //    {
    //        if (currentEvent.character == 'a')
    //        {
    //            Debug.Log("Command+A");
    //            SelectAllRoomNodes();
    //        } 
    //    }
    //}

    private void OnGUI()
    {
        if (currentRoomNodeGraph != null)
        {
            DrawBackgroundGrid(gridSmall, 0.2f, Color.gray);
            DrawBackgroundGrid(gridLarge, 0.3f, Color.gray);

            DrawDraggedLine();

            // put it in OnGUI because dragging of a node would happen.
            // if not, the connection line would stay as it was.
            DrawRoomConnections();

            ProcessEvents(Event.current);

            DrawRoomNodes();
        }

        if (GUI.changed)
        {
            Repaint();
        }
        //GUILayout.BeginArea(new Rect(new Vector2(200f, 200f), new Vector2(nodeWidth, nodeHeight)), roomNodeStyle);
        //EditorGUILayout.LabelField("Node 1");
        //GUILayout.EndArea();
    }

    private void DrawBackgroundGrid(float gridSize, float gridOpacity, Color gridColor)
    {
        int verticalLineCount = Mathf.CeilToInt((position.width + gridSize) / gridSize);
        int horizontalLineCount = Mathf.CeilToInt((position.height + gridSize) / gridSize);

        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        // 0.5 because this method gets called twice, so that graphOffset adds delta twice
        // I put it in MiddleMouseDownEvent method so it's not necessary to
        // times 0.5 anymore.
        //graphOffset += graphDrag * 0.5f;    

        Vector3 gridOffset = new(graphOffset.x % gridSize, graphOffset.y % gridSize, 0);

        for (int i = 0; i < verticalLineCount; i++)
        {
            Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0) + gridOffset,
                             new Vector3(gridSize * i, position.height + gridSize, 0) + gridOffset,
                             0);
        }

        for (int j = 0; j < horizontalLineCount; j++)
        {
            Handles.DrawLine(new Vector3(-gridSize, gridSize * j, 0) + gridOffset,
                             new Vector3(position.width + gridSize, gridSize * j, 0) + gridOffset,
                             0);
        }

        Handles.color = Color.white;
    }

    private void DrawRoomConnections()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.childRoomNodeIDList.Count > 0)
            {
                foreach (string childRoomID in roomNode.childRoomNodeIDList)
                {
                    if (currentRoomNodeGraph.roomNodeDictionary.ContainsKey(childRoomID))
                    {
                        DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[childRoomID]);
                        GUI.changed = true;
                    }
                }
            }
        }
    }

    private void DrawConnectionLine(RoomNodeSO from, RoomNodeSO to)
    {
        Vector2 fromPos = from.rect.center;
        Vector2 toPos = to.rect.center;
        Vector2 midPos = (toPos + fromPos) / 2f;

        Vector2 direction = toPos - fromPos;

        Vector2 arrowTailPoint1 = midPos - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
        Vector2 arrowTailPoint2 = midPos + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;

        Vector2 arrowHeadPoint = midPos + direction.normalized * connectingLineArrowSize;

        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, connectingLineWidth);
        Handles.DrawBezier(fromPos, toPos, fromPos, toPos, Color.white, null, connectingLineWidth);
        GUI.changed = true;
    }

    private void DrawDraggedLine()
    {
        if (currentRoomNodeGraph.linePosition != Vector2.zero)
        {
            // I think start tangent and end tangent are the two points to calculate tangent from
            // To draw a straight line, tangent is calculated using just start point and end point. 
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center,
                               currentRoomNodeGraph.linePosition,
                               currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center,
                               currentRoomNodeGraph.linePosition,
                               Color.white,
                               null,
                               connectingLineWidth);
        }
    }

    private void ProcessEvents(Event currentEvent)
    {
        //DetectSelectAllInput(currentEvent);

        graphDrag = Vector2.zero;

        // assign ismouseover node
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }

        // right click to generate context menu & draw line connection
        // & left click at blank to deselect all
        if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            ProcessRoomNodeGraphEvents(currentEvent);
        }

        else
        {
            // move node & set active
            currentRoomNode.ProcessEvents(currentEvent);
        }
    }

    private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
    {
        for (int i = currentRoomNodeGraph.roomNodeList.Count - 1; i >= 0; i--)
        {
            if (currentRoomNodeGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition))
            {
                return currentRoomNodeGraph.roomNodeList[i];
            }
        }

        return null;
    }

    private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            default:
                break;
        }
        
    }

    private void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            RoomNodeSO mouseOverRoomNode = IsMouseOverRoomNode(currentEvent);
            if (mouseOverRoomNode != null)
            {
                RoomNodeSO fromRoomNode = currentRoomNodeGraph.roomNodeToDrawLineFrom;
                if (fromRoomNode.AddChildRoomNodeIDToRoomNode(mouseOverRoomNode.id))
                {
                    mouseOverRoomNode.AddParentRoomNodeIDToRoomNode(fromRoomNode.id);
                }
            }
                
            ClearLineDrag();
        }
    }

    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // right click
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
        // left click
        else if (currentEvent.button == 0)
        {
            ClearLineDrag();
            ClearAllSelectedNodes();
        }
    }

    private void ClearAllSelectedNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.isSelected = false;
                GUI.changed = true;
            }
        }
    }


    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if (currentEvent.button == 1)    // move node
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
        if (currentEvent.button == 2)    // background grid graph moves
        {
            ProcessMiddleMouseDragEvent(currentEvent.delta);
        }
        if (currentEvent.button == 0)    // draw rectangle selection tool
        {
            //ProcessLeftMouseDragEvent(currentEvent.delta);
        }
    }

    private void ProcessMiddleMouseDragEvent(Vector2 dragDelta)
    {
        graphDrag = dragDelta;
        graphOffset += graphDrag;

        for (int i = 0; i < currentRoomNodeGraph.roomNodeList.Count; i++)
        {
            currentRoomNodeGraph.roomNodeList[i].DragNode(dragDelta);
        }

        GUI.changed = true;
    }

    //private void ProcessLeftMouseDragEvent(Vector2 delta)
    //{

    //}

    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        DragConnectingLine(currentEvent.delta);
        GUI.changed = true;
    }

    private void DragConnectingLine(Vector2 delta)
    {
        currentRoomNodeGraph.linePosition += delta;
    }

    private void ClearLineDrag()
    {
        currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }
 
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Select All"), false, SelectAllRoomNodes);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Connection"), false, DeleteConnection);
        menu.AddItem(new GUIContent("Detete Node"), false, DeleteNode);

        menu.ShowAsContext();
    }

    private void CreateRoomNode(object mousePositionObject)
    {
        if (currentRoomNodeGraph.roomNodeList.Count == 0)
        {
            CreateRoomNode(new Vector2(200f, 200f), roomNodeTypeList.list.Find(x => x.isEntrance));
        }

        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }

    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        // create room node scriptable object asset
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

        // add room node to current room node graph room node list
        currentRoomNodeGraph.roomNodeList.Add(roomNode);

        // set room node values
        roomNode.Initialize(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);

        // add room node to room node graph scriptable object asset database
        // to form a hierachy
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);

        AssetDatabase.SaveAssets();

        // Refresh graph node dictionary
        currentRoomNodeGraph.OnValidate();
    }


    private void DrawRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.Draw(roomNodeStyleActive, roomNodeStyleLabel);
            }
            else
            {
                roomNode.Draw(roomNodeStyle, roomNodeStyleLabel);
            }  
        }

        GUI.changed = true;
    }

    private void InspectorSelectionChanged()
    {
        RoomNodeGraphSO activeGraph = Selection.activeObject as RoomNodeGraphSO;

        if (activeGraph != null)
        {
            currentRoomNodeGraph = activeGraph;
            GUI.changed = true;
        }
    }

    private void SelectAllRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.isSelected = true;
        }

        GUI.changed = true;
    }

    private void DeleteNode()
    {
        Queue<RoomNodeSO> roomNodeDeletionQueue = new Queue<RoomNodeSO>();

        // Loop through all nodes
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && !roomNode.roomNodeType.isEntrance)
            {
                roomNodeDeletionQueue.Enqueue(roomNode);

                // iterate through child room nodes ids
                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    // Retrieve child room node
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(childRoomNodeID);

                    if (childRoomNode != null)
                    {
                        // Remove parentID from child room node
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }

                // Iterate through parent room node ids
                foreach (string parentRoomNodeID in roomNode.parentRoomNodeIDList)
                {
                    // Retrieve parent node
                    RoomNodeSO parentRoomNode = currentRoomNodeGraph.GetRoomNode(parentRoomNodeID);

                    if (parentRoomNode != null)
                    {
                        // Remove childID from parent node
                        parentRoomNode.RemoveChildRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        // Delete queued room nodes
        while (roomNodeDeletionQueue.Count > 0)
        {
            // Get room node from queue
            RoomNodeSO roomNodeToDelete = roomNodeDeletionQueue.Dequeue();

            // Remove node from dictionary
            currentRoomNodeGraph.roomNodeDictionary.Remove(roomNodeToDelete.id);

            // Remove node from list
            currentRoomNodeGraph.roomNodeList.Remove(roomNodeToDelete);

            // Remove node from Asset database
            DestroyImmediate(roomNodeToDelete, true);

            // Save asset database
            AssetDatabase.SaveAssets();

        }
    }

    /// <summary>
    /// Delete the links between the selected room nodes
    /// </summary>
    private void DeleteConnection()
    {
        // Iterate through all room nodes
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && roomNode.childRoomNodeIDList.Count > 0)
            {
                for (int i = roomNode.childRoomNodeIDList.Count - 1; i >= 0; i--)
                {
                    // Get child room node
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(roomNode.childRoomNodeIDList[i]);

                    // If the child room node is selected
                    if (childRoomNode != null && childRoomNode.isSelected)
                    {
                        // Remove childID from parent room node
                        roomNode.RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);

                        // Remove parentID from child room node
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        // Clear all selected room nodes
        ClearAllSelectedNodes();
    }
}
