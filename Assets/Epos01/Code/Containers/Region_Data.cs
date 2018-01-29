using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RegionRouteType { same, traderoute, cooperating, closed, dangerous }; //same = regions count as one
public enum RegionDangerType { civil, guarded, wild, dangerous }; //e.g. dangerous region type prohibits breeding of NPCs
public enum RegionEconomyState { high, normal, low }; 

public class RegionRoute
{
    Region_Data _regionA;
    Region_Data _regionB;

    RegionRouteType routeType;
    //"dangerous" would need further checking of which of the two regions is dangerous
    //if it is "closed" the player needs to interact with a guardian NPC to pass without consequences

}



[System.Serializable] //makes it visible in the editor if it is no MonoBehaviour
public class Region_Data {

    //http://answers.unity3d.com/questions/176667/create-a-new-scene-from-editor-script.html
    //Create a Scene and Save it

    #region Fields

    public readonly ushort RegionSceneIndex; //Unity loads scenes by index too
    public ushort parentRegion; //for nested regions like houses or dungeons

    public Vector3 positionOnWorldMap; //easier for calculating actual distance and travel time // vec3 for flying islands or underground

    public ulong lastRegionUpdateTime; //the region needs to be able to calculate the delta since the last update

    #region state, politics, economy
    public RegionDangerType dangerType;

    #endregion

    #region population
    public ushort IdealPopulation;

    //this could be done with events and saved in the Actor Data instead
    public List<Actor_Data> ActorsRegisteredToThisRegion; //NPC that are ouside but live here or work here, generally care for this region

    //public List<Actor_Data> ActorsCurrentlyInThisRegion;
    //public Actor_Personality _regionAveragePersonality;
    
    #endregion

    #endregion

    public Region_Data(ushort index)
    {
        RegionSceneIndex = index;
    }
}
