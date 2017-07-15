using UnityEngine;
using System.Collections;


public enum GroundState { grounded,airborne,swimming,climbing,diving,zeroG};
public enum CrouchState { standing, crouching, crawling};
public enum SpeedState { walk, sneak, run, rush};
public enum DodgeState { none, roll, dash }; //for stuff like attack while rolling or dodging
public enum RagdollState { none, partially, full}; //active or completely passive ragdoll
public enum StunState { none, dizzy, stunned, paralyzed}; //different variations of how stunned, dizzy = can still move, stunned = cant move but has stun anim, paralyted = completely frozen

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Actor_Controller))]
public class ActorMovement_Controller : MonoBehaviour
{
    #region gameObjects
    public GameObject parentGO;
    public Actor_Controller actorController;
    public Collider parentCollider;
    public Rigidbody parentRigidbody;
    #endregion

    #region actorData References
    public float movespeed;
    #endregion

    #region Movement Controller Constants
    [Header("Movement Controller Constants")]
    public float groundRayLength = 1.0f;
    public float groundRayOffset = 0.2f;
    #endregion

    #region Actor Movement States
    [Header("Actor Movement States")]
    public bool animInputDisabled = false; //disable all anim input, even from effects, cutscenes, etc
    public bool externalMovement = false; //some external movement happens, e.g. chain hook or knockback.
    public CrouchState crouchState;
    public SpeedState speedState = SpeedState.walk;
    public DodgeState dodgeState = DodgeState.none;
    public RagdollState ragdollState = RagdollState.none;
    public StunState stunState = StunState.none;
    #endregion

    #region Environment Movement States
    [Header("Environment Movement States")]
    public GroundState groundState = GroundState.grounded;
    public Vector3 groundNormal;
    public CrouchState headspaceState = CrouchState.standing;
    #endregion

    #region Actor Motion Values
    [Header("Actor Motion Values")]
    public float moveImpulse = 0.0f;
    public float turnImpulse = 0.0f;
    #endregion


    private void Awake()
    {
        parentGO = this.gameObject;
        actorController = parentGO.GetComponent<Actor_Controller>();
        parentCollider = parentGO.GetComponent<CapsuleCollider>();
        parentRigidbody = parentGO.GetComponent<Rigidbody>();
    }

    void Start()
    {
        movespeed = actorController.actorData.actorValues.Find(x => x._stattype_original == Actor_StatsEnum.Movespeed)._current;
    }

    void FixedUpdate()
    {
        Move(Random.insideUnitSphere*10000.0f);
        CheckStates();
    }

    public void CheckStates()
    {
        CheckGroundState();
    }

    public void CheckGroundState()
    {
        RaycastHit hitInfo;
        //#if UNITY_EDITOR
            // helper to visualise the ground check ray in the scene view
            Debug.DrawLine(parentGO.transform.position + (Vector3.up * groundRayOffset), parentGO.transform.position + (Vector3.up * groundRayOffset) + (Vector3.down * groundRayLength), Color.magenta);
        //#endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(transform.position + (Vector3.up * groundRayOffset), Vector3.down, out hitInfo, groundRayLength))
        {
            groundNormal = hitInfo.normal;
            groundState = GroundState.grounded;
        }
        else
        {
            groundState = GroundState.airborne;
            groundNormal = Vector3.up;
        }

    }


    protected void HandleRotation()
    {

    }

    protected void HandleMovement()
    {
        if (groundState == GroundState.grounded)
        {
            ApplyMovementGround();
        }
        else
        {

        }
    }

    protected void ApplyMovementGround()
    {
        parentRigidbody.AddForce(Vector3.forward * moveImpulse);
    }

    public void Move(Vector3 move)
    {
        Vector3 movevec = move * Time.deltaTime * movespeed;
        float dot = Vector3.Dot(movevec.normalized, groundNormal);

        movevec = Vector3.ProjectOnPlane(movevec, groundNormal);
        //movevec += movevec * dot * _SlopeFactor * _actor._ActorData._GravityMultiplier;//slide down slopes
        parentRigidbody.AddForce(movevec - parentRigidbody.velocity, ForceMode.Impulse);


        //HandleRotation();
        //HandleMovement();
    }

}

