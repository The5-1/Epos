using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace The5_Epos02
{

    [RequireComponent(typeof(CMovement))]
    [RequireComponent(typeof(CCombat))]
    public class CActor : MonoBehaviour
    {

        public ActorData data;
        public CMovement movement;
        public CCombat combat;

        private void init()
        {
            data = new ActorData();
            movement = this.gameObject.GetComponent<CMovement>();
            combat = this.gameObject.GetComponent<CCombat>();
        }

        void Awake()
        {
            init();
        }

    }
}