using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//how long a NPC needs to reside in a region to pass it along
public enum InformationPriority {secret, rumor, normal, breaking, global};


public class Information_Package {

    //this is some kind of event that got a target audience and spreads fom NPC tp NPC like a virus
    public uint _lifetime; //the information is only relevant for a set time, NPC deat for example max 100 years, King death more

}


//TODO: Possibly it is sufficient to do this per region
//e.g. Event happens in region, all region knows
//		NPC starts traveling and grabs the highest priority info of the region (or every info his "informed" skill allows him to gather)
//		when NPC arrives in other region and stays, the info is passed to that region
//		e.g. secret needs 5 years to pass, breaking just needs arrival

public class Actor_Informations
{
    //this is plugged into an actor
    //contains everything that actor knows about

    public List<Information_Package> _informations;

}


//TODO: Possibly only actors that are flagged as "traveling" carry information
public class InformationCourriers
{
    List<Actor_Data> _travelingActors;
}
