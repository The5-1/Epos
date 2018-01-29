#if false

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

//Static ? Singleton ? Grid/Verfices ? Loosely coupled Events

//Singletons or Static class?
//Static:
//- simpler
//- initialized lazy, as late as possible (you lose control over the exact behaviour)
//- Owl says lazy is a good thing

//Singleton: 
//- can inherit from MonoBehaviour (-> coroutines)
//- when you need exactly one instance, not if you ONLY want simple global access
//- more complicated to track trough code
//- you can store a reference to the singleton in a variable and pass it to a method
//      public Game_Director gd = Game_Director.singleton;
//- you can inherit from Singleton classes
//---> I can't imagine why i would need either right now...

//https://forum.unity3d.com/threads/static-class-or-singleton.106694/#post-704780
//!!!
//Use Singleton if it is important that the class remains a MonoBehaviour.
//If not, use a static class, because on unorganized/bad code singletons can become quite a mess.


//Advanced Grid system Code
//http://answers.unity3d.com/questions/552780/singleton-usage-wherever-possible.html
//detailed explaination
//old Grid-System http://answers.unity3d.com/questions/551297/how-can-i-instantiate-a-prefab-from-a-static-funct.html#comment-551387
//new "Verfices"-System https://forum.unity3d.com/threads/verfices-a-well-written-clean-documented-and-optimized-global-access-system.210080/
//                     ---> https://pastebin.com/n1SSE3h3


//http://answers.unity3d.com/questions/550725/level-independant-data.html
//--> the comment from "Jamora" about "Monostate Objects"

//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//!!! static delegate event system !!! by angrypenguin
//https://forum.unity3d.com/threads/static-class-or-singleton.106694/#post-1362317
//For instance, rather than having a "Pause" button that calls a function on a "GameManager" singleton, I have a "Pause" button that fires an event "PauseRequested". 
//Everything that needs to somehow respond to the game being paused is subscribed to that event and therefore has an appropriate method called.
//https://forum.unity3d.com/threads/verfices-a-well-written-clean-documented-and-optimized-global-access-system.210080/#post-1558563
//https://www.codeproject.com/Articles/16667/Loosely-Coupled-Event-Driven-Programming
//http://stackoverflow.com/questions/550785/c-events-or-an-observer-interface-pros-cons
//[9:36 PM] IsaacPaul: @The5_1 
// That post basically describes the observer pattern
// I use something similar with my networking
// _networkConnection.RegisterListenerWeak<MsgBattleList>(ReceivedBattleList);


//https://www.dotnetperls.com/singleton-static //Singleton VS Static


//NOTE: unity singletons work slighty different since MonoBehaviours have no custom constructor

//https://www.youtube.com/watch?v=Ozc_hXzp_KU&feature=youtu.be&t=5m20s //Singleton

//http://answers.unity3d.com/questions/552780/singleton-usage-wherever-possible.html
//http://answers.unity3d.com/questions/551297/how-can-i-instantiate-a-prefab-from-a-static-funct.html
//http://answers.unity3d.com/questions/1127071/problem-with-dontdestroyonload-and-scenelevel-load.html#answer-1128034



//It seems like it is mostly a matter of taste, untill you actually need to use a singleton.
//Just for convenience make all managing class instances globally accessible
public static class GlobalContainer {

    static public GameObject _globalContainer
    {
        get { return _globalContainer; }
        set { setonce<GameObject>(_globalContainer,value);}
    }

    static public GameObject _gameDirectorGO
    {
        get { return _gameDirectorGO; }
        set { setonce<GameObject>(_gameDirectorGO, value); }
    }

    static public Game_Director _gameDirector
    {
        get { return _gameDirector; }
        set { setonce<Game_Director>(_gameDirector, value); }
    }

    static public GameObject _timeGroup
    {
        get { return _timeGroup; }
        set { setonce<GameObject>(_timeGroup, value); }
    }

    static public World_Time_Manager _worldTimeManager
    {
        get { return _worldTimeManager; }
        set { setonce<World_Time_Manager>(_worldTimeManager, value); }
    }

    static public GameObject _actorGroup
    {
        get { return _globalContainer; }
        set { setonce<GameObject>(_globalContainer, value); }
    }

    static public Actor_Manager _actorManager
    {
        get { return _actorManager; }
        set { setonce<Actor_Manager>(_actorManager, value); }
    }

    static public GameObject _activeActorsGroup
    {
        get { return _activeActorsGroup; }
        set { setonce<GameObject>(_activeActorsGroup, value); }
    }

    static public GameObject _regionGroup
    {
        get { return _regionGroup; }
        set { setonce<GameObject>(_regionGroup, value); }
    }

    static public Region_Manager _regionManager
    {
        get { return _regionManager; }
        set { setonce<Region_Manager>(_regionManager, value); }
    }

    static public GameObject _simulatorGroup
    {
        get { return _simulatorGroup; }
        set { setonce<GameObject>(_simulatorGroup, value); }
    }
    static public Actor_Simulator _actorSimulator
    {
        get { return _actorSimulator; }
        set { setonce<Actor_Simulator>(_actorSimulator, value); }
    }


    static public void setonce<T>(T property, T value)
    {
        if (property == null)
        {
            property = value;
        }
        else
        {
            Debug.LogWarning(string.Format("GlobalContainer: attempted to assign a new instance to existing type {0} ", typeof(T)));
        }

    }
}


#endif
