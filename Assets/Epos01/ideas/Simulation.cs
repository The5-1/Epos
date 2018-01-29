#if false

Seconds
-------
local: 
- day-night-cycle simulation update (interpolate)

minutes
-------
local:
-- NPC mood/actions simulation: start a fight or start crying or something
-- patroll paths update
-- maybe something like traffic lights that update every minute

hours
-----
local:
-- NPC dayly scedule check, start moving elsewhere, start interacting
-- NPC entering/leaving region
-- building/job/shop goods updating
-- stuff like, some environment object becoming active for an hour (e.g. a shiny sword covered by moss gets lit just right to start shining/blinking for an hour)
global
-- check  far away Actors that can be here but must not currently be here! 
---- region._actorsWorkingInThisRegion; //check this actor data if he would come over to this region now
---- region._actorsLivingInThisRegion; //check this actor data if he would come over to this region now
-- check adjacent regions in detail
-- foreach(Actor in adjacentRegions) adjacentRegions._actorsInThisRegion -> check their scedules

days
----
- global









#endif