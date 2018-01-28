using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using The5.DataStructures;


[System.Serializable]
public abstract class InputModeState : The5.DataStructures.State
{
    protected abstract void collectInput(); //not all states need to gather all inputs at all times!
    protected abstract void handleInput();


    public void Update()
    {
        collectInput();
        handleInput();
    }
}

[System.Serializable]
public class MenuMode : InputModeState
{
    protected override void collectInput()
    {
    }

    protected override void handleInput()
    {
        return;
    }
}

[System.Serializable]
public class ActorMode : InputModeState
{
    public CActor playerActor;
    public CMovement cMovement;
    public CCombat cCombat;

    public float moveH;
    public float moveV;
    public float lookH;
    public float lookV;
    public float attackA;
    public float attackB;

    Vector3 moveInput;
    Vector3 lookInput;

    public ActorMode()
    {
        moveInput = new Vector3();
        lookInput = new Vector3();
    }

    public void setTargetActor(CActor actor)
    {
        playerActor = actor;
        this.cMovement = actor.GetComponent<CMovement>();
        this.cCombat = actor.GetComponent<CCombat>();
    }

    protected override void collectInput()
    {
        moveH = Input.GetAxis("Horizontal");
        moveV = Input.GetAxis("Vertical");
        lookH = Input.GetAxis("MouseX");
        lookV = Input.GetAxis("MouseY");
        attackA = Input.GetAxis("AttackA");
        attackB = Input.GetAxis("AttackB");
    }

    void calcMovmentInputs()
    {
        moveInput.Set(moveH, 0.0f, moveV);
        lookInput.Set(lookH, 0.0f, lookV);
    }

    void sendMovementInputs()
    {
        cMovement.move(moveInput);
    }

    protected override void handleInput()
    {
        if (playerActor != null)
        {
            calcMovmentInputs();
            sendMovementInputs();
        }

    }

}



public class CInputHandler : MonoBehaviour {

    public InputModeState activeMode;
    public MenuMode menuMode;
    public ActorMode actorMode;

    public void setTargetActor(CActor actor)
    {
        actorMode.setTargetActor(actor);
    }

    public void init()
    {
        menuMode = new MenuMode();
        actorMode = new ActorMode();

        activeMode = actorMode;
    }

    private void Awake()
    {
        init();
    }

    private void Update()
    {
        activeMode.Update();
    }

}
