using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Game_Loader : MonoBehaviour {

    /// <summary>
    // SINGLETON
    //-----------
    //static: can be accessed from anywhere with "Game_Loader.singleton.doStuffHere()"
    //singleton design pattern makes sure only one instance of this class can exist at any time, see Awake()
    //When to use singleton? https://stackoverflow.com/questions/519520/difference-between-static-class-and-singleton-pattern
    //- pass the singleton to a method, just so it fits existing code. You cant with a static.
    //- implement a interface, a static can't do that
    //-----------
    // If inherit from MonoBehaviour so it cant be static! ---> Singleton
    // But i could just as well just use static classes inside here.
    //-----------
    // Conclusion: I don't rally need singletons
    /// </summary>
    public static Game_Loader singleton;


    //[SerializeField]
    //public Game_Director_nonMB GameDirectorInspector;


    /// <summary>
    /// Awake:
    /// - for setting up references between scripts
    /// - initialisation
    /// - called even when the component is NOT enabled
    /// </summary>
    protected void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(singleton);
            loadGame();
        }
        else
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// Start:
    /// - called when the component is enabled
    /// - delay parts till they are really needed
    /// </summary>
    protected void Start()
    {

    }

    /// <summary>
    /// what happens when Destroy(this) is called on a MonoBehaviour
    /// </summary>
    protected void OnDestroy()
    {
        if (singleton == this)
        {
            singleton = null;

            //GameDirectorInspector = null;
        }
    }

    protected void loadGame()
    {
        loadResources();
        loadScripts();
        //LoadDataFiles();
        //LoadAssets();
        //LoadSavegame();

        //GameDirectorInspector = Game_Director_nonMB.singleton;
    }

    protected void loadResources()
    {
        //Anything with Resources.Load() should be done here first, before any other script wants to use it!
        GameObject resources_group = new GameObject("R_Resources_Loader_Group");
        resources_group.transform.parent = this.transform;
        resources_group.AddComponent<Material_Manager>();
    }

    protected void loadScripts()
    {
        GameObject game_group = new GameObject("0_Game_Group");
        game_group.transform.parent = this.transform;
        game_group.AddComponent<Game_Director>();
        game_group.AddComponent<GameTime>();

        GameObject camera_main = new GameObject("_Camera_main");
        camera_main.AddComponent<Camera_Manager>();
        DontDestroyOnLoad(camera_main);

        GameObject simulation_group = new GameObject("1_Simulation_Group");
        simulation_group.transform.parent = this.transform;

        GameObject actor_group = new GameObject("2_Actors_Group");
        actor_group.transform.parent = this.transform;
        actor_group.AddComponent<Actor_Manager>();

        GameObject region_group = new GameObject("3_Regions_Group");
        region_group.transform.parent = this.transform;
        region_group.AddComponent<Region_Manager>();

        GameObject player_group = new GameObject("4_Player_Group");
        player_group.transform.parent = this.transform;
        player_group.AddComponent<Player_Manager>();
        //player_group.AddComponent<PlayerInput_Manager>();

    }

    /// <summary>
    /// Only MonoBehaviours have FixedUpdate() or Update()
    /// other classes who need it should recieve event notifications when this Update cycle ticks
    /// </summary>
    protected void FixedUpdate()
    {
        
    }


}
