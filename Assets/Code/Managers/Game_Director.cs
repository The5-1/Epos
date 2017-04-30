using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

//[System.Serializable] //makes it visible in the editor even if it is no MonoBehaviour derived class
public class Game_Director : MonoBehaviour {

    //[SerializeField] NOTE:
    // - does not work with propperties with accessors!
    // - does not work on static variables!

    public float DEBUGTIMER = 10;
    public float DEBUGTIMERINTERVALL = 10;
    public byte DEBUGREGION = 1;

    public static Game_Director singleton; //a static field can be accesed from everywhere via Game_Director.singleton!!!

    public byte _playerCurrentRegionID;


    protected void Awake()
    {
        // Awake is for initialisation and connecting scripts!!!

        // Check if there is already a singleton instance
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(singleton);
            init();
        }
        else {
            //if there already was one, destroy this new instance again
            Destroy(this);
        }
    }

    protected void OnDestroy()
    {
        if (singleton == this)
        {
            singleton = null;
        }   
    }

    public void FixedUpdate () {

        if (Input.GetKeyDown("f"))
        {
            DEBUGREGION = (byte)Random.Range(1, 10);
            setActiveRegion(DEBUGREGION);
        }

        /*
        DEBUGTIMER += Time.deltaTime;

        if(DEBUGTIMER>DEBUGTIMERINTERVALL)
        {
            DEBUGREGION = (byte)Random.Range(1, 10);
            setActiveRegion(DEBUGREGION);
            DEBUGTIMER = 0;
        }
        */
    }

    public void init()
    {
        Debug.Log("Game_Director.init()");

    }

    public void setActiveRegion(byte regionID)
    {
        Region_Manager.singleton.setActiveRegion(regionID);
        Actor_Manager.singleton.setActiveRegion(regionID);

    }

}


#if false

//Test for a true, non-MonoBehaviour Singleton
//https://msdn.microsoft.com/en-us/library/ff650316.aspx
//http://stackoverflow.com/questions/1008019/c-singleton-design-pattern
[System.Serializable]
public class Game_Director_nonMB
{

    public float DEBUGTIMER = 0;
    public float DEBUGTIMERINTERVALL = 10;
    public byte DEBUGREGION = 0;

    private static Game_Director_nonMB _singleton;

    public static Game_Director_nonMB singleton
    {
        get
        {
            //lazy evaluation, the instance is only created when requested
            if(_singleton == null)
            {
                _singleton = new Game_Director_nonMB();
            }
            return _singleton;
        }
    }

    private Game_Director_nonMB()
    {
        //normal constructor
    }

    ~Game_Director_nonMB()
    {
        _singleton = null;
    }

    public void FixedUpdate()
    {
        DEBUGTIMER += Time.deltaTime;

        if (DEBUGTIMER > DEBUGTIMERINTERVALL)
        {
            DEBUGREGION = (byte)Random.Range(1, 10);

            DEBUGTIMER = 0;
        }
    }

}

#endif