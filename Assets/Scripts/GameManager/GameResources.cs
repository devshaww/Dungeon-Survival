using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }

    #region Header DUNGEON
    [Space(10)]
    [Header("DUNGEON")]
    #endregion
    public RoomNodeTypeListSO roomNodeTypeList;

    #region Header MATERIALS
    [Space(10)]
    [Header("MATERIALS")]
    #endregion
    public Material dimmedMaterial;

    public Material litMaterial;

    public Shader variableLitShader;

    #region Header PLAYER
    [Space(10)]
    [Header("PLAYER")]
    #endregion
    public CurrentPlayerSO currentPlayer;
}
