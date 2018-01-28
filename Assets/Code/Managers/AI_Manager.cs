using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Manager : MonoBehaviour {

    public static AI_Manager singleton;

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

    private void Update()
    {
        ControllAI();
    }

    private void init()
    {

    }

    private void ControllAI()
    {

        AI_random_Move();
    }

    private void AI_random_Move()
    {
        foreach (Actor_Controller AC in Actor_Manager.singleton.activeActors)
        {
            //AC.actorMovementController.MoveRelativeTarget(Random.insideUnitSphere);
        }
    }

}
