#if false
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// VIEW: This is the interface where the methods provided by the CONTROLLER are triggered
/// do: execute methods of Actor_Manager
/// dont: execute directly on Actor_Data
/// This takes the actorsData from the Actor_Manager and recalculates things everytime time passes
/// - Breeding: Each breed-intervall, if ready to breed, do breed simulation
/// - Regeneration: Each Day/Night regenerate some stats, if stats are at max, restore crippled stats?
/// </summary>
public class Actor_Simulatior : MonoBehaviour {

    #region Fields
    static public Actor_Simulatior singleton;
    #endregion

    #region init
    protected void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(singleton);
            init();
            subscribeToEvents();
        }
        else { Destroy(this); }
    }

    protected void OnDestroy()
    {
        if (singleton == this) { singleton = null; }
    }

    public void init()
    {
        Debug.Log("Actor_Simulator.init()");
    }
    #endregion

    #region Event Subscription

    public void subscribeToEvents()
    {

        #region Subscribe to Time Events

        GameTime.TimePassedEvent += onTimePassed;

        /*
        Time_Simulation.SecondTickedEvent += onSecondTicked;
        Time_Simulation.MinuteTickedEvent += onMinuteTicked;
        Time_Simulation.HourTickedEvent += onHourTicked;
        Time_Simulation.DayTickedEvent += onDayTicked;
        Time_Simulation.YearTickedEvent += onDayTicked;
        */

        #endregion

    }

    #endregion

    //NOTE: Think: "This does all the things that a CombatTrigger would not care for to find in Actor_Manager!"

    #region Realtime Simulation

    protected void onTimePassed(ulong delta)
    {
        simulateAllActors(delta);
    }

    /*
    void onSecondTicked()
    {
        Debug.Log("actor sim second tick");
        SimulateAllActors(1);
    }

    void onMinuteTicked()
    {
        Debug.Log("actor sim minute tick");
    }

    void onHourTicked() //FIXME: actually this should only tick if we do not tick seconds! Same for the rest, if we are at seconds-detail, tick that
    {
        Debug.Log("actor sim hour tick");
    }

    void onDayTicked()
    {
        Debug.Log("actor sim day tick");

    }

    void onYearTicked()
    {
        Debug.Log("actor sim year tick");
    }
    */
    #endregion

    #region Background Simulation

    protected void simulateAllActors(ulong seconds) 
    {
        //TODO: input time passed here

        foreach(Actor_Data actor in Actor_Manager.singleton.Actors)
        {
            Actor_Manager.singleton.AgeActor(actor, seconds);
        }

    }

    void findJobs()
    {
        //calls stuff in actor manager
    }

    void migrate()
    {
        //calls stuff in actor manager
    }

    void breedAllActors()
    {
        foreach (int regionIndex in Region_Manager.singleton.RegionsByID.Keys)
        {
            foreach (Actor_Data actorData in Region_Manager.singleton.RegionsByID[regionIndex].ActorsCurrentlyInThisRegion)
            {

            }
        }

        /*
        //calls stuff in actor manager
        foreach (byte regionID in _parentActorManager._actorsDataPerRegion.Keys)
        {
            //make sure to check each actor only once per pair
            //or check all actors and find the most suitable pair
            foreach (Actor_Data currentActor in _parentActorManager._actorsDataPerRegion[regionID])
            {
                foreach (Actor_Data otherActor in _parentActorManager._actorsDataPerRegion[regionID])
                {
                    currentActor._breedData.tryBreed(otherActor); //TODO: just breeds with the first best atm...
                }
            }
        }
        */
    }


    #endregion


}
#endif