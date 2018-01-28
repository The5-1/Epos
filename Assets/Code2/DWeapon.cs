using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DWeaponDamageType { Sharp=0, Blunt=1};


public class DWeapon {

    public DWeaponDamageType damageType;
    public float damage;
    public float reach;
    public float weight;

    public DWeapon(DWeaponDamageType damageType, float damage, float reach, float weight)
    {
        this.damageType = damageType;
        this.damage = damage;
        this.reach = reach;
        this.weight = weight;
    }

    static DWeapon makeFist()
    {
        return new DWeapon(DWeaponDamageType.Blunt, 1.0f, 1.0f, 0.0f);
    }

    static DWeapon makeSword()
    {
        return new DWeapon(DWeaponDamageType.Sharp, 2.0f, 1.5f, 1.0f);
    }
}
