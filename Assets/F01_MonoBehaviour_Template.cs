using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F01_MonoBehaviour_Template : MonoBehaviour {

    //https://docs.unity3d.com/Manual/ExecutionOrder.html

    /**1.1) Called when object is instantiated (even when inactive!)*/
    void Awake()
    {
        //Debug.Log(string.Format("Awake()"));
    }

    /**1.2) Called after Awake if the object is enabled, called every time it is re-enabled*/
    void OnEnable()
    {

    }

    /**1.3) Once after initialization at the start of the first Update phase*/
    void Start()
    {

    }

    /**2.1) Constant time step for simulation (configurable in Edit > Project Settings > Time) */
    void FixedUpdate()
    {
        //Debug.Log(string.Format("FixedUpdate() delta time {0}", Time.deltaTime));
    }

    /**2.2) Framerate dependent, called every frame for visible changes, likely needs multiplication with Time.deltaTime */
    void Update()
    {
        //Debug.Log(string.Format("Update() delta time {0}", Time.deltaTime));
    }

    /**2.3) A second pass of Updates called after Updates and Coroutines */
    void LateUpdate()
    {
        //Debug.Log(string.Format("LateUpdate() delta time {0}", Time.deltaTime));
    }


    /**3.1) framerate dependent in response to GUI events */
    void OnGui()
    {

    }

    /**4.1) Called shortly before the update loop ends, if the object is inactive */
    void OnDisable()
    {

    }

    /**5.1) Called at the verry end of the update loop, cleanup */
    void onDestroy()
    {

    }

}
