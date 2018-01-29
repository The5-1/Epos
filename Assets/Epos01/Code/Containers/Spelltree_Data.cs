using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 3D Spell Skilltree in the form of point cloud of star constelaltion
// a regions magical flow moves closer to the individual constellations the more that spell is cast
// Looking trough your upgraded god-lens into the nightsky can reveal new constellations or stars


/// <summary>
/// A Upgrade/Sidegrade effect of a spell
/// represented as a star in the constellation
/// </summary>
public class Spelltree_Star //=upgrade
{
    public string name;
    public string description;
    public Vector3 relativeConstellationPos; //pos relative to the constellations origin

    public Spelltree_Star parent;
}

/// <summary>
/// A Spell consisting of multiple tiers and up-/sidegrades
/// </summary>
public class Spelltree_Constellation //=spell
{
    public string name;
    public string description;
    public Vector3 relativeClusterPos; //pos relative to the clusters origin

    public Spelltree_Star entryNode;
    //public TreeDataStructure nodes;
}

/// <summary>
/// the school or type of a spell
/// </summary>
public class Spelltree_Cluster //=School
{
    public string name;
    public string description;
    public Vector3 pos; //pos in the 3D skill pointcloud

    public List<Spelltree_Constellation> spellConstellations;
    //public TreeDataStructure nodes;
}

/// <summary>
/// The spell skilltree represented as a star map
/// </summary>
public class Spelltree_Data //=entire skilltree
{
    public List<Spelltree_Cluster> spellClusters;
}
