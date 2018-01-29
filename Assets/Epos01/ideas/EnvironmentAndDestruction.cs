#if false

Tile-Based Terrain
------------------
Pro:
- have logic per tile (bridge, cave, climbable)
- procedural
- can be modified in code
- can be simulated
- plays well with navmesh + AI per Tile
Con:
- diagonals: like a long arm of land reaching diagonally into the sea (realism vs gameplay!)
- gentle slopes: like a winded path leading up a spire (realism vs gameplay!)
- curves: roads/rivers (we can have that as a separate layer though) http://catlikecoding.com/unity/tutorials/hex-map/part-7/
both:
- for "unique" environments we can always make big hand-crafted tiles with classic terrain and plug those in
- building interiors would need to be like that
scatter Instanced Objects:
- create the base mesh and collision from code but use GPU instancing to add details like rocky border on overhanging cliff


Base Terrain material like WC3 + region themes
- defines the general material that handles all the masks itself
-- can have grass or other objects scattered (that use a grass map)
-- the additional noise function over the mask can give the mask a different look
- the whole region uses a material set
- the tiles just define what material goes where
- the tiles have masks for decorators (grass, shrooms, etc)
-- possibly multiple masks for different themes
- concave cliffs could use tesselation to create overhangs but that would require to update the collision model with it


Terrain deformation:
- terrain that got a buiding set on its tile cant be deformed
--> this should solve most issues
- height mask, remove decorating geometry if > 0
- craters etc, only subtle e.g. for hammer attacks or spells
-- major craters or cracks would replace the whole tile


Grass/Decorator Masks:
- grass maks vertex-map to determine it's height/density
-- grass mask grows (dilation) over time till the whole area is at max grass height
-- plus random function to randomize the distribution a bit
- flow/normal map to bend the direction it
-- fades back to 0 over time = regrow
- cut-mask to determine its cut height
-- mask fades back to 0 over time = regrow
- burn or frozen maks
-- effects of burn or freeze even when heat mask returned to 0
- heat mask
-- default heat or cold of region
-- current heat or cold
- mana burn mask
-- possibly just set color in here and rest is done with shader
-- if color is 0 then there is no
- wind vector
- force fields

Rain:
- wind vector
- no terrain collision (just fake it in shader, no need to really check where rain goes)
- Matrix-like explosion shockwaves that make speres 
--> not sure if thats in the rain or the spell effect
--> unless we have force fields
- force fields

Fog
- LOD
- interior/ close up with volume raymarching
- exterior/distant one just with distance + noise

Clouds
- volumetric ray marching
- noise functions + painted mask
- height layers
- flow maps taken from global weather sim
--> you can see the weather in other regions in the distance
- volume fog beneath: godrays + rain "courtains"

Weather
- global weather simulation (low res grid)
-- possibly a "above clouds" and "underground" variant
- region XYZ coordinate fetches own weather from grid
-- distance field, rain needs to be able to only affect half a region

Destructible props:
- Bushes, Trees, Rocks
-- have their own destruction logic

#endif