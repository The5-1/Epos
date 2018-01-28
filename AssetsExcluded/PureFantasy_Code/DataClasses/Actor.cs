using UnityEngine;
using System.Collections;


//possibly inherit from interactible-->destructible

//not ABSTRACT so it can be used as a container for stats?


public enum ActorStatsEnum_main { HP,MP, EP};

public enum ActorStatsEnum_combat { Attackspeed, Castspeed, Movespeed};

public class ActorStat_main
{
    public ActorStatsEnum_main stattype;
    public float multiplier; //a multiplier to be used for percentual effects
    public int addend; //a adder to be used for flat effects
    public int max; //the modified maximum hp (main health bar)
    public int currentmax; //the currently possible hp after applying debuffs etc. (active health bar portion)
    public int current; //the remaining health value
    public int racebase; //the racial base value

    public ActorStat_main(ActorStatsEnum_main type, int basevalue)
    {
        stattype = type;
        racebase = basevalue;
        current = racebase;
        currentmax = racebase;
        max = racebase;
        addend = 0;
        multiplier = 1.0f;
    }

}


//why not ScriptableObject
//why [System.Serializable]
[System.Serializable]
public class ActorData{

    //=====================
    // Related Game Objects
    //=====================
    //public GameObject _actorGameObject; // the GameObject this DataClass is bound to
    //public string _actorGameObjectName; // the name of this gameObject the actor is assigned to
    //Focused Target


    //===========
    // Location
    //===========
    public int _region; //the region ID the actor is currently in
    //where exactly the actor is depends on the job and daily scedule that is evaluated when the player loads an area






    //===========
    // Disablers
    //===========
    public bool _mayMove = true; //disallow movement, for snares
    public bool _mayRotate = true; //disallow rotation
    public bool _mayJump = true;
    public bool _mayAttack = true; //disallow fighting --> add attackspeed
    public bool _mayCast = true; //disallow spell casting --> add castspeed
    public bool _mayAct = true;  //disallow any actions

    //Controllers
    



    //=====================================
    // active Effects / Triggers / Passives
    //=====================================
    // StatModifer{addValue, multValue, recalculateCurrent} --> inherit for HP/MP/EP
    // --> increases the actors mutliplier and addend and recalculates the current/currentmax if ture.
    // --> on remove it simply decreases the actors mutliplier and addend
    // --> i need to check the current values when adding or removing anyways if they become greater than max or cause the actor to die
    // Buff{Icon, Description, List<StatModifers>, expiration time, stacksize, maxStacks}
    // --> buffs either stack or get refreshed, maxStacks 0 = no stacking
    // --> if i want a buff that refreshes the duration witch each stack then i need to search the list if one already exists and set that ones expiration date to the new one
    // --> expiration time 0 = no expiration
    // Effects -> temporary Buffs: this list gets checked for expirationTimes
    // Trigger{BuffToApply}
    // --> each actor stat has a setter, when set it calls a event/delegate
    // --> check C# events again
    // Passives{


    // I want to be able to define a like this: Buff(string attributeToModify, multiplyer, startTime, duration).
    //1.) how can I access a Actor.value without having to have to store all 60 attributes in each of my buffs (most will be 0 since a buff only affects 1);
    //2.) How would I keep the Actors values updated according to the buffs, go trough the entire list every FixedUpdate()?
    //3.) How can i make a buff remove itself from the list? (That might work with events, maybe?)
    //4.) Later I want smarter buffs that can listen to some condition that sets their _isActive true/false.


    //ToDo:
    // Effects = List<Buff>
    // When a new entry is added or one is removed, the values 
    // When the actor gets Start/Awake the list gets checked and values updated
    // 
    // List<Triggers>
    // triggers got target values they observe
    // when the target value gets changed the trigger checks if it does something.
    // It then adds or removes a modifierand and possibly itself


    //List Inventory
    //Itemslot
    //Itemslot

    //ToDo:
    //public List<NPCaffinityContainer> <--- container class for that
    //public List<TownAffinityContainer> <--- container class for that
    //public List<RaceAffinityContainer> <--- container class for that

    //=====
    //Names
    //=====
    public string _Name;
    public string _Surname;
    public string _Nickname;
    public string _Title;

    //====
    //Race
    //====
    public Race _Race;

    //===========
    // Heathbars
    //===========
    // RaceValue = firm, only set by the race once
    // Multiplier = 1.0f initially, e.g. 50% more life: multiplier+=0.5f;
    // Addend = 0 initially, e.g. +10 life = addend+=10;
    // Max = Race*Multiplier+Addend;
    // Currentmax = fixed number, Buff can have "bool recalculateCurrent"
    //--------
    // any of these currentmax sinking to 0 can be lethal(?)
    //-----------

    //Health
    // Pool the main protion of damage is taken on
    // crippling damage here typically affects torso and arms
    //private int _HPRace; //the unmodified maximum health possible
    public float _HPmultiplier; //a multiplier to be used for percentual effects
    public int _HPaddend; //a adder to be used for flat effects
    private int _HPmax; //the modified maximum hp (main health bar)
    public int _HPcurrentmax; //the currently possible hp after applying debuffs etc. (active health bar portion)
    public int _HP; //the remaining health value

    //Mana
    // Pool for casting or skills
    // crippling damage here affects Brain and Organs (toxic potions)
    //private int _MPRace;
    public float _MPmultiplier;
    public int _MPaddend;
    private int _MPmax;
    public int _MPcurrentmax;
    public int _MP;

