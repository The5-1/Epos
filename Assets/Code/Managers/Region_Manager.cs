using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region_Manager : MonoBehaviour {

    //TODO: Some tree datastructure that saves the possible transitions to other regions for faster neighborgood traversal?

    static public Region_Manager singleton;

    public int CurrentActiveRegionIndex;

    public Dictionary<int, Region_Data> RegionsByID;

    public Dictionary<int, long> RegionDistanceInHours; //TODO: update this based on player position and use it for the LOD of the simulation


    #region Events

    public delegate void PlayerChangedRegion();
    public static event PlayerChangedRegion PlayerChangedRegionEvent;

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


    public void init()
    {
        Debug.Log("Region_Manager.init()");
        RegionsByID = new Dictionary<int, Region_Data>();

        makeRandomRegions(); //DEBUG
    }

    protected void loadUnityScenesToRegions()
    {
        //TODO
    }

    public void makeRandomRegions()
    {
        for (ushort i = 0; i < 5; i++)
        {
            RegionsByID.Add(i, new Region_Data(i));
        }
    }

    public void setActiveRegion(int index)
    {
        if (PlayerChangedRegionEvent != null) PlayerChangedRegionEvent();

        CurrentActiveRegionIndex = index;


    }



}
