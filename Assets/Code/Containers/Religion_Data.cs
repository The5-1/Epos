using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ReligionRank {leader,high,low}




/// <summary>
/// God-Specific Data
/// </summary>
public class God_Data
{
    Actor_Data actorData;

    GameObject totem;

}


public class Religion_Data
{
    God_Data god;

    List<KeyValuePair<ReligionRank, Actor_Data>> followersWithRank;


}


