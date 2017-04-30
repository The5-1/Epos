using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TimeUnit { seconds, minutes, hours, days, weeks, seasons, years };
public enum Time_daytime { morning, noon, evening, night};
public enum Time_season { spring, summer, autumn, winter};
public enum Time_zodiac {dragon, cow, tower}; //TODO: make zodiacs

public enum TimeDirection {forwards, paused}; //NO TIME REVERSAL IN SIM! Just substract from gametime to mess with the date!

[System.Serializable]
public struct TimeDate
{
    public byte second;
    public byte minute;
    public byte hour;
    public ushort day;
    public ulong year;

    public byte weekday;
    public byte season;
}



[System.Serializable]
public class TimeCountdown
{
    public ulong Countdown
    {
        get
        {
            return Countdown;
        }
        set
        {
            Countdown = value;
            if (Countdown < 0 && CountdownFinishedEvent != null) CountdownFinishedEvent();
        }
    }

    protected bool ticking;

    public delegate void CountdownFinished();
    public event CountdownFinished CountdownFinishedEvent;

    private void countDown(ulong delta)
    {
        if (!ticking) return;
        else { Countdown -= delta;}
    }

    TimeCountdown(ulong Countdown, bool beginTicking)
    {
        ticking = beginTicking;

        GameTime.TimePassedEvent += countDown;
    }

}

//===================================================================
//>>>>>>>>>>>>>>>>>> NO REVERSE TIME SIMULATION! <<<<<<<<<<<<<<<<<<<<
//===================================================================
// No time reversal directly in the sim! Overcomplicates EVERY SIMULATION! Performance!?
// Our world is supposed to be totally player driven, we cant reverse all player actions.
// How to reverse familys formig? How to reverse kingdoms falling?
// Hand-Craft Time-Reversal effects! Dont let the sim try to do it.
//      e.g. Make everyone younger: --> reduche age, check death type == age, alive = true.
//      e.g. Make people never born: --> check birthday --> delete them, break family, delete children.
//      e.g. rebuilding a kingdom: --> restore buildings, simulate some years of population.
//      e.g. revert global events: --> if their "Birthday" is higher than the date, remove them.
// Global time reversal can "visually" set the worldtime back by simply modifying the GameTime.
// Local time reversal needs caching of actor data anyways.

/// <summary>
/// Stores the current time
/// Stores time intervall definitions
/// Provides helper methods to do conversions
/// </summary>
[System.Serializable]
public class GameTime : MonoBehaviour //needs to be monobehaviour for coroutines
{

    //http://answers.unity3d.com/questions/1241916/tycoon-like-game-time-and-date.html
    //https://msdn.microsoft.com/en-us/library/bk8a3558(v=vs.110).aspx

    #region fields

    public static GameTime singleton;

    public ulong Time;
    public ulong PreviousTime;
    public TimeDate CurrentDate;

    public ulong DEBUGSecondsPerTickOverride = 1;
    //public float TimeScale = 1.0f;
    public float UpdateIntervall = 1.0f;

    private bool IsTicking;


    #endregion

    #region constants

    //http://stackoverflow.com/questions/55984/what-is-the-difference-between-const-and-readonly
    //TODO: Load this from data maybe...

    public byte seconds_per_minute = 60;
    public byte minutes_per_hour = 60;
    public byte hours_per_day = 24;
    public ushort days_per_year = 360;

    public byte days_per_week = 7;
    public byte season_per_year = 4;

    //toSecond conversion values
    public uint seconds_per_hour;
    public uint seconds_per_day;
    public uint seconds_per_week;
    public uint seconds_per_season;
    public uint seconds_per_year;

    //clamp values
    public byte secondsClamp;
    public byte minutesClamp;
    public byte hoursClamp;
    public ushort daysClamp;



    #endregion

    #region Events

    //A.) Main Simulation: Just fire a event that sends the delta since the last event! Every Reciever checks the ammount themselves and decides what to simulate!
    public delegate void TimePassed(ulong timeInSeconds);
    public static event TimePassed TimePassedEvent; //not static since multiple timers could be possible
    
    /*
    //B.) Consant Simulation (high): Fire Second Minute and Hour events for those that care for constant updates! 
    
    public delegate void SecondTicked();
    public static event SecondTicked SecondTickedEvent;

    public delegate void MinuteTicked();
    public static event MinuteTicked MinuteTickedEvent;

    public delegate void HourTicked();
    public static event HourTicked HourTickedEvent;

    //C.) Consant Simulation (low): Hours should be the biggest steps the realtime simulation updates in.
    //e.g. a "day passed" event would only be relevant for somethign that sets something for the following day

    public delegate void DayTicked();
    public static event DayTicked DayTickedEvent;

    public delegate void WeekTicked();
    public static event WeekTicked WeekTickedEvent;

    public delegate void SeasonTicked();
    public static event SeasonTicked SeasonTickedEvent;

    public delegate void yearTicked();
    public static event yearTicked YearTickedEvent;

    public delegate void DaytimesPassed(uint daytimes);
    public static event DaytimesPassed DaytimesPassedEvent;

    public delegate void DaysPassed(uint days);
    public static event DaysPassed DaysPassedEvent;

    public delegate void SeasonsPassed(uint seasons);
    public static event SeasonsPassed SeasonsPassedEvent;

    public delegate void YearsPassed(uint years);
    public static event YearsPassed YearsPassedEvent;
    */

    #endregion

