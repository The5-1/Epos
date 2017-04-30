﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;


//Make this data driven from files instead
//More scripting = lees repetetive than procedural / data driven
//allow fully arbitraty stats e.g. Lifetime de-regen
public enum Actor_StatsEnum { HP, MP, EP, Attackspeed, Castspeed, Movespeed, HP_regen, MP_regen, EP_regen };
public enum Actor_Gender { male, female, both, solo, none };

public enum Actor_CauseOfDeath { age, environment, violence, execution, assassination } //execution = legal reason

public enum Actor_SpecialAliveState { sick, undead, vampire, noAging } //TODO: those should rather be buffs/traits

public enum Actor_Region_Relationship { home, workplace, roam, attack, defend};


//TODO: Stats
//      if we want more NPCs that can be saved they should not all have the comlex stat, at least not for everything
//      when manipulating a stat we can check its type for complex or simple
//      e.g. the whole addend and multiplicator stuff could just directly calculated into the _max star for irrelevant NPCs
//      NPCs only have 4 traits, 2 positive 2 negative too, they should not be as complex as the player
//      Does a NPC (that does not belong to your family) really need all this buff stuff?
//      Make a simple Stat property

[System.Serializable]
public class ActorValue
{
    //only a current, limit and max value.
    //limit to make permanently affecting a NPC possible

    public Actor_StatsEnum _stattype_original; //the original stat type
    public Actor_StatsEnum _stattype_override; //override the stat to be something else
    public int _max; //the modified maximum hp (main health bar)
    public int _limit; //active possible limit after applying crippling effects that reduce the max (active healthbar)
    public int _current; //active current remaining health value
    public int _racebase; //the racial base value, set on breeding

    public ActorValue(Actor_StatsEnum type, int basevalue)
    {
        _stattype_original = type;
        _stattype_override = _stattype_original;
        _racebase = basevalue;
        _current = _racebase;
        _limit = _racebase;
        _max = _racebase;
    }

    //sets the current type and returns the previous
    //e.g. for something that swaps players HP and MP stats
    public Actor_StatsEnum changeStatType(Actor_StatsEnum newtype)
    {
        _stattype_override = newtype;
        return _stattype_original;
    }

}

[System.Serializable]
public class Actor_Stat_complex : ActorValue
{
    public float _multiplier; //a multiplier to be used for percentual effects
    public int _addend; //the combined addend for all additive and subtractive effects


    public Actor_Stat_complex(Actor_StatsEnum type, int basevalue) : base(type, basevalue)
    {
        _addend = 0;
        _multiplier = 1.0f;
    }
}


//https://en.wikipedia.org/wiki/Personality_psychology
[System.Serializable]
public class Actor_Personality
{
    public sbyte Personality; //TODO: calculate this from other factors or compare individual factors + traints
    public byte PersonalityTollerance; //TODO: calculate this from other factors

    //IDEA: The son of a family of clerics becomes a canibal tyran.
    //This also applies to the player: e.g. influences speed of how fast he learns skills
    //The birth personality sets some random foundation e.g. range(-20,20);
    //Every year a child spends with its family influences the child: The highest stat of each family member is added to the child
    //Impact events during childhood count 10x: Eg family member is killed by someone, young children hate that personality.
    //[Better] only driven by world stats that can be checked by the sim
    // + world event
    // + relation ship to king
    // + personality of village elder affects villagers (warlord) [village elder is the oldest npc till his deat, vilage elder can be killed and replaced by strongest by simulation]

    //When grown up:
    // External influence
    // + attacked by somebody -> "hates" the personality of that other character
    // + quest completed -> "likes" the personality
    // + weather the race does not like
    // + season the race does not like
    // + world events
    // + witnessing interaction between other NPCs
    // Own influence
    // + is hungry, kills rabbit -> change personality


    //Philisophical
    //Most stuff seems to be actually some subcategory of those
    public sbyte _heredity_environment; //genes or external influence //how strong own actions or external actions affect personaliy //    public sbyte _knowledge;
    public sbyte _active_reactive; //public sbyte _humor;//public sbyte _obedience; //follow orders or not // public sbyte _extrovert;//actively does things or rather reacts to other NPCs
    public sbyte _uniqueness_unversality; // public sbyte _likeminded; //pefers people with same traits or different traits
    public sbyte _optimistic_pessimisic; // public sbyte _helpfullness; //
    public sbyte _freedom_determinism; //

    public Actor_Personality()
    {

    }

    public void makeRandomPersonality()
    {
        Personality = (sbyte)Random.Range(-128, 127);
        PersonalityTollerance = (byte)Random.Range(0, 255);
    }

    public static bool checkPersonalityCompability(Actor_Data actorA, Actor_Data actorB)
    {
        byte diff = (byte)Mathf.Abs(actorA.personalityData.Personality - actorB.personalityData.Personality);
        if (diff <= actorA.personalityData.PersonalityTollerance && diff <= actorB.personalityData.Personality) return true;
        else return false;
    }
}


[System.Serializable]
public class Actor_BreedData
{
    //https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life
    //https://bitstorm.org/gameoflife/

