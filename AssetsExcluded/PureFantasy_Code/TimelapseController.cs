using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// sotres a copy of the actors values in a container
/// updates the container continously
/// </summary>

//[System.Serializable] // so i can see it in the inspector
public class TimelapseContainer
{
    public Vector3 _pos;
    public Quaternion _rot;
    public int _HP;
    public int _MP;
    public int _EP;

    public TimelapseContainer(Actor actor)
    {
        _pos = actor.transform.position;
        _rot = actor.transform.rotation;

        _HP = actor._ActorData._HP;
        _MP = actor._ActorData._MP;
        _EP = actor._ActorData._EP;
    }
}



public class TimelapseController : MonoBehaviour {

    public bool _running = true;

    public Actor _actor;
    public float _rewindTime = 5.0f;
    private float _updateIntervall;

    public int _numStored = 20;
    private int _currentIndex;
    public List<TimelapseContainer> _statsList;

    void Awake()
    {
        _actor = this.GetComponent<Actor>();

        _updateIntervall = _rewindTime / _numStored;
        _currentIndex = 0;

        
        _statsList = new List<TimelapseContainer>();
        _statsList.Clear();
        for (int i = 0; i < 20; i++)
        {
           TimelapseContainer container = new TimelapseContainer(this._actor);
           _statsList.Add(container);
        }
    }

    void Start()
    {
        StartRecord();
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height - 50, 200, 40), "Timelapse!"))
        {
            //LoadState();
            LoadStateInterpolated();
        }
    }

    void StartRecord()
    {
        if (_running) InvokeRepeating("SaveState", 0.0f, _updateIntervall);
    }

    void StopRecord()
    {
        CancelInvoke(); //all invokes
        //CancelInvoke("SaveState"); //cancel the invokeRepeating (does that cancel all of them)?
    }

    void SaveState()
    {
        TimelapseContainer container = new TimelapseContainer(this._actor);
        _statsList[_currentIndex] = container;
        _currentIndex = (_currentIndex + 1) % _numStored;
    }

    void LoadState()
    {
        int indextorestore = (_currentIndex + 1) % _numStored;
        this._actor._ActorData._HP = _statsList[indextorestore]._HP;
        this._actor._ActorData._MP = _statsList[indextorestore]._MP;
        this._actor._ActorData._EP = _statsList[indextorestore]._EP;
        this.transform.position = _statsList[indextorestore]._pos;
        this.transform.rotation = _statsList[indextorestore]._rot;
    }

    void LoadStateInterpolated()
    {
        this.StartCoroutine(MoveEnumerator());
    }

    //Coroutine
    IEnumerator MoveEnumerator()
    {
        //stop recording states
        StopRecord();
        int i = _currentIndex;
        while (i != (_currentIndex + 1) % _numStored)
        {
            //negative modulo does not seem to work propperly, so if i is negatetive here add _numStored
            if (i < 0) i += _numStored;
            //which might lead to meeting the exit condition, so break in that case.
            if (i == _currentIndex + 1) break;

            this._actor._ActorData._HP = _statsList[i]._HP;
            this._actor._ActorData._MP = _statsList[i]._MP;
            this._actor._ActorData._EP = _statsList[i]._EP;
            this.transform.position = _statsList[i]._pos;
            this.transform.rotation = _statsList[i]._rot;   
            i = ((i - 1) % _numStored);
            yield return null;
        }
        // at the end of the coroutine:
        // clear the list and set all of them to the point of this rewind
        _statsList.Clear();
        for (int j = 0; j < 20; j++)
        {
            TimelapseContainer container = new TimelapseContainer(this._actor);
            _statsList.Add(container);
        }
        //resume recording states
        StartRecord();
    }


    void OnDisable()
    {
        StopRecord();
    }
}



/* Does not work
void SaveStateClone()
{
    _actor.CloneActor(_clone);
}

void LoadStateClone()
{
    _clone.CloneActor(_actor);
}
*/


// attempt with creating new game objects
/*
public class TimelapseContainer
{
    public GameObject _gameObject;
    public Actor _actor;
    public Vector3 pos;
    public Vector3 rot;

    public TimelapseContainer(Transform transform)
    {
        _gameObject = new GameObject();
        _actor = _gameObject.AddComponent<Actor>();
    }
}

*/
