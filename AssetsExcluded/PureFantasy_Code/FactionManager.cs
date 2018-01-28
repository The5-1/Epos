using UnityEngine;
using System.Collections.Generic;




//family = own family members + controllable
//controllable = units you can assume controll over
//ally = offer special services
//neutral = non hostile
//enemy = attack
//hated = searches the world for enemy

public enum Affinity { max = 100, family = 95, controllable = 90, idol = 75, ally = 50, friendly = 25, neutral = 0, grudge = -25, enemy = -50, hated = -90, min=-100 };
public enum FactionType {PlayerFamily, Guild, Town, Kingdom, Religion}

/*
public class RelationshipGenericAtoB<T>
{
    public Affinity _AffinityOfAtoB;
    public T _EntryA;
    public T _EtnryB;

    public void CreateRelationship(T EntryA, T EntryB, Affinity affinityOfAtoB)
    {
        _EntryA = EntryA;
        _EntryA = EntryB;
        _AffinityOfAtoB = affinityOfAtoB;
    }
}
*/

[System.Serializable]
public class ActorRelationshipAtoB
{
    public string name;
    public Affinity _AffinityOfAtoB;
    public Actor _ActorA;
    public Actor _ActorB;

    public ActorRelationshipAtoB(Actor actorA, Actor actorB, Affinity affinityOfAtoB)
    {
        _ActorA = actorA;
        _ActorB = actorB;
        _AffinityOfAtoB = affinityOfAtoB;
    }

    public void UpdateName()
    {
        name = _ActorA._ActorData._Name + "_" + _ActorA._ActorData._Surname + "_to_" + _ActorB._ActorData._Name + "_" + _ActorB._ActorData._Surname;
    }
}

[System.Serializable]
public class RaceRelationshipAtoB
{
    public string name;
    public Affinity _AffinityOfAtoB;
    public Race _RaceA;
    public Race _RaceB;

    public RaceRelationshipAtoB(Race raceA, Race raceB, Affinity affinityOfAtoB)
    {
        _RaceA = raceA;
        _RaceB = raceB;
        _AffinityOfAtoB = affinityOfAtoB;
    }

    public void UpdateName()
    {
        name = _RaceA._name + "_to_" + _RaceB._name;
    }
}

[System.Serializable]
public class FactionRelationshipAtoB
{
    public string name;
    public Affinity _AffinityOfAtoB;
    public Faction _FactionA;
    public Faction _FactionB;

    public FactionRelationshipAtoB(Faction factionA, Faction factionB, Affinity affinityOfAtoB)
    {
        _FactionA = factionA;
        _FactionB = factionB;
        _AffinityOfAtoB = affinityOfAtoB;
    }

    public void UpdateName()
    {
        name = _FactionA._Name + "_to_" + _FactionB._Name;
    }
}


//Singleton
//dontDestroyOnLoad
public class FactionManager : MonoBehaviour {

    //defined in editor
    public List<Faction> _FactionsList;
    public List<ActorRelationshipAtoB> _ActorAtoBRelationshipsList;
    public List<RaceRelationshipAtoB> _RaceAtoBRelationshipsList;
    public List<FactionRelationshipAtoB> _FactionAtoBRelationshipsList;

    void Awake()
    {
        UpdateFactionsList();
    }

    void Start()
    {
        UpdateActorRelationshipsNames();
        UpdateRaceRelationshipsNames();
        UpdateFactionRelationshipsNames();
    }

    void FixedUpdate()
    {

    }


    void UpdateFactionsList()
    {
        GetComponents<Faction>(_FactionsList);
    }

    void UpdateActorRelationshipsNames()
    {
        for (int i = 0; i < _ActorAtoBRelationshipsList.Count; i++)
        {
            _ActorAtoBRelationshipsList[i].UpdateName();
        }
    }

    void UpdateRaceRelationshipsNames()
    {
        for (int i = 0; i < _RaceAtoBRelationshipsList.Count; i++)
        {
            _RaceAtoBRelationshipsList[i].UpdateName();
        }
    }

    void UpdateFactionRelationshipsNames()
    {
        for (int i = 0; i < _FactionAtoBRelationshipsList.Count; i++)
        {
            _FactionAtoBRelationshipsList[i].UpdateName();
        }
    }


    //should actually not be needed // Constructor does not work for Monobehaviours
    public Faction AddFactionAndReturnIt(string factionName, FactionType factionType, Affinity defaultAffinityInsideFaction, Affinity defaultAffinityToOtherFactions, Actor firstMemeber)
    {
        Faction newFaction = this.gameObject.AddComponent<Faction>();
        newFaction.InitFaction(factionName, factionType, defaultAffinityInsideFaction, defaultAffinityToOtherFactions, firstMemeber);

        UpdateFactionsList();
        return newFaction;
    }