    #region Fields
    //public Actor_Data _partnerActorData; //FIXME: This creates a serialisation loop
    public Actor_Gender gender = Actor_Gender.none;

    public bool isPregnant = false;
    public uint currentBreedCooldown = 0; //TODO: Subscribe to time ticker!
    public uint currentBirthCountdown = 0; //TODO: Subscribe to time ticker!
    public byte numOffspring = 0;

    private uint breedCooldownBase = 5; 
    private uint birthCountdownBase = 2;
    private byte numOffspringBase = 1;

    #endregion

    //events handling should be done in the manager class that then applies it to all its childs

    public Actor_BreedData()
    {

    }

    public void loadDataToActiveActor()
    {
        //any data loading work that needs to be done here
    }

    public void storeDataFromActiveActor()
    {
        //e.g. store curent position
    }


    public void makeRandomBreedData()
    {
        breedCooldownBase = 2;
        birthCountdownBase = 5;
        numOffspringBase = 1;

        isPregnant = false;
        currentBirthCountdown = 0;

        gender = (Actor_Gender)Random.Range(0, 5); //integer random numbers are rounded down so it wont ever reach the max value!
        numOffspring = 1;
        currentBreedCooldown = (ushort)(Random.Range(1, 10));// + (18 - _myActorData._age));
    }


    public bool checkCanBreed()
    {
        if (isPregnant || currentBreedCooldown > 0 || gender == Actor_Gender.none) return false;
        else return true;
    }

    private void makeCooldown()
    {
        currentBreedCooldown = breedCooldownBase;
    }

    private void makePregnant()
    {
        isPregnant = true;
        currentBreedCooldown = breedCooldownBase;
        currentBirthCountdown = birthCountdownBase;
        numOffspring = numOffspringBase;

        //TODO: add new unborn character to actor data pool!
        //TODO: add to family system
    }

    private static void makeFamily(Actor_Data actorA, Actor_Data actorB)
    {

    }

    public static bool trySoloBreed(Actor_Data actorA)
    {
        Actor_BreedData A = actorA.breedData;

        if (A.checkCanBreed() && A.gender == Actor_Gender.solo)
        {
            A.makePregnant();
            return true;
        }
        else return false;
    }

    public static bool tryBreed(Actor_Data actorA, Actor_Data actorB)
    {
        Actor_BreedData A = actorA.breedData;
        Actor_BreedData B = actorB.breedData;

        if (!(A.checkCanBreed() && B.checkCanBreed()))
        {
            return false;
        }
        else if ((A.gender == Actor_Gender.female || A.gender == Actor_Gender.both) && B.gender == Actor_Gender.male)
        {
            A.makePregnant();
            B.makeCooldown();
            makeFamily(actorB, actorA);
            return true;
        }
        else if ((B.gender == Actor_Gender.female || B.gender == Actor_Gender.both) && A.gender == Actor_Gender.male)
        {
            B.makePregnant();
            A.makeCooldown();
            makeFamily(actorA, actorB);
            return true;
        }
        else if (B.gender == Actor_Gender.both && A.gender == Actor_Gender.both)
        {
            A.makePregnant();
            B.makePregnant();
            makeFamily(actorA, actorB);
            return true;
        }
        else return false;

    }

    public ushort tryBirth()
    {
        if (isPregnant && currentBirthCountdown <= 0) return numOffspring;
        else return 0;
    }

    public void passTime(ulong time)
    {
        currentBreedCooldown -= (uint)time;
        currentBirthCountdown -= (uint)time;
        tryBirth();
    }
}



/// Region the Actor is currently in. A Actor can move via BackgroundSim. 
/// When entering a region the real XYZ coordinates are generated from the current job scedule.
/// e.g Getting up [6:00-7:00], Harvesting[8:00-12:00]: When player enters at 7:30 the Actor is placed half way between his Bed and the Field, snapped to the next walkway point.


//TODO: Serializable? Inherited? Anything? See: http://answers.unity3d.com/questions/190350/what-is-the-purpose-of-scriptableobject-versus-nor.html


[System.Serializable] //makes it visible in the editor

/// <summary>
/// MODEL: 
/// </summary>
public class Actor_Data
{
    #region Fields

    public string name = "Actor Name"; //have this first so unity uses the name for the list entry in the inspector

    #region Location
    public int currentRegionIndex; //where he currently is
    public Vector3 storedPosition; //relevant e.g. for corpses

    public Dictionary<int, Actor_Region_Relationship> SubscribedRegionsByIndex; //Any regions the actor cares about and wants to get notified for
    #endregion

    #region age

    public ulong ageInSeconds;
    public ulong ageInSecondsMax;
    public ulong birthdayInSeconds;
    public TimeDate age; //DEBUG: calculate that on the fly for the display, do not store it here
    public TimeDate birthday; //DEBUG: calculate that on the fly for the display, do not store it here

    #endregion

    #region death and deletion

    public bool Alive = true;

    public bool essential = false; //for characters that are not supposed to decay //IDEA: essential leaves permanent tombstone for necromancer to revive thousand year old king (Quest to build epic army of legendary soldiers)
    public bool metByPlayer = false; //TODO: replace with some "relevance for player" weight
    public bool playerFamilyMember = false; //TODO: replace with some "relevance for player" weight

