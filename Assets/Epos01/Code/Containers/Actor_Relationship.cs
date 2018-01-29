using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActorRelationshipType {Love, Friend, Known, Foe, Hate}; //unknown means no relationship

public struct Actor_Relationship
{
    Actor_Data _ActorA;
    Actor_Data _ActorB;
    ActorRelationshipType _relationshipType;

    //This still can mean that A attacks B first: when B's character is "peaceufll" and "reactive" he waits till he is beaten
}

public class Actor_Relationships
{
    public List<Actor_Relationship> _relationships;
}