#if false

public enum State_MovementSpeed { walkSpeed, sprintSpeed, crouchSpeed };   //check if these even get set by anything else than movement controller
public enum State_Terrain { ground, air, water, climbing, latching };            //e.g. a enemy attacking actor while in air deals more damage
public enum State_Sliding { notSliding, sliding };
public enum State_Headspace { canStand, canNotStand };
public enum State_LatchedSurface { noSurface, anySurface, climbableSurface };
public enum State_IsRagdoll { notRagdolled, ragdolled };
public enum State_Gravity { gravityOn, gravityOff };

/// <summary>
/// Provides movement Methods like Move() Sprint() Aim() Jump() LongJump()
/// Handles CONTEXT: Like executing Jump whilst in the Air or being ragdolled
/// Manages Actor Values: Sets actor to crouching if CrouchTo() is called and back to walking if WalkTo() is called
/// </summary>
[RequireComponent(typeof(Actor_Controller))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class ActorMovement_Controller : MonoBehaviour
{
#region constants
    private const float _ConstMoveScale = 5.0f; //multiplier for the movement Vector
    private const float _ConstMoveMaxScale = 0.1f; //multiplier for the speed cap
    private const float _ConstRotScale = 0.05f;  //multiplier for the default 100 movesped
    private const float _ConstJumpScale = 0.2f;  //multiplier for the default 100 movesped
    private const float _ConstForceScaleMove = 0.01f; //multiplier for the default 100 movesped
    private const float _ConstForceScaleRot = 0.05f;  //multiplier for the default 100 movesped
    private const float _ConstForceScaleJump = 0.1f;  //multiplier for the default 100 movesped

    private const float _RayRadius = 0.3f;
    private const float _GroundRayLength = 0.05f;
    private const float _HeadspaceRayLength = 0.05f;
    private const float _ClimbRayLength = 0.6f;

    private const float _SlopeFactor = 0.5f; //multiplier of how much slopes affect the movespeed, use with actorGravity
#endregion

#region initialize
    public Actor_Controller _actor;
    private GameObject _gameObject;
    private Actor_Data _actor_Data;
    private Transform _transform; //cache reference to Transform!
    private Transform _headTransfrom;  //cache reference to Transform!
    private Rigidbody _rigidbody;
    [SerializeField] private Vector3 _rigidbodyVelocityDisplay;

    public GameObject _objectStandingOn;
    public RaycastHit _groundHitInfo;
    public Vector3 _groundNormal;

    public RaycastHit _headHitInfo;

    public GameObject _objectClimbingOn;
    public RaycastHit _climbHitInfo;
    public Vector3 _climbNormal;

    //capsule collider for collision and scaling when crouching/sneaking
    private CapsuleCollider _colliderCapsule;
    private Transform _colliderTransform;
    private Vector3 _colliderCenter;
    private float _colliderYbounds; //half the height of the collider, only set once
#endregion

#region states
    public State_MovementSpeed _State_MovementSpeed;
    public State_Terrain _State_Terrain;
    public State_Sliding _State_Sliding;
    public State_Headspace _State_Headspace;
    public State_LatchedSurface _State_LatchedSurface;
    public State_IsRagdoll _State_Ragdoll;
    public State_Gravity _State_Gravity;
    //CurrentTerrain State Actor Speed Multipliers
    private float _currentSpeedMult_Terrain;
    private float _currentSpeedMult_Movement;
    float _currentCombinedMaxSpeed;
    int _remaining_Multijumps;
#endregion

#region calculations
    public Vector3 _FinalMoveVector; //resutling movement after applying speed
    private Vector3 _ToMouseDirBody;
    private Vector3 _ToMouseDirHead;
    private Quaternion _rotationBody;
    private Quaternion _rotationHead;
#endregion

    void Awake()
    {
        if (_actor == null) _actor = this.GetComponent<Actor_Controller>();
        _gameObject = _actor.gameObject;
        _actor_Data = _actor._ActorData;
        _transform = _actor.gameObject.transform;
        _headTransfrom = _actor._Bone_Head;

        _rigidbody = _actor.gameObject.GetComponent<Rigidbody>();
        _colliderCapsule = _actor.gameObject.GetComponent<CapsuleCollider>();
        _colliderTransform = _colliderCapsule.transform;
        _colliderYbounds = _colliderCapsule.bounds.extents.y; //bounding box distance from center to box
        _colliderCenter = _colliderCapsule.center;



        _State_MovementSpeed = State_MovementSpeed.walkSpeed;
        _State_Terrain = State_Terrain.ground;
        _State_Sliding = State_Sliding.notSliding;
        _State_Ragdoll = State_IsRagdoll.notRagdolled;
        _State_Headspace = State_Headspace.canStand;
        _State_LatchedSurface = State_LatchedSurface.noSurface;

        _remaining_Multijumps = _actor_Data._NumJumps;
    }

    void Start()
    {
        //=======================
        _rigidbody.interpolation = RigidbodyInterpolation.None; //!!! anything else makes it laggy
        //=======================
        _FinalMoveVector = new Vector3();
        _ToMouseDirBody = new Vector3();
        _ToMouseDirHead = new Vector3();
        _rotationBody = new Quaternion();
        _rotationHead = new Quaternion();

        _groundHitInfo = new RaycastHit();
        _headHitInfo = new RaycastHit();
        _climbHitInfo = new RaycastHit();
        _groundNormal = new Vector3();
        _climbNormal = new Vector3();

        //_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

    }

    void FixedUpdate()
    {
        _rigidbodyVelocityDisplay = _rigidbody.velocity;
        //probably not needed, Input() gets executed every FixedUpdate() so everything should go there
    }

    private void GetActorSpeedMult_TerrainState()
    {
        if (_State_Terrain == State_Terrain.ground) _currentSpeedMult_Terrain = _actor_Data._ground_SpeedMult;
        else if (_State_Terrain == State_Terrain.air) _currentSpeedMult_Terrain = _actor_Data._air_SpeedMult;
        else if (_State_Terrain == State_Terrain.water) _currentSpeedMult_Terrain = _actor_Data._water_SpeedMult;
        else if (_State_Terrain == State_Terrain.climbing || _State_Terrain == State_Terrain.latching) _currentSpeedMult_Terrain = _actor_Data._climb_SpeedMult;

    }

    private void GetActorSpeedMult_MovementState()
    {
        if (_State_MovementSpeed == State_MovementSpeed.walkSpeed) _currentSpeedMult_Movement = _actor_Data._WalkSpeedMult;
        else if (_State_MovementSpeed == State_MovementSpeed.sprintSpeed) _currentSpeedMult_Movement = _actor_Data._SprintSpeedMult;
        else if (_State_MovementSpeed == State_MovementSpeed.crouchSpeed) _currentSpeedMult_Movement = _actor_Data._SneakSpeedMult;
    }

    private void CalculateCurrentCombinedSpeedMult()
    {
        _currentCombinedMaxSpeed = _actor_Data._Movespeed;
        _currentCombinedMaxSpeed *= _currentSpeedMult_Terrain;
        if (_State_Terrain == State_Terrain.ground) _currentCombinedMaxSpeed *= _currentSpeedMult_Movement;
    }

    private void CheckGround()
    {
        //check center ray first, object directly centered below us will be the _objectStandingOn
        //this will be the case most of the playtime, walking on plain ground
        //so make this check first and return if it's true
        Debug.DrawRay(_transform.position + _transform.transform.up * _colliderYbounds * 0.5f, -_transform.transform.up * (_colliderYbounds * 0.5f + _GroundRayLength), Color.red);
        if (Physics.Raycast(_transform.position + _transform.transform.up * _colliderYbounds * 0.5f, -_transform.transform.up, out _groundHitInfo, _colliderYbounds * 0.5f + _GroundRayLength))
        {
            _State_Terrain = State_Terrain.ground;
            _groundNormal = _groundHitInfo.normal;
            _objectStandingOn = _groundHitInfo.transform.gameObject;
            return;
        }
        else // if the center ray did not hit anything, check the outer rays if we are standing on a ledge
        {
            for (int i = -1; i <= 1; i += 2)
            {
                for (int j = -1; j <= 1; j += 2)
                {
                    Vector3 offset = new Vector3(i * _RayRadius, 0, j * _RayRadius);
                    Debug.DrawRay(_transform.position + offset + _transform.transform.up * _colliderYbounds * 0.5f, -_transform.transform.up * (_colliderYbounds * 0.5f + _GroundRayLength), Color.red);
                    if (Physics.Raycast(_transform.position + offset + _transform.transform.up * _colliderYbounds * 0.5f, -_transform.transform.up, out _groundHitInfo, _colliderYbounds * 0.5f + _GroundRayLength))
                    {
                        _State_Terrain = State_Terrain.ground;
                        _groundNormal = _groundHitInfo.normal;
                        _objectStandingOn = _groundHitInfo.transform.gameObject;
                        return;
                    }
                }
            }
        }
        //if none of the above returned, we are in the air
        _State_Terrain = State_Terrain.air;
        _groundNormal = Vector3.up;
        _objectStandingOn = null;
    }

    private void CheckHeadspace()
    {
        Debug.DrawRay(_transform.position + _transform.transform.up * _colliderYbounds * 0.5f, _transform.transform.up * (_colliderYbounds * 1.5f + _HeadspaceRayLength), Color.blue);
        if (Physics.Raycast(this.transform.position + _transform.transform.up * 1.0f, _transform.transform.up, out _headHitInfo, _colliderYbounds * 1.5f + _HeadspaceRayLength))
        {
            _State_Headspace = State_Headspace.canNotStand;
            _State_MovementSpeed = State_MovementSpeed.crouchSpeed;
            return;
        }
        _State_Headspace = State_Headspace.canStand;
    }

    private void CheckClimbable()
    {
        for (int i = -1; i <= 1; i += 2)
        {

            //Vector3 offset = new Vector3();
            //offset += i * _GroundRayRadius * _transform.right;
            //offset += j * _GroundRayRadius * _transform.up;
            //offset = _transform.InverseTransformDirection(offset);
            Vector3 offset = new Vector3(0, i * _RayRadius * 2f, 0);
            offset = _transform.TransformDirection(offset);
            Debug.DrawRay(_transform.position + offset + _transform.up * _colliderYbounds, _transform.forward * _ClimbRayLength, Color.cyan);
            if (Physics.Raycast(_transform.position + _transform.up * _colliderYbounds, _transform.transform.forward, out _climbHitInfo, _ClimbRayLength))
            {
                if (_climbHitInfo.transform.gameObject.tag == "Climbable")
                {
                    _State_LatchedSurface = State_LatchedSurface.climbableSurface;
                }
                else
                {
                    _State_LatchedSurface = State_LatchedSurface.anySurface;
                }
                _objectClimbingOn = _climbHitInfo.transform.gameObject;
                _climbNormal = _climbHitInfo.normal;
                return;
            }
        }
        _State_LatchedSurface = State_LatchedSurface.noSurface;
        _climbNormal = Vector3.up;
        _objectClimbingOn = null;
    }

    private void ShrinkCollider(bool small)
    {
        if (small)
        {
            _colliderCapsule.height = 1.0f;
            _colliderCapsule.center = new Vector3(0f, 1f, 0f);
        }
        else
        {
            _colliderCapsule.height = 2.0f;
            _colliderCapsule.center = new Vector3(0f, 1f, 0f);
        }
    }

    private void SetMeshYScale(float yScale)
    {
        _transform.localScale = new Vector3(1.0f, yScale, 1.0f);
    }

    private void ScaleCrouching()
    {
        if (_State_MovementSpeed == State_MovementSpeed.crouchSpeed)
        {
            ShrinkCollider(true);
            SetMeshYScale(0.5f);
        }
        else
        {
            ShrinkCollider(false);
            SetMeshYScale(1.0f);
        }
    }

    private void LatchToClimbable(bool inputCrouch)
    {
        if (!inputCrouch)
        {
            if (_State_Terrain == State_Terrain.air && _State_LatchedSurface == State_LatchedSurface.anySurface)
            {
                Debug.Log("latched to any Surface");
                //kill existing velocity to prevent walljumping out of controll
                zeroMovement();
                zeroRotation();
                _State_Terrain = State_Terrain.latching;
            }
            else if (_State_Terrain == State_Terrain.air && _State_LatchedSurface == State_LatchedSurface.climbableSurface)
            {
                Debug.Log("latched to climbable Surface");
                zeroMovement();
                zeroRotation();
                _State_Terrain = State_Terrain.climbing;
            }
        }
        else if (_State_Terrain == State_Terrain.climbing || _State_Terrain == State_Terrain.latching)
        {
            Debug.Log("released climb");
            _State_Terrain = State_Terrain.air;
        }
    }

    private void AttachToClimbable()
    {
        if (_State_Terrain == State_Terrain.climbing)
        {
            //rotate towards surface
            _transform.rotation = Quaternion.LookRotation(-_climbNormal, _transform.up);
            //ToDo: if attached object moves, change this actors position accordingy
            //use that local to world transfrom of this position relative to the attached object
        }
    }


    private void Gravity()
    {
        if (_State_Gravity == State_Gravity.gravityOn)
        {
            if (_State_Terrain == State_Terrain.ground)
            {
                _rigidbody.AddForce(Physics.gravity * _actor_Data._GravityMultiplier, ForceMode.Acceleration);
            }
            else if (_State_Terrain == State_Terrain.air)
            {
                _rigidbody.AddForce(Physics.gravity * 2.0f * _actor_Data._GravityMultiplier, ForceMode.Acceleration);
            }
            else if (_State_Terrain == State_Terrain.latching)
            {
                _rigidbody.AddForce(Physics.gravity * 1.0f * _actor_Data._GravityMultiplier, ForceMode.Acceleration);
            }
            else if (_State_Terrain == State_Terrain.climbing)
            {
                _rigidbody.AddForce(Physics.gravity * 0.0f * _actor_Data._GravityMultiplier, ForceMode.Acceleration);
            }
        }
    }

    private void zeroMovement()
    {
        //_rigidbody.AddForce(-_rigidbody.velocity, ForceMode.VelocityChange); //this leads to unitended negative movement
        _rigidbody.velocity = Vector3.zero;
    }

    private void zeroRotation()
    {
        //_rigidbody.AddTorque(-_rigidbody.angularVelocity, ForceMode.VelocityChange); //this leads to unitended negative movement
        _rigidbody.angularVelocity = Vector3.zero;
    }


    private void GetUp()
    {
        _rigidbody.MoveRotation(Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(Vector3.up), _actor_Data._Movespeed * _ConstRotScale * Time.deltaTime));
        //if sufficiently upright, set totally upright, lock rotation Constraints
    }


    //==============================================================
    //Input() is called every FixedUpdate by the InputController!!!
    //==============================================================
    public void Input(Vector3 inputMoveDirNormalized, Vector3 inputToMouseDirNormalized, Vector3 pointUnderCursor, GameObject objectUnderCursor, bool inputSprint, bool inputCrouch, bool inputJump, bool inputJumpDown)
    {
        if (inputSprint) _State_MovementSpeed = State_MovementSpeed.sprintSpeed;
        else if (inputCrouch) _State_MovementSpeed = State_MovementSpeed.crouchSpeed;
        else _State_MovementSpeed = State_MovementSpeed.walkSpeed;

        CheckGround();
        CheckHeadspace();
        CheckClimbable();

        ScaleCrouching();

        LatchToClimbable(inputCrouch);
        AttachToClimbable();
        Gravity();

        //get current speed multipliers last
        GetActorSpeedMult_TerrainState();
        GetActorSpeedMult_MovementState();
        CalculateCurrentCombinedSpeedMult();


        if (_actor_Data._mayAct)
        {
            if (_actor_Data._mayMove)
            {
                MoveTo(inputMoveDirNormalized);
            }

            if (_actor_Data._mayRotate)
            {
                RotateAimTo(inputToMouseDirNormalized, inputMoveDirNormalized);
            }

            if (_actor_Data._mayJump)
            {
                Jump(inputJump, inputJumpDown, inputCrouch);
            }
        }

        // if _NumJumps > 0 || _JumpPower > 0
        //Falling with actor._GravityMultiplier
        //Falling Impact with crouch and send falldamage to actor
    }


    private void MoveTo(Vector3 vecMove)
    {
        //correct speed modifiers are set here
        //_FinalMoveVector = (vecMove * _ConstMoveScale * Time.deltaTime * _actor_Data._Movespeed * _currentTerrainStateMoveFactor);
        _FinalMoveVector = vecMove * Time.deltaTime * _ConstMoveScale * _currentCombinedMaxSpeed;

        //sprint + crouch = slide (no 0 input breaking)


        if (_State_Terrain == State_Terrain.ground)
        {
            float dot = Vector3.Dot(_FinalMoveVector.normalized, _groundNormal);
            _FinalMoveVector = Vector3.ProjectOnPlane(_FinalMoveVector, _groundNormal);
            _FinalMoveVector += _FinalMoveVector * dot * _SlopeFactor * _actor._ActorData._GravityMultiplier;
            //updateVelocity = InputVelocity - Existing velocity
            _rigidbody.AddForce(_FinalMoveVector - _rigidbody.velocity, ForceMode.Impulse);
        }
    }

    private void RotateAimTo(Vector3 vecToMouse, Vector3 vecMove)
    {
        _ToMouseDirBody = vecToMouse;
        _ToMouseDirBody.y = 0.0f;
        _ToMouseDirHead = vecToMouse - (_transform.up * _colliderYbounds * 1.8f);
        _rotationBody = Quaternion.LookRotation(_ToMouseDirBody);
        _rotationHead = Quaternion.LookRotation(_ToMouseDirHead);

        // weapon drawn + no target = head/body rotate to aim
        if (_actor._weaponDrawn == true && _actor._focusedObject == null)
        {
            _rotationBody = Quaternion.Slerp(this.transform.rotation, _rotationBody, _ConstRotScale * Time.deltaTime * _currentCombinedMaxSpeed);
            _rotationBody = Quaternion.Euler(new Vector3(0f, _rotationBody.eulerAngles.y, 0f));
            _rigidbody.MoveRotation(_rotationBody);
        }
        //weapon drawn + target = head headLookAt, body rotate to aim
        else if (_actor._weaponDrawn == true && _actor._focusedObject != null)
        {

        }
        //weapon not drawn + target = headLookAt
        else if (_actor._weaponDrawn != true && _actor._focusedObject != null)
        {

        }
        //weapon not drawn + no target = roate with movement instatnly, snap to 8-way run
        else
        {
            //do not caclualte a new instant-rotation when there is no input, keep the current rotation
            //--> raw input vecMove. Surface-normal aligned _MoveDir would make the whole character align to it here
            if (vecMove.sqrMagnitude > 0) { _transform.rotation = Quaternion.LookRotation(vecMove); }


            //ToDo: constantly set rotation velocity to 0 so you dont spin after something pushed you, it may still rotate you
            zeroRotation();
        }

    }

    private void Jump(bool jump, bool JumpDown, bool crouchPressed)
    {
        {
            if (_State_Terrain == State_Terrain.ground)
            {
                if (crouchPressed && jump)
                {
                    Debug.Log("crouch jump");
                    _State_Terrain = State_Terrain.air;
                    _rigidbody.AddForce(new Vector3(0f, _actor_Data._JumpPower * 1.5f * _ConstJumpScale, 0f), ForceMode.Impulse);

                }
                else if (jump)
                {
                    Debug.Log("normal jump");
                    _State_Terrain = State_Terrain.air;
                    _rigidbody.AddForce(new Vector3(0f, _actor_Data._JumpPower * _ConstJumpScale, 0f), ForceMode.Impulse);

                }
            }
            else if (_State_Terrain == State_Terrain.latching || _State_Terrain == State_Terrain.climbing)
            {
                if (jump)
                {
                    Debug.Log("walljump");
                    zeroMovement();
                    zeroRotation();
                    _State_Terrain = State_Terrain.air;
                    _rigidbody.AddForce((_climbNormal + Vector3.up).normalized * _actor_Data._JumpPower * _ConstJumpScale, ForceMode.Impulse);
                    //_rigidbody.rotation = Quaternion.LookRotation(_climbNormal * -1f);
                }
            }
        }

    }

}

#endif