    public ulong decayTime = 0; //till the corpse becomes a skeleton or something //items got their own timers
    public ulong decayTimeMax; //DEBUG: rather in some global config
    public bool decayed = false;

    public ulong deleteTime = 0; //TODO: this should be raised for actors the player met and reset when the player interacts with a corpse
    public ulong deleteTimeMax; //DEBUG: rather in some global config

    Actor_CauseOfDeath CauseOfDeath;
    Actor_Data Killer = null;

    #endregion

    #region advanced containers

    public Actor_Personality personalityData;

    public List<ActorValue> actorValues; //TODO: Unique, no duplicates! This could be a dictionary or something? = Elder Scrolls "ActorValue"

    public Actor_BreedData breedData;

    #endregion



    #endregion

    #region init

    public Actor_Data()
    {
        init();
    }

    protected void init()
    {
        Alive = true;
        currentRegionIndex = 0;
        birthday = new TimeDate();
        age = new TimeDate();
        actorValues = new List<ActorValue>();
        personalityData = new Actor_Personality(); //TODO: this can be optional at some point
        breedData = new Actor_BreedData(); //TODO: this can be optional at some point

        //DEBUG
        initToDefault();
        setRandomActor();
    }


    public bool addStat(ActorValue stat)
    {
        foreach (ActorValue check in actorValues)
        {
            //if the base or the base stat is present, do not add it again
            if (check._stattype_original == stat._stattype_original)
            {
                return false;
            }

            //TODO: What to do if one overriden stat is already the same
            //TODO: Also, what to do if 2 stats would be overriden with the same
        }
        //if none was previously found
        actorValues.Add(stat);
        return true;
    }

    public void initToDefault()
    {

        this.addStat(new ActorValue(Actor_StatsEnum.HP, 100));

    }

    public void setRandomActor()
    {
        //DEBUG: hardcoded values everywhere
        name = NameGenerator.RandomName();
        currentRegionIndex = (int)Random.Range(1, 10);
        ageInSeconds = (ulong)Random.Range(0, 100*GameTime.singleton.seconds_per_year);
        age = GameTime.singleton.secondsToDate(ageInSeconds);
        birthdayInSeconds = GameTime.singleton.Time - ageInSeconds;
        birthday = GameTime.singleton.secondsToDate(birthdayInSeconds);
        ageInSecondsMax = (ulong)Random.Range(ageInSeconds + 5 * GameTime.singleton.seconds_per_year , 120 * GameTime.singleton.seconds_per_year);

        decayTimeMax = (ulong)(0.5 * GameTime.singleton.seconds_per_year);
        deleteTimeMax = (ulong)(2 * GameTime.singleton.seconds_per_year);

        storedPosition = new Vector3(Random.Range(-20.0f, 20.0f), Random.Range(-20.0f, 20.0f), Random.Range(-20.0f, 20.0f));

        breedData.makeRandomBreedData();
        personalityData.makeRandomPersonality();
    }

    public void initFromRace(string raceFile)
    {
        //TODO: init stats from base race Data (data driven)
    }

    public void initByBreeding(Actor_Data parentA, Actor_Data parentB)
    {
        //TODO: init stats from two Actors
        //based on some actor breeding stat like:
        //- equal 50/50
        //- genetic dominance weighted
        //- random
        //- gene manipulation
    }

    #endregion

    //ATTENTION: The actor data should not directly subscribe to events!
    //The level of presicion of the simulation is determined by the ActorSimulatio and the ActorManager

    public void tickTime(ulong delta)
    {
        ageActor(delta);

    }

    public void ageActor(ulong delta)
    {
        if (Alive)
        {
            ageInSeconds += delta;
            if (ageInSeconds > ageInSecondsMax)
            {
                ageInSeconds = ageInSecondsMax;
                killActor(Actor_CauseOfDeath.age, null);
                //FIXME: this logging opperation totally kills performance when 500 actors die with every tick!
                //Debug.Log(string.Format("{0} has died in the age of {1}/{2}",actor.Name, GameTime.singleton.secondsToDate(actor.AgeInSeconds).year, GameTime.singleton.secondsToDate(actor.MaxAgeInSeconds).year));
            }
        }
        else if (!Alive)
        {
            if(!decayed)
            {
                decayTime += delta;
                if(decayTime > decayTimeMax)
                {
                    decayTime = decayTimeMax;
                    decayed = true;
                }
            }
            else if (decayed)
            {
                deleteTime += delta;
                if (deleteTime > deleteTimeMax)
                {
                    deleteActor();
                }
            }
        }
    }

    public void killActor(Actor_CauseOfDeath cause, Actor_Data killerActor)
    {
        Alive = false;

        CauseOfDeath = cause;
        if (killerActor != null) Killer = killerActor;

        decayTime = 0;
        deleteTime = 0;
    }


    public void deleteActor()
    {
        actorValues = null;
        personalityData = null;
        breedData = null;

        Actor_Manager.singleton.actorDatas.Remove(this);
    }

}