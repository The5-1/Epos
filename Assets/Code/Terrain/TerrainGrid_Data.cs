using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**************************************
Like a ordinary heightmap BUT:
- each tile checks its neighbours heigts
- and determines if it should be a cliff or ramp (or different types)
**************************************/

/**************************************
    \---^---/   A tile has a own height, like a heightmap, absolute, not relative.
    |       |   The height of the neighbours determines the tiles shape
    <   O   >   8 neighbour
    |       |       - height = height at center, the edges will interpolate between heights
    /---v---\       - thickness = thickness of the pillar/wall, interpolate towards neighbours
                                                                              
****************************************/

/**************************************
 *      !!! outdated idea !!!
    o---o---o   o---o---o
    | \ | / |   | / | \ |
    o---o---o   o---o---o   Each quarter of the tile can have a own height
    | / | \ |   | \ | / |   The tile itslef is shaped based on that
    o---o---o   o---o---o   

    +---+---+
    | | | | |
    +---+---+     4 tiles provide the same behaviour as a WC3 tile
    | \ | / |
    +---+---+

****************************************/


namespace TerrainGrid
{

    public static class Tile_Metrics
    {
        public const float tilesize = 0.5f;
        public const byte resolution = 4;

        public static Mesh makeTileMesh()
        {
            ushort numverts = (ushort)(resolution * resolution);
            Mesh mesh = new Mesh();
            List<Vector3> verts = new List<Vector3>();
            int[] indices = new int[numverts];
            for (ushort i = 0; i < numverts; i++)
            {

            }
            mesh.SetVertices(verts);
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            return mesh;
        }
    }

    public enum TileQuadrant { NW, NE, SE, SW }
    public enum TileDirection { N, NE, E, SE, S, SW, W, NW, C }

    /* //tile should not be a own class but rather a array in the grid to compact memory
    public class TerrainTile
    {
        public TerrainGrid parentGrid;
        public int index;

        public TileDirection border; //set this if a tile is a border

        public byte thickness; //[0-1] e.g. if the tile is a pillar, this sets its radius or generally, the thickness of a wall
        public float height; //height of the the center of the tile, if 0 this tile is flat

        //public TerrainTile[] neighbourTiles; //i dont think i need to store this, since it is a grid

        private float[] qudrantHeights;
        private float[] qudrantHeightsNeightbourInfluence;

        public TerrainTile(TerrainGrid parent)
        {
            parentGrid = parent;
            setParent(parent);
        }

        public void setParent(TerrainGrid parent)
        {
            parentGrid = parent;
            updateIndex();
        }

        public void updateIndex()
        {
            index = System.Array.IndexOf(parentGrid.tiles, this);
        }

        public void updateBorder()
        {
            index = System.Array.IndexOf(parentGrid.tiles, this);
        }

        public int getNeighbourIndex(TileDirection dir)
        {
            switch(dir)
            {
                case TileDirection.N:
                    {
                        if (index > (parentGrid.height - 1) * parentGrid.width) return index + parentGrid.width;
                        else return -1;
                    }
                case TileDirection.NE:
                    return index + parentGrid.width + 1;
                case TileDirection.NW:
                    return index + parentGrid.width - 1;

                case TileDirection.W:
                    return index + 1;
                case TileDirection.E:
                    return index - 1;
                case TileDirection.NW:
                    return index + parentGrid.width - 1;


                default:
                    return -1;
            }
        }

        public Mesh triangulate()
        {
            return new Mesh();
        }
    }
    */

    public class TerrainMaterial
    {
        #region textures
        //peel of textures based on steepness, trampling, explosions
        public byte layer0_ID; //grass
        public byte layer1_ID; //earth
        public byte layer2_ID; //rubble
        public byte layer3_ID; //rock
        #endregion

        #region terrain interpolation
        public byte smoothness; //possibly used for interpolation
        public byte weight; //possibly used for other features to spice up terrain
        #endregion

