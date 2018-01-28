using UnityEngine;
using System.Collections;


/*
//[System.Serializable]
public class PlayerCameraData
{
    // the players Camera GameObject
    public GameObject _playerCameraObject;
    public CameraController _playerCameraController;
    public CameraManipulator _playerCameraManipulator;
    public string _playerCameraName;
}

public class PlayerMovementData
{
    // the players Movement Controller
    public MovementController _playerMovementController;
    public MovementManipulator _playerMovementManipulator;


}
*/

//>>>>>>>>>>>>>>>>>>> Don't create your player components in code, make a prefab! <<<<<<<<<<
//>>>>>>>>>>>>>>>>>>> I do not necessarily, but i check if any are missing though.

[System.Serializable] //for inspector
public class Player : Actor
{

    //============================================================
    // Controllers
    //------------------------------------------------------------
    public PlayerInputController _PlayerInputController;
    public bool _playerInputEnabled = true;
    //============================================================

}
