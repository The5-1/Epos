#if false

DrawMeshProcedural lets you put vertices into a mesh and instantly draw it.
- we do not need the whole terrains mesh for our grass-shader just to a certain LOD distance
- ... so we probably need to make a procedural mesh covering all surfaces with grass anyways?
- ... but that wont be possible in shader then. Hence slow?



KavantWig --> The WigTemplate takes a mesh at editor-time and converts it to a WigTemplate:
Select a mesh inside an imported fbx/obj file and right click it. From the context menu, select "Kvant" -> "Wig" -> "Convert to template". 
This generates Wig Template file from the mesh. Give it to a WigController component. Then play it.
--> while i might need to do that at game-time this thing will still have good information!

The positions of the grass roots need to be calculated on CPU ---> so i can use these for collision detection




#endif