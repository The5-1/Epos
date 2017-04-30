using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Data {

    public ushort _buildTime; //time to construct
    public ushort _abandonTime; //time to become abandoned when not inhabited //e.g. a tent is after 1 season //a fortress after 2 years // --> "Ghost town XYZ"
    public ushort _collapseTime; //time to colapse when abandoned //---> "Ruins of XYZ"
    public ushort _decayTime; //time for the rubble to be fully gone from the map //---> "ancient Ruins of XYZ"

    //******* Inherit further stats like health, armor, armortype etc from Actor_Data or a more generic class above it!!! ******


    //public BuildingType _buildingType;
    //a forge can be repurposed to a school
    //rebuilding substracts the current buildings buildtime from the target buildings + 1 year padding if 0

    //public List<Building> _dependentBuildings; 
    //e.g. a bakery cant work if the farm supplying it is uninhabited? Not sure if we go that deep.

    //public List<Building> _nestedBuildings; 
    //e.g. barracks cant work if the fortress it stands in to is abandoned ---> it can, a lacking landord needs to cause something else though
}
