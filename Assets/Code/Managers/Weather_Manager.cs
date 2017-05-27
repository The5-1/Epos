using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**********************************************
    Global weather simulation
    --> low resolution 2D flow simulation of wind/weather
    --> possibly a ground layer that uses the sky layers output but considers terrain features
    --> maybe a undergrund layer for tunnels that might become inaccessible when too much wind
    + Variations: Global Mana Stream simulation, Tectonic plate movement

    Local Region Weather:
    --> grab your own Worldmap-XYZ position and interpolate wind-vector from global weather grid
    --> for wind, just get a vector that applys to the whole region
    Local Detail Wind Simulation:
    --> Compute-Shader that calculates wind-shadowing per vertex for the enire scene?
    --> screen space and project back to simulate grass etc?
    --> how to re-use whole scene geometry?
    --> compute shaders are textures... no vertices...


**********************************************/

public class Weather_Manager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
