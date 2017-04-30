using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public List<Actor_Data> Actors;
    public uint _maxActors = 100000; //ushort 16bit = 65,535 //uint 32bit = 4.3bil;

    // The pool of actual actors, they are reset on region change and get new data plugged in
    // possibly dynamically resize the pool? Delete some but not all.
    public List<Actor> ActiveActpors; //TODO: How do i need to apply DontDestroyOnLoad to those actor-GameObject pools?
    public ushort _minActiveActors = 50;

    public Dictionary<int, List<Actor_Data>> _actorsDataPerRegion; //FIXME: just go over the regions instead of making a dictionary

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

    // This method is called when the script is destroy
    protected void OnDestroy()
    {
        // If this is the currently active singleton, delete the static instance too
        if (singleton == this) { singleton = null; }
    }

    public void init()
    {
        Debug.Log("Actor_Manager.init()");

        Actors = new List<Actor_Data>();
        _actorsDataPerRegion = new Dictionary<int, List<Actor_Data>>();
        ActiveActpors = new List<Actor>();

        _activeActorsGroup = new GameObject("Active_Actors");
        _activeActorsGroup.transform.parent = this.gameObject.transform;


        //TODO: if a savegame exists with existing actors load that


        loadPrefabs();

        DEBUG_makeActorData();


        updateAllActoraDataPerRegion();

    }

    protected void loadPrefabs()
    {
        _DEBUGActorPrefab = (GameObject)Resources.Load("Debug/Debug_Actor"); //NOTE: use this with instantiate!
        if (_DEBUGActorPrefab == null) { Debug.Log("Resources.Load(\"Debug/Debug_Actor\") is null"); }
    }
    #endregion


    public void DEBUG_makeActorData()
    {
        Debug.Log("!!! init with Dummy Actor Data!");
        for (int i = 0; i < _maxActors; i++)
        {
            Actor_Data tmp = new Actor_Data();
            Actors.Add(tmp);
        }

        for (int i = 0; i < _minActiveActors; i++)
        {
            ActiveActpors.Add(new GameObject("Pooled_Actor").AddComponent<Actor>());
            ActiveActpors[i].transform.parent = _activeActorsGroup.transform;

#if false
            //HACK: the only hacky way to get a cube mesh :(
            GameObject tmp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Mesh mesh = tmp.GetComponent<MeshFilter>().sharedMesh; 
            Material mat = tmp.GetComponent<MeshRenderer>().material;
            _activeActorsPool[i]._parentGO.AddComponent<MeshFilter>().sharedMesh = mesh;
            _activeActorsPool[i]._parentGO.AddComponent<MeshRenderer>().material = mat;
            GameObject.Destroy(tmp);
#else
            GameObject debugActorTMP = Instantiate((GameObject)Resources.Load("Debug/Debug_Actor"));
            debugActorTMP.transform.SetParent(ActiveActpors[i].gameObject.transform);
#endif
            ActiveActpors[i].resetActor();
        }
    }

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

    public void actorChangeRegion(Actor_Data actor, byte newRegionID)
    {

    }

    public void addActor()
    { }


    #region Active Actors Methods

    public void setActiveRegion(byte regionID)
    {
        clearActiveActors();
        populateRegion(regionID);
    }

    protected void populateRegion(byte regionID)
    {
        foreach(Actor_Data actordata in Actors)
        {
            if(actordata.CurrentRegionIndex == regionID)
            {
                foreach (Actor actor in ActiveActpors)
                {
                    if (!actor.getActorActive())
                    {
                        actor.placeActorAtPos(actordata, new Vector3(Random.Range(-10.0f, 10.0f), 0.0f, Random.Range(-10.0f, 10.0f)));
                        break;
                    }
                }
            }
        }
    }

    public void clearActiveActors()
    {
        foreach(Actor actor in ActiveActpors)
        {
            actor.resetActor();
        }
    }

    #endregion


    #region Single Actor Methods


    public void AgeActor(Actor_Data actor, ulong timeInSeconds)
    {
        actor.ageActor(timeInSeconds);
    }

    #endregion

    #region All Actors Methods

    public void RepopulateWorld()
    {

    }

    #endregion

}