        #region physical
        public float density;
        public float hardness;
        public float elasticity;

        public float steepness_threshold_slope; //when the grass rips open and earth peeks trough but the player can still walk on it
        public float steepness_threshold_step; //when the seepness turns into a step that the player can move over when he is tall enough
        public float steepness_threshold_cliff; //when the steepness turns into a wall that the player can not move over
        #endregion

        #region gameplay
        public byte fertility;
        public byte magicalResistance;
        public byte physicalResistance;
        public byte movespeedFactor;
        #endregion
    }

    /*
    public struct Tile_Neighbour
    {
        int index;
        TileDirection direction;

        public Tile_Neighbour(int idx, TileDirection dir)
        {
            index = idx;
            direction = dir;
        }
    }

    public struct Tile_Point
    {
        Vector3 position;
        TileDirection? direction;

        public Tile_Point(Vector3 pos, TileDirection? dir)
        {
            position = pos;
            direction = dir;
        }
    }
    */

    public class TerrainGrid_Data
    {
        //classes vs native types: https://gamedev.stackexchange.com/questions/43341/more-efficient-data-structure-for-large-layered-tile-map
        //-- classes require more memory management!
        //-- Rule of Three: https://stackoverflow.com/questions/4172722/what-is-the-rule-of-three
        //-- intersting: Scope-Bound Resource Management (RAII) https://stackoverflow.com/questions/2321511/what-is-meant-by-resource-acquisition-is-initialization-raii

        //dynamic (new) 1D vs 2D array: 
        //-- https://stackoverflow.com/questions/2672085/static-array-vs-dynamic-array-in-c
        //-- https://stackoverflow.com/questions/17259877/1d-or-2d-array-whats-faster
        //-- bad for small matrices
        //-- calculating 1D indices is not slower (minimally)
        //-- 1D is better for CPU cache (memory locality + overhead)
        //-- only really when the number of columns varries per row

        public int gridWidth = 6;
        public int gridHeight = 6;
        public float tileSize = 1.0f;

        public Vector3 gridTranslation = Vector3.zero;

        #region terrain shape
        //Array vs List performace is almost similar: 
        // https://stackoverflow.com/questions/434761/array-versus-listt-when-to-use-which
        // https://stackoverflow.com/questions/454916/performance-of-arrays-vs-lists
        // But converting a list back to an array might take performance
        public List<float> tileHeights; //height of the tile
        public List<byte> tileFills; //thickness of the tile = e.g the radius of a pillar or the thickness of a wall
        public List<TerrainMaterial> material; //material of the tile
        #endregion


        #region terrain interaction
        //special effects for later
        public List<int> health; //the health of a tile //attacking on a tile tramples it down //armor and stuff is based on material //trample threshold too
        public List<byte> layer; //current texture layer override
        public List<int> temperature;
        public List<byte> magicalCharge;
        public List<bool> burnt;
        public List<bool> molten;
        public List<bool> frozen;
        public List<bool> swamp;
        #endregion


        public TerrainGrid_Data(int x, int y, Vector3 translation)
        {
            init(x,y, translation);
        }

        protected void init(int x, int y, Vector3 translation)
        {
            gridWidth = x;
            gridHeight = y;

            gridTranslation = translation;

            material = new List<TerrainMaterial>();
            material.Capacity = gridWidth * gridHeight;

            tileHeights = new List<float>();
            tileHeights.Capacity = gridWidth * gridHeight;

            tileFills = new List<byte>();
            tileFills.Capacity = gridWidth * gridHeight;

            health = new List<int>();
            health.Capacity = gridWidth * gridHeight;
        }

        #region index/cell/position conversion Helpers

        public int cellToIndex(int x, int y)
        {
            return y * gridWidth + x;
        }

        public Vector2 indexToCell(int idx)
        {
            int y = Mathf.FloorToInt(idx / gridWidth);
            return new Vector2(y, idx - y);
        }

