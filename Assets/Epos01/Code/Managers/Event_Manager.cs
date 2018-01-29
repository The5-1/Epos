using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_Manager : MonoBehaviour {

    static public Event_Manager singleton;

    public Dictionary<string, Event> _worldEventsList; //unity internal event system, see how that works

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

    protected void init()
    {
        _worldEventsList = new Dictionary<string, Event>();
    }
}
