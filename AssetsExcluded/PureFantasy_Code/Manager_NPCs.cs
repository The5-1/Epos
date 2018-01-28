using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This will create all NPCs in the active region
//maintain the list of all NPC data that is not active but stored in the simulation
//


//TIP: singleton for things that exist once
//TIP: object pooling for things that lots exist of

//singleton
//dontDestroyOnLoad
public class Manager_NPCs : MonoBehaviour {

    static public Manager_NPCs singletonInstance { get; private set; }

    public List<ActorData> _ActorDataList;

    public int maxActors = 100;

    // This method is called when the script is loaded
    protected void Awake()
    {
        // Check if there is already a singleton instance
        if (singletonInstance == null) // No? This is the first one
        {singletonInstance = this;}
        else // a instance already exists, destroy this
        { Destroy(this); }
    }

    // This method is called when the script is destroy
    protected void OnDestroy()
    {
        // If this is the currently active singleton, delete the static instance too
        if (singletonInstance == this) { singletonInstance = null;}
    }


    // Use this is called when everything in the world was loaded
    void Start () {
	    for(int i = 0; i< maxActors; i++)
        {
            _ActorDataList.Add(new ActorData());
        }
    }
	
    // Update is called once per frame
    void Update () {
		
    }


    void addRandomActor()
    {

    }


    void spawnActors(int region)
    {

    }

}
