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

    [Space(10)]
    [Header("DUNGEON")]
    public RoomNodeTypeListSO roomNodeTypeList;

	[Space(10)]
	[Header("MATERIALS")]
	public Material dimmedMaterial;
}
