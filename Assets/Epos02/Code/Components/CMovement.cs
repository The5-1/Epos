using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace The5_Epos02
{
    /* See Unity example ThirdPerson Controler
     * Generic movement component that moves actors in ThirdPerson and RTS, AI or Player input
     * exposes a Move() function
    */

    [RequireComponent(typeof(CActor))]
    [RequireComponent(typeof(CharacterController))] //!! this already creates such a component when it does not exist yet!
    public class CMovement : MonoBehaviour
    {

        public const float movementSpeedConstant = 500.0f;

        public CActor actor;
        public CharacterController cc;

        public float walk;
        public float strave;

        private void Awake()
        {
            init();
            refreshCharacterController();
        }

        public void init()
        {
            actor = this.gameObject.GetComponent<CActor>();
            cc = this.gameObject.GetComponent<CharacterController>(); //RequireComponent already crates one!
        }

        private void refreshCharacterController()
        {
            //Debug.Log(actor.data.height);
            //Debug.Log(cc.height);
            cc.height = actor.data.height;
            cc.radius = actor.data.radius;
            cc.center = this.gameObject.transform.position + new Vector3(0.0f, actor.data.height * 0.5f, 0.0f);
            cc.skinWidth = actor.data.radius * 0.1f;   //rigid body penetration tollerance
            cc.slopeLimit = 45.0f;                     //max slope angle in degrees
            cc.stepOffset = actor.data.height * 0.25f;  //max stair step height
            cc.minMoveDistance = 0.001f;               //avoid jittering, default = 0.001f;
        }


        public float getActorMoveSpeed()
        {
            return actor.data.moveSpeed;
        }


        public void move(Vector3 direction)
        {
            float length = direction.magnitude;
            if (length > 1.0f)
            {
                direction /= length;
            }
            cc.SimpleMove(direction * getActorMoveSpeed() * Time.deltaTime * movementSpeedConstant);
        }



        public void Update()
        {
            //!!! This does NOT call update, its controlled externally !!!
        }

        public void FixedUpdate()
        {
            //!!! This does NOT call update, its controlled externally !!!
        }
    }

}