using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KingdomRelationshipType { war, closed, allied };

public class Kingdom_Relationship
{
    Kingdom_Data _kingdomA;
    Kingdom_Data _kingdomB;

    KingdomRelationshipType _relationshipType;

}

//Technically Kingdoms and Regions could inherit from a similar superclass as actor, they all have relationships of some sort


public class Kingdom_Data {

    public byte _kingdomID;

    public string _kingdomName;

    public List<Region_Data> _regionsInThisKingdom;

    //kingdom politics


    public Kingdom_Data(byte ID)
    {
        _kingdomID = ID;
    }

}
