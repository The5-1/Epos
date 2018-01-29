using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//https://unity3d.com/de/learn/tutorials/topics/scripting/events-creating-simple-messaging-system
//http://answers.unity3d.com/questions/1120488/how-to-apply-loosely-coupled-design.html
//http://wiki.unity3d.com/index.php/Advanced_CSharp_Messenger
//http://m16h.com/lightweight-messaging-unity3d-tinymessenger/
//https://forum.unity3d.com/threads/using-events-within-interfaces-to-achieve-loose-coupling.252584/#post-1668906


//MEDIATOR Pattern: (loose coupling)
//The Mediator pattern: Define an object that encapsulates how a set of objects interact. 
//Mediator promotes loose coupling by keeping objects from referring to each other explicitly, and it lets you vary their interaction independently. 
//http://stackoverflow.com/questions/9226479/mediator-vs-observer-object-oriented-design-patterns
//instead of every class singing up for other classes events, this global broadcaster signs up to all events
//if it gets notified that some event was fired, it does some checking and fires its events that the other managers singed up for

//OBSERVER Pattern
//The Observer pattern: Defines a one-to-many dependency between objects so that when one object changes state, all its dependents are notified and updated automatically. 
// Observer pattern just means that the thing firing the event does not know of who recieves it... which is just the normal case for events

//PROBLEM: 
//https://forum.unity3d.com/threads/the-observer-design-pattern.290376/#post-1916585
// ---> Unnecessesary Middleman? Why let some global class subscribe to all events and then have the managers all subscribe to this.
// ---> If my Actor_Manager cares for weather it directly subscribes to the Weather manager... makes more sense does it not?
//

public static class Mediator {


    //!!! The e.g. Actor simulation is already the thing that subscribes to events and then calls the Actor_Managers methods
    //not sure if we need a mediator above that, mediators are seemingly specialized for only certain classes too
    //there is no "one does it all" mediator in the real mediator pattern.




    //register evet callbacks
    //--- make sure it is unique?

    //remove event

    //listen to events





    /*********EVENTS***********
     * New actor data is created -> broadcast to actor manager, current region
     * 
     * 
     *************************/
}
