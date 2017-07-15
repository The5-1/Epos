using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Actor_Controller : MonoBehaviour {

    public GameObject _parentGO;
    public Actor_Data actorData;


    private void Awake()
    {
        _parentGO = this.gameObject;
    }

    void Start () {
		
	}

	void Update () {
		
	}

    public bool getActorActive()
    {
        return _parentGO.activeInHierarchy;
    }

    public void setActorActive(bool active)
    {
        _parentGO.SetActive(active);
    }

    protected bool setActorData(Actor_Data data)
    {
        //if there is already some actor data then init should not do anything
        if (actorData != null) { Debug.LogWarning("Can not init Actor with Data, there already is data in this one!", this); return false; }
        else
        {
            actorData = data;
            return true;
        }
    }

    protected void releaseActorData()
    {
        actorData = null; //This should be OK as actorManager always holds a reference to all the Data
    }

    public void resetActor() //Reset() is a Unity function and defines the default state in editor only
    {
        setActorActive(false);
        _parentGO.transform.Translate(Vector3.zero, Space.World);
        releaseActorData();
    }

    public void placeActorAtPos(Actor_Data data, Vector3 pos)
    {
        setActorData(data);
        //_parentGO.transform.Translate(pos, Space.World);
        _parentGO.transform.position = pos;
        setActorActive(true);
    }

}
