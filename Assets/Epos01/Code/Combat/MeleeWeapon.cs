using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GripHandType {main, second};

public enum GripPointType { main, second,extended, mmaximum };

public struct GripPoint
{
    Vector3 posOnWeapon;
    GripHandType hand;
}


public struct WeaponGrip
{
    GripPoint grip_main; //default position for main hand
    GripPoint grip_second; //twohanded default position for 2nd hand

    GripPoint grip_extended; // ricasso

    GripPoint grip_maximum; //halfsword or spear at the 


}

public class WeaponStance
{
    //describe a stance
}

public class WeaponMove
{
    //describe any attack, transitoon to other stance, block, parry, bounce-back, ...
}

public class WeaponMoveClip
{
    //combination of move segments: stance transition --> wind up --> strike --> recover
}

public class WeaponAnimator
{
    WeaponGrip grip;
    List<WeaponStance> stances;
    List<WeaponMove> moves;

    public void ChangeGrip(GripHandType hand, GripPointType destination)
    { }

    public void ResetGrip()
    { }
}