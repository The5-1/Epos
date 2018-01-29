using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace The5_Epos02
{
    /* See Unity example ThirdPerson Controler
     * 
     */

    [RequireComponent(typeof(CMovement))]
    public class CMovementPlayerThirdperson : MonoBehaviour
    {
        public CameraSystem cameraSystem;
        public CMovement cMovement;

        private void Awake()
        {
            cMovement = this.GetComponent<CMovement>();
            cameraSystem = EposGame.cameraSystem;
        }

        void FixedUpdate()
        {

        }
    }
}
