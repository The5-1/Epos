using UnityEngine;
using System.Collections;

public class NavmeshMovementController : MonoBehaviour {

    private Actor _thisActor;
    private Rigidbody _rigidbody;
    private UnityEngine.AI.NavMeshAgent _navAgent;
    private GameObject _Target;
    private Transform _TargetTransform;
    private Actor _TargetActor;


    public bool _isRagdolled = false;
    private Vector3 _distanceVec;
    private float _distanceSqr;

    void Awake()
    {
        _thisActor = this.gameObject.GetComponent<Actor>();
        _rigidbody = this.gameObject.GetComponent<Rigidbody>();
        _navAgent = this.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        _Target = GameObject.FindGameObjectWithTag("Player");
        _TargetTransform = _Target.transform;
        _TargetActor = _Target.GetComponent<Actor>();
    }

    void Start () {
        _distanceVec = new Vector3();
    }
	
	// Update is called once per frame
	void Update () {
        _distanceVec = _TargetTransform.position - this.transform.position;
        _distanceSqr = _distanceVec.sqrMagnitude;

        _navAgent.speed = _thisActor._ActorData._Movespeed * 0.1f;

        if (!_isRagdolled)
        {
            _navAgent.enabled = true;

            if (_distanceSqr >= 20.0f)
            {
                _navAgent.SetDestination(_TargetTransform.position);
            }
            else if (_distanceSqr <= 10.0f)
            {
                _navAgent.ResetPath();
            }
        }else
        {
            _navAgent.enabled = false;
        }

    }
}
