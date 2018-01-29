#if false

Message Bus - delayed processing and priorities
-----------------------------------------------
http://www.gamasutra.com/blogs/MichaelKissner/20151027/257369/Writing_a_Game_Engine_from_Scratch__Part_1_Messaging.php
-- when something happens throw a message on the buss
-- the Message system handles priorities and calls methods when it actually got the time to process the message
===> UnityEvent: works only on MonoBehaviours
https://unity3d.com/de/learn/tutorials/topics/scripting/events-creating-simple-messaging-system
https://docs.unity3d.com/Manual/UnityEvents.html



Simulation LoD - UseCase for different intervall events
-------------------------------------------------------
- player fless a tree
- tree would need to check its decay state every second
---> rather only check every hour/day event
---> if it needs updating, toss a message on the buss (priorities?)
---> the message will be dealt with whenever there is time



NEW: Actor Relationships
------------------------
Positive or Negative Relationship
- instead of checking n² we start with "who does the actor even know?"
- everytime a actor is in the same cell with another they get a relationship
-- knows since seconds (upper cap e.g. 1 month)
-- time increments with simulation
-- signed? positive and negative impacts
-- (special cases would be if the other actor is stealthed or such)
- relationships can be built by scripts
-- has heard of
- global persons of interest per region
-- e.g. king of a country is known in all regions of the kingdom

NEW: Actor Compabitility system
-------------------------------
e.g. beggar cant marry king
- Political + Political Rank (High Royal, Low Royal, Middle, Poor, Shamed)
- Religion + Religion Rank (same religion VS opposing religions)
- Kingdom (kingdoms at war -> civilians dislike eachother)
- D&D Alignment System
- Wealth (possessions)
- Health (condition)
- Job
--> No artificial stats, just use the stats we actually have!

UseCases
--------
The player enters a region or skips time, now the simulation checks when the player last was there or the delta of time passed.
The simulation must be able to check if they breed and if they gave birth.
And it must be able to do so __multiple times__, e.g. when skipping 1000years



============================
=   Simulation Structure   =
============================
Everything has "needs" (singed bars) that are consumed or restocked (supply/demand)

1.) "Weather" sim
-----------------
Each region has singed values (bars) that fill/deplete, upon a threshold they fire an event.
Actively running events can consume or fill other bars.
Affects the condition of regions which in turn affect adjacent civilisation sim
-----------------
use the XYZ position of regions for temperature, pressure, fill
-----------------
Weather/geological:
- temperature: ice-age < blizzard < snow < rain < normal > sunny > drought > sandstorm > firestorm
- windpower: how strong
- precipitation: how much falls down: water < > dust
- fill: how much water is in the sky: water < > dust
- pressure, hummidity: ?
- duration: some bars reach a threshold, trigger the weather and then perform till the bar is depleted again
- mixtures: eg storm-mether + heat at maximum = firestorm
- rain/drought: rivers growing/shrinking, more/less harvest, water level, floated basements
- fog:
- snow:
- heatwave: wildfires
- blizzard: no visibility, ice snow
- quakes/errosion: build/remove cliffs/rocks
- heavy storms: destroy trees or structures, steal and scatter items elsewere
- lightning: fire, charge electricity, charge mana
Space:
- shooting stars/comets: small destruction with little special ressources, large destruction with large ressources
- solar flare: vapourizing areas
- magnetism: 
- gravity: low gravity
Magical / opther dimensions / annomalities:
- miasma stream: charge mana, alter elements
- mutation/radiation: alter stats
- distortion: change dimension, portas
- annomalities: portals

2.) Civilisation sim
--------------------
Regions adjacent to cilized regions affect the civilisation there.
Civlilized regions have needs (supply/demand).
--------------------
External influence:
- hunger
Internal influence:
- low combined fittness leads to sickness / pleagues
Cilvilisation needs:
- civilisation states modify needs: food, medicine, money, expansion, materials, peace/aggression, savety

3.) Plug-in Institutions
------------------------
- add further needs/"bars": Mage Academy: consume books and mana
- influence existing needs: Bandit Camp: lowers adjacent region money and savety

4.) Politics Sim
----------------
- has own needs
- consumes needs of cities that belong to it
- move military

=================================================================================================

Different Approaches to how the simulation works:
ONLY simulate areas when you travel to them: --> nothing will happen when you stay where you are
FORCE simulation of everything every day: --> laggspike every day?
FORCE simulation with distance to player: --> far awayregions update slower
Simulate broader scope the further away: --> local regions, other kingdoms as a whole
Simulate politics differently, like, more frequently.
Let events trigger simulation updates. "if nothing special happened, dont update" <--- this should be the goal, the player entering or being near is special too

=================================================================================================

I think we can have the kind of simulation where the same seed and time results in the same output.
BUT this will only work to the point before the player starts interacting.
This is also why we can't go back in time, because any interactions of the player will have kicked the simulation off it's original track.


==================================================================================================
Simulaton LOD based on region distance and region relevance:
- we do never want to update all regions at once: 
---> So something like "all far away regions every hour" combined with "near regions every minute" will still cause a update on all regions every hour.
- Instead some "updater" should walk over the regions, only updating one at a time!
---> jumping back and forth between near and far regions, hitting the near ones with higher frequency
- the closer the more frequent the update
- never "multiple" regions at once

==================================================================================================

NO REVERSE Simulation
- making people young or unborn just got to check these actors
- rebuilding kingdoms just needs to rebuild buildings and run some population sim
- actually reversing the sim will just have us cover all scenarios, instead of handcrafting spells

==================================================================================================

Simulation provides a background Scenario! But Handcrafted is Focus!
Do not try to make time cover anything!
- Events with countdowns till they do something!
- Effect durations.

==================================================================================================

Individual Simulations:
- Time / Aging
- Repopulation
- gene mixing / evolution
- Family
- Regions
- Movement / Migration
- Jobs / Bands / Clans --> first step to interactible objects
- Resources / Goods / Money / Food (Wealth) 
- Day/Night / Weather 
- Towns / Administration
- Politics
- Terrain changing / Dungeons revealed
- Bosses

- events that controll simulation parameters
- events that add custom simulation (Pestilence = every time tick issue actorManager to decrease MaxAge)

==================================================================================================

SciFi / Lost Scifi ruins
- some global tech tree?

==================================================================================================

#endif