    //stamina
    // Pool for movement or skills
    // crippling damage here affects Legs and lungs
    //private int _EPRace;
    public float _EPmultiplier;
    public int _EPaddend;
    private int _EPmax;
    public int _EPcurrentmax;
    public int _EP;

    //=============
    //action speeds
    //=============
    // no currentmax because you can not actively take normal damage, only crippling
    // e.g. if your _Movespeed gets reduced by something that is already crippling
    // EDIT: make timestop slow all down individually, else i would have to check all movement code for actionspeed too

    //attackspeed
    //private int _AttackSpeedRace;
    public float _AttackspeedMultiplier;
    public int _AttackspeedAddend;
    private int _AttackspeedMax;
    public int _Attackspeed;

    //castspeed
    //private int _CastSpeedRace;
    public float _CastspeedMultiplier;
    public int _CastspeedAddend;
    private int _CastspeedMax;
    public int _Castspeed;

    //movespeed
    //private int _MovespeedRace;
    public float _MovespeedMultiplier;
    public int _MovespeedAddend;
    private int _MovespeedMax;
    public int _Movespeed;

    public float _WalkSpeedMult;
    public float _SprintSpeedMult;
    public float _SneakSpeedMult;

    public float _ground_SpeedMult; //the speed on ground
    public float _air_SpeedMult; //the speed while jumping 0.5f
    public float _water_SpeedMult; //the speed in water 0.5f
    public float _climb_SpeedMult; //the multiplier for climb speed 0.5f

    //Jumping/Flying
    public int _JumpPower; //the force of a jump
    public int _NumJumps; //doublejump //flapping
    public int _MultijumpPower; // the power of all jumps after the first
    public float _MultijumpFalloff; // multiplier for falloff after all jumps past the first
    public float _GlideFalloff; //the multiplier for falloff of glide lift
    public float _GravityMultiplier; //the multiplier of falling gravity
    public float _FallDamageThresholdMultiplier; //the multiplier of falling gravity


    //view Distance
    //public float _viewDistanceRace;
    public float _viewDistance;
    //public float _viewDistanceDarkRace;
    public float _viewDistanceDark;

    //===========
    // Bodyparts
    //===========
    // each healthbar affects some bodyparts
    //public int _bodyBrainHPrace;
    public int _bodyBrainHP; //MP, psycic damage
    //public int _bodyTorsoHPrace;
    public int _bodyTorsoHP; //HP, stability of the body agains physics infuence (e.g. knockback, camera Shaking) modify relative mass
    //public int _bodyLungsHPrace;
    public int _bodyLungsHP; //EP, how fast you lose EP when running, diving
    //public int _bodyOrgansHPrace;
    public int _bodyOrgansHP; //MP, how effective potions are (e.g. gets worse after poision)
    //public int _bodyArmRHPrace;
    public int _bodyArmRHP; //power of the right arm e.g for attacking
    //public int _bodyArmLHPrace;
    public int _bodyArmLHP; //power of the right arm e.g for blocking
    //public int _bodyLegsHPrace;
    public int _bodyLegsHP; //legs for movement, jumping

    //========
    //  Stats
    //========
    // no stats planed, gear always equipable, rougelike
    // HP MP EP, speeds are directly modified by passives
    // skilling stats is BORING, learn new SKILLS instead!!!



    public ActorData()
    {
        if(_Race == null)
        {
            _Race = new Race();
        }
        SetDefaultStatsFromRace();
    }

    public void SetDefaultStatsFromRace()
    {

    }

    public void RecalculateStats()
    {

    }

    public void setRace(Race race)
    {
        _Race = race;
    }

    /// <summary>
    /// Copies all the fields of this ActorData over to a target ActorData
    /// </summary>
    /// <param name="target">the target actor data to copy this values to</param>
    public void CopyStatsTo(ActorData target)
    {

    }


}

public class Actor : MonoBehaviour
{
    //============================================================
    // persistent Data, initialize in code
    //------------------------------------------------------------
    public ActorData _ActorData;

    protected virtual void initActorData()
    {
        if(_ActorData == null)
        {
            _ActorData = new ActorData();

        }
        //_ActorData._actorGameObject = this.gameObject;
        //_ActorData._actorGameObjectName = _ActorData._Name;
    }
    //============================================================


    //============================================================
    // Controllers, assign in Inspector
    //------------------------------------------------------------
    public MovementController _MovementController;
    //------------------------------------------------------------
    public CameraController _CameraController; //NPCs have camereas for now, so I can toggle to them
    public Camera _camera;
    //============================================================

    //============================================================
    //Movement
    //------------------------------------------------------------
    // MovementController handles setting these states
    //------------------------------------------------------------
    //============================================================

    //============================================================
    // Model
    //------------------------------------------------------------
    // should not be handled by actor or set in Inspector
    //------------------------------------------------------------  
    public Transform _Bone_Head;
    public Transform _Bone_Root;
    //============================================================

    //============================================================
    // Focus
    //------------------------------------------------------------
    // weapon not drawn + no focus = instant body turns into walking direction
    // weapon not drawn + focus = head looks at
    // weapon drawn + no focus = free aiming with head and body
    // weapon drawn + focus = body aligns to target
    public bool _weaponDrawn = false;
    public GameObject _focusedObject = null; 
    //============================================================

    void Awake()
    {
        initActorData();
    }

}


/*
// use ObjectCloner Script here
//did not work
public void CloneActor(Actor clone)
{
    ObjectCloner.Clone<Actor>(clone);
}
*/