        public Vector3 indexToPosition(int idx)
        {
            Vector2 cell = indexToCell(idx);

            return new Vector3(cell.x * tileSize, tileHeights[idx], cell.y * tileSize);
        }

        #endregion 

        #region Terrain Select/Search Helpers

        public TileDirection? checkIsBorderTile(int idx) //? = allow null as return
        {
            if (checkIsBorderTileNorth(idx)) //N
            {
                if (checkIsBorderTileEast(idx)) return TileDirection.NE;
                else if (checkIsBorderTileWest(idx)) return TileDirection.NW;
                else return TileDirection.N;
            }
            else if (checkIsBorderTileSouth(idx)) //S
            {
                if (checkIsBorderTileEast(idx)) return TileDirection.SE;
                else if (checkIsBorderTileWest(idx)) return TileDirection.SW;
                else return TileDirection.S;
            }
            else return null;
        }

        public bool checkIsBorderTileNorth(int idx)
        {
            if (idx >= gridWidth*(gridHeight-1)) return true;
            else return false;
        }

        public bool checkIsBorderTileSouth(int idx)
        {
            if (idx < gridWidth) return true;
            else return false;
        }

        public bool checkIsBorderTileEast(int idx)
        {
            if (idx % gridWidth == gridWidth-1) return true;
            else return false;
        }

        public bool checkIsBorderTileWest(int idx)
        {
            if (idx % gridWidth == 0) return true;
            else return false;
        }

        public Vector3 offsetPointInDirection(Vector3 point, float offset, TileDirection dir)
        {
            switch (dir)
            {
                case TileDirection.N:
                    return new Vector3(point.x, point.y, point.z + offset);
                case TileDirection.NE:
                    return new Vector3(point.x + offset, point.y, point.z + offset);
                case TileDirection.E:
                    return new Vector3(point.x + offset, point.y, point.z);
                case TileDirection.SE:
                    return new Vector3(point.x + offset, point.y, point.z - offset);
                case TileDirection.S:
                    return new Vector3(point.x, point.y, point.z - offset);
                case TileDirection.SW:
                    return new Vector3(point.x - offset, point.y, point.z - offset);
                case TileDirection.W:
                    return new Vector3(point.x - offset, point.y, point.z);
                case TileDirection.NW:
                    return new Vector3(point.x - offset, point.y, point.z + offset);
                default:
                    return point;
            }
        }

        public int getNeigbourIndex(int idx, TileDirection dir)
        {
            if (checkIsBorderTileNorth(idx) && dir >= TileDirection.NW && dir <= TileDirection.NE) return -1;
            else if (checkIsBorderTileEast(idx) && dir >= TileDirection.NE && dir <= TileDirection.SE) return -1;
            else if (checkIsBorderTileSouth(idx) && dir >= TileDirection.SE && dir <= TileDirection.SW) return -1;
            else if (checkIsBorderTileWest(idx) && dir >= TileDirection.SW && dir <= TileDirection.NW) return -1;
            else
            {
                switch (dir)
                {
                    case TileDirection.N:
                        return idx + gridWidth;
                    case TileDirection.NE:
                        return idx + gridWidth + 1;
                    case TileDirection.E:
                        return idx + 1;
                    case TileDirection.SE:
                        return idx - gridWidth + 1;
                    case TileDirection.S:
                        return idx - gridWidth;
                    case TileDirection.SW:
                        return idx - gridWidth - 1;
                    case TileDirection.W:
                        return idx - 1;
                    case TileDirection.NW:
                        return idx + gridWidth - 1;
                    default:
                        return -1;
                }
            }
        }

        public Dictionary<TileDirection, int> getNeighbourIndices(int idx)
        {
            Dictionary<TileDirection, int> neighbours = new Dictionary<TileDirection, int>();

            for(int dir = 0, n = -1;  dir < 8; dir ++)
            {
                n = getNeigbourIndex(idx, (TileDirection)dir);
                if (n != -1) neighbours.Add((TileDirection)dir,n);
            }
            return neighbours;
        }

