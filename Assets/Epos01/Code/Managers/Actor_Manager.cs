﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading;


public class Actor_Entity
{
    public GameObject GO;
    public Actor_Controller actorController;
    public Actor_Movement_Controller actorMovementController;
}



/// <summary>
/// age: normal, no more details needed
/// environment: pass the object that killed
/// violence: pass the actor that killed
/// assassination: pass the actor that killed, but treat as unknown to NPCs/Player
/// </summary>



/// <summary>
/// CONTROLLER: Access Actor state and provide methods, don't trigger actions!
/// do: hold actor list, enable access to ActorData, provide Methods on Actor_Data
/// don: perform methods on actors
/// When a player enters a region this spawns the actors in that region.
/// When the region is left, this destroys the Actors (pool actors and just swap out the data bits!)
/// </summary>
public class Actor_Manager : MonoBehaviour {

    /// TODO: specialize this Manager to one for NPCs, Monsters, Players

    #region Fields
    static public Actor_Manager singleton;

    static public GameObject _activeActorsGroup;

    private GameObject _DEBUGActorPrefab;

    /// List of all actors in the game. 
    /// TODO: Other managers handle NPCs, Monsters, Players. Composition or horizontal Inheritance?
    //public List<Actor_Data> actorDatas;
    public Dictionary<ushort, List<Actor_Data>> actorDatasByRegion;
    public uint _maxActors = 1000; //ushort 16bit = 65,535 //uint 32bit = 4.3bil;
    public List<Actor_Data> DEBUGactorDatasInspector;

    // The pool of actual actors, they are reset on region change and get new data plugged in
    // possibly dynamically resize the pool? Delete some but not all.
    public List<Actor_Controller> activeActors; //TODO: How do i need to apply DontDestroyOnLoad to those actor-GameObject pools?
    public ushort _minActiveActors = 100;

    //public Dictionary<int, List<Actor_Data>> _actorsDataPerRegion; //FIXME: just go over the regions instead of making a dictionary

    /// List of actors the player recently interacted with
    /// keep the most recent interaction at the front of the list
    /// NOTE: we could just sort the real actor list. IF it is a linked list we just need to put it to the first place and not really "sort" anything
    //public List<Actor_Data> _recentActors;
    //public ushort _maxRecentActors = 100;

    /// List of actors the player recently interacted with
    /// those can be treated special by the Simulation
    public List<Actor_Data> EssentialActors;
    public ushort _maxEssentialActors = 100;


    public List<Actor_Data> _playerFamily;
    public ushort _maxPlayerFamily = 100;
    #endregion

