using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Manager : MonoBehaviour {

    static public Player_Manager singleton;

    #region main player
    public List<GameObject> playerGO;
    public List<Player_Controller> playerController;
    public List<Actor_Controller> playerActorController;
    public List<Actor_Data> playerActorData;
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
        initFields();
        addPlayer();
    }

    private void initFields()
    {
        playerGO = new List<GameObject>();
        playerController = new List<Player_Controller>();
        playerActorController = new List<Actor_Controller>();
        playerActorData = new List<Actor_Data>();
    }

    private int addPlayer()
    {
        int index = playerGO.Count;
        string name = "__Player_" + index + "__";
        playerGO.Add(new GameObject(name));
        playerController.Add(playerGO[index].AddComponent<Player_Controller>());
        playerActorController.Add(playerGO[index].AddComponent<Actor_Controller>());
        playerActorData.Add(new Actor_Data());
        playerActorController[index]._actorData = playerActorData[index];
        playerActorData[index].name = name;
        playerActorData[index].essential = true;

        DontDestroyOnLoad(playerGO[index]);

        return index;
    }

}
