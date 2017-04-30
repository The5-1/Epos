#if false

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimeState {forward, paused, backwards}


/// <summary>
/// This simulates the global time in the gameworld
/// </summary>
public class Time_Simulation : MonoBehaviour {


    #region Fields 
    //regions folding needst to be enabled! http://stackoverflow.com/a/42852157

    public static Time_Simulation singleton;

    [SerializeField]
    //in seconds for fast calcualtions and simple logic, provides helpers for conversion
    public static GameTime TimeData;

    [SerializeField]
    public TimeDate timeDate; //update for display 


    [SerializeField]
    public double timeScale; //TODO: This is only for realtime and needs cap 
    //FIXME: Speeding up time e.g. 60x currently wont cause the SecondTickedEvent. So anything faster than that should require a Background Simulation step

    [SerializeField]
    public TimeState timeState; //NOTE: We are not able to revert time simulation, its local time reversion and NPC scedule checking

    #endregion

    //PROBLEM: Time ticks are not suitable for anything but local behaviour, we need to base it on time passed
    // Think:   Simulating 500 years by doing 500 1-year-ticks might work, but
    //          simulating a 1 year tick makes one year pass within a tick!!!
    //          This is even a problem when considering a 1-day-tick.
    //          That 1-day-tick will only apply whatever it does after 24h, but then suddenly in one instant, which is wrong!
    //          Theoretically multiple days can pass in one town:
    //          --- sending a 1 day simulation tick to all neighbour regions will only update if one complete day passed!
    //          --- going to that region after 23 hours will not change a thing, then staying there for 1h will suddenly update everything!
    //SOLUTION: 
    //          - Ask yourself: What does the year tick do? --> it only does something when we simulate a year passing
    //          - The year tick does not actually simulate year but define what happens the next year? ---> thats not a simulation thats a oracle
    //          - We need some detailed ticks and some Time-Passed simulation
    //          

    //FIXME:  We might only need a time passed event, howmuch time has passed is provided with it

    //Stuff that might be on the right track:
    //  A) take all exits of a region and see the needed travel time. Simulate only ticks larger than that time
    //  ---> e.g. if it is < 1h, simulate minutes
    //  ---> Check all regions and find shortest path
    //  ---> Problem: we dont want far away regions to only update every year :(
    //  B) like above, simulate neighbours in minute / hour detail, but simulate everything else at least per day
    //Stuff that wont work:
    //  - Only simulate area when player loads into it. Remember last time plyer was there and take the difference
    //  ---> Problem: The whole world will never change, only regions the player enters :(


