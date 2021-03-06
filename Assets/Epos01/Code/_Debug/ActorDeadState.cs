﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorDeadState : MonoBehaviour {

    public Actor_Controller _parentActor;
    public GameObject _parentGO;
    public bool isAlive;


    //FIXME: this should not be called every frame but just when the actor changes! Event system...
    void FixedUpdate()
    {

        _parentGO = this.gameObject;
        _parentActor = _parentGO.GetComponent<Actor_Controller>(); //the actor data can chang so recheck it
        if (_parentActor)
        {
            isAlive = _parentActor.actorData.Alive;

            if (isAlive)
            {
                this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            else
            {
                this.transform.localScale = new Vector3(1.0f, 0.3f, 1.0f);
            }

        }
    }
}
