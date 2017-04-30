using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RegionRelationshipType { traderoute, cooperating, closed, dangerous };

public class Region_Relationship
{
    Region_Data _regionA;
    Region_Data _regionB;

    RegionRelationshipType _relationshipType;
    //"dangerous" would need further checking of which of the two regions is dangerous
    //if it is "closed" the player needs to interact with a guardian NPC to pass without consequences

}



[System.Serializable] //makes it visible in the editor if it is no MonoBehaviour
public class Region_Data {

    //http://answers.unity3d.com/questions/176667/create-a-new-scene-from-editor-script.html
    //Create a Scene and Save it


    public readonly int RegionSceneIndex; //Unity loads scenes by index too

    public ushort IdealPopulation;

    public List<Actor_Data> ActorsCurrentlyInThisRegion;

    //this could be done with events and saved in the Actor Data instead
    public List<Actor_Data> ActorsRegisteredToThisRegion; //NPC that are ouside but live here or work here, generally care for this region

    //public Actor_Personality _regionAveragePersonality;

    public Region_Data(int index)
    {
        RegionSceneIndex = index;
    }
}