    private int SearcActorRelationship(Actor actorA, Actor actorB)
    {
        for (int i = 0; i < _ActorAtoBRelationshipsList.Count; i++)
        {
            if(_ActorAtoBRelationshipsList[i]._ActorA == actorA && _ActorAtoBRelationshipsList[i]._ActorB == actorB)
            {
                return i;
            }
        }
        return -1;
    }
    public Affinity GetActorRelationshipAtoB(Actor actorA, Actor actorB)
    {
        return _ActorAtoBRelationshipsList[SearcActorRelationship(actorA, actorB)]._AffinityOfAtoB;
    }
    public void SetActorRelationshipAtoB(Actor actorA, Actor actorB, Affinity affinity)
    {
        int i = SearcActorRelationship(actorA, actorB);
        if (i == -1)
        {
            ActorRelationshipAtoB relationship = new ActorRelationshipAtoB(actorA, actorB, affinity);
        }
        else
        {
            _ActorAtoBRelationshipsList[i]._AffinityOfAtoB = affinity;
        }
    }


    private int SearchRaceRelationship(Race RaceA, Race RaceB)
    {
        for (int i = 0; i < _RaceAtoBRelationshipsList.Count; i++)
        {
            if (_RaceAtoBRelationshipsList[i]._RaceA == RaceA && _RaceAtoBRelationshipsList[i]._RaceB == RaceB)
            {
                return i;
            }
        }
        return -1;
    }
    public Affinity GetRaceRelationshipAtoB(Race RaceA, Race RaceB)
    {
        return _RaceAtoBRelationshipsList[SearchRaceRelationship(RaceA, RaceB)]._AffinityOfAtoB;
    }
    public void SetRaceRelationshipAtoB(Race RaceA, Race RaceB, Affinity affinity)
    {
        int i = SearchRaceRelationship(RaceA, RaceB);
        if (i == -1)
        {
            RaceRelationshipAtoB relationship = new RaceRelationshipAtoB(RaceA, RaceB, affinity);
        }
        else
        {
            _RaceAtoBRelationshipsList[i]._AffinityOfAtoB = affinity;
        }
    }


    private int SearchFactionRelationship(Faction FactionA, Faction FactionB)
    {
        for (int i = 0; i < _FactionAtoBRelationshipsList.Count; i++)
        {
            if (_FactionAtoBRelationshipsList[i]._FactionA == FactionA && _FactionAtoBRelationshipsList[i]._FactionB == FactionB)
            {
                return i;
            }
        }
        return -1;
    }
    public Affinity GetFactionRelationshipAtoB(Faction FactionA, Faction FactionB)
    {
        return _FactionAtoBRelationshipsList[SearchFactionRelationship(FactionA, FactionB)]._AffinityOfAtoB;
    }
    public void SetFactionRelationshipAtoB(Faction FactionA, Faction FactionB, Affinity affinity)
    {
        int i = SearchFactionRelationship(FactionA, FactionB);
        if (i == -1)
        {
            FactionRelationshipAtoB relationship = new FactionRelationshipAtoB(FactionA, FactionB, affinity);
        }
        else
        {
            _FactionAtoBRelationshipsList[i]._AffinityOfAtoB = affinity;
        }
    }

    /*
    private int SearchThisRelationshipWithFaction(Faction faction)
    {
        for (int i = 0; i < _Relationships.Count; i++)
        {
            if (_Relationships[i]._Faction == faction)
            {
                return i;
            }

        }
        return -1;
    }
    private FactionAtoBrelationshipAtoB AddNewFactionRelationship(Faction faction, Affinity affinity)
    {
        FactionAtoBrelationshipAtoB newRelationship = new FactionAtoBrelationshipAtoB(faction, affinity);
        _Relationships.Add(newRelationship);
        return newRelationship;
    }

    //Methids for this factions Relationships
    public Affinity GetThisRelationshipTo(Faction faction)
    {
        int i = SearchThisFactionAffinityListForFaction(faction);
        if (i == -1)
        { return _defaultAffinityToOtherFactions; }
        else
        { return _Relationships[i]._AffinityToFaction; }
    }
    public void SetThisRelationshipTo(Faction faction, Affinity affinity)
    {
        int i = SearchRelationshipWithFaction(faction);
        if (i == -1) //Relationship not existing yet, add a new
        { AddNewFactionRelationship(faction, affinity); }
        else //Relationship exists, override Affinity
        { _Relationships[i]._AffinityToFaction = affinity; }
    }

    */

}
