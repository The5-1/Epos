using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Family_Data {

    public Actor_Data _parentA;
    public Actor_Data _parentB;

    public List<Actor_Data> _children;

    public ushort _homeRegionID;

    public Building_Data _familyHomeBuilding;
    public Job_Data _familyJob;

}
