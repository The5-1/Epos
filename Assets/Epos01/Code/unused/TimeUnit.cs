#if false

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Keep it simple for now: Seasons will be handled by global buffs (winter in summer), Weekday and Month are the same
[System.Serializable]
public class GameTime_v2
{
    private static GameTime TimeData;

    public byte Seconds {   get { return Seconds; }   private set { Seconds = value; } }
    public byte Minutes {   get { return Minutes; }   private set { Minutes = value; } }
    public byte Hours   {   get { return Hours; }     private set { Hours = value; } }
    public ushort Days  {   get { return Days; }      private set { Days = value; } }
    public ulong Years  {   get { return Years; }     private set { Years = value; } }

    public GameTime_v2(byte seconds = 0, byte minutes = 0, byte hours = 0, ushort days = 0,  ulong years = 0)
    {
        TimeData = Time_Simulation.TimeData;
    }

    public void makeValidTime()
    {

    }

    public void ClampTime()
    {
        Seconds = MathHelper.clamp<byte>(Seconds, 0, TimeData.secondsClamp);
        Minutes = MathHelper.clamp<byte>(Minutes, 0, TimeData.minutesClamp);
        Hours = MathHelper.clamp<byte>(Hours, 0, TimeData.hoursClamp);
        Days = MathHelper.clamp<ushort>(Days, 0, TimeData.daysClamp);
        Years = Years;
    }


    public void increment(GameTime_v2 time)
    {

    }

    public static GameTime_v2 operator +(GameTime_v2 A, GameTime_v2 B)
    {
        ushort secs = (ushort)(A.Seconds + B.Seconds);
        ushort mins = (ushort)(A.Minutes + B.Minutes);
        ushort hours = (ushort)(A.Hours + B.Hours);
        uint days = (uint)(A.Days + B.Days);
        ulong years = (ulong)(A.Years + B.Years);

        return new GameTime_v2();
    }

}

[System.Serializable]
public struct TimeDate
{
    public byte second;
    public byte minute;
    public byte hour;
    public byte day;
    public byte week;
    public byte season;
    public ulong year;
}

[System.Serializable]
public class Time_Span
{
    public ulong seconds = 0;
    public ulong minutes = 0;
    public ulong hours = 0;
    public ulong days = 0;
    public ulong weeks = 0;
    public ulong seasons = 0;
    public ulong years = 0;

    public readonly ulong SpanInSeconds;

    public Time_Span(ulong seconds = 0, ulong minutes = 0, ulong hours = 0, ulong days = 0, ulong weeks = 0, ulong seasons = 0, ulong years = 0)
    {

    }

    public Time_Span(TimeDate dateToSeconds)
    {

    }

    public static long DateToSeconds(TimeDate dateToSpan)
    {

        return 0;
    }

}


#endif