        public Dictionary<TileDirection, Vector3> getNeighbourCenterPositions(int idx)
        {
            Dictionary<TileDirection, Vector3> neighbourPositions = new Dictionary<TileDirection, Vector3>();

            Dictionary<TileDirection, int> neighbourIndices = getNeighbourIndices(idx);

            for (int dir = 0; dir < 8; dir++)
            {
                //if there is a true neighbour, get its center
                if (neighbourIndices.ContainsKey((TileDirection)dir)) neighbourPositions.Add((TileDirection)dir, indexToPosition(neighbourIndices[(TileDirection)dir]));
                //if not, use the same height as this
                else neighbourPositions.Add((TileDirection)dir, getPseudoNeighbourCenterPosition(idx,(TileDirection)dir));
            }
            return neighbourPositions;
        }

        public Vector3 getPseudoNeighbourCenterPosition(int idx, TileDirection dir)
        {
            float step = tileSize;
            Vector3 center = indexToPosition(idx);
            return offsetPointInDirection(center, step, dir);
        }

        public Dictionary<TileDirection, Vector3> getTileAttachmentPoints(int idx)
        {
            Dictionary<TileDirection, Vector3> points = new Dictionary<TileDirection, Vector3>();

            float step = tileSize / 2.0f;

            Vector3 center = indexToPosition(idx);

            points.Add(TileDirection.C, center);
            points.Add(TileDirection.N, offsetPointInDirection(center, step, TileDirection.N));
            points.Add(TileDirection.NE, offsetPointInDirection(center, step, TileDirection.NE));
            points.Add(TileDirection.E, offsetPointInDirection(center, step, TileDirection.E));
            points.Add(TileDirection.SE, offsetPointInDirection(center, step, TileDirection.SE));
            points.Add(TileDirection.S, offsetPointInDirection(center, step, TileDirection.S));
            points.Add(TileDirection.SW, offsetPointInDirection(center, step, TileDirection.SW));
            points.Add(TileDirection.W, offsetPointInDirection(center, step, TileDirection.W));
            points.Add(TileDirection.NW, offsetPointInDirection(center, step, TileDirection.NW));

            return points;
        }

        public int findIndexAtCoordinate(Vector3 coord)
        {
            Vector3 point = coord - gridTranslation;
            int idx_w = Mathf.CeilToInt((point.x / tileSize)-1);
            int idx_h = Mathf.CeilToInt((point.z / tileSize) - 1);

            if (idx_w >= gridWidth || idx_w  < 0 || idx_h >= gridHeight || idx_h < 0) return -1;

            Debug.Log(string.Format("terrain grid selected at tile ({0}, {1})", idx_w, idx_h));

            return idx_h * gridWidth + idx_w;
        } 

        #endregion


        #region terrain editing

        public void setTileHeight(Vector3 coord, float height)
        {
            setTileHeight(findIndexAtCoordinate(coord), height);
        }

        public void setTileHeight(int index, float height)
        {

        }

        #endregion

        #region mesh

        public void makeTileMesh(int idx)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();

            Dictionary<TileDirection, Vector3> points = getTileAttachmentPoints(idx);
            Dictionary<TileDirection, Vector3> neighbours = getNeighbourCenterPositions(idx);

            //Go over all the outer points
            foreach (KeyValuePair<TileDirection, Vector3> point in points)
            {
                Debug.DrawLine(Vector3.zero, new Vector3(1, 0, 0), Color.red,0,depthTest:true);
            }
        }

        public Mesh makeMesh()
        {
            Mesh m = new Mesh();
            List<Vector3> vertices;
            List<Vector3> indices;
            for (int idx = 0; idx < gridWidth * gridHeight; idx++)
            {
                makeTileMesh(idx);
            }
            return m;
        }

        public void debugMakeMesh()
        {
            for (int idx = 0; idx < gridWidth * gridHeight; idx++)
            {
                makeTileMesh(idx);
            }
        }

        #endregion

    }

}