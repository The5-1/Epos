#if false

UseCases
--------
The player enters a region or skips time, now the simulation checks when the player last was there or the delta of time passed.
The simulation must be able to check if they breed and if they gave birth.
And it must be able to do so __multiple times__, e.g. when skipping 1000years


=================================================================================================


Different Approaches:
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
Updating per region: (get rid of the global all actor_data list?)
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