    #region Coroutines
    //coroutines
    // --> start the realtime time when player is in a region
    // --> pause it during loading or menus or something
    // --> reverse it on some time reversal skills

    //INFO on Coroutines
    //https://docs.unity3d.com/Manual/Coroutines.html
    //https://unity3d.com/de/learn/tutorials/topics/scripting/coroutines

    private IEnumerator countSecondsUpCR()
    {
        while (true)
        {
            this.TickSecondUp((ulong)(DEBUGSecondsPerTickOverride*UpdateIntervall));
            yield return new WaitForSeconds(UpdateIntervall); //respects unity internal timescale
        }
    }

    private IEnumerator UpdateTimeCoroutine()
    {
        Debug.Log("FireEventsCR() started");
        while (true)
        {
            UpdateTime();
            yield return new WaitForSeconds(UpdateIntervall);
        }
    }


    public void StartTicking()
    {
        StartCoroutine(countSecondsUpCR());
        StartCoroutine(UpdateTimeCoroutine());
        IsTicking = true;
    }

    public void StopTicking()
    {
        StopCoroutine(countSecondsUpCR());
        StopCoroutine(UpdateTimeCoroutine());
        IsTicking = false;
    }

    #endregion

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


    public void init()
    {

        seconds_per_hour = (uint)(seconds_per_minute * minutes_per_hour);
        seconds_per_day = (uint)(seconds_per_hour * hours_per_day);
        seconds_per_week = (uint)(seconds_per_day * days_per_week);
        seconds_per_year = (uint)(seconds_per_day * days_per_year);
        seconds_per_season = (uint)(seconds_per_year / season_per_year);

        secondsClamp = (byte)(seconds_per_minute-1);
        minutesClamp = (byte)(minutes_per_hour -1);
        hoursClamp = (byte)(hours_per_day -1);
        daysClamp = (ushort)(days_per_year -1);


        StopTicking();
        //TODO: Load from file
        Time = (ulong)TimeToSeconds(years:1600); //DEBUG
        PreviousTime = Time - 1;
    }

    public void Start()
    {
        IsTicking = true; //DEBUG
        checkIsTicking();
    }

    private void checkIsTicking()
    {
        if (IsTicking)
        {
            StartTicking();
        }
        else
        {
            StopTicking();
        }
    }

    #endregion

    #region Methods

    public void TickSecondUp(ulong seconds = 1)
    {
        Time += seconds;
    }

    public void ModTime(int seconds = 0, int minutes = 0, int hours = 0, int days = 0, int seasons = 0, int weeks = 0, int years = 0)
    {
        long sec = TimeToSeconds(seconds, minutes, hours, days, seasons, weeks, years);
        if (sec > 0) Time += (ulong)sec;
        if (sec < 0) Time -= (ulong)(-sec);

    }

    private void UpdateTime()
    {
        UpdatePreviousTime();
        UpdateCurrentDate();
    }

    private void UpdatePreviousTime()
    {

        ulong delta = (ulong)(Time - PreviousTime);

        if (delta >= 1)
        {
            PreviousTime = Time;

            //Debug.Log(string.Format("TimePassed delta = {0}", delta));

            if (TimePassedEvent != null) TimePassedEvent(delta);
        }
    }

    private void UpdateCurrentDate()
    {
        CurrentDate = secondsToDate(Time);
    }

    #endregion

    #region Helper Methods

    public long TimeToSeconds(int seconds = 0, int minutes = 0, int hours = 0, int days = 0, int seasons = 0, int weeks = 0, int years = 0)
    {
        return (long)(seconds
            + seconds_per_minute * minutes
            + seconds_per_hour * hours
            + seconds_per_day * days
            + seconds_per_week * weeks
            + seconds_per_season * seasons
            + seconds_per_year * years
            );
    }

    public TimeDate secondsToDate(ulong inputSeconds)
    {
        TimeDate date = new TimeDate();

        byte tmp = (byte)(inputSeconds % seconds_per_minute);
        date.second = tmp;

        tmp = (byte)((inputSeconds % seconds_per_hour - date.second) / seconds_per_minute);
        date.minute = tmp;

        tmp = (byte)((inputSeconds % seconds_per_day - date.minute - date.second) / seconds_per_hour);
        date.hour = tmp;

        ushort tmp2 = (ushort)((inputSeconds % seconds_per_year - date.minute - date.second - date.hour) / seconds_per_day);
        date.day = tmp2;

        ulong tmp3 = (ulong)(inputSeconds / seconds_per_year);
        date.year = tmp3;


        tmp = (byte)(date.day % days_per_week);
        date.weekday = tmp;

        tmp = (byte)((inputSeconds % seconds_per_year)/seconds_per_season);
        date.season = tmp;

        return date;
    }


    #endregion

    /*
    public void updateSecondsPer()
    {
        seconds_per_hour = seconds_per_minute * minutes_per_hour;
        seconds_per_day = seconds_per_hour * hours_per_day;
        seconds_per_daytime = seconds_per_day / daytimes_per_day;
        seconds_per_season = seconds_per_day * days_per_season;
        seconds_per_year = seconds_per_season * seasons_per_year;
    }
    */

    /*
    public Time_Data(uint hours_per_day = 24, uint daytimes_per_day = 4, uint days_per_season = 7, uint seasons_per_year = 4)
    {
        _hours_per_day = hours_per_day;
        _daytimes_per_day = daytimes_per_day;
        _days_per_season = days_per_season;
        _seasons_per_year = seasons_per_year;
    }
    */
}
