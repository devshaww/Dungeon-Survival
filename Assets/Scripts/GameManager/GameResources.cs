using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
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
    public RoomNodeTypeListSO roomNodeTypeList;
	#endregion

	#region Header MATERIALS
	[Space(10)]
    [Header("MATERIALS")]
    public Material dimmedMaterial;
    public Material litMaterial;
    public Shader variableLitShader;
    #endregion


    #region Header SOUNDS
    [Space(10)]
    [Header("SOUNDS")]
    #endregion
    public AudioMixerGroup soundMasterMixerGroup;
    public SoundEffectSO doorOpenSoundEffect;

	#region Header PLAYER
	[Space(10)]
    [Header("PLAYER")]
    #endregion
    public CurrentPlayerSO currentPlayer;

    #region Header PLAYER
    [Space(10)]
    [Header("UI")]
    public GameObject ammoIconPrefab;
	#endregion
}
