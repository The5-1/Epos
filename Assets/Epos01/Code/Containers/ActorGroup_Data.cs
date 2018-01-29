using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Family_Data
{
    public Actor_Data _parentA;
    public Actor_Data _parentB;

    public List<Actor_Data> _children;

    public ushort _homeRegionID;

    public Building_Data _familyHomeBuilding;
    public Job_Data _familyJob;
}

public struct ActorGroup_Rank
{
    public string name;
    public string description;
}


/// <summary>
/// Base for any collection of NPCs
/// </summary>
/// 
public class ActorGroup_Data
{
    public List<ActorGroup_Rank> ranks; //0 is the god, 1 the highest after that
    public List<Actor_Data> members;
}

/// <summary>
/// The games religions
/// Diety, Religion, (Member,Rank)
/// </summary>
public class ReligionGroup_Data : ActorGroup_Data
{

}