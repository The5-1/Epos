using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorDeadState : MonoBehaviour {

    public Actor _parentActor;
    public GameObject _parentGO;
    public bool isAlive;


    //FIXME: this should not be called every frame but just when the actor changes! Event system...
    void FixedUpdate()
    {

        _parentGO = this.transform.parent.gameObject;
        _parentActor = _parentGO.GetComponent<Actor>(); //the actor data can chang so recheck it
        if (_parentActor)
        {
            isAlive = _parentActor._actorData.Alive;

            if (isAlive)
            {
                this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                this.transform.localPosition = new Vector3(0.0f, 1.0f, 0.0f);
            }
            else
            {
                this.transform.localScale = new Vector3(1.5f, 0.3f, 1.5f);
                this.transform.localPosition = new Vector3(0.0f, 0.3f, 0.0f);
            }

        }
    }
}
