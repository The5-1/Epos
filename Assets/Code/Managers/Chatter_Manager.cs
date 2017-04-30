using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chatter_Manager  {

    //not really sure if this even needs delegates... looks more like a static helper class
    //I still dont understand delegates...


    public delegate string ChatterDelegate(Actor_Data actor);
    ChatterDelegate _chatterDelegate;

    public ChatterDelegate getChatter(Actor_Data actor)
    {
        _chatterDelegate = null;

        //TODO: check the actors personality
        //add some chatter functions to the delegate and return them

        _chatterDelegate += chatter_GlobalPolitics;
        _chatterDelegate += chatter_Family;

        return _chatterDelegate;
    }
    //the player does something like player.chatter(actor){ChatterDelegate my = getChatter(actor); string chatter = ChatterDelegate(actor);}
    //this really looks more like a global helper function:  player.chatter(actor){Chatter_Manager.getChatterString();}


    //call these with delegates?
    //which one is added to the delegate depends on Actor personality
    public string chatter_Family(Actor_Data actor)
    {
        return "My <Husband> is <John>";
    }

    public string chatter_Job(Actor_Data actor)
    {
        return "I am a <Blacksmith> in <Oldtown>";
    }

    public string chatter_Relationship(Actor_Data actor)
    {
        return "Do you know <Buddy>? He is a <Farmer> in <Newtown>.";
    }

    public string chatter_LocalPolitics(Actor_Data actor)
    {
        return "The region <Western Forest> suffers <Famine>.";
    }

    public string chatter_GlobalPolitics(Actor_Data actor)
    {
        return "The <Kingdom of Elves> is in war with <Jan >.";
    }

    public string chatter_Destination(Actor_Data actor)
    {
        return "I come from <Oldtown> and travel to <Newtown>.";
    }

}