    #region init
    protected void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(singleton);
            init();
        }
        else { Destroy(this); }
    }

    protected void OnDestroy()
    {
        if (singleton == this) { singleton = null; }
    }

    protected void init()
    {
        Debug.Log("Time_Manager.init()");
        TimeData = new GameTime();
        timeDate = new TimeDate();
        setTimeState(TimeState.paused);
        timeScale = 1.0f;
    }

    protected void Start()
    {
        //called only once when enabled initially
        setTimeState(TimeState.forward);
    }


    protected void OnEnable()
    {
        setTimeState(TimeState.forward);
    }

    protected void OnDisable()
    {
        setTimeState(TimeState.paused);
    }
    #endregion



    #region Realtime Simulation

    public void setTimeState(TimeState newstate = TimeState.forward)
    {
        if (timeState != newstate)
        {
            timeState = newstate;

            if (newstate == TimeState.forward)
            {
                StopCoroutine(countSecondsDownCR());
                StartCoroutine(countSecondsUpCR());
                StartCoroutine(updateTime());
            }
            else if (newstate == TimeState.backwards)
            {
                StopCoroutine(countSecondsUpCR());
                StartCoroutine(countSecondsDownCR());
                StartCoroutine(updateTime());
            }
            else if (newstate == TimeState.paused)
            {
                StopCoroutine(countSecondsUpCR()); //what happens if they do not run yet?
                StopCoroutine(countSecondsDownCR());
                StopCoroutine(updateTime());
            }

        }
        else
        {
            Debug.LogWarning("Time state has be set to the same it was before!");
        }
    }

    protected void updateDate()
    {

        //FIXME:    This currently just does not fire events if a number does not change
        //          e.g. when time scale is 60 it does not fire 60 second events but just ignores them
        //          For real time active region we need to speed up coroutines
        //          For simulation of inactive regions we need some timePassed(seconds) thing (where does that delta come from?)
        //          The simulators itself need to handle both

        byte tmp = (byte)(TimeData.Time % TimeData.seconds_per_minute);
        if (timeDate.second != tmp && SecondTickedEvent != null) SecondTickedEvent();
        timeDate.second = tmp;

        tmp = (byte)((TimeData.Time % TimeData.seconds_per_hour - timeDate.second) / TimeData.seconds_per_minute);
        if (timeDate.minute != tmp && MinuteTickedEvent != null) MinuteTickedEvent();
        timeDate.minute = tmp;

        tmp = (byte)((TimeData.Time % TimeData.seconds_per_day - timeDate.minute - timeDate.second)/ TimeData.seconds_per_hour);
        if (timeDate.hour != tmp && HourTickedEvent != null)  HourTickedEvent();
        timeDate.hour = tmp;

        tmp = (byte)((TimeData.Time % TimeData.seconds_per_week - timeDate.minute - timeDate.second - timeDate.hour)/ TimeData.seconds_per_day);
        if (timeDate.day != tmp && DayTickedEvent != null) DayTickedEvent();
        timeDate.day = tmp;

        tmp = (byte)((TimeData.Time % TimeData.seconds_per_season - timeDate.minute - timeDate.second - timeDate.hour - timeDate.day) / TimeData.seconds_per_week);
        if (timeDate.week != tmp && WeekTickedEvent != null) WeekTickedEvent();
        timeDate.week = tmp;
            
        tmp = (byte)((TimeData.Time % TimeData.seconds_per_year - timeDate.minute - timeDate.second - timeDate.hour - timeDate.day - timeDate.week) / TimeData.seconds_per_season);
        if (timeDate.season != tmp && SeasonTickedEvent != null) SeasonTickedEvent();
        timeDate.season = tmp;

        ulong tmp2 = (ulong)(TimeData.Time / TimeData.seconds_per_year);
        if (timeDate.year != tmp2 && YearTickedEvent != null) YearTickedEvent();
        timeDate.year = tmp2;

    }

    #endregion

    #region Background Simulation


    public void PassTime(uint daytimes = 0, uint days = 0, uint seasons = 0, uint years = 0)
    {
        /*
        uint total_daytimes = daytimes + (days + (seasons+ years* TimeData.seasons_per_year) * TimeData.days_per_season) * TimeData.daytimes_per_day;
        uint total_days = daytimes/TimeData.daytimes_per_day + days + (seasons + years * TimeData.seasons_per_year) * TimeData.days_per_season;
        uint total_seasons = (daytimes / TimeData.daytimes_per_day + days) / TimeData.days_per_season + seasons + years *TimeData.seasons_per_year;
        uint total_years = ((daytimes / TimeData.daytimes_per_day + days) / TimeData.days_per_season + seasons) / TimeData.seasons_per_year + years;

        Debug.Log(string.Format("World_Time_Manager.PassTime(): {0} daytimes = {1} days = {2} seasons = {3} years", total_daytimes, total_days, total_seasons, total_years));

        if(total_daytimes >= 1) DaytimesPassedEvent(total_daytimes);
        if(total_days >= 1) DaysPassedEvent(total_days);
        if(total_seasons >= 1) SeasonsPassedEvent(total_seasons);
        if(total_years >= 1) YearsPassedEvent(total_years);
        */
    }

    public void PassTimeSeconds(ulong seconds)
    {

    }


    #endregion




}


#endif