using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using The5.DataStructures;

/* We need no centralized input handler that passes inputs along, unity already does that
 * Unity already wraps them into axes and each component can care for input itself
 * we just need a global state machine to disable/toggle component states if needed
 */


#if false
namespace The5_Epos02
{

    [System.Serializable]
    public abstract class InputMode : The5.DataStructures.State
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
    public class MenuInputMode : InputMode
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
    public class RTSInputMode : InputMode
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
    public class ActorInputMode : InputMode
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

        public ActorInputMode()
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

    public class InputSystem : MonoBehaviour
    {

        public InputMode activeMode;
        public MenuInputMode menuMode;
        public ActorInputMode actorMode;

        public void setTargetActor(CActor actor)
        {
            actorMode.setTargetActor(actor);
        }

        public void init()
        {
            menuMode = new MenuInputMode();
            actorMode = new ActorInputMode();

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

}

#endif