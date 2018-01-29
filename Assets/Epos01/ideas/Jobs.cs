#if false

Job / Building
--------------
A job consists of
- a building that stores input materials and output goods
-- the interactible objects should be outside
-- the inside is for the housing of the NPCs
- a interactible object that a NPC needs to mount to start conversion
--- NPC skill quality tier
--- NPC personality determines quantity?
- a counter that a other Family Member mounts to sell things

e.g. Weaponsmith
- blacksmith building stores ores and weapons
- smith interacts with the forge and converts ores to weapons
- wife interacts with the counter to enable selling
- player interacts with the counter and that opens the shop screen
---> if counter is not occupied, player can steal


- weaponsmith
- armorsmith
- bowmaker
- gunmaker



Job Schedule
------------
- the schedule only listens to events, typically time inputs, and then notifies e.g. movement controllers to do things
- what exactly the actual job is, like carrying goods etc, is a own, unique, hand crafted script!!!

e.g. Transportation job Scedule
- goTo(region,Interactible object)
- getGoodFromContainer(Interactible object, good, amount) //max amount determined by skill
- goTo(region,Interactible object)
- putGoodIntoContainer(Interactible object, good, amount)

#endif