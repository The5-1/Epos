using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player_Entity: Actor_Entity
{
    public Player_Controller playerController;

    public Player_Entity(Actor_Entity actor)
    {
        GO = actor.GO;
        actorController = actor.actorController;
        actorMovementController = actor.actorMovementController;
    }
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
        createMainPlayer();
        //addMultiPlayer();
        //addMultiPlayer();
        //addMultiPlayer();
        //addMultiPlayer();
    }

    private void initFields()
    {
        multiPlayers = new List<Player_Entity>();
    }


    #region add players
    private Player_Entity makePlayer(string name)
    {
        Actor_Data data = new Actor_Data(); //creates new random Actor_Data
        Actor_Entity actor = Actor_Manager.singleton.createActorEntity(name);
        Player_Entity player = new Player_Entity(actor);
        player.playerController = player.GO.AddComponent<Player_Controller>();

        player.actorController.actorData = data;
        player.actorController.actorData.name = name;
        player.actorController.actorData.breedData.gender = Actor_Gender.male;
        player.actorController.actorData.ageInSecondsMax = ulong.MaxValue - 1;
        player.actorController.actorData.essential = true;

        //GameObject debugActorTMP = Instantiate((GameObject)Resources.Load("Debug/Debug_Actor"));
        //debugActorTMP.transform.SetParent(player.playerGO.gameObject.transform);

        player.GO.transform.position = new Vector3(Random.Range(10.0f, 20.0f), 0.0f, Random.Range(10.0f, 20.0f));

        DontDestroyOnLoad(player.GO);

        return player;
    }

    private void createMainPlayer()
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
