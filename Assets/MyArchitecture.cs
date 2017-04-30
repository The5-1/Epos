#if false


"Controller-Manager-Data"


Controller: 
- Think "The Simulation has methods that a CombatController would not care for to find in the Manager"
- e.g. time based simulation, combat controler on actor, 

Manager:
- Think: "This needs to accept combat, simulation, etc. triggers. Don't try to do simulations in it!"
- "But this tries to keep a valid state within it's data!"

Data:
- "This does nothing." (Possibly sanity-check values that go in, but the manager should do that.)
- constructor, initialisation, destructor
- provides methods to save it's data and recieve loaded data.




#endif