using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CActor : MonoBehaviour {

    public DActorData data;
    public CMovement movement;
    public CCombat combat;

    private void init()
    {
        data = new DActorData();
        movement = this.gameObject.AddComponent<CMovement>();
        combat = this.gameObject.AddComponent<CCombat>();
    }

    void Awake()
    {
        init();
#if true
        CInputHandler playerInput = this.gameObject.AddComponent<CInputHandler>();
        playerInput.setTargetActor(this);
#endif
    }

}
