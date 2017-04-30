#if false

[ ] Breeding: check candidates (not n² ?), sort by compability, perform breed


[ ] first think of how to handle Simulation LOD and see if directly catching events is a good idea.
[ ] -- letting the simulation hanlde the LOD is probably better.
[ ] -- e.g. simulator loops over neighbor regions, then further away, and yet further away

[ ] -- store regions in a way that is sorted by distance to player


[ ] Breeding/Repopulation
[ ] check secondToDate and make DateToSecond 
[ ] -- date does not start 1600 but has +5 days more?

[X] Get rid of negative simulation time...


Keep the time in seconds, that will save us a ton of non-stop conversion everywhere
--> TimeData provides a ToSeconds(time) method for easy conversion!!!

MOST RECENT (+ done) (- todo)
---------------
+ specified the borders between Simulator, Manager and Data
- simulator iterates over All Actors, but then only calls Manager methods
- still unclear: any iteration over the array should be done by the Manager
- but the manager does not need that e.g. for combat
+ actors age by second
- but the time tick events are pointless
- pass the time span with the time event
- let the time manager handle the whole active region / inactive region and fast forward logic


NEXT
----
+ repopulation
+ player changing region (actual level loading
    --> create new, empty level in code
+ actors changing regions
+ kingdoms and regions with population count
    --> count new values on time event in the region and kingdom managers





GLOBAL PROBLEMS
---------------
[ ] Think about what exactly is simulated per day, (week?), season, year
[ ] Any time handling should not be substracting a ulong but rather some time calss
[ ] replace any time data with a proper time class that does conversion

[ ] Loading Prefabs and storing them once to instantiate them anytime, HOW? Or just ignore prefabs?
[ ] When actor data changes it should fire a event to everything that cares for it! Event System rather than permanent listening in Update()
[ ] See if it can handle more than 65k actors in the background

[ ] GENERALLY: When the sim becomes to slow, move stuff to regions and make it affect all actors inside rather than check for each actor.


[X] Actor dummy mesh with color
[ ] Actor Name Floater
[ ] Actor Breeding

[ ] RandomNameGenerator load from XML or CSV
[ ]-- one CSV per race
[ ]-- mix completed names: e.g. firstname orc, lastname elve



Family has House/Job
-> Band has Region
-> Kingdom works like regions, NPCs have a kingdom

Random roaming of non-family characters, target region selected by:
[ ] - gender diversity
[ ] - region state and actor class (war attracks fighters/bandits)




/*******************************
Prefabs
-------
- Spawning prefabs is faster than building them in code!!!
- you CAN save the reference to the prefab somehow...

Only one Game Object
--------------------
- is OK, procedural games always start with only one Game object

Global/Static
-------------
- is OK, it does not harm performance or anything

MonoBehaviour
-------------
- only one where you need update()
- the rest can get it via events


Timer
- class with seconds, days etc
- fire event in setter or in updateDate()
- who cares for seconds can subscribe to it


Model: Data
Controller: big calculations
View: User Input

EVENT SYSTEM
------------
Unity has builtin Events/Actions!!
[10:31 PM] jab: public UnityEvent myTestEvent;
[10:35 PM] jab: assume you had a method
[10:36 PM] jab: private void Speak() {Debug.Log("Hello");}
[10:36 PM] jab: GetComponent<MyComponentWithAnEvent>().myTestEvent.AddListener(Speak);
[10:36 PM] jab: then when you do
[10:36 PM] jab: myTestEvent.Invoke()
[10:36 PM] jab: every methd you did 'AddListener' with, will be called

https://forum.unity3d.com/threads/static-class-or-singleton.106694/#post-1362346
For instance, rather than having a "Pause" button that calls a function on a "GameManager" singleton, I have a "Pause" button that fires an event "PauseRequested". 
Everything that needs to somehow respond to the game being paused is subscribed to that event and therefore has an appropriate method called.
[9:36 PM] IsaacPaul: @The5_1 
     That post basically describes the observer pattern
     I use something similar with my networking
     _networkConnection.RegisterListenerWeak<MsgBattleList>(ReceivedBattleList);
[10:37 PM] IsaacPaul: if you're curious about implementation. I'm using 'Actions' for my networking:
     https://hastebin.com/isolibasal.cs
     You can look down at everything past line 88 and see the implementation. Though.. I wish I had a less complex example.. lol
[10:45 PM] IsaacPaul: here's a quick example of a memory leak
    public class Controller : MonoBehaviour {
        public Button MyButton;
        public void Start() {
            MyButton.onClick.AddListener(ButtonPressed); //MemoryLeak
        }

        public void ButtonPressed() {  }
    }


What is a callback?


Coroutines
----------
https://www.youtube.com/watch?v=ciDD6Wl-Evk
- yield
- IEnumerator






Relationships Hirarchy Tree
===========================
Saving what object contains which other ones in each object is not needed
Save it in a hirarchial tree!
The top most checks its children
--> might require unique IDs

Relationships:
search for Network datastructures

Informations:
spread like a virus, stick to actors forever, decay after a time




SIMULATOR CONCEPTS
==================
Dwarf fortress has stuff like information traveling between NPCs
https://forum.unity3d.com/threads/data-driven-story-and-npc-behavior.365836/
http://www.pixelcrushers.com/dialogue_system/manual/html/
http://www.pixelcrushers.com/love-hate/
https://www.reddit.com/r/IAmA/comments/1avszc/im_tarn_adams_of_bay_12_games_cocreator_of_dwarf/
http://bay12games.com/dwarves/features.html
https://www.reddit.com/r/IAmA/comments/1avszc/im_tarn_adams_of_bay_12_games_cocreator_of_dwarf/c91d9rp/
https://www.raywenderlich.com/4946/introduction-to-a-pathfinding
https://www.reddit.com/r/IAmA/comments/1avszc/im_tarn_adams_of_bay_12_games_cocreator_of_dwarf/c919fo8/
Top_to_bottom:
- region has houses
- for empty houses check all actors

Traits:
- every NPC has 6 traits
- every NPC categorizes them as positive or negative (how?)
- a child gets 4 from their parents in the same category and 2 at random in random categories
- e.g. trait "different race" or specialited ones like "dwarf race"
-> in positive = weights other races over own
-> in negative = weights own over other
-> trait not on this npc = weights other race with 0
- this trait can have a weight itself
--> "different race 5"
- if both parents inherit the same trait it gets summed up and 2 random ones are added

Trait Database!!!
- each trait has a list of other traits and scores for them
[worker]:(lazy = -1, strong = +4,...)
[necro]:(sick = +1, rich = +1, ...)
[sick]: (helpfull = +5, good = +5,...)
Problems e.g. 
- [likes other races]:(check other race) needs to do calculations and not just check some value
- [likes dwarfes]:(check other race)
PROBLEM:
- A) if it is simple weights like that, we just save each weight alone and do calculations in the simulations, so the traits change
- B) if we want more "alive" behaviour traits should have scripts attached ---> HOW???





Information Traveling:
- NPC has died --> reach any NPCs that care --> requires something like a "who knows who network"




********************************/

#endif