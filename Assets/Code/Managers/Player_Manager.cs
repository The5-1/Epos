using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player_Entity
{
    public GameObject playerGO;
    public Player_Controller playerController;
    public Actor_Controller playerActorController;
    public Actor_Data playerActorData;
    public ActorMovement_Controller playerActorMovementController;
}


public class Player_Manager : MonoBehaviour {

    static public Player_Manager singleton;

    #region main player
    [Header("Main Player")]
    public Player_Entity mainPlayer;
    #endregion

    #region main player
    [Header("Multi Players")]
    public List<Player_Entity> multiPlayers;
    #endregion


    protected void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(singleton);
            init();
        }
        else { Destroy(this); }
    }

    protected void OnDestroy()
    {
        if (singleton == this) { singleton = null; }
    }

    private void init()
    {
        Debug.Log(string.Format("{0}.init()", this));
        initFields();
        initMainPlayer();
        addMultiPlayer();
        addMultiPlayer();
        addMultiPlayer();
        addMultiPlayer();
    }

    private void initFields()
    {
        multiPlayers = new List<Player_Entity>();
    }


    #region add players
    private Player_Entity makePlayer(string name)
    {
        Player_Entity player = new Player_Entity();

        player.playerGO = new GameObject(name);
        player.playerGO.transform.position = new Vector3(Random.Range(5.0f, 20.0f), 0.0f, Random.Range(5.0f, 20.0f));
        player.playerController = player.playerGO.AddComponent<Player_Controller>();
        player.playerActorController = player.playerGO.AddComponent<Actor_Controller>();
        player.playerActorData = new Actor_Data();
        player.playerActorController._actorData = player.playerActorData;
        player.playerActorData.name = name;
        player.playerActorData.breedData.gender = Actor_Gender.male;
        player.playerActorData.ageInSecondsMax = ulong.MaxValue - 1;
        player.playerActorData.essential = true;
        player.playerActorMovementController = player.playerGO.AddComponent<ActorMovement_Controller>();

        GameObject debugActorTMP = Instantiate((GameObject)Resources.Load("Debug/Debug_Actor"));
        debugActorTMP.transform.SetParent(player.playerGO.gameObject.transform);

        DontDestroyOnLoad(player.playerGO);

        return player;
    }

    private void initMainPlayer()
    {
        string name = "__Player_main__";
        Player_Entity p = makePlayer(name);
        mainPlayer = p;
    }

    private int addMultiPlayer()
    {
        int index = multiPlayers.Count;
        string name = "__MultiPlayer_" + index + "__";
        Player_Entity p = makePlayer(name);
        multiPlayers.Add(p);
        return index;
    }

    #endregion
}
