using UnityEngine;
using System.Collections;

public class Player_Input_Manager : MonoBehaviour
{

    public static Player_Input_Manager singleton;

    public Actor_Controller playerActorController;
    public Actor_Movement_Controller playerMovementController;
    public GameObject playerGO;

    protected void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(singleton);
        }
        else { Destroy(this); }
    }

    protected void Start()
    {
        playerActorController = Player_Manager.singleton.mainPlayer.actorController;
        playerMovementController = Player_Manager.singleton.mainPlayer.actorController.actorMovementController;
        playerGO = playerActorController.gameObject;
    }

    protected void OnDestroy()
    {
        if (singleton == this) { singleton = null; }
    }

    public float axisH;
    public float axisV;

    private void Update()
    {
        CollectInput();
        HandleInput();
    }

    private void CollectInput()
    {
        axisH = Input.GetAxis("Horizontal");
        axisV = Input.GetAxis("Vertical");
    }

    private void HandleInput()
    {

        HandleMovementInput();
    }


    private void HandleMovementInput()
    {
        //Vector3 moveVec = playerGO.transform.forward*axisV + playerGO.transform.right*axisH;
        //playerMovementController.MoveTarget(moveVec);
        playerMovementController.MoveDirection(axisV, axisH);
    }

}



#if false


/// <summary>
/// This handles input, key-sequences, key-combos
/// Calls appropriate methods like Move() Sprint() Aim() Jump() LongJump() from MovementController
/// It does NOT handle CONTEXT!!! MovementController handles e.g. jumping against a wall and bouncing off.
/// </summary>
//[RequireComponent(typeof(Actor))]
//[RequireComponent(typeof(MovementController))]
public class PlayerInput_Manager : MonoBehaviour
{


    public Actor_Controller _actor; //actor reference to get the right camera to align movement
    private Transform _transform; //cache reference to Transform!
    private Camera _camera;
    private ActorMovement_Controller _movementController; //so it knows where to send the inputs

#region movement
    private Vector3 _moveDirNormalized; //WADS movement, normalized
    private Vector3 _camForwardDir;
    private Vector3 _camRightDir;
    private Quaternion _rotFromActorToCamera; //rotation to align player coordiante system with camera
    //----------------
    private Ray _mouseRay; // ray cast through mouse position into scene
    private GameObject _objectUnderCursor; //object hit by the ray
    private Vector3 _pointUnderCursor; //coordinate the ray hit at
    private Vector3 _toMouseDirNormalized;  //Vector from Actor to Mouse Pointer position
    private Plane _yPlane; //current Y-plane of the actor, used to raycast as ground-level if nothing else is hit
#endregion

#region inputstates
    private bool _InputSprint;
    private bool _InputCrouch;
    private bool _InputJump;
    private bool _InputJumpDown;
#endregion

    void Awake()
    {
        if (_actor == null) _actor = this.GetComponent<Actor_Controller>();
        _transform = _actor.transform;
        _movementController = _actor._MovementController;
        _camera = _actor._camera;
    }

    void Start()
    {
        //initialize all the rest here
        _mouseRay = new Ray();
        _moveDirNormalized = new Vector3();
        _camForwardDir = new Vector3();
        _camRightDir = new Vector3();
        _toMouseDirNormalized = new Vector3();
        _rotFromActorToCamera = new Quaternion();
        _pointUnderCursor = new Vector3();
        _yPlane = new Plane(Vector3.up, _transform.position);

    }

    void QuerryButtons()
    {
        //Querry all other button presses here
        _InputJump = Input.GetAxisRaw("Jump") != 0;
        _InputJumpDown = Input.GetButtonDown("Jump");
        _InputSprint = Input.GetAxisRaw("Sprint") != 0;
        _InputCrouch = Input.GetAxisRaw("Crouch") != 0;
    }

    void Update()
    {
        QuerryButtons();
        Calc_moveVector();
        Calc_rotVector();
    }

    //=================================================
    // Send everything over to the Movement Cotnroller
    //=================================================
    void FixedUpdate()
    {
        Calc_moveVector();
        Calc_rotVector();
        //Send everything over to the Movement Manager
        _movementController.Input(_moveDirNormalized, _toMouseDirNormalized, _pointUnderCursor, _objectUnderCursor, _InputSprint, _InputCrouch, _InputJump, _InputJumpDown);
        //Debug.DrawRay(_transform.position, _moveDirNormalized, Color.blue, 1f);
        //Debug.DrawRay(_transform.position, _toMouseDirNormalized, Color.green, 1f);

    }


    void Calc_moveVector()
    {
        _moveDirNormalized.Set(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")); //WADS
        //Debug.DrawRay(_transform.position, _moveDirNormalized, Color.red, 10f);

        //align to camera based on 3rdPerson Standard Asset
        _camForwardDir = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1)).normalized;
        _camRightDir = -Vector3.Cross(_camForwardDir, new Vector3(0, 1, 0)).normalized;
        //Debug.DrawRay(_transform.position, _camForwardDir, Color.red, 10f);
        //Debug.DrawRay(_transform.position, _camRightDir, Color.blue, 10f);
        _moveDirNormalized = Input.GetAxisRaw("Vertical") * _camForwardDir + Input.GetAxisRaw("Horizontal") * _camRightDir;
        //align to camera attempt 1 (wrong horizontal angle!)
        //_rotFromActorToCamera = Quaternion.FromToRotation(Vector3.forward, _camera.transform.forward); //rotation to align player system with camera system
        //_moveDirNormalized = _rotFromActorToCamera * _moveDirNormalized; //!!quaternion * vector!!

        //Debug.DrawRay(_transform.position, _moveDirNormalized, Color.blue, 10f);
        _moveDirNormalized.y = 0.0f; //cut y because the base transformation introduced a y value
        _moveDirNormalized.Normalize();
    }

    void Calc_rotVector()
    {
        _yPlane.SetNormalAndPosition(Vector3.up, _actor.transform.position);
        UpdatePointUnderCursor();
        _toMouseDirNormalized = _pointUnderCursor - _actor.transform.position; //vector from actor to mouse
        _toMouseDirNormalized.Normalize();
    }

    /// <summary>
    /// Gets the position of the cursor in the scene
    /// Uses player y-plane if nothing is there hot raycast
    /// Sets _objectUnderCursor if a GameObject was hit
    /// </summary>
    void UpdatePointUnderCursor()
    {
        // Ray from camera trough point, mouse cursor here
        Vector3 hitPos;

        // Raycast with actual scene
        Ray camRay = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit pointHit;
        if (Physics.Raycast(camRay, out pointHit))
        {
            hitPos = pointHit.point;
            _objectUnderCursor = pointHit.transform.gameObject;
        }
        // if nothing in the scene was hit raycast with players current Y-plane
        else
        {
            float distance = 0;
            //write the distance of the rays origin to the y-plane
            _yPlane.Raycast(camRay, out distance);
            //select the point on the ray at that distance
            hitPos = camRay.GetPoint(distance);
            _objectUnderCursor = null;
        }
        _pointUnderCursor = hitPos;
    }

}

#endif