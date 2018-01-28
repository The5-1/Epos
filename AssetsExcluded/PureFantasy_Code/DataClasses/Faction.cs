using UnityEngine;
using System.Collections.Generic;




//i could make it MonoBehaviour so i can save idividual Objects of it as Prefabs

public class Faction : MonoBehaviour
{

    public  string _Name = "no name given!";
    public FactionType _FactionType = FactionType.Town;
    private Affinity _defaultAffinityInsideFaction = Affinity.ally;
    private Affinity _defaultAffinityToOtherFactions = Affinity.neutral;
    private List<Actor> _FactionMemebers;
 

    public void InitFaction(string factionName, FactionType factionType, Affinity defaultAffinityInsideFaction, Affinity defaultAffinityToOtherFactions, Actor firstMemeber)
    {

        _Name = factionName;
        _FactionType = factionType;
        _defaultAffinityInsideFaction = defaultAffinityInsideFaction;
        _defaultAffinityToOtherFactions = defaultAffinityToOtherFactions;
        _FactionMemebers = new List<Actor>();
        AddMember(firstMemeber);
    }

    private int SearchMember(Actor actor)
    {
        for (int i = 0; i < _FactionMemebers.Count; i++)
        {
            if (actor == _FactionMemebers[i])
            {
                return i;
            }

        }

        return -1;
    }
   
    //Methods for looking up this factions Actors
    public bool GetActorIsMember(Actor actor)
    {
        if (SearchMember(actor) != -1) return true;
        else return false;
    }
    public void AddMember(Actor actor)
    {
        if(SearchMember(actor) == -1)
        {
            _FactionMemebers.Add(actor);
        }
    }
    public void RemoveMember(Actor actor)
    {
        int index = SearchMember(actor);
        if (index != -1)
        {
            _FactionMemebers.RemoveAt(index);
        }
    }

}
