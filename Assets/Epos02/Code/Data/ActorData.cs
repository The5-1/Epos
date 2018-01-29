using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace The5_Epos02
{


    /* Store ALL the possible Actor Data a regular actor (e.g. Soldier/Townsman) needs!
     * Special actors (e.g. Immobile) get special treatment to disable what they dont need.
     */
    [System.Serializable]
    public class ActorData
    {
        /* basic stats */
        public string name = "nameless Actor";

        public float health = 100.0f;
        public float stamina = 100.0f;
        public float mana = 100.0f;

        /* Body */
        public float height = 2.0f;
        public float radius = 0.3f;

        /* movement */
        public float moveSpeed = 1.0f;
        public float turnSpeed = 1.0f;

        /* combat */
        public float attackPower = 1.0f;
        public float attackSpeed = 1.0f;

        //public Skeleton skeleton;
        //public List<BodyParts> bodyParts;


    }

}