    #region init
    // This method is called when the script is loaded, but the component does not need to be enabled yet
    protected void Awake()
    {
        // Check if there is already a singleton instance
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(singleton);
            init();
        }
        else { Destroy(this); }
    }

    protected void Start()
    {
            //init();
    }

    // This method is called when the script is destroy
    protected void OnDestroy()
    {
        // If this is the currently active singleton, delete the static instance too
        if (singleton == this) { singleton = null; }
    }

    public void init()
    {
        Debug.Log(string.Format("{0}.init()", this));

        //actorDatas = new List<Actor_Data>();
        actorDatasByRegion = new Dictionary<ushort, List<Actor_Data>>();
        activeActors = new List<Actor_Controller>();

        _activeActorsGroup = new GameObject("Active_Actors");
        _activeActorsGroup.transform.parent = this.gameObject.transform;


        //TODO: if a savegame exists with existing actors load that


        loadPrefabs();

        subscribeToEvents();

        createActorGameObjectPool();
        createActorData();


        //updateAllActoraDataPerRegion();

    }

    protected void loadPrefabs()
    {
        //_DEBUGActorPrefab = (GameObject)Resources.Load("Debug/Debug_Actor"); //NOTE: use this with instantiate!
        //if (_DEBUGActorPrefab == null) { Debug.Log("Resources.Load(\"Debug/Debug_Actor\") is null"); }
    }
    #endregion

    #region Event Subscriptions

    public void subscribeToEvents()
    {

        #region Subscribe to Time Events

        GameTime.TimeDeltaEvent += onTimePassed;

        #endregion

    }

    #endregion

    #region Event Reactions

    protected void onTimePassed(ulong delta)
    {
        simulateAllActors(delta);

    }

    #endregion

    public Actor_Entity createActorEntity(string name)
    {
        Actor_Entity actor = new Actor_Entity();
        actor.GO = Instantiate((GameObject)Resources.Load("Debug/Debug_Actor"));
        actor.GO.name = name;
        actor.actorController = actor.GO.AddComponent<Actor_Controller>();
        actor.actorMovementController = actor.GO.AddComponent<Actor_Movement_Controller>();
        actor.actorController.actorMovementController = actor.actorMovementController;
        return actor;
    }


    private void createActorGameObjectPool()
    {
        for (int i = 0; i < _minActiveActors; i++)
        {
            Actor_Entity actor = createActorEntity("Pooled_Actor_" + i);
            activeActors.Add(actor.actorController);
            actor.GO.transform.parent = _activeActorsGroup.transform;

            //activeActors.Add(new GameObject("Pooled_Actor").AddComponent<Actor_Controller>());
            //activeActors[i].transform.parent = _activeActorsGroup.transform;
            //GameObject debugActorTMP = Instantiate((GameObject)Resources.Load("Debug/Debug_Actor"));
            //debugActorTMP.transform.SetParent(activeActors[i].gameObject.transform);
#if false
            //HACK: the only hacky way to get a cube mesh :(
            GameObject tmp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Mesh mesh = tmp.GetComponent<MeshFilter>().sharedMesh; 
            Material mat = tmp.GetComponent<MeshRenderer>().material;
            _activeActorsPool[i]._parentGO.AddComponent<MeshFilter>().sharedMesh = mesh;
            _activeActorsPool[i]._parentGO.AddComponent<MeshRenderer>().material = mat;
            GameObject.Destroy(tmp);
#endif

            activeActors[i].resetActor();
        }
    }

    private void createActorData()
    {
        for (int i = 0; i < _maxActors; i++)
        {
            //the random data is generated in the Actor_Data Class itself!
            Actor_Data actor = new Actor_Data();
            moveActorToRegion(actor, actor.region); //generate region lists
        }
    }

    /*
    public void updateAllActoraDataPerRegion() //FIXME: This should call the Region Manager
    {
        foreach(Actor_Data actor in Actors)
        {
            int actorRegionID = actor.CurrentRegionIndex;
            //if there is no list for the current ID add a new one
            if (!_actorsDataPerRegion.ContainsKey(actorRegionID)) { _actorsDataPerRegion.Add(actorRegionID, new List<Actor_Data>()); }

            _actorsDataPerRegion[actorRegionID].Add(actor);
        }

        //TODO: clean up enmpty lists
    }
    */

    #region Simulation

    protected void simulateAllActors(ulong delta)
    {
        //Modifying list while iterating: http://stackoverflow.com/questions/1582285/how-to-remove-elements-from-a-generic-list-while-iterating-over-it


        //TODO: do update regions according to simulation LOD, store last time region was updated
        foreach (ushort region in actorDatasByRegion.Keys)
        {

            deleteMakredActors(actorDatasByRegion[region]);

            foreach (Actor_Data actor in actorDatasByRegion[region])
            {
                actor.tickTime(delta);
            }
        }

        if(Player_Manager.singleton.mainPlayer != null)
        { 
            Player_Manager.singleton.mainPlayer.actorController.actorData.tickTime(delta);
        }

        foreach (Player_Entity mp in Player_Manager.singleton.multiPlayers)
        {
            mp.actorController.actorData.tickTime(delta);
        }

        /*
        foreach (Actor_Data actor in actorDatas)
        {
            actor.ageActor(delta);
        }
        */

    #if false
        //Parallel http://answers.unity3d.com/questions/486584/how-to-create-a-parallel-for-in-unity.html
        //http://stackoverflow.com/questions/831009/thread-with-multiple-parameters
        for (int i = 0; i < actorDatas.Count; i += 100)
        {
            int start = i;
            int end = i += 100;
            ParameterizedThreadStart pts = new ParameterizedThreadStart(obj => AgeActorParallel(start, end, delta));
            Thread threadForOneRegion = new Thread(pts);
            threadForOneRegion.Start();
        }
    #endif

    }

    private void deleteMakredActors(List<Actor_Data> actorList)
    {
        //TODO: do not delete actors/corpses that are active while the player can interact with them!
        //actorDatas.RemoveAll(item => item.markedForDelete == true);

        actorList.RemoveAll(item => item.markedForDelete == true);
    }

    #endregion

    // !!! Methods operating on a single ActorData go into ActorData itself!

    #region ActorData relationship Methods: e.g. with other Actors or Regions

    public void moveActorToRegion(Actor_Data actor, ushort regionID)
    {
        actor.region = regionID; //set actors current region

        if (actorDatasByRegion.ContainsKey(regionID)) //add to existing region list for fast access
        {
            actorDatasByRegion[regionID].Add(actor);
        }
        else //or create a new region list if the right one does not exist yet
        {
            actorDatasByRegion.Add(regionID, new List<Actor_Data> { actor });
        }
    }

    void breedAllActorsInRegion()
    {

    }

    void findJobsInRegion()
    {

    }

    void wanderRegion()
    {

    }



    #endregion


    #region parallel Methods

    #endregion

    #region Active Actors Methods

    public void setActiveRegion(byte regionID)
    {
        clearActiveActors();
        populateRegion(regionID);
    }

    protected void populateRegion(byte regionID)
    {
        foreach (Actor_Data actordata in actorDatasByRegion[regionID])
        {
            foreach (Actor_Controller actorController in activeActors)
            {
                if (!actorController.getActorActive())
                {
                    actorController.placeActorAtPos(actordata, new Vector3(Random.Range(0.0f, 50.0f), 0.0f, Random.Range(0.0f, 50.0f)));
                    break;
                }
            }
        }
    }

    public void clearActiveActors()
    {
        foreach(Actor_Controller actor in activeActors)
        {
            actor.resetActor();
        }
    }

    #endregion



}
