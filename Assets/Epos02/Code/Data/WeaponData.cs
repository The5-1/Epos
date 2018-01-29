using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace The5_Epos02
{

    public enum DWeaponDamageType { Sharp = 0, Blunt = 1 };


    public class WeaponData
    {

        public DWeaponDamageType damageType;
        public float damage;
        public float reach;
        public float weight;

        public WeaponData(DWeaponDamageType damageType, float damage, float reach, float weight)
        {
            this.damageType = damageType;
            this.damage = damage;
            this.reach = reach;
            this.weight = weight;
        }

        static WeaponData makeFist()
        {
            return new WeaponData(DWeaponDamageType.Blunt, 1.0f, 1.0f, 0.0f);
        }

        static WeaponData makeSword()
        {
            return new WeaponData(DWeaponDamageType.Sharp, 2.0f, 1.5f, 1.0f);
        }
    }

}